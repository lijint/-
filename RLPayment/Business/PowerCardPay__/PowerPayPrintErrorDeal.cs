using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayPrintErrorDeal : Activity
    {
        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("����֧���������");
        }

        protected override void OnEnter()
        {
            GetElementById("Message").InnerText = "ƾ����ӡʧ��";
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
        }
    }
}
