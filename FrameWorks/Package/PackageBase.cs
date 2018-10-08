using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks.Iso8583;
using Landi.FrameWorks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using log4net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Landi.FrameWorks.HardWare;
using System.Xml;

namespace Landi.FrameWorks
{
    public enum TransResult : int
    {
        E_SUCC = 0,               // �ɹ�����
        E_SEND_FAIL = 1,          // ����Ҫ����
        E_RECV_FAIL = 2,          // ��Ҫ����
        E_HOST_FAIL = 3,          // �������ش������
        E_MAC_ERROR = 4,          // ����˱���macУ���
        E_CHECK_FAIL = 5,	      // ����
        E_UNPACKET_FAIL = 6,     // ���ʧ��
        E_KEYVERIFY_FAIL = 7,    // ǩ��У��ʧ��
        E_PACKET_FAIL = 8,      //���ʧ��
        E_INVALID = 9,            //��Ч����
    }

    public abstract class PackageBase
    {
        public delegate void ResultHandler(TransResult result);
        protected enum AlgorithmType : int
        {
            X99 = 1,      // X9.9�㷨
            ECB = 2,      // ECB�㷨
            CBC = 3,      // CBC�㷨
        }

        protected enum DesType : int
        {
            Des = 1, //��des
            TripleDes = 3, //3des
        }

        protected enum EncryptType
        {
            Soft, //�����
            Hardware, //Ӳ����
        }

        protected EncryptType EnType = EncryptType.Soft; //��������
        protected DesType DType = DesType.TripleDes; //Des����
        protected AlgorithmType AlType = AlgorithmType.X99; //�㷨����
        protected int KeyLength = 16;//��Կ����

        protected BaseEntity BaseBusinessEntity
        {
            get { return (BaseEntity)businessBundle.Get(Activity.ENTITYKEY); }
        }

        private string mSchemaFile;
        protected Iso8583Package SendPackage;
        protected Iso8583Package RecvPackage;

        private int headLen;
        internal string serverIP;
        internal int serverPort = -1;
        internal int sendTimeOut = -1;
        internal int recvTimeOut = -1;
        internal static byte[] sRecvBuffer = new byte[1024 * 1024];

        public string ReturnCode;
        public string ReturnMean;
        public string ReturnMessage;

        internal bool mInvokeSetResult = false;
        protected string mSectionName;
        internal int RepeatTimes = 1;
        private bool mRealEnv = false;
        /// <summary>
        /// 0��ʾ��������ʵͨѶ�������ʾ������ʵͨѶ
        /// </summary>
        protected bool RealEnv
        {
            get { return mRealEnv; }
            private set { mRealEnv = value; }
        }
        private static readonly object sLock = new object();
        internal ResultHandler OnResult;
        protected TransResult Result = TransResult.E_SUCC;

        protected static readonly string StartupPath = Application.StartupPath;

        #region ����������Ϣ����
        private static readonly object sErrorLock = new object();
        private struct MeanAndShow
        {
            public string Mean;
            public string Show;
        }

        private static Dictionary<string, Dictionary<string, MeanAndShow>> sParsedInfo = new Dictionary<string, Dictionary<string, MeanAndShow>>();
        private string mErrConfigFile;
        protected virtual void ParseRespMessage(string code, ref string mean, ref string show)
        {
            ParseRespMessage(mErrConfigFile, code, ref mean, ref show);
            //����ҵ����������
            if (BaseBusinessEntity != null)
                BaseBusinessEntity.ParseRespMessage(code, ref mean, ref show);
        }

