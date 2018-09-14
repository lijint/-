using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;
using YAPayment.Package;
using YAPayment.Package.Creditcard;

namespace YAPayment.Business.Creditcard
{
    class CreditcardProcessDeal : Activity
    {
        private EMVTransProcess emv = new EMVTransProcess();
        private bool bemvInit;
        private bool bisICCard;
        private QMEntity entity;

        protected override void OnEnter()
        {

            DestroySelf();//���ó��Զ����٣�ÿ����������
            bemvInit = false;
            bisICCard = false;
            entity = (GetBusinessEntity() as QMEntity);

            if (SyncTransaction(new CReverse_YAPaymentPay()) == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("���׳�ʱ��������");
                return;
            }

            if (QueryProcess() != TransResult.E_SUCC)
                return;

            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bisICCard = true;

            entity.SendField55 = null;
            if (bisICCard)//�����IC�������Ǹ��Ͽ�
            {
                PostSync(EMVProcess);
                if (!bemvInit)
                {
                    ShowMessageAndGotoMain("IC����ʼ��ʧ�ܣ�������");
                    return;
                }
            }

            PayProcess();
        }


        private void EMVProcess()
        {
            //����֧�����
            int state = emv.EMVTransInit(entity.TotalAmount, EMVTransProcess.PbocTransType.PURCHASE);
            if (state == 0)
            {
                if (emv.EMVTransDeal() == 0)
                {
                    CommonData.BankCardNum = emv.EMVInfo.CardNum;
                    CommonData.BankCardSeqNum = emv.EMVInfo.CardSeqNum;
                    CommonData.BankCardExpDate = emv.EMVInfo.CardExpDate;
                    CommonData.Track2 = emv.EMVInfo.Track2;
                    entity.SendField55 = emv.EMVInfo.SendField55;
                    bemvInit = true;
                }
            }
        }

        private TransResult QueryProcess()
        {
            CCreditcardQuery query = new CCreditcardQuery();
            TransResult result = SyncTransaction(query);
            if (result == TransResult.E_SUCC)
            {
            }
            else if (result == TransResult.E_HOST_FAIL)
            {
                ShowMessageAndGotoMain(query.ReturnCode + "-" +
                            query.ReturnMessage);
            }
            else if (result == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("���׳�ʱ��������");
            }
            else
            {
                ShowMessageAndGotoMain("����ʧ�ܣ�������");
            }

            return result;
        }

        private void PayProcess()
        {
            CCreditcardPay pay = new CCreditcardPay();
            TransResult result = SyncTransaction(pay);
            CReverse_YAPaymentPay rev = new CReverse_YAPaymentPay(pay);

            if (result == TransResult.E_SUCC)
            {
                if (bisICCard)
                {
                    int state = emv.EMVTransEnd(entity.RecvField55, entity.RecvField38);
                    if (state != 0)
                    {
                        rev.Reason = "06";
                        SyncTransaction(rev);
                        ShowMessageAndGotoMain("ICȷ�ϴ��󣬽���ʧ�ܣ�������");
                        return;
                    }
                }

                if (ReceiptPrinter.ExistError())
                    StartActivity("���ÿ������ӡ�ɹ�");
                else
                    StartActivity("���ÿ�����׳ɹ�");
            }
            else if (result == TransResult.E_HOST_FAIL)
            {
                if (pay.ReturnCode == "51")
                    ShowMessageAndGotoMain("��ܰ��ʾ����Ǹ������ʧ�ܣ�" + "\n" +
                        "���������㣡");
                else if (pay.ReturnCode == "55")
                    StartActivity("���ÿ������������");
                else
                    ShowMessageAndGotoMain(pay.ReturnCode + "-" +
                        pay.ReturnMessage);
            }
            else if (result == TransResult.E_RECV_FAIL)
            {
                rev.Reason = "98";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("���׳�ʱ��������");
                return;
            }
            else if (result == TransResult.E_CHECK_FAIL)
            {
                rev.Reason = "96";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("ϵͳ�쳣�����Ժ�����");
                return;
            }
            else
            {
                ShowMessageAndGotoMain("����ʧ�ܣ�������");
            }

            rev.ClearReverseFile();//�ڲ��������ļ�������£�����������ļ�
        }

    }
}
