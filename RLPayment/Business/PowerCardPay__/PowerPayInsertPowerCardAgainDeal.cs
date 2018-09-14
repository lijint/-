using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;
using YAPayment.Package.PowerCardPay;
using Landi.FrameWorks.HardWare;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayInsertPowerCardAgainDeal : LoopReadActivity
    {
        protected override string ReturnId
        {
            get { return "Return"; }
        }

        protected override void HandleResult(Result result)
        {
            PowerEntity entity = GetBusinessEntity() as PowerEntity;
            switch (result)
            {
                case Result.Success:
                    {
                        ReportSync("PowerWriteCard");
                        //System.Threading.Thread.Sleep(5000);
                        if (!new CPowerCard().WritePowerCard(entity.PowerCardData))
                        {
                            //�Զ����ò�д������
                            ReportSync("PowerWriteCardAgain");
                            //System.Threading.Thread.Sleep(5000);
                            TransResult transR = new CPowerCardWriteAgain().WritePowerCardAgain();
                            if (transR != TransResult.E_SUCC)
                                goto case Result.Fail;
                        }

                        if (ReceiptPrinter.ExistError())
                            StartActivity("����֧���������");
                        else
                            StartActivity("����֧�����׳ɹ��Ƿ��ӡ");
                    }
                    break;
                case Result.HardwareError:
                case Result.Fail:
                case Result.Cancel:
                case Result.TimeOut:
                    StartActivity("����֧��д��ʧ��");
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
            if (msg == "PowerWriteCard")
            {
                GetElementById("Msg").InnerText = "����д���翨��Ϣ�����Ե�";
            }
            else if (msg == "PowerWriteCardAgain")
            {
                GetElementById("Msg").InnerText = "д��ʧ�ܣ��Զ���д���У����Ե�";
            }
        }

        protected override void OnLeave()
        {
            if (!CommonData.BIsCardIn)
                CardReader.CancelCommand();
        }
    }
}
