using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Business.YAPublishPay;
using YAPayment.Business.TrafficPolice;

namespace YAPayment.Entity
{
    enum YaPublishPayType : int
    {
        Water,
        Gas,
        Power,
        TV,
        None,
    }

    class YAEntity : BaseEntity
    {
        #region ����

        public const string SECTION_NAME = "YAPayment";

        #endregion

        public YaPublishPayType PublishPayType = YaPublishPayType.None;

        #region ��ѯ
        
        /// <summary>
        /// �û���
        /// </summary>
        public string UserID = "";
        /// <summary>
        /// �˵���ѯ��ˮ
        /// </summary>
        public string QueryTraceNo = "";
        /// <summary>
        /// �û���
        /// </summary>
        public string UserName = "";
        /// <summary>
        /// �û���ַ
        /// </summary>
        public string UserAddress = "";
        /// <summary>
        /// ��ѯǷ�ѽ��
        /// </summary>
        public double QueryAmount = 0;
        /// <summary>
        /// ʹ�ÿ�ʼ����
        /// </summary>
        public string QueryDateStart = "";
        /// <summary>
        /// ʹ�ý�������
        /// </summary>
        public string QueryDateEnd = "";

        //Water
        /// <summary>
        /// ˮ�����ɽ�
        /// </summary>
        public double WaterFee = 0;
        /// <summary>
        /// ˮ���ܽ��
        /// </summary>
        public double WaterTotalAmount = 0;

        //TV
        /// <summary>
        /// �ʷ�1
        /// </summary>
        public double Price1 = 0;
        /// <summary>
        /// �ʷ�2
        /// </summary>
        public double Price2 = 0;
        /// <summary>
        /// �ʷ���Ϣ
        /// </summary>
        public string PriceInfo = "";
        /// <summary>
        /// ѡ����ʷ�
        /// </summary>
        public double SelectPrice = 0;
        /// <summary>
        /// ѡ���ʱ��
        /// </summary>
        public int SelectMonth = 0;

        #endregion

        #region ֧��

        /// <summary>
        /// �˵�֧����ˮ
        /// </summary>
        public string PayFlowNo = "";
        /// <summary>
        /// �˵�������ˮ
        /// </summary>
        public string ConfirmTraceNo = "";

        #endregion

        #region ������û����

        #region �Ϸ�
        /// <summary>
        /// ���ƺ�
        /// </summary>
        public string LicensePlant = "";
        /// <summary>
        /// ���ܺţ�ĩ6λ��
        /// </summary>
        public string CarId = "";
        /// <summary>
        /// ��ʻ֤��
        /// </summary>
        public string LicenseNo = "";
        /// <summary>
        /// �������
        /// </summary>
        public string CarType = "";
        /// <summary>
        /// ����ҳ��
        /// </summary>
        public string CurrentIndex = "1";//��һ��Ϊ1
        /// <summary>
        /// Υ����ϸ����
        /// </summary>
        public string InquiryInfo = "";//���|Υ��ʱ��|�۷�|������|Υ����ַ&���|Υ��ʱ��|�۷�|������|Υ����ַ&���|Υ��ʱ��|�۷�|������|Υ����ַ&����ÿ����Ϣѭ����ʾ��
        #endregion


        #region ����
        /// <summary>
        /// ����֧����ˮ��
        /// </summary>
        public string TPPayFlowNo2 = "";

        #endregion

        #region ��ѯ
        /// <summary>
        /// ��������
        /// </summary>
        public string TPDecisionNo = "";
        /// <summary>
        /// ��ѯ��ˮ��
        /// </summary>
        public string TPQueryTraceNo = "";
        /// <summary>
        /// �û���
        /// </summary>
        public string TPUserName = "";
        /// <summary>
        /// ���֤����
        /// </summary>
        public string TPUserID = "";
        /// <summary>
        /// �����
        /// </summary>
        public double TPPrinAmount = 0;
        /// <summary>
        /// ���ɽ�
        /// </summary>
        public double TPFeeAmount = 0;
        /// <summary>
        /// �����ܽ��
        /// </summary>
        public double TPPayAmount = 0;

        #endregion

        #region ֧��
        /// <summary>
        /// ֧����ˮ
        /// </summary>
        public string TPPayFlowNo = "";
        #endregion

        #region ȷ��
        ///<summary>
        /// �˵�������ˮ
        /// </summary>
        public string TPConfirmTraceNo = "";
        #endregion

        #region ����ҵ�񷵻���
        private void TPRespMessage(string code, ref string mean, ref string show)
        {
            switch (code)
            {
                case "M1": { mean = "�ѽɷ����"; show = "�ѽɷ����,�벻Ҫ���½ɷ�"; } break;
                case "M3": { mean = "�ɷ�δ���"; show = "�ɷ�δ��ϣ����Ժ�����"; } break;
                case "M4": { mean = "�������Ų�����"; show = "�������Ų����ڣ���������ľ�������"; } break;
                case "M5": { mean = "����ʧ��"; show = "����ʧ��"; } break;
                case "N1": { mean = "�÷����ѽɷѣ���ȴ�����������"; show = "�÷����ѽɷѣ���ȴ�����������"; } break;
                case "N2": { mean = "������ų�������"; show = "������ų�������"; } break;
                case "N3": { mean = "������ű�����51��ͷ"; show = "������ű�����51��ͷ"; } break;
                case "N4": { mean = "22:30 - 01:30ֹͣ����"; show = "22:30 - 01:30ֹͣ����"; } break;
                case "EE": { mean = "����ʧ��"; show = "����ʧ��"; } break;
            }
        }
        #endregion

        #endregion

        public YAEntity()
        {

#if DEBUG
            QueryTraceNo = "000000123456";
            UserName = "����";
            UserAddress = "����ʡ�����й�¥��";
            QueryAmount = 12;
            QueryDateStart = "201406";
            QueryDateEnd = "201407";

            Price1 = 25.00;
            Price2 = 30.00;

            WaterFee = 1;
            WaterTotalAmount = 12;

            PayFlowNo = "000000123478";
            ConfirmTraceNo = "00000012347800000012347800000000";
            PayReferenceNo = "000000123478";
            RecvField38 = "";
            RecvField55 = new byte[0];
#endif
        }

        public override string SectionName
        {
            get { return SECTION_NAME; }
        }

        public override void ParseRespMessage(string code, ref string mean, ref string show)
        {
            switch (BusinessName)
            {
                case YATrafficPoliceStratagy.BUSINESSNAME: { TPRespMessage(code, ref mean, ref show); } break;
            }
        }
    }
}
