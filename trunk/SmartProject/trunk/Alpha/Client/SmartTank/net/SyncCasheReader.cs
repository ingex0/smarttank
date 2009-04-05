using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SmartTank.GameObjs;
using System.Reflection;

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

        // 还没经过测试
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

                foreach (ObjEventSyncInfo info in inputCashe.ObjEventInfoList)
                {
                    if (PurviewMgr.IsMainHost && PurviewMgr.IsSlaveMgObj(info.objMgPath))
                    {
                        IGameObj obj = sceneMgr.GetGameObj(info.objMgPath);

                        Type objType = obj.GetType();
                        MethodInfo method = objType.GetMethod("Call" + info.EventName);

                        object[] newParams = new object[info.values.Length];
                        for (int i = 0; i < info.values.Length; i++)
                        {
                            if (info.values[i] is GameObjSyncInfo)
                            {
                                IGameObj gameobj = sceneMgr.GetGameObj(((GameObjSyncInfo)info.values[i]).MgPath);
                                newParams[i] = gameobj;
                            }
                            else
                            {
                                newParams[i] = info.values[i];

                            }
                        }
                        method.Invoke(obj, newParams);

                        //objType.InvokeMember("Call" + info.EventName,  BindingFlags. BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance, null, obj, info.values);
                    }
                }

                if (!PurviewMgr.IsMainHost)
                {
                    foreach (ObjMgSyncInfo info in inputCashe.ObjMgInfoList)
                    {
                        if (info.objMgKind == (int)ObjMgKind.Create)
                        {
                            Type[] argTypes = new Type[info.args.Length];
                            for (int i = 0; i < info.args.Length; i++)
                            {
                                if (info.args[i] == null)
                                {
                                    argTypes[i] = null;
                                }
                                else
                                {
                                    argTypes[i] = info.args[i].GetType();
                                }
                            }

                            //object newObj = info.objType.GetConstructor(argTypes).Invoke(info.args);
                            //sceneMgr.AddGameObj(info.objPath, (IGameObj)newObj);
                        }
                        else if (info.objMgKind == (int)ObjMgKind.Delete)
                        {
                            sceneMgr.DelGameObj(info.objPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SyncCashReader 解析过程出现异常： " + ex);
            }
            finally
            {
                inputCashe.ObjStaInfoList.Clear();
                inputCashe.ObjEventInfoList.Clear();
                inputCashe.ObjMgInfoList.Clear();
                Monitor.Exit(inputCashe);
            }
        }
    }
}
