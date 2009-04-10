using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using SmartTank.GameObjs;
using System.Reflection;
using SmartTank.Helpers.DependInject;

namespace SmartTank.net
{
    public static class SyncCasheReader
    {

        static SyncCashe inputCashe;
        static public SyncCashe InputCashe
        {
            get { return inputCashe; }
            set { inputCashe = value; }
        }

        public delegate void CreateObjInfoHandler(IGameObj obj);
        public static event CreateObjInfoHandler onCreateObj;
        public delegate void UserDefineInfoHandler(string infoName, string infoID, object[] args);
        public static event UserDefineInfoHandler onUserDefineInfo;



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
                            object[] newArgs = new object[info.args.Length];

                            Type[] argTypes = new Type[info.args.Length];
                            for (int i = 0; i < info.args.Length; i++)
                            {
                                if (info.args[i] == null)
                                {
                                    argTypes[i] = null;
                                }
                                else
                                {
                                    if (info.args[i] is GameObjSyncInfo)
                                    {
                                        IGameObj gameobj = sceneMgr.GetGameObj(((GameObjSyncInfo)info.args[i]).MgPath);
                                        argTypes[i] = gameobj.GetType();
                                        newArgs[i] = gameobj;
                                    }
                                    else
                                    {
                                        argTypes[i] = info.args[i].GetType();
                                        newArgs[i] = info.args[i];
                                    }
                                }
                            }

                            Type newObjType = DIHelper.GetType(info.objType);
                            object newObj = newObjType.GetConstructor(argTypes).Invoke(newArgs);
                            sceneMgr.AddGameObj(info.objPath, (IGameObj)newObj);
                            if (onCreateObj != null)
                                onCreateObj((IGameObj)newObj);
                        }
                        else if (info.objMgKind == (int)ObjMgKind.Delete)
                        {
                            sceneMgr.DelGameObj(info.objPath);
                        }
                    }
                }

                if (!PurviewMgr.IsMainHost)
                {
                    foreach (UserDefineInfo info in inputCashe.UserDefineInfoList)
                    {
                        object[] newArgs = new object[info.args.Length];
                        for (int i = 0; i < newArgs.Length; i++)
                        {
                            if (info.args[i] is GameObjSyncInfo)
                            {
                                IGameObj gameobj = sceneMgr.GetGameObj(((GameObjSyncInfo)info.args[i]).MgPath);
                                newArgs[i] = gameobj;
                            }
                            else
                            {
                                newArgs[i] = info.args[i];
                            }
                        }
                        if (onUserDefineInfo != null)
                            onUserDefineInfo(info.infoName, info.infoID, newArgs);
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
                inputCashe.UserDefineInfoList.Clear();
                Monitor.Exit(inputCashe);
            }
        }
    }
}
