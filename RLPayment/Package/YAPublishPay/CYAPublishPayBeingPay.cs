﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using YAPayment.Package;
using Landi.FrameWorks;

namespace YAPayment.Package.YAPublishPay
{
    class CYAPublishPayBeingPay:YAPaymentPay
    {
        protected override void Packet()
        {
            bool bIsIC = false;
            if (CommonData.UserCardType == UserBankCardType.ICCard ||
                CommonData.UserCardType == UserBankCardType.IcMagCard)
                bIsIC = true;

            SendPackage.SetString(0, "0200");
            if (!string.IsNullOrEmpty(CommonData.BankCardNum) && CommonData.BankCardNum.Length != 0)
            {
                SendPackage.SetString(2, CommonData.BankCardNum);
            }
            SendPackage.SetString(3, "190000");
            SendPackage.SetString(4, Utility.AmountToString(CommonData.Amount.ToString()));
            PayEntity.PayTraceNo = GetTraceNo();
            SendPackage.SetString(11, PayEntity.PayTraceNo);
            if (!string.IsNullOrEmpty(CommonData.BankCardExpDate) && CommonData.BankCardExpDate.Length != 0)//卡有效期
            {
                SendPackage.SetString(14, CommonData.BankCardExpDate);
            }
            if (bIsIC)//22
                SendPackage.SetString(22, "051");
            else
                SendPackage.SetString(22, "021");
            if (!string.IsNullOrEmpty(CommonData.BankCardSeqNum) && CommonData.BankCardSeqNum.Length != 0)//卡序列号
            {
                SendPackage.SetString(23, CommonData.BankCardSeqNum);
            }
            SendPackage.SetString(25, "81"); //服务点条件代码
            SendPackage.SetString(26, "06");
            if (!string.IsNullOrEmpty(CommonData.Track2) && CommonData.Track2.Length != 0)
            {
                SendPackage.SetString(35, CommonData.Track2.Replace('=', 'D'));
            }
            if (!string.IsNullOrEmpty(CommonData.Track3) && CommonData.Track3.Length != 0)
            {
                SendPackage.SetString(36, CommonData.Track3.Replace('=', 'D'));
            }
            SendPackage.SetArrayData(48, PacketField48());
            SendPackage.SetString(49, "156");
            SendPackage.SetArrayData(52, Utility.str2Bcd(CommonData.BankPassWord));
            switch (DType)
            {
                case DesType.Des:
                    SendPackage.SetString(53, "2000000000000000");
                    break;
                case DesType.TripleDes:
                    SendPackage.SetString(53, "2600000000000000");
                    break;
            }
            //55
            if (bIsIC && PayEntity.SendField55 != null && PayEntity.SendField55.Length != 0)
            {
                SendPackage.SetArrayData(55, PayEntity.SendField55);
            }
            if (PayEntity.UseICCard)
                SendPackage.SetString(60, "22" + GetBatchNo() + "00050");
            else
                SendPackage.SetString(60, "22" + GetBatchNo() + "000");

            //创建冲正文件 98 96 06
            CReverse_YAPaymentPay cr = new CReverse_YAPaymentPay(this);
            cr.CreateReverseFile("98");
        }

        private byte[] PacketField48()
        {
            byte[] tmp = HeandField48();

            TLVHandler handler = new TLVHandler();
            handler.AddTag("1F50", "0004");
            handler.AddTag("FF28", GetMerchantNo());
            handler.AddTag("FF29", GetTerminalNo());
            handler.AddTag("BF05", PayEntity.QueryTraceNo);
            if (PayEntity.PublishPayType == Entity.YaPublishPayType.TV)
            {
                handler.AddTag("2F1A", PayEntity.SelectMonth.ToString());
                handler.AddTag("2F2A", Utility.AmountToString(PayEntity.SelectPrice.ToString()).TrimStart('0'));
            }
            byte[] content = handler.GetTLVWithLength(3);
            byte[] result = new byte[tmp.Length + content.Length + 1];
            Array.Copy(tmp, result, tmp.Length);
            Array.Copy(content, 0, result, tmp.Length, content.Length);

            Array.Copy(Encoding.Default.GetBytes("#"), 0, result, tmp.Length + content.Length, 1);

            return result;
        }

        private byte[] HeandField48()
        {
            string field48 = "";
            switch (PayEntity.PublishPayType)
            {
                case Entity.YaPublishPayType.Gas:
                    {
                        field48 = "U1V256ZO67700000" + PayEntity.UserID.PadRight(50, ' ') + "000000";
                        break;
                    }
                case Entity.YaPublishPayType.Water:
                    {
                        field48 = "U0V256XO67700000" + PayEntity.UserID.PadRight(50, ' ') + "000000";
                        break;
                    }
                case Entity.YaPublishPayType.TV:
                    {
                        field48 = "U0V258A167700000" + PayEntity.UserID.PadRight(50, ' ') + "000000";
                        break;
                    }
            }

            return Encoding.Default.GetBytes(field48); ;
        }

        protected override void OnSucc()
        {
            //37域 系统参考号
            PayEntity.PayReferenceNo = RecvPackage.GetString(37);
            //38域
            PayEntity.RecvField38 = RecvPackage.ExistValue(38) ? RecvPackage.GetString(38) : "";
            //55域
            PayEntity.RecvField55 = RecvPackage.ExistValue(55) ? RecvPackage.GetArrayData(55) : new byte[0];
            //48域
            TLVHandler handler = new TLVHandler();
            handler.ParseTLV(Get48TLVBytes());
            PayEntity.PayFlowNo = handler.GetStringValue("BF05");
        }
    }
}
