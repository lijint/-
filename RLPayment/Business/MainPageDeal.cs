using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Landi.FrameWorks;


namespace RLPayment.Business
{
    class MainPageDeal : FrameActivity, ITimeTick
    {

        private bool _isClick = false;
        protected override void OnEnter()
        {
            try
            {
                _isClick = false;

                //GetElementById("AdImage").SetAttribute("src", mPictures[m_currpicture]);
                //GetElementById("PublicFee").Click += new HtmlElementEventHandler(PublicFee_Click);
                //GetElementById("Gas").Click += new HtmlElementEventHandler(PublicFee_Click);
                //GetElementById("Water").Click += new HtmlElementEventHandler(PublicFee_Click);
                //GetElementById("TV").Click += new HtmlElementEventHandler(PublicFee_Click);
                //GetElementById("Power").Click += new HtmlElementEventHandler(Power_Click);
                //GetElementById("Mobile").Click += new HtmlElementEventHandler(Mobile_Click);
                //GetElementById("CreditCard").Click += new HtmlElementEventHandler(CreditCard_Click);
                //GetElementById("TrafficPolice").Click += new HtmlElementEventHandler(TrafficPolice_Click);
                //GetElementById("CarTicket").Click += CarTicket_Click;
                //SetManageEntryInfo("ManageEntry");
                IsConDisplay(false);
                Log.Info("Version : " + Assembly.GetExecutingAssembly().GetName().Version.ToString());

                INIClass gasCardReaderIni = new INIClass(AppDomain.CurrentDomain.BaseDirectory + "Versionfile.ini");
                gasCardReaderIni.IniWriteValue("Version", "VersionNo", Assembly.GetExecutingAssembly().GetName().Version.ToString());

                GetElementById("btnPay").Click += new HtmlElementEventHandler(btnPay_Click);

                //SetElement(MenuEnable.CreditCard, "CreditCard");
                //SetElement(MenuEnable.TV, "TV");
                //SetElement(MenuEnable.Gas, "Gas");
                //SetElement(MenuEnable.Water, "Water");
                //SetElement(MenuEnable.Power, "Power");
                //SetElement(MenuEnable.Mobile, "Mobile");
                //SetElement(MenuEnable.TrafficPolice, "TrafficPolice");
                //SetElement(MenuEnable.CarTicket, "CarTicket");
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }

        private void btnPay_Click(object sender, HtmlElementEventArgs e)
        {
            if (_isClick) return;
            _isClick = true;
            EnterBusiness(new RLCZ.RLCZStratagy());
            StartActivity("������ֵ����������");
            //if (CarPay.HasSignIn)
            //{
            //    StartActivity(ReceiptPrinter.CheckedByManager() ? "��ƱԤ��������" : "�Ű�������û��ӡ�����ϼ���");
            //}
            //else
            //{
            //    StartActivity("����ǩ��");
            //}
        }

        void CarTicket_Click(object sender, HtmlElementEventArgs e)
        {
            //if (_isClick) return;
            //_isClick = true;
            //EnterBusiness(new CarTicketStratagy());
            //if (CarPay.HasSignIn)
            //{
            //    StartActivity(ReceiptPrinter.CheckedByManager() ? "��ƱԤ��������" : "�Ű�������û��ӡ�����ϼ���");
            //}
            //else
            //{
            //    StartActivity("����ǩ��");
            //}
        }

        private void SetElement(bool isShow, string eleName)
        {
            if (GetElementById(eleName) == null) return;
            if (isShow)
            {
                if (GetElementById(eleName).Style != null && GetElementById(eleName).Style.Contains("visibility"))
                    GetElementById(eleName).Style.Replace("hidden", "visible");
                else
                    GetElementById(eleName).Style += ";visibility:visible;";
            }
            else
            {
                if (GetElementById(eleName).Style != null && GetElementById(eleName).Style.Contains("visibility"))
                    GetElementById(eleName).Style.Replace("visible", "hidden");
                else
                    GetElementById(eleName).Style += ";visibility:hidden;";
            }
        }

        private void TrafficPolice_Click(object sender, HtmlElementEventArgs e)
        {
            //StartActivity("��ҵ����δ��ͨ");
            //return;
            //if(_isClick) return;
            //_isClick = true;
            //EnterBusiness(new YATrafficPoliceStratagy());
            //if (YAPaymentPay.HasSignIn)
            //{
            //    StartActivity(ReceiptPrinter.CheckedByManager() ? "�Ű�������û�˵�" : "�Ű�������û��ӡ�����ϼ���");
            //}
            //else
            //{
            //    StartActivity("����ǩ��");
            //}
        }

        private void CreditCard_Click(object sender, HtmlElementEventArgs e)
        {
            //StartActivity("��ҵ����δ��ͨ");
            //return;
            //if (_isClick) return;
            //_isClick = true;
            //EnterBusiness(new CreditcardStratagy());
            //if (QMPay.HasSignIn)
            //{
            //    StartActivity(ReceiptPrinter.CheckedByManager() ? "���ÿ�������ܰ��ʾ" : "���ÿ���ӡ�����ϼ���");
            //}
            //else
            //{
            //    StartActivity("����ǩ��");
            //}
        }

        private void Mobile_Click(object sender, HtmlElementEventArgs e)
        {
            ////StartActivity("��ҵ����δ��ͨ");
            ////return;
            //if (_isClick) return;
            //_isClick = true;
            //EnterBusiness(new MobileStratagy());
            //if (QMPay.HasSignIn)
            //{
            //    StartActivity(ReceiptPrinter.CheckedByManager() ? "�ֻ���ֵ������" : "�ֻ���ֵ��ӡ�����ϼ���");
            //}
            //else
            //{
            //    StartActivity("����ǩ��");
            //}
        }

        private void PublicFee_Click(object sender, HtmlElementEventArgs e)
        {
            ////if (_isClick) return;
            ////_isClick = true;
            ////EnterBusiness(new YAPublishPayStratagy());

            ////string id;
            ////if (sender is HtmlElement)
            ////    id = (sender as HtmlElement).Id;
            ////else
            ////    id = (string)sender;

            ////YAEntity entity = GetBusinessEntity() as YAEntity;
            ////switch (id)
            ////{
            ////    case "Gas":
            ////        entity.PublishPayType = YaPublishPayType.Gas;
            ////        break;
            ////    case "Water":
            ////        entity.PublishPayType = YaPublishPayType.Water;
            ////        break;
            ////    case "Power":
            ////        entity.PublishPayType = YaPublishPayType.Power;
            ////        break;
            ////    case "TV":
            ////        entity.PublishPayType = YaPublishPayType.TV;
            ////        break;
            ////}
            ////Log.Info("�Ű�������ҵ�ɷѣ�" + entity.PublishPayType.ToString());


            ////if (YAPaymentPay.HasSignIn)
            ////{
            ////    if (ReceiptPrinter.CheckedByManager())
            ////    {
            ////        //StartActivity("�Ű�֧���˵�");
            ////        StartActivity("�Ű�֧�������û���");
            ////    }
            ////    else
            ////        StartActivity("�Ű�֧����ӡ�����ϼ���");
            ////}
            ////else
            ////{
            ////    StartActivity("����ǩ��");
            ////}
        }

        private void Power_Click(object sender, HtmlElementEventArgs e)
        {
            //if (_isClick) return;
            //_isClick = true;
            ////Test
            ////StartActivity("��ҵ����δ��ͨ");
            ////return;
            //EnterBusiness(new PowerPayStratagy());
            //if (PowerPay.HasSignIn)
            //{
            //    if (ReceiptPrinter.CheckedByManager())
            //    {
            //        string temp = "";
            //        if (ValidateOutOfBussinessTime(ref temp))
            //            StartActivity("����֧���˵�");
            //        else
            //            ShowMessageAndGotoMain(temp);  
            //    }
            //    else
            //    {
            //        ShowMessageAndGotoMain("��ӡ��ȱֽ����ϣ�������ͣ��");                
            //    }
            //        //StartActivity("����֧����ӡ�����ϼ���");
            //}
            //else
            //{
            //    StartActivity("����ǩ��");
            //}
        }

        /// <summary>
        /// ����ʱ����֤
        /// </summary>
        /// <returns></returns>
        private bool ValidateOutOfBussinessTime(ref string message)
        {
            try
            {
                DateTime time = DateTime.Now;
                DateTime beginH = ConvertToTime(ConfigFile.ReadConfigAndCreate("Power", "BeginHour", "23:30"));
                DateTime endH = ConvertToTime(ConfigFile.ReadConfigAndCreate("Power", "EndHour", "1:00"));
                message = string.Format("ʱ���({0}-24:00,0:00-{1})����̨������ͣ��", beginH.ToString("HH:mm"), endH.ToString("HH:mm"));
                if (time >= beginH || time < endH)
                    return false;
                else
                    return true;
            }
            catch (Exception)
            {
                Log.Error("BeginHour����EndHour�������ø�ʽ���쳣��");
                message = "BeginHour����EndHour�������ø�ʽ���쳣��";
                return false;
            }
        }

        private DateTime ConvertToTime(string value)
        {

            string[] time = value.Split(new char[] { ':' }, StringSplitOptions.None);
            DateTime result = DateTime.Today;
            int hour = int.Parse(time[0]);
            if (hour >= 24)
            {
                Log.Error("Сʱ�����ܴ���24��");
                throw new Exception("Сʱ�����ܴ���24");
            }
            result = result.AddHours(hour);
            int min = int.Parse(time[1]);
            if (min >= 60)
            {
                Log.Error("���������ܴ���60��");
                throw new Exception("���������ܴ���60");
            }
            result = result.AddMinutes(min);
            return result;
        }

        protected override void OnTimeOut()
        {
            ShowAd();
        }

        public override bool CanQuit()
        {
            return true;
        }

        protected override void OnLeave()
        {

        }

        bool no_service()
        {
            //Test
            //return false;

            bool bRet = false;
            TimeSpan t = DateTime.Now.TimeOfDay;
            if ((t.Hours == 22 && t.Minutes >= 30) ||
                t.Hours == 23 ||
                t.Hours == 0)
            {
                StartActivity("��ҵ����δ��ͨ");
                bRet = true;
            }

            return bRet;
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            base.OnKeyDown(keyCode);

            //string ID = "";
            //switch (keyCode)
            //{
            //    case Keys.D1:
            //        {
            //            if (MenuEnable.Gas)
            //            {
            //                ID = "Gas";
            //                PublicFee_Click(ID, null);
            //            }
            //        }
            //        break;
            //    case Keys.D2:
            //        {
            //            if (MenuEnable.Water)
            //            {
            //                ID = "Water";
            //                PublicFee_Click(ID, null);
            //            }
            //        }
            //        break;
            //    case Keys.D3:
            //        {
            //            if (MenuEnable.TV)
            //            {
            //                ID = "TV";
            //                PublicFee_Click(ID, null);
            //            }
            //        }
            //        break;
            //    case Keys.D4:
            //        {
            //            if (MenuEnable.Power)
            //            {
            //                Power_Click(null, null);
            //            }
            //        }
            //        break;
            //    case Keys.D5:
            //        {
            //            if (MenuEnable.Mobile)
            //            {
            //                Mobile_Click(null, null);
            //            }
            //            //ID = "Power";
            //            //PublicFee_Click(ID, null);
            //        }
            //        break;
            //    case Keys.D6:
            //        {
            //            if (MenuEnable.CreditCard)
            //            {
            //                CreditCard_Click(null, null);
            //            }
            //        }
            //        break;
            //    case Keys.D7:
            //        {
            //            if (MenuEnable.TrafficPolice)
            //            {
            //                TrafficPolice_Click(null, null);
            //            }
            //        }
            //        break;
            //    case Keys.D8:
            //        {
            //            if (MenuEnable.CarTicket)
            //            {
            //                CarTicket_Click(null, null);
            //            }
            //        }
            //        break;
            //}
        }


        #region ITimeTick ��Ա

        public void OnTimeTick(int count)
        {
            //string sDate = DateTime.Now.ToString("yyyy��MM��dd��");
            //string sW = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("zh-cn"));
            //string sTime = DateTime.Now.ToString("HH:mm:ss");

            //string temp = sDate + " " + sW + " " + sTime;
            //GetElementById("DateTime").InnerText = temp;

            //if (count % m_AdSwitchInterval == 0)
            //{
            //    m_currpicture = (m_currpicture + 1) % mPictures.Count;
            //    GetElementById("AdImage").SetAttribute("src", mPictures[m_currpicture]);
            //}
        }

        #endregion

        #region �м���
        private List<string> mPictures = new List<string>();
        int m_currpicture = 0;
        int m_AdSwitchInterval = 10;
        /// <summary>
        /// ��ȡ����ͼƬ
        /// </summary>
        /// <param name="dir"></param>
        private void getAllPicture(string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))
                    {
                        string ext = Path.GetExtension(d).ToLower();
                        if (ext == ".jpg" || ext == ".bmp")
                        {
                            mPictures.Add(d);
                        }
                    }
                    else
                        getAllPicture(d);
                }
            }
        }
        #endregion
    }
}
#region 
/*
private void CreditCard_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("��ҵ����δ��ͨ");
            return;
            EnterBusiness(new CreditcardStratagy());
            if (QMPay.HasSignIn)
            {
                if (ReceiptPrinter.CheckedByManager())
                    StartActivity("���ÿ�������ܰ��ʾ");
                else
                    StartActivity("���ÿ���ӡ�����ϼ���");
            }
            else
            {
                StartActivity("����ǩ��");
            }
        }

        private void Mobile_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("��ҵ����δ��ͨ");
            return;
            EnterBusiness(new MobileStratagy());
            if (QMPay.HasSignIn)
            {
                if (ReceiptPrinter.CheckedByManager())
                    StartActivity("�ֻ���ֵ������");
                else
                    StartActivity("�ֻ���ֵ��ӡ�����ϼ���");
            }
            else
            {
                StartActivity("����ǩ��");
            }
        }

        private void PetroPay_Click(object sender, HtmlElementEventArgs e)
        {
            EnterBusiness(new PetroPayStratagy());
            if (PetroChinaPay.HasSignIn)
            {
                if (ReceiptPrinter.CheckedByManager())
                {
                    if (R80.IsUse)
                        StartActivity(typeof(PetroPayShowUserCardDeal));
                    else
                        StartActivity(typeof(PetroPayUserLoginDeal));
                }
                else
                    StartActivity("��ʯ��֧����ӡ�����ϼ���");
            }
            else
            {
                StartActivity("����ǩ��");
            }
        }
 */
#endregion

