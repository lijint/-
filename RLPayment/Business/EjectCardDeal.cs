using Landi.FrameWorks;
using System;
using TerminalLib;

namespace RLPayment.Business
{
    class EjectCardDeal : FrameActivity, ITimeTick
    {

        protected override void FrameReturnClick()
        {
            GotoMain();
        }


        protected override void OnTimeOut()
        {
            GotoMain();
        }

        #region ITimeTick ��Ա

        public void OnTimeTick(int count)
        {
            //RequestData _request = new RequestData();
            //Global.gTerminalPay.BusinessLib = String.Format("{0}.CardReaderService", Global.gBankCardLibName);
            //Global.gTerminalPay.EjectCard(_request);
            //ResponseData ResponseEntity = Global.gTerminalPay.ResponseEntity;
            //string strHtml = ResponseEntity.args;
            //if (ResponseEntity.StepCode == "ProceduresEnd")
            //{
            //    if (ResponseEntity.returnCode == "00")
            //    {
            //        StartActivity("������");
            //        //strHtml = "���˿�";
            //    }
            //    else
            //    {
            //        Log.Error("�˿�ʧ��" + strHtml);
            //    }
            //}
        }
        #endregion
    }
}
