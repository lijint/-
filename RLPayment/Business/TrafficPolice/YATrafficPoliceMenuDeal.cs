using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceMenuDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Pay").Click += Pay_Click;
            GetElementById("Confirm").Click += Confirm_Click;
            GetElementById("Inquery").Click += Inquery_Click;
            
            GetElementById("Return").Click += Return_Click;
        }

        private void Inquery_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("�Ű�������û�������");
        }

        private void Pay_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("�Ű�������û�����������");
        }

        private void Confirm_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("�Ű�������û����ɷ���ˮ��");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            base.OnKeyDown(keyCode);

            switch (keyCode)
            {
                case Keys.D1:
                {
                    Pay_Click(null, null);
                }
                    break;
                case Keys.D2:
                {
                    Confirm_Click(null, null);
                }
                    break;
                case Keys.D3:
                {
                    Inquery_Click(null, null);
                }
                    break;
            }
        }
    }
}
