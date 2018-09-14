using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.YAPublishPay
{
    class YAPublishPayConfirmPayInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("PayAMount").InnerText = CommonData.Amount.ToString("########0.00");
  
            GetElementById("Back").Click += Back_Click;
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            switch ((GetBusinessEntity() as YAEntity).PublishPayType)
            {
                case YaPublishPayType.Gas:
                    StartActivity("�Ű������˵���Ϣ");
                    break;
                case YaPublishPayType.Water:
                    StartActivity("�Ű�ˮ���˵���Ϣ");
                    break;
                case YaPublishPayType.Power:
                    break;
                case YaPublishPayType.TV:
                    StartActivity("�Ű�����ѡ���������");
                    break;
            }
            
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("�Ű�֧���������п�");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
