using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayWritePowerCardFailDeal : Activity
    {
        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            if (ReceiptPrinter.ExistError())
                StartActivity("����֧���������");
            else
                StartActivity("����֧�����׳ɹ��Ƿ��ӡ");
        }

        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
        }
    }
}
