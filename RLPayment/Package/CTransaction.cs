using System;
using System.Collections.Generic;
using System.Text;
using System.CommonLib;
using System.ISO8583;
using System.EquipLib;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;
using System.Security.Cryptography;

namespace PetroChina
{
    public enum TransResult : int
    {
        E_SUCC = 0,               // �ɹ�����
        E_FILE_FAIL = 1,          // �����ļ�����
        E_SEND_FAIL = 2,          // ����Ҫ����
        E_RECV_FAIL = 3,          // ��Ҫ����
        E_HOST_FAIL = 4,          // �������ش������
        E_THREAD_FAIL = 5,        // ���������߳�ʧ��
        E_WAIT_FILE = 6,          // ���ڴ����ļ�
        E_MAC_ERROR = 7,          // ����˱���macУ���
        E_CHECK_FAIL = 10,	      // ����
        E_UNPACKET_FAIL = 11,     // ���ʧ��
        E_KEYVERIFY_FAIL = 12,    // ǩ��У��ʧ��
    }

    public enum AlgorithmType : int
    {
        X99 = 1,      // X9.9�㷨
        ECB = 2,      // ECB�㷨
        CBC = 3,      // CBC�㷨
    }

    public enum DesType : int
    {
        Des = 1, //��des
        TripleDes = 3, //3des
    }

    public enum EncryptType
    {
        Soft, //�����
        Hardware, //Ӳ����
    }

    class CTransaction
    {
        private const int SendTimeOutLong = 30 * 1000;
        private const int RecvTimeOutLong = 90 * 1000;

        protected Iso8583Package TransUpPack;
        protected Iso8583Package TransDownPack;
        public Iso8583Package RevTransData //������
        {
            get { return TransUpPack; }
        }
        
        protected EncryptType EnType; //��������
        protected DesType DType; //Des����
        protected AlgorithmType AlType; //�㷨����
        protected static byte[] MasterKey = new byte[0]; //���������Կ����
        protected static byte[] WorkKey = new byte[0]; //������Կ����
        public static byte[] PinKey = new byte[0]; //PIN��Կ����

        private byte[] SendBytes = new byte[0];
        private byte[] RecvBytes = new byte[0];
        private int SendLen = 0;

        public bool NeedCalcMac = true;

        protected static string userID = "";
        public static int Flag;

        /// <summary>
        /// �ϴ�����ҵ�����ݽṹ��
        /// </summary>
        public struct TransUpData
        {
            public string Field2; //2 ���п���
            public double Field4; //4 ���
            public string Field11; //11 �ն˽�����ˮ��
            public string Field12; //12 ����ʱ��
            public string Field13; //13 ��������
            public string Field14; //14 ����Ч��
            public string Field15; //15 ��������
            public string Field23; //23 �����к�
            public string Field31; //31 ҵ������
            public string Field32; //32 ������ʶ��
            public string Field34; //34 Ӧ������
            public string Field35; //35 ���ŵ�
            public string Field36; //36 ���ŵ�
            public string Field37; //37 ϵͳ�����ο���
            public string Field38; //38 ��Ȩ��ʶӦ����
            public string Field39; //39 ����ԭ��
            public string Field44; //44 ҵ�����
            public string Field48; //48 �Զ�����
            public string Field49; //49 ���Ҵ���
            public string Field52; //52 ����
            public string Field55; //55 IC������
            public string Field60; //60 �Զ�����
            public string Field63; //63 �Զ�����
        }
        public TransUpData SendData; 

        /// <summary>
        /// �´�����ҵ�����ݽṹ��
        /// </summary>
        public struct TransDownData
        {
            public string Field2; //2 ����/���˺�
            public double Field4; //4 ���
            public string Field11; //11 �ն˽�����ˮ�ţ�ԭֵ���أ�
            public string Field12; //12 ����ʱ��
            public string Field13; //13 ��������
            public string Field14; //14 ������Ч��
            public string Field15; //15 ��������
            public string Field25; //25 �����������
            public string Field32; //32 ������ʶ��
            public string Field37; //37 ϵͳ�����ο���/��ˮ��
            public string Field38; //38 ��Ȩ��ʶӦ����
            public string Field39; //39 ϵͳ���صĴ�����
            public string Field48; //48 ϵͳ���ص�ҵ������
            public string Field54; //54 ���
            public string Field55; //55 IC������
            public string Field60; //60 ���κ�
            public string SysErrMsg; //������
        }
        public TransDownData RecvData;

