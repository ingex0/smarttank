using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SmartTank.GameObjs;

namespace SmartTank.net
{
    static class SyncCasheReader
    {

        static SyncCashe inputCashe;
        static public SyncCashe InputCashe
        {
            get { return inputCashe; }
            set { inputCashe = value; }
        }

        internal static void ReadCashe(SmartTank.Scene.ISceneKeeper sceneMgr)
        {
            Monitor.Enter(inputCashe);

            try
            {
                foreach (ObjStatusSyncInfo info in inputCashe.ObjStaInfoList)
                {
                    if ((PurviewMgr.IsMainHost && PurviewMgr.IsSlaveMgObj(info.objMgPath))
                        || (!PurviewMgr.IsMainHost && !PurviewMgr.IsSlaveMgObj(info.objMgPath)))
                    {
                        IGameObj obj = sceneMgr.GetGameObj(info.objMgPath);

                        Type objType = obj.GetType();
                        objType.GetProperty(info.statusName).SetValue(obj, info.values[0], null); //暂时只处理一个值的情况
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                inputCashe.ObjStaInfoList.Clear();
                Monitor.Exit(inputCashe);
            }
        }
    }
}
