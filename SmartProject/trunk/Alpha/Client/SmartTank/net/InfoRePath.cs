
using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.GameObjs;

namespace SmartTank.net
{
    static public class InfoRePath
    {


        static public void CallEvent(string objMgPath, string eventName, MulticastDelegate delgt, bool SlaveConceal, params object[] eventParams)
        {
            if (PurviewMgr.IsMainHost)
                delgt.DynamicInvoke(eventParams);
            else if (!SlaveConceal)
            {
                object[] newparams = new object[eventParams.Length];

                for (int i = 0; i < eventParams.Length; i++)
                {
                    if (eventParams[i] is IGameObj)
                    {
                        newparams[i] = new GameObjSyncInfo(((IGameObj)eventParams[i]).MgPath);
                    }
                    else
                    {
                        newparams[i] = eventParams[i];
                    }

                }

                // 通过网络协议传递给主机
                SyncCasheWriter.SubmitNewEvent(objMgPath, eventName, newparams);
            }
        }
    }
}
