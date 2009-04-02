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
        public string objName;
        public string statusName;
        public object[] values;
    };

    struct ObjEventSyncInfo
    {

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
            get { return objStaInfoList.Count == 0 
                && objEventInfoList.Count == 0 
                && objMgInfoList.Count == 0; }
        }


        public void AddObjEventSyncInfo()
        {

        }

        public void AddObjMgSyncInfo()
        {

        }

        internal void AddObjStatusSyncInfo(string objName, string statueName, object[] values)
        {
            ObjStatusSyncInfo newStatus;
            newStatus.objName = objName;
            newStatus.statusName = statueName;
            newStatus.values = values;
            objStaInfoList.Add(newStatus);
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
