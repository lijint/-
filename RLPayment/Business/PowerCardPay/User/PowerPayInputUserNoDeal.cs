using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.PowerCardPay.User
{
    class PowerPayInputUserNoDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;

#if DEBUG
            GetElementById("UserNo").InnerText = "2030082827";
#endif
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string DecisionNo = GetElementById("UserNo").GetAttribute("value").Trim();

            if (DecisionNo.Length == 0)
            {
                GetElementById("ErrMsg").InnerText = "�û��Ų���Ϊ��";
                GetElementById("ErrMsg").Style = "display:block";
                return;
            }
            (GetBusinessEntity() as PowerEntity).DBNo = DecisionNo;//"4446318730";//DecisionNo;//Test
            StartActivity("����֧���û��˵���ѯ");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("ErrMsg").Style = "display:none";
            InputNumber("UserNo", keyCode);
            base.OnKeyDown(keyCode);
        }
    }
}
