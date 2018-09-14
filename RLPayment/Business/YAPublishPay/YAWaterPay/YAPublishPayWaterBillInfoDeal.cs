using System.Windows.Forms;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.YAPublishPay.YAWaterPay
{
    class YAPublishPayWaterBillInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            YAEntity entity = GetBusinessEntity() as YAEntity;
            GetElementById("UserNo").InnerText = entity.UserID;
            GetElementById("UserName").InnerText = entity.UserName;
            GetElementById("Address").InnerText = entity.UserAddress;
            GetElementById("Fee").InnerText = entity.WaterFee.ToString("########0.00") + " Ԫ";
            GetElementById("Amount").InnerText = entity.QueryAmount.ToString("########0.00") + " Ԫ";
            GetElementById("PayAmount").InnerText = entity.WaterTotalAmount.ToString("########0.00") + " Ԫ";

            GetElementById("Back").Click += Back_Click;
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("�Ű�֧�������û���");
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            CommonData.Amount = (GetBusinessEntity() as YAEntity).WaterTotalAmount;
            StartActivity("�Ű�֧�����ȷ��");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
