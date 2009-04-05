using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

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

    };


    public class SyncCashe
    {
        List<ObjStatusSyncInfo> objStaInfoList = new List<ObjStatusSyncInfo>();
        List<ObjEventSyncInfo> objEventInfoList = new List<ObjEventSyncInfo>();
        List<ObjMgSyncInfo> objMgInfoList = new List<ObjMgSyncInfo>();
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

        bool IsCasheEmpty
        {
            get
            {
                return objStaInfoList.Count == 0
              && objEventInfoList.Count == 0
              && objMgInfoList.Count == 0;
            }
        }


        public void AddObjEventSyncInfo()
        {

        }

        public void AddObjMgSyncInfo()
        {

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
            newEvent.values = values;
            objEventInfoList.Add(newEvent);
        }

        internal void SendPackage()
        {
            if (!IsCasheEmpty)
            {
                SocketMgr.SendPackge(this);
                ClearAllList();
            }
        }

        private void ClearAllList()
        {
            objStaInfoList.Clear();
            objEventInfoList.Clear();
            objMgInfoList.Clear();
        }




    }
}
