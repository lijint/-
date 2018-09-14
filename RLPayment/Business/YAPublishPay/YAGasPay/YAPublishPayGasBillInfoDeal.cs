using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using YAPayment.Package;
using YAPayment.Entity;

namespace YAPayment.Business.YAPublishPay.YAGasPay
{
    class YAPublishPayGasBillInfoDeal : Activity
    {
        protected override void OnEnter()
        {
            YAEntity entity = GetBusinessEntity() as YAEntity;
            GetElementById("UserNo").InnerText = entity.UserID;
            GetElementById("UserName").InnerText = entity.UserName;
            GetElementById("Address").InnerText = entity.UserAddress;
            GetElementById("Amount").InnerText = entity.QueryAmount.ToString("########0.00") + " Ԫ";
            GetElementById("StartDate").InnerText = entity.QueryDateStart;
            GetElementById("EndDate").InnerText = entity.QueryDateEnd;

            GetElementById("Back").Click += new HtmlElementEventHandler(Back_Click);
            GetElementById("Ok").Click += new HtmlElementEventHandler(Ok_Click);
            GetElementById("Return").Click += new HtmlElementEventHandler(Return_Click);
        }

        private void Back_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("�Ű�֧�������û���");
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            CommonData.Amount = (GetBusinessEntity() as YAEntity).QueryAmount;
            StartActivity("�Ű�֧�����ȷ��");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
