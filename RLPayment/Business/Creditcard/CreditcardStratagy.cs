using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.Creditcard
{
    class CreditcardStratagy : BusinessStratagy
    {
        public override string BusinessName
        {
            get { return "CreditCard"; }
        }

        public override string MessageActivity
        {
            get { return "���ÿ�����ͨ�ô�����ʾ"; }
        }


        public override BaseEntity BusinessEntity
        {
            get { return new QMEntity(); }
        }
    }
}
