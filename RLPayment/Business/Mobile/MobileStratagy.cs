using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.Mobile
{
    class MobileStratagy : BusinessStratagy
    {
        public override string BusinessName
        {
            get { return "Mobile"; }
        }

        public override string MessageActivity
        {
            get { return "�ֻ���ֵͨ�ô�����ʾ"; }
        }

        public override BaseEntity BusinessEntity
        {
            get { return new QMEntity(); }
        }
    }
}
