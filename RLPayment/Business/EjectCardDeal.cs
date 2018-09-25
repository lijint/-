using Landi.FrameWorks;
using System;
using TerminalLib;

namespace RLPayment.Business
{
    class EjectCardDeal : FrameActivity, ITimeTick
    {
        //private bool mUserTakeCard;
        //protected override void OnEnter()
        //{
        //    mUserTakeCard = false;
        //    EjectCard();
        //    //PostAsync(OnResult);
        //}

        //private void OnResult()
        //{
        //    while (!mUserTakeCard)
        //    {
        //        if (TimeIsOut)
        //        {
        //            Log.Warn("TakeCard TimeOut Capture Card.");
        //            break;
        //        }
        //        Sleep(200);
        //    }
        //    StartActivity("������");
        //}

        //protected override void OnTimeOut()
        //{
        //}

        //private void EjectCard()
        //{
        //    RequestData _request = new RequestData();

        //    Global.gTerminalPay.BusinessLib = String.Format("{0}.CardReaderService", Global.gBankCardLibName);
        //    Global.gTerminalPay.EjectCard(_request);

        //    ResponseData ResponseEntity = Global.gTerminalPay.ResponseEntity;
        //    string strHtml = ResponseEntity.args;
        //    if (ResponseEntity.StepCode == "ProceduresEnd")
        //    {
        //        if (ResponseEntity.returnCode == "00")
        //        {
        //            StartActivity("������");
        //            //strHtml = "���˿�";
        //        }
        //        else
        //        {
        //            Log.Error("�˿�ʧ��" + strHtml);
        //        }
        //    }
        //}

        #region ITimeTick ��Ա

        public void OnTimeTick(int count)
        {
            ResponseData ResponseEntity = Global.gTerminalPay.ResponseEntity;
            string strHtml = ResponseEntity.args;
            if (ResponseEntity.StepCode == "ProceduresEnd")
            {
                if (ResponseEntity.returnCode == "00")
                {
                    StartActivity("������");
                    //strHtml = "���˿�";
                }
                else
                {
                    Log.Error("�˿�ʧ��" + strHtml);
                }
            }
        }
        #endregion
    }
}
