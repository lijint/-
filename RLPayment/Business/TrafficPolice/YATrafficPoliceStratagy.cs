using Landi.FrameWorks;
using YAPayment.Entity;

namespace YAPayment.Business.TrafficPolice
{
    class YATrafficPoliceStratagy : BusinessStratagy
    {
        public const string BUSINESSNAME = "YATrafficPolice";

        public override string BusinessName
        {
            get { return BUSINESSNAME; }
        }

        public override string MessageActivity
        {
            get { return "�Ű�������ûͨ�ô�����ʾ"; }
        }

        public override BaseEntity BusinessEntity
        {
            get { return new YAEntity(); }
        }
    }
}
