using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.Windows.Forms;
using YAPayment.Package;
using YAPayment.Entity;

namespace YAPayment.Business.YAPublishPay.YATVPay
{
    class YAPublishPayTVBillInfoDeal : Activity
    {
        private YAEntity m_entity = null;

        protected override void OnEnter()
        {
            m_entity = GetBusinessEntity() as YAEntity;

            ////�ʷѺͰ�ʱ��
            //StringBuilder strArr = new StringBuilder();
            //strArr.Append(m_entity.Price1.ToString("########0.00"));
            //strArr.Append(",");
            //strArr.Append(m_entity.Price2.ToString("########0.00"));
            //strArr.Append("|");
            //strArr.Append("6,9,12");
            ////ѡ����ʷѺͰ�ʱ��
            //string strIndex = "";
            //if (m_entity.SelectPrice != 0 && m_entity.SelectMonth != 0)
            //    strIndex = m_entity.SelectPrice.ToString("########0.00") + "," + m_entity.SelectMonth;

            //InvokeScript("initSelect", new object[] { strArr.ToString(), strIndex });

            GetElementById("UserNo").InnerText = m_entity.UserID;
            GetElementById("UserName").InnerText = m_entity.UserName;
            GetElementById("Amount").InnerText = m_entity.QueryAmount.ToString("########0.00") +  " Ԫ";
            GetElementById("EndDate").InnerText = m_entity.QueryDateEnd;
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
            //CommonData.Amount = double.Parse(GetElementById("PayAmount").InnerText) + m_entity.QueryAmount;
            //m_entity.SelectPrice = double.Parse(GetElementById("PriceType").GetAttribute("value"));
            //m_entity.SelectMonth = int.Parse(GetElementById("MonthType").GetAttribute("value"));

            StartActivity("�Ű�����ѡ��ɷ�����");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }
    }
}
