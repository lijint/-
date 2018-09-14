using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.PowerCardPay.User
{
    class PowerPayConfirmPayUserInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("PayAMount").InnerText = CommonData.Amount.ToString("########0.00");
            //GetElementById("ShowMsg").InnerText = ConfigFile.ReadConfigAndCreate("Power", "UserPayMessage", "�����������Ű���������������û��ɷ�!");
            GetElementById("Back").Click += Back_Click;
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("����֧���û��˵���Ϣ");
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("����֧���������п�");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
