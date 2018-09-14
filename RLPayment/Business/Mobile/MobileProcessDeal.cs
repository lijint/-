using Landi.FrameWorks;
using Landi.FrameWorks.HardWare;
using YAPayment.Entity;
using YAPayment.Package;
using YAPayment.Package.Mobile;

namespace YAPayment.Business.Mobile
{
    class MobileProcessDeal : Activity
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
            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bisICCard = true;

            if (SyncTransaction(new CReverse_YAPaymentPay()) == TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain("���׳�ʱ��������");
                return;
            }

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
            int state = emv.EMVTransInit(CommonData.Amount, EMVTransProcess.PbocTransType.PURCHASE);
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

        private void PayProcess()
        {
            CMobileRecharge mobileRecharge = new CMobileRecharge();
            TransResult retPay = SyncTransaction(mobileRecharge);
            CReverse_YAPaymentPay rev = new CReverse_YAPaymentPay(mobileRecharge);
            //Test
            //retPay = TransResult.E_RECV_FAIL;
            if (retPay == TransResult.E_SUCC)
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

                rev.ClearReverseFile();//��������ֱ��return���������Ҫ��������ļ�
                if (ReceiptPrinter.ExistError())
                    StartActivity("�ֻ���ֵ��ӡ�ɹ�");
                else
                    StartActivity("�ֻ���ֵ���׳ɹ�");

                #region no use
                //�ɷѳɹ�������ȷ�Ͻ���
                //CMobileConfirm mobileConfirm = new CMobileConfirm();
                //TransResult retConfirm = SyncTransaction(mobileConfirm);
                //if (retConfirm == TransResult.E_SUCC)
                //{
                
                //}
                //else if (retConfirm == TransResult.E_HOST_FAIL)
                //{
                //    if (mobileConfirm.ReturnCode.ToUpper() == "ET")
                //    {
                //        //����
                //        CReverse_QMPay rev = new CReverse_QMPay(mobileRecharge);
                //        rev.Reason = "06";
                //        SyncTransaction(rev);
                //        ShowMessageAndGotoMain("����ʧ��");
                //    }
                //    else
                //    {
                //        ShowPringPage();
                //    }
                //}
                //else if (retConfirm == TransResult.E_RECV_FAIL)
                //{
                //    //����ѯ���׷��ؽ��
                //    CMobileQuery mobileQuery = new CMobileQuery();
                //    TransResult retQuery = TransResult.E_SUCC;
                //    for (int iPer = 0; iPer < 3; iPer++)
                //    {
                //        retQuery = SyncTransaction(mobileQuery);
                //        if (retQuery == TransResult.E_SUCC)
                //        {
                //            break;
                //        }
                //        else if (retQuery == TransResult.E_HOST_FAIL)
                //        {
                //            if (mobileQuery.ReturnCode.ToUpper() == "ET")
                //            {
                //                //����
                //                CReverse_QMPay rev = new CReverse_QMPay(mobileRecharge);
                //                rev.Reason = "06";
                //                SyncTransaction(rev);
                //            }
                //            break;
                //        }
                //        else if (retQuery != TransResult.E_RECV_FAIL)
                //        {
                //            break;
                //        }
                //    }

                //    if (retQuery == TransResult.E_SUCC)
                //    {
                //        ShowPringPage();
                //    }
                //    else if (retQuery == TransResult.E_HOST_FAIL)
                //    {
                //        if (mobileQuery.ReturnCode.ToUpper() == "ET")
                //        {
                //            ShowMessageAndGotoMain("����ʧ��");
                //        }
                //        else
                //        {
                //            ShowPringPage();
                //        }
                //    }
                //    else //��ѯ�����������
                //    {
                //        ShowPringPage();
                //    }
                //}
                //else //ȷ�Ͻ����������
                //{
                //    ShowPringPage();
                //}
                #endregion
            }
            else if (retPay == TransResult.E_HOST_FAIL)
            {
                if (mobileRecharge.ReturnCode == "51")
                {
                    ShowMessageAndGotoMain("��ܰ��ʾ����Ǹ������ʧ�ܣ�" + "\n" +
                        "���������㣡");
                }
                else if (mobileRecharge.ReturnCode == "55")
                {
                    StartActivity("�ֻ���ֵ�������");
                }
                else
                {
                    ShowMessageAndGotoMain(mobileRecharge.ReturnCode + "-" +
                        mobileRecharge.ReturnMessage);
                }

            }
            else if (retPay == TransResult.E_RECV_FAIL)
            {
                rev.Reason = "98";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("����ʧ��");
                return;
            }
            else if (retPay == TransResult.E_CHECK_FAIL)
            {
                rev.Reason = "96";
                SyncTransaction(rev);
                ShowMessageAndGotoMain("ϵͳ�쳣�����Ժ�����");
                return;
            }
            else
            {
                ShowMessageAndGotoMain("����ʧ��");
            }
            rev.ClearReverseFile();//��������ֱ��return���������Ҫ��������ļ�
        }
    }
}