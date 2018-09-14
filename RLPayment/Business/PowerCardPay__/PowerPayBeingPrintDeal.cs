using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayBeingPrintDeal : PrinterActivity
    {
        protected override void OnEnter()
        {
            PrintReceipt(new PowerPay().GetReceipt());
        }

        protected override void HandleResult(Result result)
        {
            if (result == Result.Success || result == Result.PaperFew)
            {
                StartActivity("����֧���������");
            }
            else
            {
                StartActivity("����֧����ӡʧ��");
            }
        }
    }
}
