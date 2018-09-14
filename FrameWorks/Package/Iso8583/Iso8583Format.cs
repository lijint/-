using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks.Iso8583
{
    /// <summary>
    /// ��ʾ ISO 8583 �����ֶθ�ʽ
    /// </summary>
    public enum Iso8583Format
    {
        /// <summary>
        /// ������
        /// </summary>
        None,
        /// <summary>
        /// �ɱ䳤��ĳ���ֵ��һλ����ռ��1���ַ���
        /// </summary>
        LVAR,
        /// <summary>
        /// �ɱ䳤��ĳ���ֵ����λ����ռ��2���ַ���
        /// </summary>
        LLVAR,
        /// <summary>
        /// �ɱ䳤��ĳ���ֵ����λ����ռ��3���ַ���
        /// </summary>
        LLLVAR,
        /// <summary>
        /// ������
        /// </summary>
        YYMMDD,
        /// <summary>
        /// ����
        /// </summary>
        YYMM,
        /// <summary>
        /// ����
        /// </summary>
        MMDD,
        /// <summary>
        /// ʱ����
        /// </summary>
        hhmmss,
        /// <summary>
        /// ����ʱ����
        /// </summary>
        MMDDhhmmss
    }
}
