using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayErrorDeal : MessageActivity
    {
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("������");
        }

        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("������");
        }

        protected override void DoMessage(object message)
        {
            GetElementById("Message").InnerText = (string)message;
            if (IsBack)
            {
                GetElementById("Ok").Style = "display:block;";
                GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            }
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }
    }
}
