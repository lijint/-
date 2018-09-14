using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;

namespace YAPayment.Package.TrafficPolice
{
    class CYATrafficPoliceBillQuery : YAPaymentPay
    {
        protected override void Packet()
        {
            SendPackage.SetString(0, "0100");
            SendPackage.SetString(3, "310000");
            SendPackage.SetString(11, GetTraceNo());
            SendPackage.SetString(25, "92"); //�������������
            SendPackage.SetArrayData(48, PacketField48());
            SendPackage.SetString(49, "156");
            SendPackage.SetString(60, "00" + GetBatchNo() + "000");
        }

        private byte[] PacketField48()
        {
            string temp = "C0V2571165000000" + "".PadLeft(50, ' ') + "000000";
            byte[] tmp = Encoding.Default.GetBytes(temp);
            TLVHandler handler = new TLVHandler();
            handler.AddTag("1F50", "0004");
            handler.AddTag("FF28", GetMerchantNo());
            handler.AddTag("FF29", GetTerminalNo());
            handler.AddTag("1F1A", PayEntity.TPDecisionNo);

            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length + 1];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);

            Array.Copy(Encoding.Default.GetBytes("#"), 0, result, tmp.Length + content.Length, 1);

            return result;
        }

        protected override void OnSucc()
        {
            //48��
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());

            PayEntity.TPQueryTraceNo = handler.GetStringValue("BF05");
            PayEntity.TPUserName = handler.GetStringValue("1F1B");
            PayEntity.TPUserID = handler.GetStringValue("1F2B");
            PayEntity.TPPrinAmount = double.Parse(handler.GetStringValue("1F3B")) / 100;
            PayEntity.TPFeeAmount = double.Parse(handler.GetStringValue("1F4B")) / 100;
            PayEntity.TPPayAmount = double.Parse(handler.GetStringValue("1F5B")) / 100;
        }
    }
}
