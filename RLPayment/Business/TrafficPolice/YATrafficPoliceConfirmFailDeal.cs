using System.Collections;
using System.Windows.Forms;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Package;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceConfirmFailDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            GetElementById("Return").Click += Return_Click;
            if (!ReceiptPrinter.ExistError())
            {
                ArrayList al = new YAPaymentPay().GetTPReceipt();
                al.Add("   ");
                al.Add("   �����ۿ�ɹ�������ȷ��ʧ�ܣ�");
                al.Add("   ��ʹ�������������ܽ��к�����");
                al.Add("   �����ĵȴ�24Сʱ��ϵͳ���Զ�����");
                PrintReceipt(al);
            }
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void HandleResult(Result result)
        {

        }
    }
}
