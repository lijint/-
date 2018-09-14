using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayPowerPayMenuDeal : Activity
    {
        private bool _changeFlag = false;
        protected override void OnEnter()
        {
            GetElementById("Card").Click += Pay_Click;
            GetElementById("Card1").Click += Change_Click;
            GetElementById("User").Click += WriteCard_Click;
            GetElementById("ReWrite").Click += ReWrite_Click;
            GetElementById("Return").Click += Return_Click;
            GetElementById("ShowMsg").InnerText = ConfigFile.ReadConfigAndCreate("Power", "UserPayMessage", "�����������Ű���������������û��ɷ�!");
        }

        private void Change_Click(object sender, HtmlElementEventArgs e)
        {
            _changeFlag = true;
            GetElementById("first").Style = GetElementById("first").Style.Replace("block", "none");
            GetElementById("second").Style = GetElementById("second").Style.Replace("none", "block");
        }

        void ReWrite_Click(object sender, HtmlElementEventArgs e)
        {
            (GetBusinessEntity() as PowerEntity).PowerBusiness = 1;
            StartActivity("����֧����д��������ϸ��Ϣ");
        }

        private void Pay_Click(object sender, HtmlElementEventArgs e)
        {
            (GetBusinessEntity() as PowerEntity).PowerBusiness = 1;
            StartActivity("����֧������翨");
        }

        private void WriteCard_Click(object sender, HtmlElementEventArgs e)
        {
            (GetBusinessEntity() as PowerEntity).PowerBusiness = 2;
            StartActivity("����֧�������û���");
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
                        if(_changeFlag)
                            Pay_Click(null,null);
                        else
                        WriteCard_Click(null, null);
                    }
                    break;
                case Keys.D2:
                    {
                        if (_changeFlag)
                            ReWrite_Click(null,null);
                        else
                        Change_Click(null, null);
                    }
                    break;
                //case Keys.D3:
                //    ReWrite_Click(null, null);
                //    break;
            }
        }
    }
}
