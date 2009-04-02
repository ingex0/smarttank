using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace SmartTank.net
{
    /* �����������ͬ����������
     * �������������״̬������Ϣ
     * ����������¼�֪ͨ
     * ��������Ĵ�����ɾ����Ϣ��ֻ���������ͣ�
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
