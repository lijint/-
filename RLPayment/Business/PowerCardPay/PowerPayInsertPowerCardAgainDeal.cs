using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;
using YAPayment.Package.PowerCardPay;
using Landi.FrameWorks.HardWare;
using YAPayment.Package;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayInsertPowerCardAgainDeal : LoopReadActivity
    {
        protected override string ReturnId
        {
            get { return "Return"; }
        }

        TransResult retConfirm = TransResult.E_RECV_FAIL;

        private void ConfirmTrans()
        {
            CPowerPayBillConfirm billConfirm = new CPowerPayBillConfirm();
            retConfirm = billConfirm.BillConfirm();
        }

        protected override void HandleResult(Result result)
        {
            int nTryConfirm = 3;
            PowerEntity entity = GetBusinessEntity() as PowerEntity;

            CPowerCard powerCard = new CPowerCard();
            if (!powerCard.ReadPowerCard(entity.PowerCardData))
            {
                StartActivity("����֧��д��ʧ��");//
                return;
            }
            CONFIRM:
            //�ɷѳɹ�������ȷ������
            retConfirm = TransResult.E_RECV_FAIL;
            //PostAsync(ConfirmTrans);
            ConfirmTrans();
            if (retConfirm != TransResult.E_SUCC)//�������ʧ�ܽ������ʱ���ظ�3��
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
                string temp = string.Format(temp0, CommonData.BankCardNum, entity.PayTraceNo,
                    entity.PayReferenceNo, entity.PayFlowNo);
                Log.Warn(temp);
                StartActivity("����֧������ʧ��");//
            }
            else
            {
                Log.Info("�û��ţ�" + entity.PowerCardNo + "�����̻������ţ�" + entity.PowerPayConfirmCode);
                ReportSync("PowerWriteCard");
                //System.Threading.Thread.Sleep(5000);
                //if (!new CPowerCard().WritePowerCard(entity.PowerCardData))
                //{
                    //�Զ����ò�д������
                    //ReportSync("PowerWriteCardAgain");
                System.Threading.Thread.Sleep(10000);
                for (int i = 0; i < 4; i++)
                {
                    TransResult transRet = new CPowerCardWriteAgain().WritePowerCardAgain();
                    if (transRet != TransResult.E_SUCC && i==2)
                    {
                        StartActivity("����֧��д��ʧ��");
                        return;
                    }
                    if (transRet == TransResult.E_SUCC)
                    {
                        if (ReceiptPrinter.ExistError())
                            StartActivity("����֧���������");
                        else
                            StartActivity("����֧�����׳ɹ��Ƿ��ӡ");
                        break;
                    }
                    System.Threading.Thread.Sleep(500);
                }
                //}
                //if (!powerCard.ReadPowerCard(entity.PowerCardData))
                //{
                //    StartActivity("����֧��д��ʧ��");//
                //    return;
                //}
                //д���ɹ�
                //for (int i = 0; i < 3; i++)
                //{
                //    //����У��
                //    ReportSync("PowerCheckAmount");
                //    retConfirm = ConfirmBillProcess();
                //    if (retConfirm == TransResult.E_SUCC)
                //    {
                //        Log.Info("entity.CheckBuyAmount:" + double.Parse(Utility.StringToAmount(entity.CheckWriteAmount)) + " entity.PowerPayAmount:" + CommonData.Amount + " entity.CheckWriteCardAmount:" + double.Parse(Utility.StringToAmount(entity.CheckRechargeAmount)));
                //        if ((double.Parse(Utility.StringToAmount(entity.CheckWriteAmount)) == CommonData.Amount) || (double.Parse(Utility.StringToAmount(entity.CheckRechargeAmount)) == CommonData.Amount))
                //        {
                //            //У��ɹ�
                //            if (ReceiptPrinter.ExistError())
                //                StartActivity("����֧���������");
                //            else
                //                StartActivity("����֧�����׳ɹ��Ƿ��ӡ");
                //            break;
                //        }
                //        else
                //        {
                //            //У��ʧ�ܣ����в�д��
                //            if (i < 2)
                //            {
                //                ReportSync("PowerWriteCardAgain");
                //                //System.Threading.Thread.Sleep(5000);
                //                TransResult transR = new CPowerCardWriteAgain().WritePowerCardAgain();
                //                if (transR != TransResult.E_SUCC)
                //                {
                //                    StartActivity("����֧��д��ʧ��");
                //                    break;
                //                }
                //                continue;
                //            }
                //            else
                //            {
                //                ShowMessageAndGotoMain("����У��ʧ��, �뵽��̨����");
                //                break;
                //            }
                //        }
                //    }
                //    if (retConfirm == TransResult.E_RECV_FAIL)
                //    {
                //        //���׳�ʱ���ٴ�У��
                //        if (i < 2)
                //        {
                //            continue;
                //        }
                //        else
                //        {
                //            ShowMessageAndGotoMain("����У�鳬ʱ, �뵽��̨����");
                //            break;
                //        }
                //    }
                //    else
                //        break;
                //}
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
                GetElementById("Msg").InnerText = "��д���У����Ե�";
            }

            else if (msg == "PowerCheckAmount")
            {
                GetElementById("Msg").InnerText = "д����ɣ����У���У����Ե�";
            }
        }

        /// <summary>
        /// ������ɺ󣬹���ȷ�Ͻ���
        /// </summary>
        /// <returns></returns>
        private CPowerCardBillCheck billConfirm = new CPowerCardBillCheck();

        private TransResult ConfirmBillProcess()
        {
            TransResult retConfirm = TransResult.E_RECV_FAIL;

            PostAsync(billcheck);
            if (retConfirm != TransResult.E_SUCC && retConfirm != TransResult.E_RECV_FAIL)
            {
                ShowMessageAndGotoMain(billConfirm.ReturnCode + "-" + billConfirm.ReturnMessage);
            }
            return retConfirm;
        }

        private void billcheck()
        {
            try
            {
                retConfirm = billConfirm.BillCheck();
            }
            catch (Exception ex)
            {
                Log.Debug("bullcheck err:", ex);
            }
        }

        protected override void OnLeave()
        {
            if (!CommonData.BIsCardIn)
                CardReader.CancelCommand();
        }
    }
}
