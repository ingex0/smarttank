using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using SmartTank.GameObjs;

namespace SmartTank.net
{
    /* 场景物体更新同步包缓冲区
     * 包括场景物体的状态更新信息
     * 场景物体的事件通知
     * 场景物体的创建与删除信息（只有主机发送）
     * */
    struct ObjStatusSyncInfo
    {
        public string objMgPath;
        public string statusName;
        public object[] values;
    };

    struct ObjEventSyncInfo
    {
        public string objMgPath;
        public string EventName;
        public object[] values;
    };

    struct ObjMgSyncInfo
    {
        public string objPath;
        public string objType;
        public int objMgKind;
        public object[] args;
    };

    struct UserDefineInfo
    {
        public string infoName;
        public string infoID;
        public object[] args;
    }

    struct GameObjSyncInfo
    {
        public string MgPath;

        public GameObjSyncInfo(string mgPath)
        {
            this.MgPath = mgPath;
        }
    };


    public class SyncCashe
    {
        List<ObjStatusSyncInfo> objStaInfoList = new List<ObjStatusSyncInfo>();
        List<ObjEventSyncInfo> objEventInfoList = new List<ObjEventSyncInfo>();
        List<ObjMgSyncInfo> objMgInfoList = new List<ObjMgSyncInfo>();
        List<UserDefineInfo> userDefineInfoList = new List<UserDefineInfo>();
        internal List<ObjStatusSyncInfo> ObjStaInfoList
        {
            get { return objStaInfoList; }
        }
        internal List<ObjEventSyncInfo> ObjEventInfoList
        {
            get { return objEventInfoList; }
        }
        internal List<ObjMgSyncInfo> ObjMgInfoList
        {
            get { return objMgInfoList; }
        }
        internal List<UserDefineInfo> UserDefineInfoList
        {
            get { return userDefineInfoList; }
        }

        public bool IsCasheEmpty
        {
            get
            {
                return objStaInfoList.Count == 0
                    && objEventInfoList.Count == 0
                    && objMgInfoList.Count == 0
                    && userDefineInfoList.Count == 0;
            }
        }

        internal void AddObjStatusSyncInfo(string objMgPath, string statueName, object[] values)
        {
            ObjStatusSyncInfo newStatus;
            newStatus.objMgPath = objMgPath;
            newStatus.statusName = statueName;
            newStatus.values = values;
            objStaInfoList.Add(newStatus);
        }

        internal void AddObjEventSyncInfo(string objMgPath, string EventName, object[] values)
        {
            ObjEventSyncInfo newEvent;
            newEvent.objMgPath = objMgPath;
            newEvent.EventName = EventName;
            newEvent.values = ConvertObjArg(values);
            objEventInfoList.Add(newEvent);
        }

        internal void AddObjMgCreateSyncInfo(string objPath, Type objType, object[] args)
        {
            ObjMgSyncInfo newObjMg;
            newObjMg.objPath = objPath;
            newObjMg.objType = objType.ToString();
            newObjMg.objMgKind = (int)ObjMgKind.Create;

            object[] newparams = ConvertObjArg(args);

            newObjMg.args = newparams;
            objMgInfoList.Add(newObjMg);
        }



        internal void AddObjMgDeleteSyncInfo(string objPath)
        {
            ObjMgSyncInfo newObjMg;
            newObjMg.objPath = objPath;
            newObjMg.objMgKind = (int)ObjMgKind.Delete;
            newObjMg.objType = null;
            newObjMg.args = new object[0];
            objMgInfoList.Add(newObjMg);
        }

        internal void AddUserDefineInfo(string infoName, string infoID, params object[] args)
        {
            UserDefineInfo info;
            info.infoName = infoName;
            info.infoID = infoID;
            info.args = ConvertObjArg(args);
            userDefineInfoList.Add(info);
        }

        internal void SendPackage()
        {
            if (!IsCasheEmpty)
            {
                SocketMgr.SendGameLogicPackge(this);
                ClearAllList();
            }
        }

        private void ClearAllList()
        {
            objStaInfoList.Clear();
            objEventInfoList.Clear();
            objMgInfoList.Clear();
            userDefineInfoList.Clear();
        }


        private static object[] ConvertObjArg(object[] args)
        {
            object[] newparams = new object[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is IGameObj)
                {
                    newparams[i] = new GameObjSyncInfo(((IGameObj)args[i]).MgPath);
                }
                else
                {
                    newparams[i] = args[i];
                }

            }
            return newparams;
        }





    }
}
