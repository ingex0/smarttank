using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.net
{
    public enum SyncImportant
    {
        Immediate,
        HighFrequency,
        MidFrequency,
        LowFrequency,
    };

    public enum ObjMgKind
    {
        Create,
        Delete,
    }

    static public class SyncCasheWriter
    {
        const float highSpace = 0.01f;
        const float midSpace = 0.05f;
        const float lowSpace = 1.0f;

        static public SyncCashe outPutCashe;
        static public SyncCashe OutPutCashe
        {
            get { return outPutCashe; }
            set { outPutCashe = value; }
        }

        static private Dictionary<string, float> Timer = new Dictionary<string, float>();

        // 此函数由场景物体类调用，因此函数中要判断主从关系，发包频率
        static public void SubmitNewStatus(string objMgPath, string statueName, SyncImportant improtant, params object[] values)
        {

            if (improtant == SyncImportant.Immediate)
            {
                PushNewStatus(objMgPath, statueName, values);
            }
            else
            {
                string key = objMgPath + statueName;
                if (Timer.ContainsKey(key))
                {
                    float leftTime = Timer[key];
                    switch (improtant)
                    {
                        case SyncImportant.HighFrequency:
                            if (leftTime > 0)
                            {
                                PushNewStatus(objMgPath, statueName, values);
                                Timer[key] = -highSpace;
                            }
                            break;
                        case SyncImportant.MidFrequency:
                            if (leftTime > 0)
                            {
                                PushNewStatus(objMgPath, statueName, values);
                                Timer[key] = -midSpace;
                            }
                            break;
                        case SyncImportant.LowFrequency:
                            if (leftTime > 0)
                            {
                                PushNewStatus(objMgPath, statueName, values);
                                Timer[key] = -lowSpace;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (improtant)
                    {
                        case SyncImportant.HighFrequency:
                            Timer.Add(key, -highSpace);
                            break;
                        case SyncImportant.MidFrequency:
                            Timer.Add(key, -midSpace);
                            break;
                        case SyncImportant.LowFrequency:
                            Timer.Add(key, -lowSpace);
                            break;
                        default:
                            break;
                    }
                }
            }

        }

        static public void SubmitNewEvent(string objMgPath, string EventName, params object[] values)
        {
            if (!PurviewMgr.IsMainHost && PurviewMgr.IsSlaveMgObj(objMgPath))
            {
                outPutCashe.AddObjEventSyncInfo(objMgPath, EventName, values);
            }
        }

        static public void SubmitCreateObjMg(string objPath, Type objType, params object[] createParams)
        {
            if (PurviewMgr.IsMainHost)
            {
                outPutCashe.AddObjMgCreateSyncInfo(objPath, objType, createParams);
            }
        }

        static public void SubmitDeleteObjMg(string objPath)
        {
            if (PurviewMgr.IsMainHost)
            {
                outPutCashe.AddObjMgDeleteSyncInfo(objPath);
            }
        }

        static public void SubmitUserDefineInfo(string infoName, string infoID, params object[] args)
        {
            outPutCashe.AddUserDefineInfo(infoName, infoID, args);
        }

        static void PushNewStatus(string objMgPath, string statueName, object[] values)
        {
            outPutCashe.AddObjStatusSyncInfo(objMgPath, statueName, values);
        }

        static public void Update(float seconds)
        {
            string[] Keys = new string[Timer.Count];
            Timer.Keys.CopyTo(Keys, 0);
            for (int i = 0; i < Timer.Count; i++)
            {
                Timer[Keys[i]] += seconds;
            }
        }

    }
}
