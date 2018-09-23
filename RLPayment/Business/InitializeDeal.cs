using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.IO;
using System.ComponentModel;
using Landi.FrameWorks;
using TerminalLib;
using Landi.Tools;

namespace RLPayment.Business
{
    class InitializeDeal : FrameActivity, ITimeTick
    {
        /// <summary>
        /// ��ǰ����
        /// </summary>
        private int step = 0;
        private int readyTime = 3;
        public static bool Initialized;
        Global.TransDelegate transDelegate;

        private void processing(int index)
        {
            GetElementById("img" + (index + 1)).SetAttribute("src", "images/ing.gif");
        }

        private void success(int index)
        {
            GetElementById("img" + (index + 1)).SetAttribute("src", "images/csh_success.png");
        }


        /// <summary>
        /// ��ʼ���������̿��Ʒ���--���ڴ���
        /// </summary>
        /// <param name="index"></param>
        //private void processing(int index)
        //{
        //    GetElementById("Wait" + index.ToString()).Style = "visibility:hidden";
        //    GetElementById("Success" + index.ToString()).Style = "visibility:hidden";
        //    GetElementById("Flash" + index.ToString()).Style = "height: 32px; width: 36px; visibility:visible";
        //}

        /// <summary>
        /// ��ʼ���������̿��Ʒ���2--���
        /// </summary>
        /// <param name="index"></param>
        //private void success(int index)
        //{
        //    GetElementById("Flash" + index.ToString()).Style = "height: 32px; width: 36px; visibility:hidden";
        //    GetElementById("Wait" + index.ToString()).Style = "visibility:hidden";
        //    GetElementById("Success" + index.ToString()).Style = "visibility:visible";
        //}
        
        private void initdata()
        {
            step = 0;
            Initialized = false;
            readyTime = 3;
            SetManageEntryInfo("ManageEntry");
            setRetName("back");
            setComName("componnent");
            Global.gTerminalPay.MutliThread = true;
            transDelegate = Ini_ResponseEvent;
            Global.gTerminalPay.ResponseEvent -= new ResponseHandle(transDelegate);
            Global.gTerminalPay.ResponseEvent += new ResponseHandle(transDelegate);
        }

        protected override void OnLeave()
        {
            Global.gTerminalPay.ResponseEvent -= new ResponseHandle(transDelegate);
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            initdata();
            processing(step);
            Global.gTerminalPay.BusinessLib = String.Format("{0}.InitService", "TDefaultLib");
            Global.gTerminalPay.Init();
        }

        private void Ini_ResponseEvent(ResponseData ResponseEntity)
        {
            try
            {
                //��ӡ����
                //SetValue(rtbxMessage, String.Format("{0}����:{1}, ����:{2}, ����:{3}, ����:{4}", "", ResponseEntity.ProcedureCode, ResponseEntity.StepCode, ResponseEntity.returnCode, ResponseEntity.args));
                if (ResponseEntity.returnCode == "EE")
                {
                    //�޷�������쳣
                     StopServiceDeal.Message =  "ϵͳ����|��ͣ����";
                    StartActivity("��ͣ����");
                    return;
                }

                switch (ResponseEntity.ProcedureCode)
                {
                    case "Initialization":
                        //1,��ʼ��
                        InitCallback(ResponseEntity);

                        break;
                    case "SignIn":
                        //ǩ��
                        SignInCallback(ResponseEntity);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                AppLog.Write("[Ini_ResponseEvent Err]"+ex.Message, AppLog.LogMessageType.Error);
            }
        }

        /// <summary>
        /// ��ʼ��ҵ��
        /// </summary>
        /// <param name="ResponseEntity"></param>
        private void InitCallback(ResponseData ResponseEntity)
        {
            try
            {
                if (ResponseEntity.StepCode == "ProceduresEnd")
                {
                    if (ResponseEntity.returnCode == "00")
                    {
                        success(step);
                        Global.gTerminalPay.BusinessLib = String.Format("{0}.SignInService", Global.gBankCardLibName);
                        Global.gTerminalPay.SignIn();
                        step = 1;
                        processing(step);
                    }
                    else
                    {
                       StopServiceDeal.Message= "ϵͳ��ʼ��ʧ��|" + ResponseEntity.returnCode + "," + ResponseEntity.args;
                       StartActivity("��ͣ����");
                    }
                }
            }
            catch (Exception ex)
            {
                  StopServiceDeal.Message=  "InitCallback" + ex.Message;
                  StartActivity("��ͣ����");
            }
        }

        /// <summary>
        /// ǩ��ҵ��
        /// </summary>
        /// <param name="ResponseEntity"></param>
        private void SignInCallback(ResponseData ResponseEntity)
        {
            if (ResponseEntity.StepCode == "ProceduresEnd")
            {
                if (ResponseEntity.returnCode == "00")
                {
                    //GetElementById("msg").Style = "height: 70%; visibility:block;";
                    readyTime = 3;
                    success(step);
                    step += 1;
                    processing(step);
                    success(step);
                    step = 3;
                }
                else
                {
                      StopServiceDeal.Message = "ǩ��ʧ��|" + ResponseEntity.returnCode + "," + ResponseEntity.args;
                      StartActivity("��ͣ����");
                }
            }
        }
        protected override void OnCreate()
        {
            TimerConfig config = TimerConfig.Default();
            config.Top = 685;
            config.Left = 772;
            config.Color = "red";
            config.Font_Size = 20;
            SetTimerConfig(config);
            if (ConfigFile.ReadConfigAndCreate("AppData", "AutoRun","1").Trim() == "1")
            {
                if (SetAutoRunCtrlRegInfo(true))
                    Log.Info("���ÿ����������ɹ�");
            }
            //��װ��ע���ļ�
            if (GlobalAppData.GetInstance().AppFirst && RegsvrStarTrans())
            {
                Log.Info("ע��ɹ�");
                GlobalAppData.GetInstance().AppFirst = false;
            }
        }

        public void OnTimeTick(int count)
        {
            if (step == 3)
            {
                if (readyTime == 0)
                {
                    Initialized = true;
                    StartActivity("������");
                }
                else
                    GetElementById("procNum").InnerText = (readyTime--).ToString();
            }
        }
    }
}
