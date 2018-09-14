using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPaySuccessDeal : Activity
    {
        void Print_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("����֧�����ڴ�ӡ");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnEnter()
        {
            GetElementById("Print").Click += Print_Click;
            GetElementById("Return").Click += Return_Click;
        }
    }
}
