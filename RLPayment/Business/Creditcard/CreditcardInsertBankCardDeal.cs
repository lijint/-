using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;

namespace YAPayment.Business.Creditcard 
{
    /// <summary>
    /// �������п�
    /// </summary>
    class CreditcardInsertBankCardDeal : LoopReadActivity
    {
        protected override string ReturnId
        {
            get { return "Return"; }
        }

        protected override void HandleResult(Result result)
        {
            switch (result)
            {
                case Result.Success:
                    {
                        CommonData.BankCardNum = BankCardNum;
                        CommonData.BankCardSeqNum = CardSeqNum;
                        CommonData.BankCardExpDate = ExpDate;
                        CommonData.Track1 = Track1;
                        CommonData.Track2 = Track2;
                        CommonData.Track3 = Track3;
                        CommonData.UserCardType = BankCardType;
                        StartActivity("���ÿ�������������");
                    }
                    break;
                case Result.HardwareError:
                    ShowMessageAndGotoMain("����������");
                    break;
                case Result.Fail:
                    CardReader.CardOut();
                    StartActivity("���ÿ������������");
                    break;
                case Result.Cancel:
                    StartActivity("���ÿ��������뻹����");
                    break;
                case Result.TimeOut:
                    StartActivity("������");
                    break;
            }
        }

        protected override Result ReadOnce()
        {
            return DefaultRead4();
        }

        protected override void OnLeave()
        {
            if (!CommonData.BIsCardIn)
                CardReader.CancelCommand();
        }
    }
}
