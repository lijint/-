using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceInputDecisionNoDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string DecisionNo = GetElementById("DecisionNo").GetAttribute("value").Trim();

            if (!DecisionNo.StartsWith("51"))
            {
                GetElementById("ErrMsg").InnerText = "�������ű�������51��ͷ!";
                GetElementById("ErrMsg").Style = "display:block";
                return;
            }

            if (DecisionNo.Length != 15)
            {
                GetElementById("ErrMsg").InnerText = "�������ų��Ȳ���15λ!";
                GetElementById("ErrMsg").Style = "display:block";
                return;
            }

            (GetBusinessEntity() as YAEntity).TPDecisionNo = DecisionNo;
            StartActivity("�Ű�������ûΥ�²�ѯ");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            GetElementById("ErrMsg").Style = "display:none";
            InputNumber("DecisionNo", keyCode);
            base.OnKeyDown(keyCode);
        }
    }
}
