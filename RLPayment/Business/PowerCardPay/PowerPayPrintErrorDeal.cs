using System.Windows.Forms;
using Landi.FrameWorks;

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
            GetElementById("Ok").Click += Ok_Click;
        }
    }
}
