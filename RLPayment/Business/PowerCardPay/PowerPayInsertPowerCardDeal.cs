using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;
using YAPayment.Package.PowerCardPay;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayInsertPowerCardDeal : LoopReadActivity
    {
        protected override string ReturnId
        {
            get { return "Return"; }
        }

        protected override void HandleResult(Result result)
        {
            PowerEntity entity = GetBusinessEntity() as PowerEntity;
            //Test
            //result = Result.Fail;
            switch (result)
            {
                case Result.Success:
                    {
                        ReportSync("PowerReadCard");
                        if(!new CPowerCard().ReadPowerCard(entity.PowerCardData))
                            goto case Result.Fail;
                        StartActivity("����֧���˵���ѯ");
                    }
                    break;
                case Result.HardwareError:
                    ShowMessageAndGotoMain("����������");
                    break;
                case Result.Fail:
                    CardReader.CardOut();
                    StartActivity("����֧�����翨����");
                    break;
                case Result.Cancel:
                    StartActivity("������");
                    break;
                case Result.TimeOut:
                    StartActivity("������");
                    break;
            }
        }

        protected override Result ReadOnce()
        {
            return InsertICCard();
        }

        protected override void OnReport(object progress)
        {
            base.OnReport(progress);
            string msg = (string)progress;
            if (msg == "PowerReadCard")
            {
                GetElementById("Msg").InnerText = "���ڶ�ȡ���翨��Ϣ�����Ե�";
            }
        }

        protected override void OnLeave()
        {
            if (!CommonData.BIsCardIn)
                CardReader.CancelCommand();
        }
    }
}
