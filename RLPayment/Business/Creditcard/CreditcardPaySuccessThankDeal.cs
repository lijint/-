using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Creditcard  
{
    /// <summary>
    /// ��ӡ���
    /// </summary>
    class CreditcardPaySuccessThankDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Return").Click += Return_Click;
        }
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("������");
        }
        

    }
}
