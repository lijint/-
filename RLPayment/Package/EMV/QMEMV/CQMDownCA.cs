using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.IO;

namespace YAPayment.Package.EMV.QMEMV
{
    class CQMDownCA : QMPay
    {
        /// <summary>
        /// ���ع�Կ����0-��ѯ 1-���� 2-����
        /// </summary>
        private int m_process = 0;

        internal class CaItem
        {
            /// <summary>
            /// RID tlv 9F06
            /// </summary>
            public byte[] cRid;
            /// <summary>
            /// ���� 9F22
            /// </summary>
            public byte[] cIndex;
            /// <summary>
            /// ��Ч�� DF05
            /// </summary>
            public byte[] cDate;
        }

        #region ��ѯCA
        /// <summary>
        /// 30û����Ϣ��31�Ѿ������������Ϣ��32�ֶ�Σ�33����:AID1,AID2
        /// </summary>
        private byte m_bContinue = 0x30;
        /// <summary>
        /// ���в�ѯCA����
        /// </summary>
        private List<CaItem> m_caList = new List<CaItem>();
        #endregion

        #region ����CA
        /// <summary>
        /// ���ص���CA�б� RID + ����
        /// </summary>
        private byte[] m_downCA = new byte[0];
        /// <summary>
        /// ���ص���CA��ϸ����
        /// </summary>
        private byte[] m_downCAItem = new byte[0];
        /// <summary>
        /// ��������CA��ϸ����
        /// </summary>
        private byte[] m_downCAAll = new byte[4 * 1024];
        /// <summary>
        /// ����CA��ϸ���ݳ���
        /// </summary>
        private int m_nAllCA = 0;
        #endregion


        public bool DownPublishCA()
        {
            bool result = false;
            try
            {
                m_process = 0;
                if (CAQuery() != TransResult.E_SUCC)
                    return false;
                m_process = 1;
                if (CADown() != TransResult.E_SUCC)
                    return false;
                m_process = 2;
                if (CAEnd() != TransResult.E_SUCC)
                    return false;

                string strPath = Path.Combine(StartupPath, SectionName + "pbocCA.txt");
                result = CreateFile(strPath, m_downCAAll, m_nAllCA);

            }
            catch (System.Exception ex)
            {
                result = false;
                Log.Error("[CQMDownCA][DownPublishCA]Error", ex);
            }

            return result;
        }

        protected TransResult CAQuery()
        {
            m_caList.Clear();
            TransResult eRet = TransResult.E_SUCC;
            do
            {
                System.Threading.Thread.Sleep(2000);//�Ĵ���ÿ������֮��Ҫͣ��2S
                eRet = Communicate();
            } while (eRet == TransResult.E_SUCC && m_bContinue == 0x32);

            return eRet;
        }

        protected TransResult CADown()
        {
            TransResult eRet = TransResult.E_SUCC;

            foreach (CaItem item in m_caList)
            {
                System.Threading.Thread.Sleep(2000);//�Ĵ���ÿ������֮��Ҫͣ��2S

                int nLen = item.cRid.Length + item.cIndex.Length;
                m_downCA = new byte[nLen];
                Array.Copy(item.cRid, m_downCA, item.cRid.Length);//RID
                Array.Copy(item.cIndex, 0, m_downCA, item.cRid.Length, item.cIndex.Length);//����

                eRet = Communicate();
                if (eRet != TransResult.E_SUCC)
                    break;

                Array.Copy(m_downCAItem, 0, m_downCAAll, m_nAllCA, m_downCAItem.Length);
                m_nAllCA += m_downCAItem.Length;
            }

            return eRet;
        }

        protected TransResult CAEnd()
        {
            System.Threading.Thread.Sleep(2000);//�Ĵ���ÿ������֮��Ҫͣ��2S
            return Communicate();
        }

        protected override void Packet()
        {
            switch (m_process)
            {
                case 0:
                    {
                        string strCount = "1" + m_caList.Count.ToString("00");
                        SendPackage.SetString(0, "0820");
                        SendPackage.SetString(60, "00" + GetBatchNo() + "372");
                        SendPackage.SetArrayData(63, Encoding.Default.GetBytes(strCount), 0, 3);
                    } break;
                case 1:
                    {
                        SendPackage.SetString(0, "0800");
                        SendPackage.SetString(60, "00" + GetBatchNo() + "370");
                        SendPackage.SetArrayData(63, m_downCA);
                    } break;
                case 2:
                    {
                        SendPackage.SetString(0, "0800");
                        SendPackage.SetString(60, "00" + GetBatchNo() + "371");
                    } break;
            }
        }

