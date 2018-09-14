using Landi.FrameWorks;
using YAPayment.Package.TrafficPolice;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceBillQueryDeal : Activity
    {
        protected override void OnEnter()
        {
            CYATrafficPoliceBillQuery infoQuery = new CYATrafficPoliceBillQuery();
            TransResult ret = SyncTransaction(infoQuery);

            if (ret == TransResult.E_SUCC)
            {
                StartActivity("�Ű�������ûΥ����ʾ");
            }
            else if (ret == TransResult.E_HOST_FAIL)
            {
                if (infoQuery.ReturnCode == "D3")
                    ShowMessageAndGotoMain("��֤����ʧ��!���ṩ��ȷ�������ţ�");
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