        public CTransaction()
        {
            if (Esam.IsUse)
                EnType = EncryptType.Hardware;
            else
                EnType = EncryptType.Soft;

            DType = (DesType)Global.gConfig.TransConfig.DESType;
            AlType = (AlgorithmType)Global.gConfig.TransConfig.AlgorithmType;
            if (DType == DesType.Des)
            {
                MasterKey = new byte[8];
                MasterKey = Utility.str2Bcd(Global.gConfig.TransConfig.MasterKey);
            }
            else if (DType == DesType.TripleDes)
            {
                MasterKey = new byte[16];
                MasterKey = Utility.str2Bcd(Global.gConfig.TransConfig.MasterKey);
            }

            TransUpPack = new Iso8583Package(Global.gAppPath + "\\iso8583.xml");
            TransUpPack.SmartBitmap = true;
            TransUpPack.MessageTypeLen = 4;

            TransDownPack = new Iso8583Package(Global.gAppPath + "\\iso8583.xml");
            TransDownPack.SmartBitmap = true;
            TransDownPack.MessageTypeLen = 4;
        }

        protected bool GPRSConnect()
        {
            if (!GPRS.IsUse) return true;
            bool ret = GPRS.CreateConnection(3, Global.gConfig.TransConfig.HostIP, Int32.Parse(Global.gConfig.TransConfig.HostPort));
            if (ret)
            {
                AppLog.Write("GPRS���ӳɹ���", AppLog.LogMessageType.Info);
            }
            else
            {
                AppLog.Write("GPRS����ʧ�ܣ�", AppLog.LogMessageType.Info);
            }
            return ret;
        }

        public TransResult TransExecute()
        {
            while (IsTransDoing())
            {
                System.Threading.Thread.Sleep(500);
            }
            UpdateTransDoing(true);

            TransResult nRet = TransResult.E_SEND_FAIL;
            if (!GPRSConnect())
            {
                UpdateTransDoing(false);
                return nRet;
            }

            //����
            CReverse rev = new CReverse();
            rev.DoLastReverse();

            //����
            nRet = Trans();

            UpdateTransDoing(false);
            return nRet;
        }

        protected byte[] Get48TLVBytes()
        {
            byte[] tmp = TransDownPack.GetArrayData(48);

            int len = int.Parse(Encoding.Default.GetString(tmp, 61, 3));
            byte[] tmp1 = new byte[len];
            Array.Copy(tmp, 64, tmp1, 0, len);
            return tmp1;
        }

        protected byte[] GetTLVBytesWithLen(string tlvStr)
        {
            byte[] tmp = Utility.str2Bcd(tlvStr);
            byte[] result = new byte[3 + tmp.Length];
            Array.Copy(Encoding.Default.GetBytes(tmp.Length.ToString().PadLeft(3, '0')), result, 3);
            Array.Copy(tmp, 0, result, 3, tmp.Length);
            return result;
        }

        protected virtual void PackFix()
        {
            TransUpPack.SetString(41, Global.gConfig.TerConfig.TerminalNo);
            TransUpPack.SetString(42, Global.gConfig.TransConfig.MerchantNo);
        }

        protected virtual void Packet()
        {
            AppLog.Write("[CTransaction][Packet]", AppLog.LogMessageType.Info);
        }

        protected virtual TransResult Trans()
        {
            TransResult nRet = TransResult.E_SEND_FAIL;
            try
            {
                PackFix();
                Packet();
                nRet = Communicate();
                AppLog.Write("[" + this.GetType().Name + "][Trans]nRet = " + Convert.ToString(nRet), AppLog.LogMessageType.Info, this.GetType());
                if (nRet == TransResult.E_HOST_FAIL || nRet == TransResult.E_SUCC)
                {
                    UnpackFix();
                }
                if (nRet == TransResult.E_SUCC)
                {
                    if (!UnPacket()) nRet = TransResult.E_UNPACKET_FAIL;
                }
            }
            catch (Exception err)
            {
                AppLog.Write("[" + this.GetType().Name + "][Trans]Error!\n", AppLog.LogMessageType.Error, err, this.GetType());
            }
            return nRet;            
        }

        protected TransResult Communicate()
        {
            TransResult ret = TransResult.E_SEND_FAIL;
            IPAddress ip = IPAddress.Parse(Global.gConfig.TransConfig.HostIP);
            IPEndPoint ipe = new IPEndPoint(ip, Int32.Parse(Global.gConfig.TransConfig.HostPort)); //��ip�Ͷ˿�ת��ΪIPEndPointʵ��
            Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                ClientSocket.SendTimeout = SendTimeOutLong;
                ClientSocket.ReceiveTimeout = RecvTimeOutLong;
                ClientSocket.Connect(ipe);
            }
            catch (Exception err)
            {
                AppLog.Write("[Communicate]Error!\n", AppLog.LogMessageType.Error, err,  this.GetType());
                return ret;
            }

