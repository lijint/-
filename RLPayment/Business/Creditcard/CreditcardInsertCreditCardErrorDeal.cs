using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Creditcard
{
    class CreditcardInsertCreditCardErrorDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Retry").Click += Retry_click;
            GetElementById("Return").Click += Return_Click;
        }

        public void Retry_click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("���ÿ�����������ÿ�");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("������");
        }
    }
}
