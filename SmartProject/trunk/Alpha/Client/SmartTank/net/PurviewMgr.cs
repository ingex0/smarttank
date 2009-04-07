using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.net
{
    static public class PurviewMgr
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

        //static Dictionary<string, object> MainHostMgObjList = new Dictionary<string, object>();
        static Dictionary<string, object> SlaveMgList = new Dictionary<string, object>();


        //static public void RegistMainMgObj(string objMgPath)
        //{
        //    MainHostMgObjList.Add(objMgPath, null);
        //}

        static public void RegistSlaveMgObj(string objMgPath)
        {
            SlaveMgList.Add(objMgPath, null);
        }

        //internal static bool IsMainHostMgObj(string objMgPath)
        //{
        //    return MainHostMgObjList.ContainsKey(objMgPath);
        //}

        public static bool IsSlaveMgObj(string objMgPath)
        {
            return SlaveMgList.ContainsKey(objMgPath);
        }

        internal static void Close()
        {
            SlaveMgList.Clear();
        }
    }
}
