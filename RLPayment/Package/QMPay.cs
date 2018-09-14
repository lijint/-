using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Xml;
using Landi.FrameWorks.ChinaUnion;
using System.Collections;
using System.IO;
using YAPayment.Entity;
using YAPayment.Package.EMV.YAPaymentEMV;

namespace YAPayment.Package
{
    class QMPay: ChinaUnionPay
    {
        private static bool sHasSignIn;
        public static bool HasSignIn
        {
            get { return sHasSignIn; }
            set { sHasSignIn = value; }
        }

        protected QMEntity PayEntity
        {
            get { return BaseBusinessEntity as QMEntity; }
        }

        public ArrayList GetCreditCardReceipt()
        {
            string sTitle = "***��������\"ȫ��\"��������ƾ��***";
            int splitStringLen = Encoding.Default.GetByteCount("---------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);

            Lprint.Add("  ");
            Lprint.Add("   �������� :  ���ÿ�����");
            Lprint.Add("   �̻���� :  " + GetMerchantNo());
            Lprint.Add("   �ն˱�� :  " + GetTerminalNo());
            Lprint.Add("   ��ǿ��� :  " + Utility.GetPrintCardNo(CommonData.BankCardNum));
            Lprint.Add("   ���ÿ��� :  " + Utility.GetPrintCardNo(PayEntity.CreditcardNum));

            Lprint.Add("   ����/ʱ��:  " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Lprint.Add("   �� �� �� :  " + PayEntity.PayReferenceNo);
            Lprint.Add("   ƾ ֤ �� :  " + PayEntity.PayTraceNo);
            Lprint.Add("   �� �� �� :  " + GetBatchNo());
            Lprint.Add("   ---------------------------------------------");
            Lprint.Add("   ������ :  " + CommonData.Amount.ToString("########0.00") + "Ԫ");
            Lprint.Add("   �� �� �� :  " + PayEntity.Fee.ToString("########0.00") + "Ԫ");
            Lprint.Add("   ���׽�� :  " + PayEntity.TotalAmount.ToString("########0.00") + "Ԫ");
            Lprint.Add("   ");
            Lprint.Add("           *** ȫ��,ȫ����������� ***");
            Lprint.Add("             �ͷ��绰: 95534");
            return Lprint;
        }

        public ArrayList GetMobileReceipt()
        {
            string sTitle = "***��������\"ȫ��\"��������ƾ��***";
            int splitStringLen = Encoding.Default.GetByteCount("---------------------------------------");
            ArrayList Lprint = new ArrayList();

            int iLeftLength = splitStringLen / 2 - Encoding.Default.GetByteCount(sTitle) / 2;
            string sPadLeft = ("").PadLeft(iLeftLength, ' ');
            Lprint.Add("  " + sPadLeft + sTitle);

            Lprint.Add("  ");
            Lprint.Add("   �������� :  �ֻ�����ֱ��");
            Lprint.Add("   �̻���� :  " + GetMerchantNo());
            Lprint.Add("   �ն˱�� :  " + GetTerminalNo());
            Lprint.Add("   ���п��� :  " + Utility.GetPrintCardNo(CommonData.BankCardNum));

            Lprint.Add("   ����/ʱ��:  " + System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            Lprint.Add("   �� �� �� :  " + PayEntity.PayReferenceNo);
            Lprint.Add("   ƾ ֤ �� :  " + PayEntity.PayTraceNo);
            Lprint.Add("   �� �� �� :  " + GetBatchNo());
            Lprint.Add("   ---------------------------------------------");
            Lprint.Add("   ���׽�� :  " + CommonData.Amount.ToString("########0.00") + "Ԫ");
            Lprint.Add("   �ֻ����� :  " + PayEntity.PhoneNo);
            Lprint.Add("   (ֱ�佻�׿��������ӳ�,�糤ʱ��δ�ܵ���,����ѯ�ͷ�");
            Lprint.Add("   ����)");
            Lprint.Add("   ");
            Lprint.Add("           *** ȫ��,ȫ����������� ***");
            Lprint.Add("             �ͷ��绰: 95534");
            return Lprint;
        }

        public QMPay() { }

        public QMPay(PackageBase pb)
            : base(pb) { }

        protected override void HandleFrontBytes(byte[] headBytes)
        {
            byte tmp = headBytes[9];
            switch (tmp & 0x0F)
            {
                case 0x03:
                    EnqueueWork(new CSignIn_YAPaymentPay());
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

        protected override string SectionName
        {
            get { return QMEntity.SECTION_NAME; }
        }

        protected override void OnHostFail(string returnCode, string returnMessage)
        {
            if (returnCode == "99" || returnCode == "A0")
            {
                HasSignIn = false;
                EnqueueWork(new CSignIn_YAPaymentPay());
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
            catch(Exception ex)
            {
                Log.Warn("[QMPay][CreateFile]�����ļ�ʧ��", ex);
                bRet = false;
            }
            return bRet;
        }
    }
}