            try
            {
                //�������
                SendBytes = TransUpPack.GetSendBuffer();
                SendLen = SendBytes.Length;
                if (SendLen <= 0)
                {
                    return ret;
                }

                //����mac����mac�����ȥ
                if (TransUpPack.MessageType != "0800")
                {
                    if (NeedCalcMac)
                    {
                        Esam.EsamSetWorkmode(Esam.WorkMode.Encrypt);
                        Esam.EsamSetMasterkeyNo(Global.gConfig.TransConfig.KEKIndex);
                        //����������64�򣬽�λͼ���㣬�����滻����ȷ��macָ
                        TransUpPack.SetArrayData(64, new byte[8], 8);
                        SendBytes = TransUpPack.GetSendBuffer();
                        byte[] sendBytes = new byte[SendBytes.Length - 8];
                        Array.Copy(SendBytes, sendBytes, sendBytes.Length);
                        byte[] MAC = new byte[8];
                        //AppLog.Write("CalcMAC Before", AppLog.LogMessageType.Warn);
                        bool bRet = CalcMAC(sendBytes, MAC);
                        //AppLog.Write("CalcMAC after", AppLog.LogMessageType.Warn);
                        Esam.EsamSetWorkmode(Esam.WorkMode.Default);
                        if (bRet)
                        {
                            //���´��
                            //AppLog.Write("DATA:" + bcd2str(sendBytes, sendBytes.Length), AppLog.LogMessageType.Debug);
                            //AppLog.Write("MAK:" + bcd2str(Global.gMackey, 8), AppLog.LogMessageType.Debug);
                            //AppLog.Write("MAC:" + bcd2str(MAC, 8), AppLog.LogMessageType.Debug);
                            TransUpPack.SetArrayData(64, MAC, 16);
                            SendBytes = TransUpPack.GetSendBuffer();
                            SendLen = SendBytes.Length;
                            if (SendLen <= 0)
                            {
                                return ret;
                            }
                        }
                        else
                        {
                            return ret;
                        }
                    }
                    else
                    {
                        //TransUpPack.SetArrayData(64, new byte[8], 8);
                        SendBytes = TransUpPack.GetSendBuffer();
                        SendLen = SendBytes.Length;
                    }
                }

                int sendLen_all = SendLen + 13;
                byte[] sendstr_all = new byte[sendLen_all];

                //����λ
                sendstr_all[0] = (byte)((sendLen_all - 2) / 256);
                sendstr_all[1] = (byte)((sendLen_all - 2) % 256);

                //TPDU
                byte[] TPDU = new byte[5];
                if (TransUpPack.MessageType == "0800")
                {
                    TPDU = Utility.str2Bcd(Global.gConfig.TransConfig.QDTpdu);
                }
                else
                {
                    TPDU = Utility.str2Bcd(Global.gConfig.TransConfig.Tpdu);
                }
                Array.Copy(TPDU, 0, sendstr_all, 2, 5);

                //��ͷ
                byte[] head = new byte[12];
                head = Utility.str2Bcd(Global.gConfig.TransConfig.ReportHead);
                Array.Copy(head, 0, sendstr_all, 7, 6);

                //����
                Array.Copy(SendBytes, 0, sendstr_all, 13, SendLen);

                //��¼ԭʼ������־
                CLog.Write("[Len]" + sendLen_all.ToString(), CLog.LogType.Send);
                CLog.Write(sendstr_all, 16, CLog.LogType.Send);
                CLog.Write(TransUpPack.GetSendLogText(), CLog.LogType.Send);

                //������Ϣ 
                int sendLen =ClientSocket.Send(sendstr_all, sendLen_all, 0);
                if (sendLen == 0)
                {
                    AppLog.Write("Send Failed!", AppLog.LogMessageType.Warn);
                    return ret;
                }

                //�ӷ������˽��ܷ�����Ϣ
                ret = TransResult.E_RECV_FAIL;
                int length = 1024 * 8;
                byte[] recvBytesAll = new byte[length];
                int recvLen;
                recvLen = ClientSocket.Receive(recvBytesAll, recvBytesAll.Length, SocketFlags.None);
                if (recvLen <= 13)
                {
                    AppLog.Write("RecvLen Error! Len = " + recvLen.ToString(), AppLog.LogMessageType.Warn);
                    return ret;
                }
                RecvBytes = new byte[recvLen - 13];
                Array.Copy(recvBytesAll, 13, RecvBytes, 0, recvLen - 13);
                Flag = recvBytesAll[9] & 0x0F;

                //��¼ԭʼ������־
                //byte[] logRecv = new byte[recvLen];
                //Array.Copy(recvBytesAll, logRecv, recvLen);
                //CLog.Write("[Len]" + recvLen.ToString(), CLog.LogType.Recv);
                //CLog.Write(logRecv, 16, CLog.LogType.Recv);
                //CLog.Write(TransDownPack.GetRecvLogText(), CLog.LogType.Recv);

                //���
                ret = TransResult.E_UNPACKET_FAIL;
                TransDownPack.ParseBuffer(RecvBytes, true);

                //��¼ԭʼ������־
                byte[] logRecv = new byte[recvLen];
                Array.Copy(recvBytesAll, logRecv, recvLen);
                CLog.Write("[Len]" + recvLen.ToString(), CLog.LogType.Recv);
                CLog.Write(logRecv, 16, CLog.LogType.Recv);
                CLog.Write(TransDownPack.GetRecvLogText(), CLog.LogType.Recv);

                if (TransDownPack.GetString(39) != "00")
                {
                    ret = TransResult.E_HOST_FAIL;
                }
                else
                {
                    ret = TransResult.E_SUCC;
                }
            }
            catch (Exception err)
            {
                AppLog.Write("[Communicate]Error!\r\n" + err.ToString(), AppLog.LogMessageType.Error);                
            }
            finally
            {
                ClientSocket.Close();
            }