        protected override bool NeedCalcMac()
        {
            return false;
        }

        protected override void OnBeforeTrans()
        {
            SendPackage.Clear();
            RecvPackage.Clear();
        }

        protected override void OnSucc()
        {
            switch (m_process)
            {
                case 0:
                    {
                        SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //��¼���κ�
                        byte[] bField63 = RecvPackage.GetArrayData(63);
                        m_bContinue = bField63[0];
                        GetCARID(bField63);
                    } break;
                case 1:
                    {
                        SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //��¼���κ�
                        byte[] bField63 = RecvPackage.GetArrayData(63);
                        if (bField63[0] == 0x31)
                        {
                            m_downCAItem = new byte[bField63.Length - 1];
                            Array.Copy(bField63, 1, m_downCAItem, 0, bField63.Length - 1);
                            Dictionary<string, byte[]> ht = new Dictionary<string, byte[]>();
                            TLVHandler.ParseTLV(m_downCAItem, ht);
                            if (!ValidatorCA(ht))
                                SetResult(TransResult.E_UNPACKET_FAIL);
                        }
                    } break;
                case 2:
                    {
                        SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //��¼���κ�
                    } break;
            }
        }


        /// <summary>
        /// 0û�й�Կ��Ϣ��1�Ѿ���������й�Կ��Ϣ��2�ֶ�Σ�3����:RID,����,��Ч��
        /// </summary>
        /// <param name="field62"></param>
        /// <returns></returns>
        private void GetCARID(byte[] field62)
        {
            if (field62[0] == 0x30)
            {
                return;
            }
            byte[] bCa = new byte[field62.Length - 1];
            Array.Copy(field62, 1, bCa, 0, bCa.Length);
            int nOffset = 0;
            if (field62[0] == 0x31 || field62[0] == 0x32 || field62[0] == 0x33)
            {
                for (; nOffset < bCa.Length; )
                {
                    if (bCa[nOffset] == 0x9F && bCa[nOffset + 1] == 0x06)
                    {
                        CaItem caItem = new CaItem();
                        nOffset += 2;//TAG 9F06
                        int ridLen = bCa[nOffset];//LEN
                        caItem.cRid = new byte[ridLen + 3];
                        nOffset += 1;
                        caItem.cRid[0] = 0x9F;
                        caItem.cRid[1] = 0x06;
                        caItem.cRid[2] = (byte)ridLen;
                        Array.Copy(bCa, nOffset, caItem.cRid, 3, ridLen);//RID
                        nOffset = nOffset + ridLen;

                        nOffset += 2;//TAG 9F22
                        int indexLen = bCa[nOffset];//LEN
                        caItem.cIndex = new byte[indexLen + 3];
                        nOffset += 1;
                        caItem.cIndex[0] = 0x9F;
                        caItem.cIndex[1] = 0x22;
                        caItem.cIndex[2] = (byte)indexLen;
                        Array.Copy(bCa, nOffset, caItem.cIndex, 3, indexLen);//INDEX
                        nOffset = nOffset + indexLen;

                        nOffset += 2;//TAG DF05
                        int dateLen = bCa[nOffset];//LEN
                        caItem.cDate = new byte[dateLen + 3];
                        nOffset += 1;
                        caItem.cDate[0] = 0xDF;
                        caItem.cDate[1] = 0x05;
                        caItem.cDate[2] = (byte)dateLen;
                        Array.Copy(bCa, nOffset, caItem.cDate, 3, dateLen);//DATE
                        nOffset = nOffset + dateLen;

                        m_caList.Add(caItem);
                    }
                    else
                        nOffset++;
                }
            }
        }

        private bool ValidatorCA(Dictionary<string, byte[]> ht)
        {
            bool ret = true;
            try
            {
                string[] TagName = new string[] { "9F06", "9F22", "DF05", "DF06", "DF07", "DF02", "DF04", "DF03" };
                for (int i = 0; i < TagName.Length; i++)
                {
                    if (!ht.ContainsKey(TagName[i]))
                    {
                        return false;
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.Error("CDownPublicCA ValidatorCA Failed!", e);
                ret = false;
            }

            return ret;
        }
    }
}
