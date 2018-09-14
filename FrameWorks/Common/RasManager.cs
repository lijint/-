using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Landi.FrameWorks
{
    public class RasManager
    {
        public const int RAS_MaxDeviceName = 32;
        public const int RAS_MaxEntryName = 256;
        public const int RAS_MaxPhoneNumber = 128;
        public const int UNLEN = 256;
        public const int PWLEN = 256;
        public const int DNLEN = 15;
        public const int MAX_PATH = 260;
        public const int RAS_MaxDeviceType = 16;
        public const int RAS_MaxCallbackNumber = RAS_MaxPhoneNumber;
        private IntPtr m_ConnectedRasHandle;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct RASCONN
        {
            public int dwSize;
            public IntPtr hrasconn;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
            public string szEntryName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceType + 1)]
            public string szDeviceType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceName + 1)]
            public string szDeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szPhonebook;
            public int dwSubEntry;
        }
        public delegate void Callback(uint unMsg, int rasconnstate, int dwError);


        [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
        public struct RASDIALPARAMS
        {
            public int dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
            public string szEntryName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxPhoneNumber + 1)]
            public string szPhoneNumber;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxCallbackNumber + 1)]
            public string szCallbackNumber;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = UNLEN + 1)]
            public string szUserName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PWLEN + 1)]
            public string szPassword;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DNLEN + 1)]
            public string szDomain;
            public int dwSubEntry;
            public int dwCallbackId;
        }

        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        public extern static uint RasHangUp(
            IntPtr hrasconn  // handle to the RAS connection to hang up
            );

        [DllImport("Rasapi32.dll", EntryPoint = "RasEnumConnectionsA",
             SetLastError = true)]
        public static extern int RasEnumConnections
            (
                ref RASCONN lprasconn, // buffer to receive connections data
                ref int lpcb, // size in bytes of buffer
                ref int lpcConnections // number of connections written to buffer
            );

        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern int RasDial(int lpRasDialExtensions, string lpszPhonebook,
            ref RASDIALPARAMS lprasdialparams, int dwNotifierType,
            Callback lpvNotifier, ref int lphRasConn);

        private RASDIALPARAMS RasDialParams;
        private int Connection = 0;

        public RasManager()
        {
            Connection = 0;
            RasDialParams = new RASDIALPARAMS();
            RasDialParams.dwSize = Marshal.SizeOf(RasDialParams);
        }

        #region Properties
        public string UserName
        {
            get
            {
                return RasDialParams.szUserName;
            }
            set
            {
                RasDialParams.szUserName = value;
            }
        }

        public string Password
        {
            get
            {
                return RasDialParams.szPassword;
            }
            set
            {
                RasDialParams.szPassword = value;
            }
        }

        public string PhoneNumber
        {
            get
            {
                return RasDialParams.szPhoneNumber;
            }
            set
            {
                RasDialParams.szPhoneNumber = value;
            }
        }
        #endregion

        /// <summary>
        /// �κ�
        /// </summary>
        /// <returns>�Ƿ�ɹ�</returns>
        public int Connect()
        {
            Callback rasDialFunc = new Callback(RasManager.RasDialFunc);
            RasDialParams.szPhoneNumber += "\0";
            RasDialParams.szUserName += "\0";
            RasDialParams.szPassword += "\0";
            Log.Info("Connect Function! RasDialNumber=" + RasDialParams.szPhoneNumber);
            int result = RasDial(0, null, ref RasDialParams, 0, null, ref Connection);
            return result;
        }

        public static void RasDialFunc(uint unMsg, int rasconnstate, int dwError)
        {
            string strMsg = "";
           
            switch (rasconnstate)
            {
                case 0:

                    strMsg = "���ڴ򿪣�����";
                    break;
                case 1:
                    strMsg = "�˿��Ѿ��򿪣�";
                    break;
                case 2:
                    strMsg = "���������豸������";
                    break;
                case 3:
                    strMsg = "�豸�Ѿ�����";
                    break;
                case 4:
                    strMsg = "�����豸�Ѿ�����";
                    break;
                case 5:
                    strMsg = "������֤�û������������";
                    break;
                case 6:
                    strMsg = "��֤ͨ�森����";
                    break;
                case 7:
                    strMsg = "��֤���ԣ�����";
                    break;
                case 8:
                    strMsg = "��֤�ؽУ�����";
                    break;
                case 9:
                    strMsg = "��֤�ؽУ�����";
                    break;
                case 10:
                    strMsg = "��֤��Ŀ������";
                    break;
                case 11:
                    strMsg = "��֤�����ٶȣ�����";
                    break;
                case 12:
                    strMsg = "��֤���󣮣���";
                    break;
                case 13:
                    strMsg = "������֤������";
                    break;
                case 14:
                    strMsg = "��֤��ɣ�";
                    break;
                case 15:
                    strMsg = "׼���ؽУ�����";
                    break;
                case 16:
                    strMsg = "�ȴ����ƽ������λ";
                    break;
                case 17:
                    strMsg = "�ȴ��ؽУ�����";
                    break;
                case 18:
                    strMsg = "projected";
                    break;
                case 19:
                    strMsg = "��ʼ����������";
                    break;
                case 20:
                    strMsg = "�ؽ���ɣ�";
                    break;
                case 21:
                    strMsg = "���ڵ�¼���磮����";
                    break;
                case 4096:
                    strMsg = "�����Ѿ��ɹ���";
                    break;
                case 4097:
                    strMsg = "���¼���������";
                    break;
                case 4098:
                    strMsg = "���ûؽУ�����";
                    break;
                case 4099:
                    strMsg = "�������";
                    break;
                case 8192:
                    strMsg = "�Ѿ����ӣ�";
                    break;
                case 8193:
                    strMsg = "�Ѿ��Ͽ���";
                    break; 
            }
            Log.Info("rasManager return " + strMsg);
        }
        /// <summary>
        /// �Ƿ���������
        /// </summary>
        /// <returns></returns>
        public bool isConnect()
        {
            bool m_connected = true;
            RASCONN lprasConn = new RASCONN();
            //lprasConn.dwSize = Marshal.SizeOf(typeof(RASCONN));
            lprasConn.dwSize = 412;
            lprasConn.hrasconn = IntPtr.Zero;

            int lpcb = 0;
            int lpcConnections = 0;
            int nRet = 0;
            lpcb = Marshal.SizeOf(typeof(RASCONN));
            nRet = RasEnumConnections(ref lprasConn, ref lpcb, ref lpcConnections);
            if (nRet != 0)
            {
                m_connected = false;

            } 
            if (lpcConnections == 0)
            {
                m_connected = false;
            }
            if (lpcConnections > 0)
            {
                m_ConnectedRasHandle = lprasConn.hrasconn;
            }

            return m_connected;
        }

        /// <summary>
        /// �Ҷ�
        /// </summary>
        public void Disconnect()
        {
            RasHangUp(m_ConnectedRasHandle);
        }


    }
}