            return ret;
        }

        /// <summary>
        /// ���ڳ����Ľ��׺���
        /// </summary>
        /// <param name="sendData">��������</param>
        /// <returns></returns>
        protected TransResult Communicate(byte[] sendData)
        {
            if (sendData == null || sendData.Length == 0)
            {
                return TransResult.E_SUCC;
            }

            TransResult ret = TransResult.E_SEND_FAIL;
            IPAddress ip = IPAddress.Parse(Global.gConfig.TransConfig.HostIP);
            IPEndPoint ipe = new IPEndPoint(ip, Int32.Parse(Global.gConfig.TransConfig.HostPort)); //��ip�Ͷ˿�ת��ΪIPEndPointʵ��
            Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                ClientSocket.SendTimeout = SendTimeOutLong;
                ClientSocket.ReceiveTimeout = RecvTimeOutLong;
                ClientSocket.Connect(ipe);
            }
            catch (Exception err)
            {
                AppLog.Write("[Communicate]Error!\r\n" + err.ToString(), AppLog.LogMessageType.Error, this.GetType());
                return ret;
            }

            try
            {
                TransUpPack.Clear();
                TransUpPack.ParseBuffer(sendData, true);
                if (TransUpPack.MessageType != "0800")
                {
                    if (NeedCalcMac)
                    {
                        Esam.EsamSetWorkmode(Esam.WorkMode.Encrypt);
                        Esam.EsamSetMasterkeyNo(Global.gConfig.TransConfig.KEKIndex);
                        //����������64�򣬽�λͼ���㣬�����滻����ȷ��macָ
                        TransUpPack.SetArrayData(64, new byte[8], 8);
                        SendBytes = TransUpPack.GetSendBuffer();
                        byte[] sendBytes = new byte[SendBytes.Length - 8];
                        Array.Copy(SendBytes, sendBytes, sendBytes.Length);
                        byte[] MAC = new byte[8];
                        bool bRet = CalcMAC(sendBytes, MAC);
                        Esam.EsamSetWorkmode(Esam.WorkMode.Default);
                        if (bRet)
                        {
                            //���´��
                            //AppLog.Write("DATA:" + bcd2str(sendBytes, sendBytes.Length), AppLog.LogMessageType.Debug);
                            //AppLog.Write("MAK:" + bcd2str(Global.gMackey, 8), AppLog.LogMessageType.Debug);
                            //AppLog.Write("MAC:" + bcd2str(MAC, 8), AppLog.LogMessageType.Debug);
                            TransUpPack.SetArrayData(64, MAC, 16);
                            SendBytes = TransUpPack.GetSendBuffer();
                            SendLen = SendBytes.Length;
                            if (SendLen <= 0)
                            {
                                return ret;
                            }
                        }
                        else
                        {
                            return ret;
                        }
                    }
                    else
                    {
                        //TransUpPack.SetArrayData(64, new byte[8], 8);
                        SendBytes = TransUpPack.GetSendBuffer();
                        SendLen = SendBytes.Length;
                    }
                }

                int sendLen_all = SendBytes.Length + 13;
                byte[] sendstr_all = new byte[sendLen_all];

                //����λ
                sendstr_all[0] = (byte)((sendLen_all - 2) / 256);
                sendstr_all[1] = (byte)((sendLen_all - 2) % 256);

                //TPDU
                byte[] TPDU = new byte[5];
                if (TransUpPack.MessageType == "0800")
                {
                    TPDU = Utility.str2Bcd(Global.gConfig.TransConfig.QDTpdu);
                }
                else
                {
                    TPDU = Utility.str2Bcd(Global.gConfig.TransConfig.Tpdu);
                }
                Array.Copy(TPDU, 0, sendstr_all, 2, 5);

                //��ͷ
                byte[] head = new byte[12];
                head = Utility.str2Bcd(Global.gConfig.TransConfig.ReportHead);
                Array.Copy(head, 0, sendstr_all, 7, 6);

                //����
                Array.Copy(sendData, 0, sendstr_all, 13, sendData.Length);

                //int sendLen_all = sendData.Length;
                //byte[] sendstr_all = new byte[sendLen_all];
                //Array.Copy(sendData, sendstr_all, sendLen_all);

                //��¼ԭʼ������־
                CLog.Write("[Len]" + sendLen_all.ToString(), CLog.LogType.Send);
                CLog.Write(sendstr_all, 16, CLog.LogType.Send);
                CLog.Write(TransUpPack.GetSendLogText(), CLog.LogType.Send);

                //������Ϣ 
                ClientSocket.Send(sendstr_all, sendLen_all, 0);

                //�ӷ������˽��ܷ�����Ϣ
                ret = TransResult.E_RECV_FAIL;
                int length = 1024 * 8;
                byte[] recvBytes = new byte[length];
                int recvLen;
                recvLen = ClientSocket.Receive(recvBytes, recvBytes.Length, 0);
                if (recvLen <= 13)
                {
                    AppLog.Write("RecvLen=" + recvLen.ToString(), AppLog.LogMessageType.Warn);
                    return ret;
                }
                byte[] precv = new byte[recvLen - 13];
                Array.Copy(recvBytes, 13, precv, 0, recvLen - 13);

                //���
                ret = TransResult.E_UNPACKET_FAIL;
                TransDownPack.ParseBuffer(precv, true);

                //��¼ԭʼ������־
                byte[] logRecv = new byte[recvLen];
                Array.Copy(recvBytes, logRecv, recvLen);
                CLog.Write("[Len]" + recvLen.ToString(), CLog.LogType.Recv);
                CLog.Write(logRecv, 16, CLog.LogType.Recv);
                CLog.Write(TransDownPack.GetRecvLogText(), CLog.LogType.Recv);

                if (TransDownPack.GetString(39) != "00")
                {
                    ret = TransResult.E_HOST_FAIL;
                }
                else
                {
                    ret = TransResult.E_SUCC;
                }
            }
            catch (Exception err)
            {
                AppLog.Write("[Communicate]Error!\n" + err.ToString(), AppLog.LogMessageType.Error);
            }
            finally
            {
                ClientSocket.Close();
            }

