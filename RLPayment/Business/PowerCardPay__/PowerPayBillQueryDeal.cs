using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;
using YAPayment.Package.PowerCardPay;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayBillQueryDeal : Activity
    {
        protected override void OnEnter()
        {
            PowerEntity entity = GetBusinessEntity() as PowerEntity;
            CPowerCardBillQuery infoQuery = new CPowerCardBillQuery();
            TransResult ret = infoQuery.BillQuery();// SyncTransaction(infoQuery);
            if (ret == TransResult.E_SUCC)
            {
                StartActivity("����֧���˵���Ϣ");
            }
            else if (ret == TransResult.E_HOST_FAIL)
            {
                ShowMessageAndGotoMain(infoQuery.ReturnCode + "-" + infoQuery.ReturnMessage);
            }
            else if (ret == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("���׳�ʱ��������");
            }
            else
            {
                ShowMessageAndGotoMain("����ʧ�ܣ�������");
            }
        }
    }
}
