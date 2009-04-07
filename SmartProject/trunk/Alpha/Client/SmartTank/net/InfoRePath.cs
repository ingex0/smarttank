
using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.GameObjs;

namespace SmartTank.net
{
    static public class InfoRePath
    {


        static public void CallEvent(string objMgPath, string eventName, MulticastDelegate delgt, params object[] eventParams)
        {
            delgt.DynamicInvoke(eventParams);
            if (!PurviewMgr.IsMainHost)
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

                // ͨ������Э�鴫�ݸ�����
                SyncCasheWriter.SubmitNewEvent(objMgPath, eventName, newparams);
            }
        }
    }
}
