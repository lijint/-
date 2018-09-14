﻿using System.Windows.Forms;
using Landi.FrameWorks;

namespace YAPayment.Business.TrafficPolice
{
    /// <summary>
    /// 交易完成，不打印
    /// </summary>
    class YATrafficPoliceSuccessThankDeal : Activity
    {
        void Return_Click(object sender, HtmlElementEventArgs e)
        {
            GotoMain();
        }

        protected override void OnEnter()
        {
            GetElementById("Return").Click += Return_Click;
        }
    }
}
