using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package.PetroModifyPass;

namespace YAPayment.Business.PetroModifyPass
{
    class PetroModifyPassProcessDeal:Activity
    {
        protected override void OnEnter()
        {
            CPetroModifyPassProcess PetroModifyPassProcess = new CPetroModifyPassProcess();
            TransResult ret = SyncTransaction(PetroModifyPassProcess);
           
            if (ret == TransResult.E_SUCC)
            {
                StartActivity("��ʯ���޸�����ɹ�");
            }
            else if (ret == TransResult.E_HOST_FAIL)
            {
                if (PetroModifyPassProcess.ReturnCode == "D3")
                    ShowMessageAndGoBack("��֤����ʧ��!���ṩ��ȷ�û���������!");
                else
                    ShowMessageAndGoBack(PetroModifyPassProcess.ReturnCode + "-" + PetroModifyPassProcess.ReturnMessage);
            }
            else if (ret == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGoBack("���׳�ʱ��������");
            }
            else
            {
                ShowMessageAndGoBack("����ʧ�ܣ�������");
            }
        }
    }
}
