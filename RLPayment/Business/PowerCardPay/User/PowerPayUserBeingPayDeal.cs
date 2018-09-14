using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;
using YAPayment.Package;
using YAPayment.Package.PowerCardPay;

namespace YAPayment.Business.PowerCardPay.User
{
    class PowerPayUserBeingPayDeal : Activity
    {
        private PowerEntity m_entity;
        private EMVTransProcess emv;
        private bool bemvInit;
        private bool bisICCard;

        protected override void OnEnter()
        {
            DestroySelf();//���ó��Զ����٣�ÿ����������
            bemvInit = false;
            bisICCard = false;
            m_entity = GetBusinessEntity() as PowerEntity;
            emv = new EMVTransProcess();

            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bisICCard = true;

            if (SyncTransaction(new CReverse_PowerPay()) == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("���׳�ʱ��������");
                return;
            }

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
            int state = emv.EMVTransInit(CommonData.Amount, EMVTransProcess.PbocTransType.PURCHASE);
            if (state == 0)
            {
                if (emv.EMVTransDeal() == 0)
                {
                    CommonData.BankCardNum = emv.EMVInfo.CardNum;
                    CommonData.BankCardSeqNum = emv.EMVInfo.CardSeqNum;
                    CommonData.BankCardExpDate = emv.EMVInfo.CardExpDate;
                    CommonData.Track2 = emv.EMVInfo.Track2;
                    m_entity.SendField55 = emv.EMVInfo.SendField55;
                    bemvInit = true;
                }
            }
        }

        private void PayProcess()
        {
            int nTryConfirm = 3;

            CPowerPayUserBeingPay beingPay = new CPowerPayUserBeingPay();
            TransResult retPay = SyncTransaction(beingPay);
            CReverse_PowerPay rev = new CReverse_PowerPay(beingPay);
            //Test
            //retPay = TransResult.E_RECV_FAIL;
            //beingPay.ReturnCode = "55";
            if (retPay == TransResult.E_SUCC)
            {
                if (bisICCard)
                {
                    int state = emv.EMVTransEnd(m_entity.RecvField55, m_entity.RecvField38);
                    if (state != 0)
                    {
                        rev.Reason = "06";
                        SyncTransaction(rev);
                        ShowMessageAndGotoMain("ICȷ�ϴ��󣬽���ʧ�ܣ�������");
                        return;
                    }
                }

                rev.ClearReverseFile();//�ɷѳɹ�֮������������̣����ڷ��������ģ���������ļ�

            CONFIRM:
                //�ɷѳɹ�������ȷ������
                CPowerPayUserBillConfirm billConfirm = new CPowerPayUserBillConfirm();
                TransResult retConfirm = SyncTransaction(billConfirm);
                //Test
                //retConfirm = TransResult.E_RECV_FAIL;
                if (retConfirm != TransResult.E_SUCC)
                {
                    //��ʱ����Ӧѭ������ȷ�ϱ���
                    if (nTryConfirm != 0 && (retConfirm == TransResult.E_RECV_FAIL))
                    {
                        nTryConfirm--;
                        goto CONFIRM;
                    }

                    //����ʧ��
                    //�ɷ�ȷ��δ�ɹ���48Сʱ��ϵͳ���Զ����������ĵȴ�����Ҫ�ظ��ɷ�
                    string temp0 = "���п��ۿ�ɹ���������ʧ�ܣ���ϵͳ���д�������={0}��ƾ֤��={1}��ϵͳ�ο���={2}���ɷ���ˮ��={3}";
                    string temp = string.Format(temp0, m_entity.DBNo, m_entity.PayTraceNo, m_entity.PayReferenceNo, m_entity.PayFlowNo);
                    Log.Warn(temp);

                    StartActivity("����֧���û�����ʧ��");
                }
                else
                {
                    string temp = string.Format("���˳ɹ��������:{0}��������ˮ:{1}�����ɽ��:{2}", m_entity.DBNo, m_entity.ConfirmTraceNo, CommonData.Amount.ToString("#######0.00"));
                    Log.Info(temp);
                    if (ReceiptPrinter.ExistError())
                        StartActivity("����֧���������");
                    else
                        StartActivity("����֧�����׳ɹ��Ƿ��ӡ");
                }
            }
            else if (retPay == TransResult.E_HOST_FAIL)
            {
                if (beingPay.ReturnCode == "51")
                {
                    ShowMessageAndGotoMain("��ܰ��ʾ����Ǹ������ʧ�ܣ�" + "\n" +
                        "���������㣡");
                }
                else if (beingPay.ReturnCode == "55")
                {
                    StartActivity("����֧���������");
                }
                else
                {
                    ShowMessageAndGotoMain(beingPay.ReturnCode + "-" + beingPay.ReturnMessage);
                }
            }
            else if (retPay == TransResult.E_RECV_FAIL)
            {
                rev.Reason = "98";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("���׳�ʱ��������");
                return;
            }
            else if (retPay == TransResult.E_UNPACKET_FAIL)
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