            return ret;
        }

        public virtual bool UnPacket()
        {
            AppLog.Write("[CTransaction][UnPacket]", AppLog.LogMessageType.Info);
            return true;
        }

        protected virtual bool UnpackFix()
        {
            try
            {
                RecvData.Field39 = TransDownPack.GetString(39);
                string msgMean = "", msgShow = "";
                if (ParseRespMessage(RecvData.Field39, ref msgMean, ref msgShow))
                {
                    RecvData.SysErrMsg = msgShow;
                    AppLog.Write("[UnpackFix]ReturnCode = " + RecvData.Field39 + " Message = " + msgMean, AppLog.LogMessageType.Info);
                }
                else
                {
                    RecvData.SysErrMsg = "���󣬽���ʧ��";
                    AppLog.Write("[UnpackFix]ReturnCode = " + RecvData.Field39 + " Message = δ֪����", AppLog.LogMessageType.Info);
                }
            }
            catch (System.Exception e)
            {
                AppLog.Write("[UnpackFix]Error!\n" + e.ToString(), AppLog.LogMessageType.Error, this.GetType());
                return false;
            }
            return true;
        }

        protected bool IsTransDoing()
        {
            return Global.gTransDoing;
        }

        protected void UpdateTransDoing(bool isDoing)
        {
            Global.gTransDoing = isDoing;
        }

        private bool CalcMAC(byte[] DATA, byte[] MAC)
        {
            switch (AlType)
            {
                case AlgorithmType.ECB:
                    return CalcMac_ECB(DATA, MAC);
                case AlgorithmType.CBC:
                case AlgorithmType.X99:
                    return CalcMac_CBC_X99(DATA, Global.gMackey, WorkKey, AlType, MAC);
                default:
                    return false;
            }
        }

        private bool CalcMac_ECB(byte[] DATA, byte[] MAC)
        {
            try
            {
                Esam.FuncRet eRet = Esam.FuncRet.ESAM_FAIL;
                byte[] xorRet = CalcMacXor(DATA);
                string strXor = bcd2str(xorRet, 8);
                byte[] newByte = Encoding.ASCII.GetBytes(strXor);
                byte[] beforeByte = new byte[8];
                byte[] afterByte = new byte[8];
                Array.Copy(newByte, beforeByte, 8);
                Array.Copy(newByte, 8, afterByte, 0, 8);
                byte[] encryptData = new byte[8];
                #region ��һ�μ���
                switch (EnType)
                {
                    case EncryptType.Hardware:
                        //������EsamCalcMac����������㣬Ч����ͬ
                        eRet = Esam.EsamCalcMac(Global.gMackey, beforeByte, 8, encryptData);
                        break;
                    case EncryptType.Soft:
                        switch (DType)
                        {
                            case DesType.Des:
                                //DES�����
                                encryptData = DesEncrypt(beforeByte, WorkKey);
                                break;
                            case DesType.TripleDes:
                                //3DES�����
                                encryptData = TripleDesEncrypt(beforeByte, WorkKey);
                                break;
                        }
                        eRet = Esam.FuncRet.ESAM_SUCC;
                        break;
                }
                #endregion
                if (eRet == Esam.FuncRet.ESAM_SUCC)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        xorRet[i] = Convert.ToByte(encryptData[i] ^ afterByte[i]);
                    }
                    #region �ڶ��μ���
                    switch (EnType)
                    {
                        case EncryptType.Hardware:
                            //������EsamCalcMac����������㣬Ч����ͬ
                            eRet = Esam.EsamCalcMac(Global.gMackey, xorRet, 8, encryptData);
                            break;
                        case EncryptType.Soft:
                            switch (DType)
                            {
                                case DesType.Des:
                                    //DES�����
                                    encryptData = DesEncrypt(xorRet, WorkKey);
                                    break;
                                case DesType.TripleDes:
                                    //3DES�����
                                    encryptData = TripleDesEncrypt(xorRet, WorkKey);
                                    break;
                            }
                            eRet = Esam.FuncRet.ESAM_SUCC;
                            break;
                    }
                    #endregion
                    if (eRet == Esam.FuncRet.ESAM_SUCC)
                    {
                        strXor = bcd2str(encryptData, 8);
                        newByte = Encoding.ASCII.GetBytes(strXor);
                        MAC = new byte[8];
                        Array.Copy(newByte, MAC, 8);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CalcMac_ECB]Error!\n" + e.ToString(), AppLog.LogMessageType.Error, this.GetType());
                return false;
            }
        }

        protected bool CalcMac_CBC_X99(byte[] DATA, byte[] hardKey, byte[] softKey, AlgorithmType alType, byte[] MAC)
        {
            try
            {
                Esam.FuncRet eRet = Esam.FuncRet.ESAM_FAIL;
                if (EnType == EncryptType.Hardware && alType == AlgorithmType.X99)
                {
                    eRet = Esam.EsamCalcMac(hardKey, DATA, DATA.Length, MAC);
                    if (eRet != Esam.FuncRet.ESAM_SUCC)
                        return false;
                    else
                        return true;
                }
                else if (EnType == EncryptType.Hardware && alType == AlgorithmType.CBC && DType == DesType.Des)
                {
                    eRet = Esam.EsamCalcMac(hardKey, DATA, DATA.Length, MAC);
                    if (eRet != Esam.FuncRet.ESAM_SUCC)
                        return false;
                    else
                        return true;
                }
                else
                {
                    EncryptType tmp = EnType;
                    EnType = EncryptType.Soft;
                    int len = (DATA.Length % 8 == 0 ? DATA.Length : DATA.Length + 8 - DATA.Length % 8);
                    byte[] cData = new byte[len];
                    Array.Copy(DATA, cData, DATA.Length);
                    byte[] tmpRet = new byte[8];
                    for (int i = 0; i < len; i += 8)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            tmpRet[j] ^= cData[j + i];
                        }
                        byte[] tmpRetEncrypt = new byte[8];
                        //if (DType == DesType.Des && EnType == EncryptType.Hardware)
                        //{
                        //    //������EsamCalcMac����������㣬Ч����ͬ
                        //    eRet = Esam.EsamCalcMac(Global.gMackey, tmpRet, 8, tmpRetEncrypt);
                        //}
                        //else 
                        if (DType == DesType.Des && EnType == EncryptType.Soft)
                        {
                            //DES�����
                            tmpRetEncrypt = DesEncrypt(tmpRet, softKey);
                            eRet = Esam.FuncRet.ESAM_SUCC;
                        }
                        else if (DType == DesType.TripleDes && alType == AlgorithmType.CBC)
                        {
                            switch (EnType)
                            {
                                case EncryptType.Hardware:
                                    //������EsamCalcMac����������㣬Ч����ͬ
                                    eRet = Esam.EsamCalcMac(hardKey, tmpRet, 8, tmpRetEncrypt);
                                    break;
                                case EncryptType.Soft:
                                    //3DES����� 
                                    tmpRetEncrypt = TripleDesEncrypt(tmpRet, softKey);
                                    eRet = Esam.FuncRet.ESAM_SUCC;
                                    break;
                            }
                        }
                        else if (DType == DesType.TripleDes && alType == AlgorithmType.X99)
                        {
                            //����ֻ�������һ�ֿ��ܣ�Ӳ����ǰ���Ѵ���
                            if (i == len - 8)
                            {
                                //3DES�����
                                tmpRetEncrypt = TripleDesEncrypt(tmpRet, softKey);
                                //AppLog.Write("����һ��", AppLog.LogMessageType.Debug);
                                //AppLog.Write(Utility.bcd2str(softKey, softKey.Length), AppLog.LogMessageType.Debug);
                                //AppLog.Write(Utility.bcd2str(tmpRet, tmpRet.Length), AppLog.LogMessageType.Debug);
                                //AppLog.Write(Utility.bcd2str(tmpRetEncrypt, tmpRetEncrypt.Length), AppLog.LogMessageType.Debug);
                                eRet = Esam.FuncRet.ESAM_SUCC;
                            }
                            else
                            {
                                //DES����� 
                                byte[] tmpKey = new byte[8];
                                Array.Copy(softKey, tmpKey, 8);
                                tmpRetEncrypt = DesEncrypt(tmpRet, tmpKey);
                                eRet = Esam.FuncRet.ESAM_SUCC;
                            }
                        }

                        if (eRet == Esam.FuncRet.ESAM_SUCC)
                        {
                            Array.Copy(tmpRetEncrypt, tmpRet, 8);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    Array.Copy(tmpRet, MAC, 8);
                    EnType = tmp;
                    return true;
                }
            }
            catch (System.Exception e)
            {
                AppLog.Write("[CalcMac_CBC_X99]Error!\n" + e.ToString(), AppLog.LogMessageType.Error, this.GetType());
                return false;
            }
        }

        private byte[] DesEncrypt(byte[] originalData, byte[] key)
        {
            byte[] encryptData = new byte[0];
            encryptData = Encrypt.DESEncrypt(originalData, key);
            return encryptData;
        }

        private byte[] TripleDesEncrypt(byte[] originalData, byte[] key)
        {
            byte[] encryptData = new byte[0];
            encryptData = Encrypt.DES3Encrypt(originalData, key);
            return encryptData;
        }

        /// <summary>
        /// �����ݽ���ÿ8���ֽ�ѭ�����
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] CalcMacXor(byte[] data)
        {
            int len = (data.Length % 8 == 0 ? data.Length : data.Length + 8 - data.Length % 8);
            byte[] cData = new byte[len];
            Array.Copy(data, cData, data.Length);
            byte[] ret = new byte[8];
            for (int i = 0; i < len; i += 8 )
            {
                for (int j = 0; j < 8; j++ )
                {
                    ret[j] ^= cData[j + i];
                }
            }
            return ret;
        }

        /// <summary>
        /// ��bcd��ת�����ַ���
        /// </summary>
        /// <param name="bcd_buf"></param>
        /// <param name="conv_len"></param>
        /// <returns></returns>
        private string bcd2str(byte[] bcd_buf, int conv_len)
        {
            int i = 0, j = 0;
            byte tmp = 0x00;
            byte[] ret = new byte[conv_len * 2];
            for (i = 0, j = 0; i < conv_len; i++)
            {
                tmp = (byte)(bcd_buf[i] >> 4);
                tmp += (byte)((tmp > 9) ? ('A' - 10) : '0');
                ret[j++] = tmp;
                tmp = (byte)(bcd_buf[i] & 0x0f);
                tmp += (byte)((tmp > 9) ? ('A' - 10) : '0');
                ret[j++] = tmp;
            }
            return Encoding.Default.GetString(ret).TrimEnd('\0');
        }

        /// <summary>
        /// ���ַ���ת����bcd��
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public byte[] str2bcd(string s)
        {
            s = s.Replace(" ", "");
            if (s.Length % 2 != 0) s = "0" + s;
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }

        /// <summary>
        /// Ӧ�������
        /// </summary>
        /// <param name="code">Ӧ����</param>
        /// <param name="mean">����</param>
        /// <param name="show">��ʾ������</param>
        /// <returns></returns>
        private bool ParseRespMessage(string code, ref string mean, ref string show)
        {
            XMLConfig config = new XMLConfig(Global.gAppPath + "\\errconf.xml");
            XmlNode node;
            int elementCount = config.GetNodeElementCount("/Config");
            for (int iPer = 0; iPer <= elementCount - 1; iPer++ )
            {
                node = config.GetNodeElementById("/Config", iPer);
                if (config.GetAttributeValue(node, "Code") == code)
                {
                    mean = config.GetAttributeValue(node, "Mean");
                    show = config.GetAttributeValue(node, "Show");
                    return true;
                }
            }
            return false;
        }
    }

    
}


