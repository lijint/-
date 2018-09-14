using System.IO;
using System.Text;
using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Creditcard 
{
    class CreditcardNoticeDeal : Activity
    {
        private string mNoticeString = "";

        protected override void OnEnter()
        {
            GetElementById("Notice").InnerText = mNoticeString;
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("���ÿ�����������ÿ�");
        }

        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("������");
        }

        protected override void OnCreate()
        {
            string noticePath = Path.Combine(StartupPath, @"Notice\xykhk.txt");
            FileInfo tfile = new FileInfo(noticePath);
            if (tfile.Exists)
            {
                using (StreamReader sr = new StreamReader(noticePath, Encoding.Default))
                {
                    mNoticeString = sr.ReadToEnd();
                    sr.Close();
                }
            }

            if (string.IsNullOrEmpty(mNoticeString))
            {
                mNoticeString = "\n\n\n" + "".PadRight(50, ' ') + "��ӭʹ�����ÿ������";
            }
        }
    }
}
