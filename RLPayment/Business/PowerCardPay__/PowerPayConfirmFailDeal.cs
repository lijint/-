using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using Landi.FrameWorks.HardWare;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayConfirmFailDeal : Activity
    {
        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            if (ReceiptPrinter.ExistError())
                StartActivity("����֧���������");
            else
                StartActivity("����֧�����׳ɹ��Ƿ��ӡ");
        }

        protected override void OnEnter()
        {
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
        }
    }
}
