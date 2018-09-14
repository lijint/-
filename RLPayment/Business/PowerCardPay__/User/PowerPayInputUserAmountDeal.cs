using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using YAPayment.Entity;
using Landi.FrameWorks;

namespace YAPayment.Business.PowerCardPay.User
{
    class PowerPayInputUserAmountDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("UserAmount").InnerText = (GetBusinessEntity() as PowerEntity).UserPayAmount.ToString("#######0.00");
            GetElementById("Back").Click += new HtmlElementEventHandler(Back_Click);
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string amount = GetElementById("Amount").GetAttribute("value").Trim();
            if (amount.Length == 0 || double.Parse(amount) == 0)
            {
                GetElementById("ErrMsg").InnerText = "�ɷѽ���Ϊ0!";
                GetElementById("ErrMsg").Style = "display:block";
                GetElementById("Amount").SetAttribute("value", "");
                return;
            }
            CommonData.Amount = double.Parse(amount);

            //�û��ɷ� �ɷѽ�������ڵ���Ӧ�ɽ��
            PowerEntity pe = (GetBusinessEntity() as PowerEntity);
            if (CommonData.Amount < pe.UserPayAmount)
            {
                GetElementById("ErrMsg").InnerText = "�ɷѽ�������ڵ��ڱ���Ӧ�����" + pe.UserPayAmount.ToString("#######0.00");
                GetElementById("ErrMsg").Style = "display:block";
                GetElementById("Amount").SetAttribute("value", "");
                return;
            }

            StartActivity("����֧���û����ȷ��");
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("����֧���û��˵���Ϣ");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("ErrMsg").Style = "display:none";
            InputAmount("Amount", keyCode);
            base.OnKeyDown(keyCode);
        }
    }
}
