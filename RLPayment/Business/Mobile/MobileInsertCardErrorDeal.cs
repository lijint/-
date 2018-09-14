using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Mobile
{
    class MobileInsertCardErrorDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Retry").Click += Retry_click;
            GetElementById("Return").Click += Return_Click;
        }

        public void Retry_click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("�ֻ���ֵ�������п�");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
        
       
    }
}