        /// <summary>
        /// Ӧ�������
        /// </summary>
        /// <param name="code">Ӧ����</param>
        /// <param name="mean">����</param>
        /// <param name="show">��ʾ������</param>
        /// <returns></returns>
        private static void ParseRespMessage(string configFile, string code, ref string mean, ref string show)
        {
            if (string.IsNullOrEmpty(configFile) || Path.GetExtension(configFile) != ".xml")
                return;
            lock (sErrorLock)
            {
                if (!sParsedInfo.ContainsKey(configFile))
                {
                    XMLConfig config = new XMLConfig(configFile);
                    XmlNode node;
                    int elementCount = config.GetNodeElementCount("/Config");
                    if (elementCount > 0)
                    {
                        Dictionary<string, MeanAndShow> infos = new Dictionary<string, MeanAndShow>();
                        for (int iPer = 0; iPer <= elementCount - 1; iPer++)
                        {
                            node = config.GetNodeElementById("/Config", iPer);
                            MeanAndShow ins = new MeanAndShow();
                            ins.Mean = config.GetAttributeValue(node, "Mean");
                            ins.Show = config.GetAttributeValue(node, "Show");

                            infos.Add(config.GetAttributeValue(node, "Code"), ins);
                        }
                        sParsedInfo.Add(configFile, infos);
                    }
                }
                Dictionary<string, MeanAndShow> ret = sParsedInfo[configFile];
                if (ret.ContainsKey(code))
                {
                    mean = ret[code].Mean;
                    show = ret[code].Show;
                }
            }
        }
        #endregion

        #region EnqueueWork and DequeueWork ��һ�ν����У��������������Ҫ���ͣ����Խ���˶���
        private static Dictionary<string, List<PackageBase>> mDeferWorks;
        protected void EnqueueWork(PackageBase instance)
        {
            EnqueueWork(mSectionName, instance);
        }

        private static void EnqueueWork(string Name, PackageBase instance)
        {
            bool add = true;
            lock (sLock)
            {
                if (mDeferWorks == null)
                    mDeferWorks = new Dictionary<string, List<PackageBase>>();
                List<PackageBase> list = null;
                if (mDeferWorks.ContainsKey(Name))
                    list = mDeferWorks[Name];
                else
                    list = new List<PackageBase>();
                for (int i = 0; i < list.Count; i++)
                    if (list[i].GetType().FullName == instance.GetType().FullName)
                    {
                        add = false;
                        break;
                    }
                if (add)
                    list.Add(instance);
                if (!mDeferWorks.ContainsKey(Name))
                    mDeferWorks.Add(Name, list);
            }
        }

        protected bool RemoveWork(PackageBase instance)
        {
            return RemoveWork(mSectionName, instance);
        }

        private static bool RemoveWork(string Name, PackageBase instance)
        {
            bool remove = false;
            lock (sLock)
            {
                if (mDeferWorks != null && mDeferWorks.ContainsKey(Name) && mDeferWorks[Name].Count > 0)
                {
                    for (int i = 0; i < mDeferWorks[Name].Count; i++)
                        if (mDeferWorks[Name][i].GetType().FullName == instance.GetType().FullName)
                        {
                            mDeferWorks[Name].RemoveAt(i);
                            remove = true;
                        }
                }
            }
            return remove;
        }

        internal PackageBase DequeueWork()
        {
            return DequeueWork(mSectionName);
        }

        private static PackageBase DequeueWork(string Name)
        {
            PackageBase pb = null;
            lock (sLock)
            {
                if (mDeferWorks != null && mDeferWorks.ContainsKey(Name) && mDeferWorks[Name].Count > 0)
                {
                    pb = mDeferWorks[Name][0];
                    mDeferWorks[Name].RemoveAt(0);
                }
            }
            return pb;
        }
        #endregion

        protected void SetResult(TransResult ret)
        {
            Result = ret;
        }

        protected void WriteIniFile(string key, string value)
        {
            ConfigFile.WriteConfig(mSectionName, key, value);
        }

        protected string ReadIniFile(string key)
        {
            return ConfigFile.ReadConfigAndCreate(mSectionName, key);
        }

        protected PackageBase()
        {
            readConfig();
            SendPackage = new Iso8583Package(mSchemaFile);
            RecvPackage = new Iso8583Package(mSchemaFile);
        }

        protected PackageBase(PackageBase pb)
        {
            readConfig();
            SendPackage = new Iso8583Package(pb.SendPackage);
            RecvPackage = new Iso8583Package(mSchemaFile);
        }

        protected void SetRepeatTimes(int times)
        {
            if (times >= 1)
                RepeatTimes = times;
        }

