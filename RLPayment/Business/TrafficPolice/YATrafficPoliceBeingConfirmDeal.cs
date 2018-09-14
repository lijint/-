using Landi.FrameWorks;
using YAPayment.Package.TrafficPolice;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceBeingConfirmDeal : Activity
    {
        protected override void OnEnter()
        {
            CommonData.Amount = 0;

            CYATrafficPoliceBillConfirm billConfirm = new CYATrafficPoliceBillConfirm();
            TransResult retConfirm = SyncTransaction(billConfirm);

            if (retConfirm == TransResult.E_SUCC)
            {
                StartActivity("�Ű������������ɹ�");
            }
            else if (retConfirm == TransResult.E_HOST_FAIL)
            {
                ShowMessageAndGotoMain(billConfirm.ReturnCode + "-" + billConfirm.ReturnMessage);
            }
            else if (retConfirm == TransResult.E_RECV_FAIL)
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
