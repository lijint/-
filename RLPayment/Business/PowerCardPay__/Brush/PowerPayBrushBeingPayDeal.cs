using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package;
using YAPayment.Package.PowerCardPay;
using YAPayment.Entity;

namespace YAPayment.Business.PowerCardPay.Brush
{
    class PowerPayBrushBeingPayDeal : Activity
    {
        private PowerEntity m_entity = null;
        private EMVTransProcess emv = null;
        private bool bemvInit = false;
        private bool bisICCard = false;

        protected override void OnEnter()
        {
            DestroySelf();//���ó��Զ����٣�ÿ����������
            bemvInit = false;
            bisICCard = false;
            m_entity = GetBusinessEntity() as PowerEntity;
            emv = new EMVTransProcess();
            emv.PayType = 1;
            CommonData.UserCardType = UserBankCardType.ICCard;
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

            CPowerPayBeingPay beingPay = new CPowerPayBeingPay();
            TransResult retPay = SyncTransaction(beingPay);
            CReverse_PowerPay rev = new CReverse_PowerPay(beingPay);
            //Test
            //retPay = TransResult.E_HOST_FAIL;
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
                retConfirm = TransResult.E_RECV_FAIL;
                PostSync(ConfirmTrans);
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
                    string temp0 = "���п��ۿ�ɹ���������ʧ�ܣ���ϵͳ���д���CardNo={0}��ƾ֤��={1}��ϵͳ�ο���={2}���ɷ���ˮ��={3}";
                    string temp = string.Format(temp0, CommonData.BankCardNum, m_entity.PayTraceNo, m_entity.PayReferenceNo, m_entity.PayFlowNo);
                    Log.Warn(temp);

                    StartActivity("����֧������ʧ��");
                }
                else
                {
                    StartActivity("����֧���ٴβ���翨");
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
                    StartActivity("����֧�������Ӧ�������");
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

        TransResult retConfirm = TransResult.E_RECV_FAIL;
        void ConfirmTrans()
        {
            CPowerPayBillConfirm billConfirm = new CPowerPayBillConfirm();
            retConfirm = billConfirm.BillConfirm();
        }
    }
}
