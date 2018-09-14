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
using YAPayment.Package.EMV.YAPaymentEMV;
using YAPayment.Entity;

namespace YAPayment.Package
{
    class YAPaymentPay: ChinaUnionPay
    {
        private static bool sHasSignIn;
        public static bool HasSignIn
        {
            get { return sHasSignIn; }
            protected set { sHasSignIn = value; }
        }

        protected YAEntity PayEntity
        {
            get { return BaseBusinessEntity as YAEntity; }
        }

        public ArrayList GetReceipt()
        {
            //string sTitle = "***��������\"��ʯ������\"��������ƾ��***";
            //string sTitle = "��������\"�Ű�������ҵ�ɷ�ϵͳ\"��������ƾ��";
            string sTitle = "***\"�ǻ��Ű�  ��о����\"���������ն˽���***";
            int splitStringLen = Encoding.Default.GetByteCount("------------------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = "";
            if(iLeftLength > 0)
                sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);
            Lprint.Add("  ");
            Lprint.Add("   �������� :  �˵���֧��");
            Lprint.Add("   �̻���� :  " + GetMerchantNo());
            Lprint.Add("   �ն˱�� :  " + GetTerminalNo());
            Lprint.Add("   ���п��� :  " + Utility.GetPrintCardNo(CommonData.BankCardNum));
            Lprint.Add("   ����/ʱ��:  " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Lprint.Add("   �� �� �� :  " + PayEntity.PayReferenceNo);
            Lprint.Add("   ƾ ֤ �� :  " + PayEntity.PayTraceNo);
            Lprint.Add("   �� �� �� :  " + GetBatchNo());
            Lprint.Add("   ---------------------------------------");
            Lprint.Add("   ���׽�� :  " + CommonData.Amount.ToString("########0.00") + "Ԫ");
            Lprint.Add("   �� �� �� :  " + PayEntity.UserID);
            return Lprint;
        }

        public ArrayList GetTPReceipt()
        {
            string sTitle = "***\"�ǻ��Ű�  ��о����\"���������ն˽���***";
            int splitStringLen = Encoding.Default.GetByteCount("------------------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = "";
            if (iLeftLength > 0)
                sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);
            Lprint.Add("  ");
            Lprint.Add("   �������� :  ������û");
            Lprint.Add("   �̻���� :  " + GetMerchantNo());
            Lprint.Add("   �ն˱�� :  " + GetTerminalNo());
            Lprint.Add("   ���п��� :  " + Utility.GetPrintCardNo(CommonData.BankCardNum));
            Lprint.Add("   ����/ʱ��:  " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Lprint.Add("   �� �� �� :  " + PayEntity.PayReferenceNo);
            Lprint.Add("   ƾ ֤ �� :  " + PayEntity.PayTraceNo);
            Lprint.Add("   �� �� �� :  " + GetBatchNo());
            Lprint.Add("   ---------------------------------------");
            Lprint.Add("   �ɷ���ˮ�� :  " + PayEntity.TPPayFlowNo);
            Lprint.Add("   ���׽�� :  " + CommonData.Amount.ToString("########0.00") + "Ԫ");
            Lprint.Add("   �������� :  " + PayEntity.TPDecisionNo);
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

        public YAPaymentPay() { }

        public YAPaymentPay(PackageBase pb)
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
                            //EnqueueWork(new CSignIn_YAPaymentPay());
                        }
                        break;
                    case 0x04://���¹�Կ
                        CYADownCA ca = new CYADownCA();
                        ca.DownPublishCA();
                        break;
                    case 0x05://����IC������
                        CYADownAID aid = new CYADownAID();
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
            get { return YAEntity.SECTION_NAME; }
        }

        protected override void OnHostFail(string returnCode, string returnMessage)
        {
            //���ǩ����һֱ����A0|99�����ܴ�����ѭ��
            //���������н���
            if (returnCode == "99" || returnCode == "A0")
            {
                HasSignIn = false;
                //EnqueueWork(new CSignIn_YAPaymentPay());
            }
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

        //public ResponseData GetResponseData()
        //{
        //    ResponseData rd = new ResponseData();
        //    rd.BankCardNo = CommonData.BankCardNum;
        //    rd.Amount = CommonData.Amount.ToString("########0.00");
        //    rd.BatchNo = GetBatchNo();
        //    rd.RefNo = PayEntity.PayReferenceNo;
        //    rd.TraceNo = PayEntity.PayTraceNo;
        //    rd.PayFlowNo = PayEntity.PayFlowNo;
        //    rd.UserID = PayEntity.UserID;

        //    return rd;
        //}
    }

    [SerializableAttribute]
    class ResponseData
    {
        public string BankCardNo = "";
        public string Amount = "";
        public string BatchNo = "";
        public string TraceNo = "";
        public string RefNo = "";
        public string PayFlowNo = "";
        public string UserID = "";
    }


    [SerializableAttribute]
    class ConfirmFailInfo
    {
        public string BankCardNo;
        public string PayTraceNo;
        public string PayReferenceNo;
        public DateTime TransDate = DateTime.Now;
        public override string ToString()
        {
            StringBuilder temp = new StringBuilder();
            temp.AppendLine("ʱ�䣺" + TransDate.ToString("yyyy/MM/dd HH:mm:ss"));
            temp.AppendLine("���ţ�" + BankCardNo);
            temp.AppendLine("��ˮ�ţ�" + PayTraceNo);
            temp.AppendLine("ϵͳ�ο��ţ�" + PayReferenceNo);
            temp.AppendLine("=====================================");

            return temp.ToString();
        }
    }
}
