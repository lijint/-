using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using System.Windows.Forms;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayWritePowerCardFailDeal : Activity
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
