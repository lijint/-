﻿using Landi.FrameWorks;
using RLPayment.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RLPayment.Business.RLCZ
{
    class ChoosePayTypeDeal : FrameActivity
    {
        private RLCZEntity _entity;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;
                GetElementById("btnYHK").Click += new HtmlElementEventHandler(YHKClick);
                GetElementById("btnWX").Click += new HtmlElementEventHandler(WXClick);
                GetElementById("btnZFB").Click += new HtmlElementEventHandler(ZFBClick);
            }
            catch (Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }

        private void YHKClick(object sender, HtmlElementEventArgs e)
        {
            _entity.PayType = 0;
            StartActivity("热力充值插入银行卡");
        }

        private void WXClick(object sender, HtmlElementEventArgs e)
        {
            _entity.PayType = 1;
            StartActivity("热力充值正在查询二维码");
        }
        private void ZFBClick(object sender, HtmlElementEventArgs e)
        {
            _entity.PayType = 2;
            StartActivity("热力充值正在查询二维码");
        }



        protected override void FrameReturnClick()
        {
            StartActivity("热力充值查询结果");
        }
    }
}