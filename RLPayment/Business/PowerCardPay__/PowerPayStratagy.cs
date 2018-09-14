using System;
using System.Collections.Generic;
using System.Text;
using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.PowerCardPay
{
    class PowerPayStratagy : BusinessStratagy
    {
        public const string BUSINESSNAME = "PowerPay";

        public override string BusinessName
        {
            get { return BUSINESSNAME; }
        }

        public override string MessageActivity
        {
            get { return "����֧��ͨ�ô�����ʾ"; }
        }

        public override BaseEntity BusinessEntity
        {
            get { return new PowerEntity(); }
        }
    }
}