/*
//C#ʵ��cbc��9.9�㷨
case AlgorithmType.CBC:
case AlgorithmType.X99:
    int len = (DATA.Length % 8 == 0 ? DATA.Length : DATA.Length + 8 - DATA.Length % 8);
    byte[] cData = new byte[len];
    Array.Copy(DATA, cData, DATA.Length);
    byte[] tmpRet = new byte[8];
    for (int i = 0; i < len; i += 8)
    {
        for (int j = 0; j < 8; j++)
        {
            tmpRet[j] ^= cData[j + i];
        }

        byte[] tmpRetEncrypt = new byte[8];
        if (DType == DesType.Des && EnType == EncryptType.Hardware)
        {
            fRet = Esam.EsamEncrypt(tmpRet, 8, tmpRetEncrypt);
        }
        else if (DType == DesType.Des && EnType == EncryptType.Soft)
        {
            //DES�����

            fRet = Esam.FuncRet.ESAM_SUCC;
        }
        else if (DType == DesType.TripleDes && AlType == AlgorithmType.CBC)
        {
            switch (EnType)
            {
                case EncryptType.Hardware:
                    fRet = Esam.EsamEncrypt(tmpRet, 8, tmpRetEncrypt);
            	    break;
                case EncryptType.Soft:
                    //3DES����� 

                    fRet = Esam.FuncRet.ESAM_SUCC;
                    break;
            }
        }
        else if (DType == DesType.TripleDes && AlType == AlgorithmType.X99)
        {
            if (i == len - 8)
            {
                //���һ��
                switch (EnType)
                {
                    case EncryptType.Hardware:
                        fRet = Esam.EsamEncrypt(tmpRet, 8, tmpRetEncrypt);
                        break;
                    case EncryptType.Soft:
                        //3DES����� 

                        fRet = Esam.FuncRet.ESAM_SUCC;
                        break;
                }
            }
            else
            {
                //ģ�ⵥdes����
                byte[] dblTmpData = new byte[16];
                Array.Copy(tmpRet, dblTmpData, 8);
                Array.Copy(tmpRet, 0, dblTmpData, 8, 8);
                switch (EnType)
                {
                    case EncryptType.Hardware:
                        fRet = Esam.EsamEncrypt(dblTmpData, 8, tmpRetEncrypt);
                        break;
                    case EncryptType.Soft:
                        //3DES����� 

                        fRet = Esam.FuncRet.ESAM_SUCC;
                        break;
                }
            }
        }

        if (fRet == Esam.FuncRet.ESAM_SUCC)
        {
            Array.Copy(tmpRetEncrypt, tmpRet, 8);
        }
        else
        {
            return false;
        }
    }
    Array.Copy(tmpRet, MAC, 8);
    return true;

*/