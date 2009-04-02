using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.net
{
    static class PurviewMgr
    {
        static bool isMainHost;
        static public bool IsMainHost
        {
            get { return isMainHost; }
            set { isMainHost = value; }
        }

        static String serverAddress;
        static public String ServerAddress
        {
            get { return serverAddress; }
        }
    }
}
