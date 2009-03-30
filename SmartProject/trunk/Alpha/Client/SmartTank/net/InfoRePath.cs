
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.net
{
    static class InfoRePath
    {
        static bool isMainHost;
        static bool IsMainHost
        {
            get { return isMainHost; }
            set { isMainHost = value; }
        }



        static public void CallEvent(MulticastDelegate delgt, params object[] evenetParams)
        {
            if (isMainHost)
                delgt.DynamicInvoke(evenetParams);
            else
            {
                // TODO 通过网络协议传递给主机
            }
        }
    }
}
