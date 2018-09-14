using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using System.IO;
using YAPayment.Entity;

namespace YAPayment.Package.EMV.YAPaymentEMV
{
    class CYADownAID : YAPaymentPay
    {
        /// <summary>
        /// ���ز�������0-��ѯ 1-���� 2-����
        /// </summary>
        private int m_process = 0;

        #region ��ѯAID
        /// <summary>
        /// 30û����Ϣ��31�Ѿ������������Ϣ��32�ֶ�Σ�33����:AID1,AID2
        /// </summary>
        private byte m_bContinue = 0x30;
        /// <summary>
        /// ���в�ѯaid����
        /// </summary>
        private List<byte[]> m_aidList = new List<byte[]>();
        #endregion

        #region ����AID
        /// <summary>
        /// ���ص���AID�б�
        /// </summary>
        private byte[] m_downAID = new byte[0];
        /// <summary>
        /// ���ص���AID��ϸ����
        /// </summary>
        private byte[] m_downAIDItem = new byte[0];
        /// <summary>
        /// ��������AID��ϸ����
        /// </summary>
        private byte[] m_downAIDAll = new byte[4 * 1024];
        /// <summary>
        /// ����AID��ϸ���ݳ���
        /// </summary>
        private int m_nAllAID = 0;
        #endregion

        public bool DownAID()
        {
            bool result = false;
            try
            {
                m_process = 0;
                if (AIDQuery() != TransResult.E_SUCC)
                    return false;
                m_process = 1;
                if (AIDDown() != TransResult.E_SUCC)
                    return false;
                m_process = 2;
                if (AIDEnd() != TransResult.E_SUCC)
                    return false;

                string strPath = Path.Combine(StartupPath,SectionName + "pbocaid.txt");
                result = CreateFile(strPath, m_downAIDAll, m_nAllAID);
            }
            catch (System.Exception ex)
            {
                result = false;
                Log.Error("[CYADownAID][DownAID]Error", ex);
            }

            return result;
        }

        protected TransResult AIDQuery()
        {
            m_aidList.Clear();
            TransResult eRet = TransResult.E_SUCC;
            do 
            {
                System.Threading.Thread.Sleep(2000);//�Ĵ���ÿ������֮��Ҫͣ��2S
                eRet = Communicate();
            } while (eRet == TransResult.E_SUCC && m_bContinue == 0x32);

            return eRet;
        }

        protected TransResult AIDDown()
        {
            TransResult eRet = TransResult.E_SUCC;
            
            foreach (byte[] item in m_aidList)
            {
                System.Threading.Thread.Sleep(2000);//�Ĵ���ÿ������֮��Ҫͣ��2S

                m_downAID = item;
                m_downAID = new byte[item.Length];
                Array.Copy(item, m_downAID, item.Length);
                eRet = Communicate();
                if (eRet != TransResult.E_SUCC)
                    break;
                Array.Copy(m_downAIDItem, 0, m_downAIDAll, m_nAllAID, m_downAIDItem.Length);
                m_nAllAID += m_downAIDItem.Length;
            }

            return eRet;
        }

        protected TransResult AIDEnd()
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
                        string strCount = "1" + m_aidList.Count.ToString("00");
                        SendPackage.SetString(0, "0820");
                        SendPackage.SetString(60, "00" + GetBatchNo() + "382");
                        SendPackage.SetArrayData(63, Encoding.Default.GetBytes(strCount), 0, 3);
                    } break;
                case 1:
                    {
                        SendPackage.SetString(0, "0800");
                        SendPackage.SetString(60, "00" + GetBatchNo() + "380");
                        SendPackage.SetArrayData(63, m_downAID);
                    } break;
                case 2:
                    {
                        SendPackage.SetString(0, "0800");
                        SendPackage.SetString(60, "00" + GetBatchNo() + "381");
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
                        GetAID(bField63);
                    } break;
                case 1:
                    {
                        SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //��¼���κ�
                        byte[] bField63 = RecvPackage.GetArrayData(63);
                        if (bField63[0] == 0x31)
                        {
                            m_downAIDItem = new byte[bField63.Length - 1];
                            Array.Copy(bField63, 1, m_downAIDItem, 0, bField63.Length - 1);
                        }
                    } break;
                case 2:
                    {
                        SetBatchNo(RecvPackage.GetString(60).Substring(2, 6)); //��¼���κ�
                    } break;
            }
        }

        /// <summary>
        /// field62[0]:0û����Ϣ��1�Ѿ������������Ϣ��2�ֶ�Σ�3����:AID1,AID2
        /// </summary>
        /// <param name="field62"></param>
        /// <returns></returns>
        private void GetAID(byte[] field62)
        {
            if (field62[0] == 0x30)
            {
                return;
            }
            byte[] bAID = new byte[field62.Length - 1];
            Array.Copy(field62, 1, bAID, 0, field62.Length - 1);
            int nOffset = 0;
            int nCurr = 0;
            if (field62[0] == 0x31 || field62[0] == 0x32 || field62[0] == 0x33)
            {
                for (; nOffset < bAID.Length; )
                {
                    if (bAID[nOffset] == 0x9F && bAID[nOffset + 1] == 0x06)
                    {
                        nOffset += 2;//TAG 9F06
                        int aidLen = bAID[nOffset];//LEN
                        nOffset += 1;
                        nOffset = nOffset + aidLen;//AID
                        byte[] bAIDItem = new byte[aidLen + 3];
                        Array.Copy(bAID, nCurr, bAIDItem, 0, aidLen + 3);
                        m_aidList.Add(bAIDItem);
                        nCurr = nOffset;
                    }
                    else
                        nOffset++;
                }
            }
        }
    }
}