        private static List<string> mNameList = new List<string>();
        private void readConfig()
        {
            mSectionName = SectionName;
            if (!string.IsNullOrEmpty(mSectionName))
            {
                if (!mNameList.Contains(mSectionName))
                {
                    defaultPrepareConfig();
                    mNameList.Add(mSectionName);
                }
            }
            else
                throw new Exception(this + "��������������");

            mErrConfigFile = ReadIniFile("ErrConfFile");
            if (!string.IsNullOrEmpty(mErrConfigFile))
                mErrConfigFile = Path.Combine(Application.StartupPath, mErrConfigFile);
            else
                mErrConfigFile = null;
            mSchemaFile = ReadIniFile("SchemaFile");
            if (string.IsNullOrEmpty(mSchemaFile))
                throw new Exception("�����������ļ�����Ϊ��");
            else
                mSchemaFile = Path.Combine(Application.StartupPath, mSchemaFile);

            if (ReadIniFile("Use") == "0")
                RealEnv = false;
            else
                RealEnv = true;
            if (Esam.IsUse)
                EnType = EncryptType.Hardware;
            string content = ReadIniFile("Des");
            if (content != "1" && content != "3")
                throw new Exception("DES�������ô���");
            if (content == "1")
            {
                DType = DesType.Des;
                KeyLength = 8;
            }
            content = ReadIniFile("MacAlgorithm");
            if (content != "1" && content != "2" && content != "3")
                throw new Exception("MAC�㷨���ô���");
            if (content == "2")
                AlType = AlgorithmType.ECB;
            else if (content == "3")
                AlType = AlgorithmType.CBC;

            if (RealEnv)
            {
                IPAddress.Parse(ReadIniFile("ServerIP"));
                serverIP = ReadIniFile("ServerIP");
                serverPort = int.Parse(ReadIniFile("ServerPort"));
                sendTimeOut = int.Parse(ReadIniFile("SendTimeOut"));
                recvTimeOut = int.Parse(ReadIniFile("RecvTimeOut"));
            }
        }

        protected abstract string SectionName
        {
            get;
        }

        protected virtual string GetTraceNo()
        {
            string TraceNo = ReadIniFile("TraceNo");
            if (TraceNo == "")
                TraceNo = "0";
            int tmp;
            if (!int.TryParse(TraceNo, out tmp))
                return null;
            if (tmp == 999999)
                tmp = 0;
            tmp++;
            WriteIniFile("TraceNo", tmp.ToString());
            return tmp.ToString().PadLeft(6, '0');
        }

        protected abstract void PackFix();

        protected abstract bool UnPackFix();

        protected virtual void Packet()
        {

        }

        protected abstract byte[] PackBytesAtFront(int dataLen);

        /// <summary>
        /// ����MACֵ
        /// </summary>
        /// <param name="macBytes">����MAC���������</param>
        /// <param name="DataOut">�������MACֵ</param>
        /// <returns>true��ʾ����ɹ��������ʾʧ��</returns>
        protected virtual bool CalcMacByMackey(byte[] macBytes, byte[] MAC)
        {
            bool ret = false;
            switch (AlType)
            {
                case AlgorithmType.ECB:
                    ret = CalcMac_ECB(this, macBytes, KeyManager.GetEnMacKey(mSectionName),KeyManager.GetDeMacKey(mSectionName), MAC);
                    break;
                case AlgorithmType.CBC:
                case AlgorithmType.X99:
                    ret = CalcMac_CBC_X99(this, macBytes, KeyManager.GetEnMacKey(mSectionName), KeyManager.GetDeMacKey(mSectionName), AlType, MAC);
                    break;
            }
            return ret;
        }

        protected virtual bool CalcMacByPinkey(byte[] macBytes, byte[] MAC)
        {
            bool ret = false;
            switch (AlType)
            {
                case AlgorithmType.ECB:
                    ret = CalcMac_ECB(this, macBytes, KeyManager.GetEnPinKey(mSectionName), KeyManager.GetDePinKey(mSectionName), MAC);
                    break;
                case AlgorithmType.CBC:
                case AlgorithmType.X99:
                    ret = CalcMac_CBC_X99(this, macBytes, KeyManager.GetEnPinKey(mSectionName), KeyManager.GetDePinKey(mSectionName), AlType, MAC);
                    break;
            }
            return ret;
        }

