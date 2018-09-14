using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks
{
    [SerializableAttribute]
    public abstract class BaseEntity
    {
        #region IC����

        /// <summary>
        /// IC������55��
        /// </summary>
        public byte[] SendField55 = new byte[0];
        /// <summary>
        /// IC������55��
        /// </summary>
        public byte[] RecvField55 = new byte[0];
        /// <summary>
        /// 38��
        /// </summary>
        public string RecvField38 = "";
        /// <summary>
        /// �Ƿ���IC��
        /// </summary>
        public bool IsICCard = false;
        #endregion

        /// <summary>
        /// ҵ������
        /// </summary>
        public string BusinessName = "";
        /// <summary>
        /// ֧����ˮ��
        /// </summary>
        public string PayTraceNo = "";
        /// <summary>
        /// ϵͳ�ο���
        /// </summary>
        public string PayReferenceNo = "";

        /// <summary>
        /// ���ýڵ�����
        /// </summary>
        public abstract string SectionName
        {
            get;
        }

        /// <summary>
        /// ҵ�񷵻���
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mean"></param>
        /// <param name="show"></param>
        public virtual void ParseRespMessage(string code, ref string mean, ref string show)
        {
        }

        /// <summary>
        /// ���ݿ⿪��
        /// </summary>
        public bool AccessSwitch
        {
            get { return ReadIniFile("AccessSwitch") == "1"; }
            set { WriteIniFile("AccessSwitch", value ? "1" : "0"); }
        }
        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public string AccessProviderName
        {
            get { return ReadIniFile("AccessProviderName"); }
            set { WriteIniFile("AccessProviderName", value); }
        }
        /// <summary>
        /// ���ݿ��ļ�
        /// </summary>
        public string AccessFile
        {
            get { return ReadIniFile("AccessFile"); }
            set { WriteIniFile("AccessFile", value); }
        }
        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public string AccessPin
        {
            get { return ReadIniFile("AccessPin"); }
            private set { WriteIniFile("AccessPin", value); }
        }

        /// <summary>
        /// ��ʼ������AID��CA
        /// </summary>
        public bool DownLoadAidAndCA
        {
            get { return ReadIniFile("DownLoadAidAndCA") == "1"; }
            set { WriteIniFile("DownLoadAidAndCA", value ? "1" : "0"); }
        }

        /// <summary>
        /// �Ƿ�����IC��
        /// </summary>
        public bool UseICCard
        {
            get { return ReadIniFile("UseICCard") == "1"; }
            set { WriteIniFile("UseICCard", value ? "1" : "0"); }
        }

        /// <summary>
        /// ����ʵ���Ƿ�����
        /// </summary>
        public bool IsUseEntity
        {
            get { return ReadIniFile("Use") == "1"; }
            private set { WriteIniFile("Use", value ? "1" : "0"); }
        }

        protected void WriteIniFile(string key, string value)
        {
            ConfigFile.WriteConfig(SectionName, key, value);
        }

        protected string ReadIniFile(string key)
        {
            return ConfigFile.ReadConfigAndCreate(SectionName, key);
        }
    }
}
