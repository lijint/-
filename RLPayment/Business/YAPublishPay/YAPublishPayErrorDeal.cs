using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.YAPublishPay
{
    class YAPublishPayErrorDeal : MessageActivity
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
                GetElementById("Ok").Click += Ok_Click;
            }
            GetElementById("Return").Click += Return_Click;

        }
    }
}