        /// <summary>
        /// �����ݽ���ÿ8���ֽ�ѭ�����
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static byte[] CalcMacXor(byte[] data)
        {
            int len = (data.Length % 8 == 0 ? data.Length : data.Length + 8 - data.Length % 8);
            byte[] cData = new byte[len];
            Array.Copy(data, cData, data.Length);
            byte[] ret = new byte[8];
            for (int i = 0; i < len; i += 8)
            {
                for (int j = 0; j < 8; j++)
                {
                    ret[j] ^= cData[j + i];
                }
            }
            return ret;
        }


        private void defaultPrepareConfig()
        {
            string[] keys = new string[] { "Use", "ServerIP", "ServerPort", "MerchantNo", "TerminalNo", "TPDU", "Head", "SendTimeOut", "RecvTimeOut", "BatchNo", "TraceNo", "SchemaFile", "ErrConfFile", "SoftMasterKey", "KeyIndex", "Des", "MacAlgorithm" };
            for (int i = 0; i < keys.Length; i++)
            {
                if (ReadIniFile(keys[i]) == "")
                {
                    if (keys[i] == "Use")
                        WriteIniFile(keys[i], "0");
                    else if (keys[i] == "SendTimeOut")
                        WriteIniFile(keys[i], "30");
                    else if (keys[i] == "RecvTimeOut")
                        WriteIniFile(keys[i], "30");
                }
            }
        }

        protected string GetMerchantNo()
        {
            string MerchantNo = ReadIniFile("MerchantNo");
            if (MerchantNo == "")
            {
                throw new Exception("��δ�����̻���");
            }
            return MerchantNo;
        }

        protected string GetTerminalNo()
        {
            string TerminalNo = ReadIniFile("TerminalNo");
            if (TerminalNo == "")
            {
                throw new Exception("��δ�����ն˺�");
            }
            return TerminalNo;
        }

        protected string GetBatchNo()
        {
            string BatchNo = ReadIniFile("BatchNo");
            if (BatchNo == "")
            {
                throw new Exception("��δ�������κ�");
            }
            return BatchNo;
        }

        protected void SetBatchNo(string BatchNo)
        {
            WriteIniFile("BatchNo", BatchNo);
        }

        protected string GetTPDU()
        {
            string TPDU = ReadIniFile("TPDU");
            if (TPDU == "")
            {
                throw new Exception("��δ����TPDU");
            }
            return TPDU;
        }

        protected string GetHead()
        {
            string Head = ReadIniFile("Head");
            if (Head == "")
            {
                throw new Exception("��δ���ð�ͷ");
            }
            return Head;
        }

        protected byte[] GetSoftMasterKey()
        {
            string SoftMasterKey = ReadIniFile("SoftMasterKey");
            if (SoftMasterKey == "")
            {
                throw new Exception("��δ��������Կ����");
            }
            if (((DType == DesType.Des && SoftMasterKey.Length != 16) || (DType == DesType.TripleDes && SoftMasterKey.Length != 32)) && EnType == EncryptType.Soft)
                throw new Exception("����Կ���ĳ��Ȳ��Ϸ�");
            return Utility.str2Bcd(SoftMasterKey);
        }

        protected int GetKeyIndex()
        {
            string KeyIndex = ReadIniFile("KeyIndex");
            if (KeyIndex == "")
            {
                throw new Exception("��δ������Կ����");
            }
            int index = 0;
            if (!int.TryParse(KeyIndex, out index) || index < 0)
                throw new Exception("�Ƿ�����Կ����");
            return index;
        }

        /// <summary>
        /// �жϱ��Ĵ��ʱ�Ƿ���Ҫ����MAC
        /// </summary>
        /// <returns>true��ʾ��Ҫ����MAC��������CalcMac������false��ʾ�෴</returns>
        protected abstract bool NeedCalcMac();

        protected void SavePackageToFile()
        {
            FileStream fileStream = new FileStream(Path.Combine(Application.StartupPath, this.GetType().FullName + ".dat"), FileMode.Create);
            BinaryFormatter b = new BinaryFormatter();
            b.Serialize(fileStream, SendPackage);
            fileStream.Close();
            //Test ����
            //File.Copy(Path.Combine(Application.StartupPath, this.GetType().FullName + ".dat"),
            //    Path.Combine(@"d:\", this.GetType().FullName + ".dat"), true);
        }

