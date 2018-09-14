using System;
using System.Collections.Generic;
using System.Text;

namespace Landi.FrameWorks.Iso8583
{
    /// <summary>
    /// ��ʾ ISO 8583 ������������
    /// </summary>
    public enum Iso8583DataType
    {
        // ����A��ʾ��ѹ����B��ʾѹ���⣬R��ʾ��ѹ����������������Ϊbyte�ͣ�������ʱû��

        /// <summary>
        /// ��ĸ�ַ���A��Z��a��z���󿿣��Ҳ����ಿ����ո�
        /// </summary>
        A,
        /// <summary>
        /// ������λ���󿿣��Ҳ����ಿ�����㡣
        /// </summary>
        B,
        /// <summary>
        /// 16�������ݣ����ǲ�ѹ��
        /// </summary>
        R,

        /// <summary>
        /// �����ַ���0��9���ҿ�����߶��ಿ�����㡣����ʾ����С������ţ������λ��ʾ�Ƿ�
        /// </summary>
        N,
        /// <summary>
        /// �����ַ�
        /// </summary>
        S,
        /// <summary>
        /// ������ţ�����Ϊ��C�������Ϊ��D�������һ�������ͽ������Ԫ��
        /// </summary>
        X,
        /// <summary>
        /// ��ISO 7811��ISO 7813�ƶ��Ĵſ��ڶ������ŵ�����
        /// </summary>
        Z,
        /// <summary>
        /// ��ĸ�����֣��󿿣��Ҳ����ಿ����ո�
        /// </summary>
        AN,
        /// <summary>
        /// ��ĸ�����ֻ�������ţ��󿿣��Ҳ����ಿ����ո�
        /// </summary>
        ANS
    }
}