using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.YAPublishPay
{
    class YAPublishPayStratagy : BusinessStratagy
    {
        public override string BusinessName
        {
            get { return "YAPublishPay"; }
        }

        public override string MessageActivity
        {
            get { return "�Ű�֧��ͨ�ô�����ʾ"; }
        }

        public override BaseEntity BusinessEntity
        {
            get { return new YAEntity(); }
        }
    }
}
