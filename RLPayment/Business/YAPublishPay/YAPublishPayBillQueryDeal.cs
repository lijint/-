using Landi.FrameWorks;
using YAPayment.Entity;
using YAPayment.Package.YAPublishPay;

namespace YAPayment.Business.YAPublishPay
{
    class YAPublishPayBillQueryDeal : Activity
    {
        protected override void OnEnter()
        {
            YAEntity entity = GetBusinessEntity() as YAEntity;
            CYAPublishPayBillQuery infoQuery = new CYAPublishPayBillQuery();
            TransResult ret = SyncTransaction(infoQuery);

            if (ret == TransResult.E_SUCC)
            {
                switch (entity.PublishPayType)
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
                        StartActivity("�Ű������˵���Ϣ");
                        break;
                }
            }
            else if (ret == TransResult.E_HOST_FAIL)
            {
                if (infoQuery.ReturnCode == "D3")
                    ShowMessageAndGotoMain("��֤����ʧ��!���ṩ��ȷ�û���!");
                else
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
