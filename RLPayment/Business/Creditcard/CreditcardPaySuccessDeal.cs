using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Creditcard  
{
    /// <summary>
    /// ���׳ɹ��Ƿ��ӡ
    /// </summary>
    class CreditcardPaySuccessDeal : Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Yes_Click;
            GetElementById("Return").Click += No_Click;
        }
        void No_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("������");
        }
        void Yes_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("���ÿ��������ڴ�ӡ");
        }
       
    }
}
