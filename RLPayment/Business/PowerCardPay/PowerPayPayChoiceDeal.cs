using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayPayChoiceDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Flush").Click += Flush_Click;
            GetElementById("Hand").Click += Hand_Click;
            GetElementById("Back").Click += Back_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("����֧�����ȷ��");
        }

        private void Flush_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("����֧�������Ӧ���п�����");
        }

        private void Hand_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("����֧���˳��翨");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
