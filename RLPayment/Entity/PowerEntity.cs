using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Package.PowerCardPay;
using YAPayment.Business.PowerCardPay;

namespace YAPayment.Entity
{
    class PowerEntity : BaseEntity
    {
        #region ����

        public const string SECTION_NAME = "Power";

        #endregion

        public PowerCardInfo PowerCardData = new PowerCardInfo();

        /// <summary>
        /// �����ɷ����� Ĭ�� 1=���翨��ֵ 2=�û��Žɷ�
        /// </summary>
        public int PowerBusiness = 0;

        #region ��ѯ
        
        /// <summary>
        /// �翨�û���
        /// </summary>
        public string UserID = "";
        /// <summary>
        /// �˵���ѯ��ˮ
        /// </summary>
        public string QueryTraceNo = "";
        /// <summary>
        /// �û��������
        /// </summary>
        public int PowerPayCount = 0;
        /// <summary>
        /// �û�����
        /// </summary>
        public string UserName = "";
        /// <summary>
        /// �����ʽ���
        /// </summary>
        public string EleFeeNum = "";
        /// <summary>
        /// �����ʽ�λ������
        /// </summary>
        public string EleFeeAccountNum = "";
        /// <summary>
        /// �����ʽ�λ���㵥λ
        /// </summary>
        public string EleFeeAccountName = "";
        /// <summary>
        /// ����������
        /// </summary>
        public string PowerAreaNum = "";
        /// <summary>
        /// �翨����
        /// </summary>
        public string PowerCardNo = "";
        /// <summary>
        /// ���
        /// </summary>
        public double PayAmount = 0;
        /// <summary>
        /// ���ܱ��ʶ
        /// </summary>
        public string PowerIdentity = "";
        /// <summary>
        /// �û����絥λ����
        /// </summary>
        public string PowerAdvName = "";
        /// <summary>
        /// ������Ӧ˵��
        /// </summary>
        public string PowerReturnMsg = "";

        /// <summary>
        /// �û���(�󸶷ѣ�
        /// </summary>
        public string DBNo = "";
        /// <summary>
        /// �û���ַ
        /// </summary>
        public string UserAddress = "";
        /// <summary>
        /// �û����
        /// </summary>
        public double UserAmount = 0;
        /// <summary>
        /// ����Ӧ�����
        /// </summary>
        public double UserPayAmount = 0;
        /// <summary>
        /// �����
        /// </summary>
        public List<UserQueryInfo> UserQueryInfo;


        /// <summary>
        /// ���ڳ��е�ҵ�ֱ���
        /// </summary>
        public string CityPowerNo
        {
            get { return ReadIniFile("CityPowerNo"); }
        }

        #endregion

        #region ֧��

        /// <summary>
        /// �˵�֧����ˮ
        /// </summary>
        public string PayFlowNo = "";
        #endregion

        #region ȷ��
        ///<summary>
        /// �˵�ȷ����ˮ
        /// </summary>
        public string ConfirmTraceNo = "";
        ///<summary>
        /// ��Ϣ��־λ
        /// </summary>
        public string MsgFlag = "";
        ///<summary>
        /// ����κ�
        /// </summary>
        public string PowerSegNo = "";
        ///<summary>
        /// ���ι�����
        /// </summary>
        public double PowerPayAmount = 0;
        ///<summary>
        /// �õ����
        /// </summary>
        public string PowerUseType = "";
        ///<summary>
        /// ��������˵��
        /// </summary>
        public string PowerRetMsg = ""; 
        ///<summary>
        /// �������񶩵���
        /// </summary>
        public string PowerPayConfirmCode = "";
        ///<summary>
        /// д�����
        /// </summary>
        public double WriteCardAmount = 0;

        #endregion

        #region ��д��

