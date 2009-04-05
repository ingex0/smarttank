using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using SmartTank.GameObjs;

namespace SmartTank.net
{
    /* �����������ͬ����������
     * �������������״̬������Ϣ
     * ����������¼�֪ͨ
     * ��������Ĵ�����ɾ����Ϣ��ֻ���������ͣ�
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

        public bool IsCasheEmpty
        {
            get
            {
                return objStaInfoList.Count == 0
              && objEventInfoList.Count == 0
              && objMgInfoList.Count == 0;
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
            newEvent.values = values;
            objEventInfoList.Add(newEvent);
        }

        internal void AddObjMgCreateSyncInfo(string objPath, Type objType, object[] args)
        {
            ObjMgSyncInfo newObjMg;
            newObjMg.objPath = objPath;
            newObjMg.objType = objType.ToString();
            newObjMg.objMgKind = (int)ObjMgKind.Create;

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