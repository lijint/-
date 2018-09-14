using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using Landi.FrameWorks.HardWare;
using YAPayment.Package;
using System.Collections;

namespace YAPayment.Business.PowerCardPay.User
{
    class PowerPayUserConfirmFailDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
            if (!ReceiptPrinter.ExistError())
            {
                ArrayList al = new PowerPay().GetReceipt();
                al.Add("   ");
                al.Add("   �ɷ�ȷ��δ�ɹ���48Сʱ��ϵͳ���Զ�����");
                al.Add("   �����ĵȴ�����Ҫ�ظ��ɷ�");
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