        protected void RestorePackageFromFile()
        {
            string reverseFile = Path.Combine(Application.StartupPath, this.GetType().FullName + ".dat");
            if (!File.Exists(reverseFile))
            {
                Result = TransResult.E_INVALID;
                return;
            }
            try
            {
                FileStream fileStream = new FileStream(reverseFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                if (fileStream != null)
                {
                    BinaryFormatter b = new BinaryFormatter();
                    SendPackage = b.Deserialize(fileStream) as Iso8583Package;
                    fileStream.Close();
                }
            }
            catch (System.Exception)
            {
                Result = TransResult.E_INVALID;
            }
        }

        protected void DeletePackageFile()
        {
            File.Delete(Path.Combine(Application.StartupPath, this.GetType().FullName + ".dat"));
        }

        protected bool GPRSConnect()
        {
            if (!GPRS.IsUse) return true;
            bool ret = GPRS.CreateConnection(3, serverIP, serverPort);
            if (ret)
            {
                Log.Info("GPRS���ӳɹ���");
            }
            else
            {
                Log.Info("GPRS����ʧ�ܣ�");
            }
            return ret;
        }

        /// <summary>
        /// ��private��Ϊinternal virtual��Ϊ����Ŀ��ֹʹ��iso8583����ͨѶ
        /// </summary>
        /// <returns></returns>
        internal virtual TransResult transact()
        {
            TransResult ret = TransResult.E_SEND_FAIL;
            Socket socket = null;
            if (RealEnv)
            {
                if (!GPRSConnect()) return ret;
                IPAddress ip = IPAddress.Parse(serverIP);
                IPEndPoint ipe = new IPEndPoint(ip, serverPort); //��ip�Ͷ˿�ת��ΪIPEndPointʵ��
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.SendTimeout = sendTimeOut * 1000;
                    socket.ReceiveTimeout = recvTimeOut * 1000;
                    socket.Connect(ipe);
                }
                catch (Exception err)
                {
                    Log.Error(this.GetType().Name, err);
                    return ret;
                }
            }

            try
            {
                ret = TransResult.E_PACKET_FAIL;
                byte[] SendBytes = new byte[0];
                PackFix();
                Packet();
                if (SendPackage.IsNull())
                    return TransResult.E_INVALID;
                byte[] MAC = new byte[8];

                //����mac����mac�����ȥ
                if (NeedCalcMac())
                {
                    byte[] macKey = null;
                    if (EnType == EncryptType.Hardware)
                        macKey = KeyManager.GetEnMacKey(mSectionName);
                    else
                        macKey = KeyManager.GetDeMacKey(mSectionName);
                    if (macKey == null)
                        throw new Exception("��δ����MAC��Կ");
                    if ((DType == DesType.Des && macKey.Length == 16) || (DType == DesType.TripleDes && macKey.Length == 8))
                    {
                        throw new Exception("MAC��Կ���Ȳ�����DES�㷨");
                    }
                    int macField = 64;
                    if (SendPackage.ExistBit(1))
                        macField = 128;
                    SendPackage.SetArrayData(macField, new byte[8]);
                    byte[] tmp = SendPackage.GetSendBuffer();
                    byte[] macBytes = new byte[tmp.Length - 8];
                    Array.Copy(tmp, macBytes, macBytes.Length);
                    if (CalcMacByMackey(macBytes, MAC))
                    {
                        SendPackage.SetArrayData(macField, MAC);
                    }
                    else
                    {
                        SendPackage.ClearBitAndValue(macField);
                        throw new Exception("����MACʧ��");
                    }
                }
                SendBytes = SendPackage.GetSendBuffer();
                if (SendBytes.Length <= 0)
                {
                    return ret;
                }
                byte[] head = PackBytesAtFront(SendBytes.Length);
                headLen = head.Length;
                int sendLen_all = SendBytes.Length + head.Length;

                byte[] sendstr_all = new byte[sendLen_all];
                Array.Copy(head, sendstr_all, head.Length);
                Array.Copy(SendBytes, 0, sendstr_all, head.Length, SendBytes.Length);

                //��¼ԭʼ������־
                //CLog.LogPackage(sendstr_all, SendPackage, CLog.LogType.Send);
                CLog.Info(CLog.GetLog(sendstr_all, SendPackage, this, CLog.LogType.Send));

                ret = TransResult.E_SEND_FAIL;
                if (RealEnv)
                {
                    int sendLen = 0;
                    sendLen = socket.Send(sendstr_all, sendLen_all, 0);
                    if (sendLen <= 0)
                    {
                        socket.Close();
                        return ret;
                    }
                }

                //�ӷ������˽��ܷ�����Ϣ
                ret = TransResult.E_RECV_FAIL;
                int recvLen = 0;
                if (RealEnv)
                {
                    sRecvBuffer.Initialize();
                    recvLen = socket.Receive(sRecvBuffer, sRecvBuffer.Length, 0);

                    if (recvLen <= 0)
                    {
                        socket.Close();
                        return ret;
                    }
                    byte[] RecvBytes = new byte[recvLen - headLen];
                    Array.Copy(sRecvBuffer, headLen, RecvBytes, 0, recvLen - headLen);
                    byte[] headBytes = new byte[headLen];
                    Array.Copy(sRecvBuffer, headBytes, headLen);
                    //���
                    ret = TransResult.E_UNPACKET_FAIL;
                    FrontBytes = headBytes;
                    HandleFrontBytes(headBytes);//���ݱ���ͷ���ж��Ƿ�Ҫ������Կ
                    RecvPackage.ParseBuffer(RecvBytes, SendPackage.ExistValue(0));

                    //��¼ԭʼ������־
                    byte[] logRecv = new byte[recvLen];
                    Array.Copy(sRecvBuffer, logRecv, recvLen);
                    //CLog.LogPackage(logRecv, RecvPackage, CLog.LogType.Recv);
                    CLog.Info(CLog.GetLog(logRecv, RecvPackage, this, CLog.LogType.Recv));
                    bool nRet = UnPackFix();
                    if (!mInvokeSetResult)
                        throw new Exception("should invoke SetRespInfo() in UnPackFix()");
                    mInvokeSetResult = false;
                    if (nRet)
                    {
                        ret = TransResult.E_SUCC;
                    }
                    else
                    {
                        ret = TransResult.E_HOST_FAIL;
                    }
                }
                else
                {
                    ret = TransResult.E_SUCC;
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(this.GetType().Name, ex);
            }
            finally
            {
                if (socket != null && socket.Connected)
                    socket.Close();
            }
            return ret;
        }

        public byte[] FrontBytes;
        protected abstract void HandleFrontBytes(byte[] headBytes);

        protected void SetRespInfo(string returnCode, string returnMean, string returnMessage)
        {
            ReturnCode = returnCode;
            ReturnMean = returnMean;
            ReturnMessage = returnMessage;
            mInvokeSetResult = true;
        }

        protected virtual void OnBeforeTrans() { }
        //�޸ĸ÷����Ļ�ȡ��Χ��internal ��Ϊ public
        public TransResult Communicate()
        {
            lock (sLock)
            {
                ActivityManager.SystemStatus |= AppStatus.OnCommunicating;
                OnBeforeTrans();
                if (Result == TransResult.E_INVALID)
                {
                    ActivityManager.SystemStatus &= ~AppStatus.OnCommunicating;
                    return Result;
                }
                Result = transact();
                ActivityManager.SystemStatus &= ~AppStatus.OnCommunicating;
            }

            handleResult();
            return Result;
        }

        private void handleResult()
        {
            try
            {
                if (RealEnv)
                {
                    switch (Result)
                    {
                        case TransResult.E_SUCC:
                            try
                            {
                                OnSucc();
                            }
                            catch (System.Exception ex)
                            {
                                Log.Error(this.GetType().Name, ex);
                                SetResult(TransResult.E_UNPACKET_FAIL);
                            }
                            break;
                        case TransResult.E_HOST_FAIL:
                            OnHostFail(ReturnCode, ReturnMessage);
                            break;
                        case TransResult.E_RECV_FAIL:
                            OnRecvFail();
                            break;
                        default:
                            OnOtherResult();
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(this.GetType().Name, ex);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(this.GetType().Name + ":" + Result.ToString());
            if (!RealEnv)
            {
                Log.Info(sb.ToString());
                return;
            }
            if (Result == TransResult.E_INVALID)
            {

            }
            else if (Result == TransResult.E_SUCC)
            {
                Log.Info(sb.ToString());
                sb.AppendLine();
                CLog.Info(sb.ToString());
            }
            else if (Result == TransResult.E_HOST_FAIL)
            {
                sb.Append(",ReturnCode:" + ReturnCode + ",ReturnMessage:" + ReturnMean);
                Log.Warn(sb.ToString());
                sb.AppendLine();
                CLog.Warn(sb.ToString());
            }
            else
            {
                Log.Warn(sb.ToString());
                sb.AppendLine();
                CLog.Warn(sb.ToString());
            }
        }

        protected virtual void OnRecvFail()
        {

        }

        protected virtual void OnHostFail(string returnCode, string returnMessage)
        {

        }

        protected virtual void OnSucc()
        {

        }

        protected virtual void OnOtherResult()
        {

        }

        #region save or restore business data
        private static Bundle businessBundle = Activity.businessBundle;

        protected static void SaveString(string key, string value)
        {
            businessBundle.PutString(key, value);
        }

        protected static void SaveInt(string key, int value)
        {
            businessBundle.PutInt(key, value);
        }

        protected static void SaveDouble(string key, double value)
        {
            businessBundle.PutDouble(key, value);
        }

        protected static void SaveBoolean(string key, Boolean value)
        {
            businessBundle.PutBoolean(key, value);
        }

        protected static void SaveStringArray(string key, string[] value)
        {
            businessBundle.PutStringArray(key, value);
        }

        protected static void SaveIntArray(string key, int[] value)
        {
            businessBundle.PutIntArray(key, value);
        }

        protected static void Save(string key, object value)
        {
            businessBundle.Put(key, value);
        }

        protected static string RestoreString(string key)
        {
            return businessBundle.GetString(key);
        }

        protected static int RestoreInt(string key)
        {
            return businessBundle.GetInt(key);
        }

        protected static double RestoreDouble(string key)
        {
            return businessBundle.GetDouble(key);
        }


        protected static Boolean RestoreBoolean(string key)
        {
            return businessBundle.GetBoolean(key);
        }

        protected static string[] RestoreStringArray(string key)
        {
            return businessBundle.GetStringArray(key);
        }

        protected static int[] RestoreIntArray(string key)
        {
            return businessBundle.GetIntArray(key);
        }

        protected static object Restore(string key)
        {
            return businessBundle.Get(key);
        }
        #endregion

        #region save or restore global data
        internal static Bundle globalBundle = Activity.globalBundle;

        protected static void SaveStringGlobal(string key, string value)
        {
            globalBundle.PutString(key, value);
        }

        protected static void SaveIntGlobal(string key, int value)
        {
            globalBundle.PutInt(key, value);
        }

        protected static void SaveDoubleGlobal(string key, double value)
        {
            globalBundle.PutDouble(key, value);
        }

        protected static void SaveBooleanGlobal(string key, Boolean value)
        {
            globalBundle.PutBoolean(key, value);
        }

        protected static void SaveStringArrayGlobal(string key, string[] value)
        {
            globalBundle.PutStringArray(key, value);
        }

        protected static void SaveIntArrayGlobal(string key, int[] value)
        {
            globalBundle.PutIntArray(key, value);
        }

        protected static void SaveGlobal(string key, object value)
        {
            globalBundle.Put(key, value);
        }

        protected static string RestoreStringGlobal(string key)
        {
            return globalBundle.GetString(key);
        }

        protected static int RestoreIntGlobal(string key)
        {
            return globalBundle.GetInt(key);
        }

        protected static double RestoreDoubleGlobal(string key)
        {
            return globalBundle.GetDouble(key);
        }


        protected static Boolean RestoreBooleanGlobal(string key)
        {
            return globalBundle.GetBoolean(key);
        }

        protected static string[] RestoreStringArrayGlobal(string key)
        {
            return globalBundle.GetStringArray(key);
        }

        protected static int[] RestoreIntArrayGlobal(string key)
        {
            return globalBundle.GetIntArray(key);
        }

        protected static object RestoreGlobal(string key)
        {
            return globalBundle.Get(key);
        }
        #endregion
    }
}
