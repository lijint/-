using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Landi.FrameWorks;
using Landi.FrameWorks.ChinaUnion;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using YAPayment.Entity;
using YAPayment.Package.EMV.PowerEMV;

namespace YAPayment.Package
{
    class PowerPay: ChinaUnionPay
    {
        private static bool sHasSignIn;
        public static bool HasSignIn
        {
            get { return sHasSignIn; }
            protected set { sHasSignIn = value; }
        }

        protected PowerEntity PayEntity
        {
            get { return BaseBusinessEntity as PowerEntity; }
        }


        public ArrayList GetUserReceipt()
        {
            string sTitle = "��������\"�Ű�������ҵ�ɷ�ϵͳ\"��������ƾ��";
            int splitStringLen = Encoding.Default.GetByteCount("------------------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = "";
            if(iLeftLength > 0)
                sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);
            Lprint.Add("  ");
            Lprint.Add("   �������� :  �û��Žɷ�");
            Lprint.Add("   �̻���� :  " + GetMerchantNo());
            Lprint.Add("   �ն˱�� :  " + GetTerminalNo());
            Lprint.Add("   ���п��� :  " + Utility.GetPrintCardNo(CommonData.BankCardNum));
            Lprint.Add("   ����/ʱ��:  " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Lprint.Add("   �� �� �� :  " + PayEntity.PayReferenceNo);
            Lprint.Add("   ƾ ֤ �� :  " + PayEntity.PayTraceNo);
            Lprint.Add("   �� �� �� :  " + GetBatchNo());
            Lprint.Add("   ---------------------------------------");
            Lprint.Add("   ���׽�� :  " + CommonData.Amount.ToString("########0.00") + "Ԫ");
            Lprint.Add("   �� �� �� :  " + PayEntity.DBNo);
            return Lprint;
        }

        public ArrayList GetCardReceipt(bool containTail=false)
        {
            string sTitle = "��������\"�Ű�������ҵ�ɷ�ϵͳ\"��������ƾ��";
            int splitStringLen = Encoding.Default.GetByteCount("------------------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = "";
            if (iLeftLength > 0)
                sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);
            Lprint.Add("  ");
            Lprint.Add("   �������� :  ���翨�ɷ�");
            Lprint.Add("   �̻���� :  " + GetMerchantNo());
            Lprint.Add("   �ն˱�� :  " + GetTerminalNo());
            Lprint.Add("   ���п��� :  " + Utility.GetPrintCardNo(CommonData.BankCardNum));
            Lprint.Add("   ����/ʱ��:  " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Lprint.Add("   �� �� �� :  " + PayEntity.PayReferenceNo);
            Lprint.Add("   ƾ ֤ �� :  " + PayEntity.PayTraceNo);
            Lprint.Add("   �� �� �� :  " + GetBatchNo());
            Lprint.Add("   �� �� �� :  " + PayEntity.PowerCardNo);
            Lprint.Add("   �����̻������� :  " + PayEntity.PowerPayConfirmCode);
            Lprint.Add("   ---------------------------------------");
            Lprint.Add("   ���׽�� :  " + CommonData.Amount.ToString("########0.00") + "Ԫ");
            Lprint.Add("   �û����� :  " + PayEntity.UserName);
            Lprint.Add("   �翨���� :  " + PayEntity.PowerCardNo);
            if (containTail)
            {
                Lprint.Add("  ---------------------------------------");
                Lprint.Add(" ��ע�⡿:ȷ�ϳ�ʱ������ϵ95534�������˿лл������ϣ�");
            }
            return Lprint;
        }

        protected byte[] Get48TLVBytes()
        {
            byte[] tmp = RecvPackage.GetArrayData(48);
            int len = int.Parse(Encoding.Default.GetString(tmp, 72, 3));
            byte[] tmp1 = new byte[len];
            Array.Copy(tmp, 75, tmp1, 0, len);
            return tmp1;
        }

        public PowerPay() { }

        public PowerPay(PackageBase pb)
            : base(pb) { }

        /// <summary>
        /// ���ݱ���ͷ���������»����ز���
        /// </summary>
        /// <param name="headBytes"></param>
        protected override void HandleFrontBytes(byte[] headBytes)
        {
            byte tmp = headBytes[9];
            try
            {
                switch (tmp & 0x0F)
                {
                    case 0x03:
                        {
                            HasSignIn = false;
                            //EnqueueWork(new CSignIn_PowerPay());
                        }
                        break;
                    case 0x04://���¹�Կ
                        CPowerDownCA ca = new CPowerDownCA();
                        ca.DownPublishCA();
                        break;
                    case 0x05://����IC������
                        CPowerDownAID aid = new CPowerDownAID();
                        aid.DownAID();
                        break;
                }
            }
            catch(Exception ex)
            {
                Log.Warn("HandleFrontBytesy�쳣", ex);
            }
        }

        protected override string SectionName
        {
            get { return PowerEntity.SECTION_NAME; }
        }

        protected override void OnHostFail(string returnCode, string returnMessage)
        {
            //���ǩ����һֱ����A0|99�����ܴ�����ѭ��
            //���������н���
            if (returnCode == "99" || returnCode == "A0")
            {
                HasSignIn = false;
                //EnqueueWork(new CSignIn_PowerPay());
            }
        }

        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <returns></returns>
        protected string GetBranchNo()
        {
            string BranchNo = ReadIniFile("BranchNo");
            if (BranchNo == "")
            {
                throw new Exception("��δ����������");
            }
            return BranchNo;
        }

        /// <summary>
        /// ��ȡ����Ա���
        /// </summary>
        /// <returns></returns>
        protected string GetOperatorNo()
        {
            string OperatorNo = ReadIniFile("OperatorNo");
            if (OperatorNo == "")
            {
                throw new Exception("��δ���ò���Ա���");
            }
            return OperatorNo;
        }

        public static bool CreateFile(string filname, byte[] szBuffer, int len)
        {
            bool bRet = true;
            try
            {
                if (File.Exists(filname))
                {
                    File.Delete(filname);
                }
                FileStream fs = new FileStream(filname, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(szBuffer, 0, len);
                bw.Close();
            }
            catch
            {
                Log.Warn("�����ļ�ʧ��");
                bRet = false;
            }
            return bRet;
        }

        protected string GetFieldString(byte[] source, int startIndex, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(source, startIndex, result, 0, length);
            return Encoding.Default.GetString(result);
        }

        protected double GetFieldDouble(byte[] source, int startIndex, int length)
        {
            string value = GetFieldString(source, startIndex, length);
            double temp = Convert.ToDouble(value) / 100;
            return temp;
        }
    }
}
