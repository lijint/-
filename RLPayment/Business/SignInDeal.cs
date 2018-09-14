using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package;
using Landi.FrameWorks.HardWare;
using YAPayment.Business.Creditcard;
using YAPayment.Business.Mobile;

namespace YAPayment.Business
{
    class SignInDeal:Activity
    {
        protected override void OnEnter()
        {
            bool succ = false;
            string businessName = GetBusinessName();
            if (businessName == "PowerPay")
            {
                SyncTransaction(new CSignIn_PowerPay());
                succ = PowerPay.HasSignIn;
            }
            else if (businessName == "Car")
            {
                SyncTransaction(new CSignIn_CarPay());
                succ = CarPay.HasSignIn;
            }
            else 
            {
                SyncTransaction(new CSignIn_YAPaymentPay());
                succ = YAPaymentPay.HasSignIn;
                QMPay.HasSignIn = YAPaymentPay.HasSignIn;
            }
   
            if (!succ)
            {
                ShowMessageAndGotoMain("ǩ��ʧ�ܣ���ҵ����ʱ����ʹ��");
            }
            else
            {
                switch (GetBusinessName())
                {
                    case "CreditCard":
                        StartActivity(ReceiptPrinter.CheckedByManager() ? "���ÿ�������ܰ��ʾ" : "���ÿ���ӡ�����ϼ���");
                        break;
                    case "Mobile":
                        StartActivity(ReceiptPrinter.CheckedByManager() ? "�ֻ���ֵ������" : "�ֻ���ֵ��ӡ�����ϼ���");
                        break;
                    case "YAPublishPay":
                        StartActivity(ReceiptPrinter.CheckedByManager() ? "�Ű�֧�������û���" : "�Ű�֧����ӡ�����ϼ���");
                        break;
                    case "PowerPay":
                        StartActivity(ReceiptPrinter.CheckedByManager() ? "����֧���˵�" : "����֧����ӡ�����ϼ���");
                        break;
                    case "YATrafficPolice":
                        StartActivity(ReceiptPrinter.CheckedByManager() ? "�Ű�������û�˵�" : "�Ű�������û��ӡ�����ϼ���");
                        break;
                    case "Car":
                        StartActivity(ReceiptPrinter.CheckedByManager() ? "��ƱԤ��������" : "�Ű�������û��ӡ�����ϼ���");
                        break;
                }
            }
        }
    }
}
