using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Entity
{
    class QMEntity : BaseEntity
    {
        #region ����

        public const string SECTION_NAME = "YAPayment";

        #endregion

        #region ���ÿ�����
        /// <summary>
        /// ���ÿ���
        /// </summary>
        public string CreditcardNum;
        /// <summary>
        /// �����ܽ��
        /// </summary>
        public double TotalAmount;
        /// <summary>
        /// ����������
        /// </summary>
        public double Fee;
        /// <summary>
        /// ��������
        /// </summary>
        public string Field15;
        #endregion
        
        #region �ֻ�����ֱ��

        /// <summary>
        /// ��Ӫ�����(01-�ƶ� 02-��ͨ 03-����)
        /// </summary>
        public string MobileType; //��Ӫ�����(01-�ƶ� 02-��ͨ 03-����)
        /// <summary>
        /// �ֻ�����
        /// </summary>
        public string PhoneNo; //�ֻ�����
        /// <summary>
        /// �м�ҵ����ˮ��
        /// </summary>
        public string MiddleFlowNo; //�м�ҵ����ˮ��

        #endregion

        public QMEntity()
        {
#if DEBUG
            Fee = 2;
            TotalAmount = CommonData.Amount + Fee;

            MiddleFlowNo = "00000012347800000012347800000000";
            PayReferenceNo = "000000123478";
            RecvField38 = "";
            RecvField55 = new byte[0];
#endif
        }

        public override string SectionName
        {
            get { return SECTION_NAME; }
        }
    }
}
