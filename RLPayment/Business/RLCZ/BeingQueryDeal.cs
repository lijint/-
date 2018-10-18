﻿using Landi.FrameWorks;
using RLPayment.Entity;
using RLPayment.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Landi.FrameWorks.Package.Other.SocketCommunicate;

namespace RLPayment.Business.RLCZ
{
    class BeingQueryDeal : FrameActivity
    {
        private RLCZEntity _entity;
        protected override void OnEnter()
        {
            base.OnEnter();
            try
            {
                _entity = GetBusinessEntity() as RLCZEntity;
                if (querymsg() == 0)
                {
                    StartActivity("热力充值查询结果");
                }
                else
                {
                    ShowMessageAndGoBack("查询出错|" + _entity.ReturnCode + _entity.ReturnMsg);
                }
            }
            catch(Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
        }

        private int querymsg()
        {
            int ret = -1;
            try
            {
                CRLQueryTrans cSPospTrans = new CRLQueryTrans(_entity);
                cSPospTrans.setIPAndPort(_entity.RLServerIP, _entity.RLServerPort);
                TransResult result = cSPospTrans.transact();
                if (result == TransResult.E_SUCC)
                {
                    ret = 0;
                }
            }
            catch(Exception ex)
            {
                Log.Error("[" + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name + "][" + System.Reflection.MethodBase.GetCurrentMethod().Name + "] err" + ex);
            }
            return ret;
        }

    }
}
