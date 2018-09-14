using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using YAPayment.Package;
using Landi.FrameWorks.HardWare;
using System.Collections;

namespace YAPayment.Business.PetroPay
{
    class PetroPayConfirmFailDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
            if (!ReceiptPrinter.ExistError())
            {
                ArrayList al = new YAPaymentPay().GetReceipt();
                al.Add("   ");
                al.Add("   �����ɹ�����̨�����쳣������ʧ�ܣ��벻Ҫ�����ɷ�");
                al.Add("   �ȴ�ϵͳ�Զ�������������4:00�Ժ����в鿴�ɷ����");
                PrintReceipt(al);
            }
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void HandleResult(Result result)
        {
            
        }
    }
}
