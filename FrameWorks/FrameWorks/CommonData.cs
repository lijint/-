using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Landi.FrameWorks
{
    /// <summary>
    /// �û�ʹ�õĿ�����
    /// </summary>
    public enum UserBankCardType
    {
        None, //��ǰ�޿�
        Magcard, //�ſ�
        ICCard, //IC
        IcMagCard,//IC+����
    }

    public class CommonData
    {
        /// <summary>
        /// ���п���
        /// </summary>
        public static string BankCardNum;
        /// <summary>
        /// �ŵ�1
        /// </summary>
        public static string Track1;
        /// <summary>
        /// �ŵ�2
        /// </summary>
        public static string Track2;
        /// <summary>
        /// �ŵ�3
        /// </summary>
        public static string Track3;
        /// <summary>
        /// ����
        /// </summary>
        public static string BankPassWord;
        /// <summary>
        /// ���׽��
        /// </summary>
        public static double Amount;
        /// <summary>
        /// ���к�
        /// </summary>
        public static string BankCardSeqNum;
        /// <summary>
        /// ��Ч����
        /// </summary>
        public static string BankCardExpDate;
        /// <summary>
        /// �Ƿ�����ɹ�����������ʱ���ж�����
        /// </summary>
        public static bool BIsCardIn;
        /// <summary>
        /// ��������
        /// </summary>
        public static UserBankCardType UserCardType;

        public static void Clear()
        {
            BankCardNum = null;
            BankCardSeqNum = null;
            BankCardExpDate = null;
            Track1 = null;
            Track2 = null;
            Track3 = null;
            BankPassWord = null;
            Amount = 0;
            BIsCardIn = false;
            UserCardType = UserBankCardType.None;
        }
    }

    public class GlobalAppData : Singleton<GlobalAppData>
    {
        /// <summary>
        /// ���ػ�ѡ��
        /// </summary>
        public enum ECloseAcmType : int
        {
            Close = 0, //�ػ�
            Restart = 1, //����
        }

        private const string mSectionName = "AppData";

        public static void ReadGlobalAppData()
        {
            Hashtable hs = ConfigFile.ReadConfig(mSectionName);
            foreach (DictionaryEntry value in hs)
            {
                Activity.globalBundle.PutString(value.Key.ToString(), value.Value.ToString());
            }
        }

        ///// <summary>
        ///// ��ʼ������AID��CA
        ///// </summary>
        //public bool DownLoadAidAndCA
        //{
        //    get { return ReadIniFile("DownLoadAidAndCA") == "1"; }
        //    set { WriteIniFile("DownLoadAidAndCA", value ? "1" : "0"); }
        //}
        ///// <summary>
        ///// �Ƿ�����IC��
        ///// </summary>
        //public bool UseICCard
        //{
        //    get { return ReadIniFile("UseICCard") == "1"; }
        //    set { WriteIniFile("UseICCard", value ? "1" : "0"); }
        //}

        /// <summary>
        /// ��װ��
        /// </summary>
        public bool AppFirst
        {
            get { return ReadIniFile("AppFirst") == "1"; }
            set { WriteIniFile("AppFirst", value ? "1" : "0"); }
        }

        /// <summary>
        /// Eudemon����
        /// </summary>
        public bool EudemonSwitch
        {
            get { return ReadIniFile("EudemonSwitch") == "1"; }
            //set { WriteIniFile("EudemonSwitch", value ? "1" : "0"); }
        }

        /// <summary>
        /// �Զ����ػ�����
        /// </summary>
        public bool CloseAcmSwitch
        {
            get { return ReadIniFile("CloseAcmSwitch") == "1"; }
            set { WriteIniFile("CloseAcmSwitch", value ? "1" : "0"); }
        }
        /// <summary>
        /// ���ػ����� 0=�ػ� 1=���� 
        /// </summary>
        public ECloseAcmType CloseAcmType
        {
            get { return ReadIniFile("CloseAcmType") == "0" ? ECloseAcmType.Close : ECloseAcmType.Restart; }
            set { WriteIniFile("CloseAcmType", ((int)value).ToString()); }
        }
        /// <summary>
        /// ���ػ�ʱ��
        /// </summary>
        public string CloseAcmTime
        {
            get { return ReadIniFile("CloseAcmTime"); }
            set { WriteIniFile("CloseAcmSwitch", value); }
        }
        /// <summary>
        /// TMS����
        /// </summary>
        public bool TMSSwitch
        {
            get { return ReadIniFile("TMSSwitch") == "1"; }
            set { WriteIniFile("TMSSwitch", value ? "1" : "0"); }
        }
        /// <summary>
        /// ���ʱ����
        /// </summary>
        public long MonitorTimeInterval
        {
            get 
            {
                long interval = 0;
                long.TryParse(ReadIniFile("MonitorTimeInterval"), out interval);
                return interval == 0 ? 60 : interval;
            }
            set { WriteIniFile("MonitorTimeInterval", value.ToString()); }
        }
        /// <summary>
        /// ��ط�������ַ
        /// </summary>
        public string TMSHost
        {
            get
            {
                System.Net.IPAddress ad = null;
                string adStr = ReadIniFile("TMSHost");
                System.Net.IPAddress.TryParse(adStr, out ad);
                return ad == null ? adStr : null;
            }
            set { WriteIniFile("TMSHost", value); }
        }
        /// <summary>
        /// ��ط������˿�
        /// </summary>
        public int TMSPort
        {
            get
            {
                int port = 0;
                int.TryParse(ReadIniFile("TMSPort"), out port);
                return port;
            }
            set { WriteIniFile("TMSPort", value.ToString()); }
        }
        /// <summary>
        /// ����ǩ�����(��)
        /// </summary>
        public long RetrySignInInterval
        {
            get
            {
                long interval = 0;
                long.TryParse(ReadIniFile("RetrySignInInterval"), out interval);
                return interval == 0 ? 60 : interval;
            }
            set { WriteIniFile("RetrySignInInterval", value.ToString()); }
        }
        /// <summary>
        /// ���´�ӡ����
        /// </summary>
        public long PrintAgainCount
        {
            get
            {
                long count = 0;
                long.TryParse(ReadIniFile("PrintAgainCount"), out count);
                return count == 0 ? 60 : count;
            }
            set { WriteIniFile("PrintAgainCount", value.ToString()); }
        }
        public static string EncryptKey = "landi123456";
        /// <summary>
        /// �����������
        /// </summary>
        public string EntryPwd
        {
            get 
            {
                string psd = ReadIniFile("EntryPwd");
                if (string.IsNullOrEmpty(psd))
                {
                    psd = Encrypt.AESEncrypt("888888", EncryptKey);
                    WriteIniFile("EntryPwd", psd);
                }
                return psd;
            }
            set { WriteIniFile("EntryPwd", value); }
        }

        private Dictionary<string, string> m_businessFun = new Dictionary<string, string>();
        /// <summary>
        /// ҵ��key=ҵ�����ƣ�value=���Ľ���
        /// </summary>
        public Dictionary<string, string> BusinessFun
        {
            get
            {
                if (m_businessFun.Count != 0)
                    return m_businessFun;

                m_businessFun.Clear();
                string busi = ReadIniFile("BusinessFun");
                if (!string.IsNullOrEmpty(busi) && busi.Length > 0)
                {
                    string[] busis = busi.Split(',');
                    foreach (string item in busis)
                    {
                        string[] sectionName = item.Split('|');
                        if (sectionName.Length > 1)
                        {
                            bool use = ConfigFile.ReadConfig(sectionName[0], "Use") == "1";
                            if (use)
                                m_businessFun.Add(sectionName[0], sectionName[1]);
                        }
                    }
                }
                return m_businessFun;
            }
            set { m_businessFun.Clear(); }
        }

        private Dictionary<string, string> m_busiServerIpAndPort = new Dictionary<string, string>(); 
        /// <summary>
        /// ҵ��IP�Ͷ˿ڣ�key=ҵ�����ƣ�value=ip:port
        /// </summary>
        public Dictionary<string,string> BusiServerIpAndPort
        {
            get
            {
                if (m_busiServerIpAndPort.Count != 0)
                    return m_busiServerIpAndPort;
                
                foreach (KeyValuePair<string,string> item in BusinessFun)
                {
                    string ip = ConfigFile.ReadConfig(item.Key, "ServerIP");
                    string port = ConfigFile.ReadConfig(item.Key, "ServerPort");
                    if (!string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(port))
                        m_busiServerIpAndPort.Add(item.Key, ip + ":" + port);
                }
                return m_busiServerIpAndPort;
            }
            set { m_busiServerIpAndPort.Clear(); }
        }

        protected void WriteIniFile(string key, string value)
        {
            ConfigFile.WriteConfig(mSectionName, key, value);
        }

        protected string ReadIniFile(string key)
        {
            return ConfigFile.ReadConfigAndCreate(mSectionName, key);
        }
    }
}
