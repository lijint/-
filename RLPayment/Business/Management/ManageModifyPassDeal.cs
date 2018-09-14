using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.Management
{
    class ManageModifyPassDeal:Activity
    {
        protected override void OnEnter()
        {
            GetElementById("Ok").Click += Ok_Click;
            GetElementById("Return").Click += Return_Click;
        }

        private void Ok_Click(object sender, HtmlElementEventArgs e)
        {
            string oldPass = GetElementById("oldPass").GetAttribute("value");
            string newPass = GetElementById("newPass").GetAttribute("value");
            string newPassConfirm = GetElementById("newPassConfirm").GetAttribute("value");
            if (Encrypt.AESEncrypt(oldPass, GlobalAppData.EncryptKey) != GlobalAppData.GetInstance().EntryPwd)
            {
                GetElementById("info").InnerText = "�������������";
                GetElementById("oldPass").SetAttribute("value", "");
                GetElementById("newPass").SetAttribute("value", "");
                GetElementById("newPassConfirm").SetAttribute("value", "");
                return;
            }
            if (newPass.Length != 6)
            {
                GetElementById("info").InnerText = "�����볤�ȴ���";
                GetElementById("oldPass").SetAttribute("value", "");
                GetElementById("newPass").SetAttribute("value", "");
                GetElementById("newPassConfirm").SetAttribute("value", "");
                return;
            }
            if (newPass != newPassConfirm)
            {
                GetElementById("info").InnerText = "���������벻һ�£�";
                GetElementById("oldPass").SetAttribute("value", "");
                GetElementById("newPass").SetAttribute("value", "");
                GetElementById("newPassConfirm").SetAttribute("value", "");
                return;
            }
            GlobalAppData.GetInstance().EntryPwd = Encrypt.AESEncrypt(newPass, GlobalAppData.EncryptKey);
            GetElementById("info").InnerText = "�����޸ĳɹ���";
            GetElementById("oldPass").SetAttribute("value", "");
            GetElementById("newPass").SetAttribute("value", "");
            GetElementById("newPassConfirm").SetAttribute("value", "");
        }

        private void Return_Click(object sender, HtmlElementEventArgs e)
        {
            StartActivity("����������");
        }
    }
}
