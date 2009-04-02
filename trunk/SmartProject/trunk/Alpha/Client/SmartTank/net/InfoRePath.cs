
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.net
{
    static public class InfoRePath
    {
        

        static public void CallEvent(MulticastDelegate delgt, params object[] evenetParams)
        {
            if (PurviewMgr.IsMainHost)
                delgt.DynamicInvoke(evenetParams);
            else
            {
                // TODO 通过网络协议传递给主机
            }
        }
    }
}
