using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.Creditcard 
{
    class CreditcardInputCardNoAgainDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Confirm_Click;
            GetElementById("Return").Click += Return_Click;
            GetElementById("InputText").GotFocus += InputText_OnFocus;
        }

        public void InputText_OnFocus(object sender, HtmlElementEventArgs e)
        {
            GetElementById("InputText").Focus();
            GetElementById("ErrMsg").Style = "display:none";
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("������");
        }

        void Confirm_Click(object sender, HtmlElementEventArgs e)
        {
            string account = GetElementById("InputText").GetAttribute("value");
            if (account.Length >= 16 && account.Length <= 19)
            {
                if ((GetBusinessEntity() as QMEntity).CreditcardNum == account)
                {
                    StartActivity("���ÿ��������뻹����");
                }
                else
                {
                    StartActivity("���ÿ��������뿨�Ų�һ��");
                }
            }
            else
            {
                GetElementById("InputText").SetAttribute("value", "");
                GetElementById("ErrMsg").Style = "display:block";
            }
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("ErrMsg").Style = "display:none";
            InputNumber("InputText", keyCode);
            switch (keyCode)
            {
                case Keys.Enter:
                    GetElementById("Ok").InvokeMember("Click");
                    break;
                case Keys.Escape:
                    GetElementById("Return").InvokeMember("Click");
                    break;
            }
        }

    }
}