        ///<summary>
        /// ��д����һ����ˮ��(��һ�η���)
        /// </summary>
        public string ReWriteCardTraceNo1 = "";
        ///<summary>
        /// ��д����һ����ˮ��(�ڶ��η���)
        /// </summary>
        public string ReWriteCardTraceNo2 = "";
        ///<summary>
        /// ��д����һ����ˮ��(�����η���)
        /// </summary>
        public string ReWriteCardTraceNo3 = "";
        ///<summary>
        /// ��д����������˵����Ϣ
        /// </summary>
        public string ReWriteCardReturnMsg = "";
        ///<summary>
        /// ��д��д�����
        /// </summary>
        public string ReWriteCardAmount = "";
        ///<summary>
        /// ��д���û����
        /// </summary>
        public string ReWriteUserAmount = "";
        #endregion

        #region У��
        ///<summary>
        /// У�鷵����һ����ˮ��(��һ�η���)
        /// </summary>
        public string CheckTraceNo = "";
        ///<summary>
        /// У�鷵��д�����
        /// </summary>
        public string CheckWriteAmount = "";
        ///<summary>
        /// У�鷵���û����
        /// </summary>
        public string CheckUserAmount = "";
        ///<summary>
        /// У�鷵�ض���״̬
        /// </summary>
        public string CheckOrderStatus = "";
        ///<summary>
        /// У�鷵�س�ֵ���
        /// </summary>
        public string CheckRechargeAmount = "";
        ///<summary>
        /// У�鷵�س��
        /// </summary>
        public string CheckRemuneration = "";
        ///<summary>
        /// У�鷵�ع������
        /// </summary>
        public string CheckBuyEleTimes = "";
        ///<summary>
        /// У�鷵�ص�������˵����Ϣ
        /// </summary>
        public string CheckReturnMsg = ""; 
        ///<summary>
        /// У�鷵��File1
        /// </summary>
        public string CheckBF1 = "";
        ///<summary>
        /// У�鷵��File2
        /// </summary>
        public string CheckBF2 = "";
        ///<summary>
        /// У�鷵��File31
        /// </summary>
        public string CheckBF31 = "";
        ///<summary>
        /// У�鷵��File32
        /// </summary>
        public string CheckBF32 = "";
        ///<summary>
        /// У�鷵��File41
        /// </summary>
        public string CheckBF41 = "";
        ///<summary>
        /// У�鷵��File42
        /// </summary>
        public string CheckBF42 = "";
        ///<summary>
        /// У�鷵��File5
        /// </summary>
        public string CheckBF5 = "";
        ///<summary>
        /// У�鷵��sKey1
        /// </summary>
        public string Check57sKey1 = "";
        ///<summary>
        /// У�鷵��sKey2
        /// </summary>
        public string Check57sKey2 = "";
        ///<summary>
        /// У�鷵��sKey3
        /// </summary>
        public string Check57sKey3 = "";
        #endregion


        public PowerEntity()
        {

#if DEBUG
            QueryTraceNo = "000000123456";
            UserID = "12345678";
            UserName = "��ƽƷ";
            UserAddress = "����ʡ������ϼ���س�������ˮ��26��";
            PowerCardNo = "12345678";
            DBNo = "N12345678";
            UserAmount = 1.23;
            UserPayAmount = 13.00;

            PayFlowNo = "000000123478";
            ConfirmTraceNo = "00000012347800000012347800000000";
            PayReferenceNo = "000000123478";
            RecvField38 = "";
            RecvField55 = new byte[0];
            PayAmount = 20;
#endif
        }

        #region ����ҵ�񷵻���
        private void PowerRespMessage(string code, ref string mean, ref string show)
        {
            switch (code)
            {
                case "M3": { mean = "������"; show = "������"; } break;
                case "N0": { mean = "������ϣ�������"; show = "������ϣ�������"; } break;
                case "N1": { mean = "22:50-00:10���ܽɷ�"; show = "22:50-00:10���ܽɷ�"; } break;
                case "N2": { mean = "�ڲ��쳣��������"; show = "�ڲ��쳣��������"; } break;
            }
        }

        public override void ParseRespMessage(string code, ref string mean, ref string show)
        {
            switch (BusinessName)
            {
                case PowerPayStratagy.BUSINESSNAME: { PowerRespMessage(code, ref mean, ref show); } break;
            }
        }

        #endregion

        public override string SectionName
        {
            get { return SECTION_NAME; }
        }
    }
}
