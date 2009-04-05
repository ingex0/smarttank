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


    static public class SyncCasheWriter
    {
        static float highTimer = -highSpace;
        static float midTimer = -midSpace;
        static float lowTimer = -lowSpace;

        const float highSpace = 0.1f;
        const float midSpace = 0.3f;
        const float lowSpace = 1.0f;

        static public SyncCashe outPutCashe;
        static public SyncCashe OutPutCashe
        {
            get { return outPutCashe; }
            set { outPutCashe = value; }
        }

        // 此函数由场景物体类调用，因此函数中要判断主从关系，发包频率
        static public void SubmitNewStatus(string objMgPath, string statueName, SyncImportant improtant, params object[] values)
        {
            if ((PurviewMgr.IsMainHost && !PurviewMgr.IsSlaveMgObj(objMgPath))
                || (!PurviewMgr.IsMainHost && PurviewMgr.IsSlaveMgObj(objMgPath)))
            {
                switch (improtant)
                {
                    case SyncImportant.Immediate:
                        PushNewStatus(objMgPath, statueName, values);
                        break;
                    case SyncImportant.HighFrequency:
                        if (highTimer > 0)
                        {
                            PushNewStatus(objMgPath, statueName, values);
                            highTimer = -highSpace;
                        }
                        break;
                    case SyncImportant.MidFrequency:
                        if (midTimer > 0)
                        {
                            PushNewStatus(objMgPath, statueName, values);
                            highTimer = -midSpace;
                        }
                        break;
                    case SyncImportant.LowFrequency:
                        if (lowTimer > 0)
                        {
                            PushNewStatus(objMgPath, statueName, values);
                            lowTimer = -lowSpace;
                        }
                        break;
                    default:
                        break;
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

        static void PushNewStatus(string objMgPath, string statueName, object[] values)
        {
            outPutCashe.AddObjStatusSyncInfo(objMgPath, statueName, values);
        }

        static public void Update(float seconds)
        {
            highTimer += seconds;
            midTimer += seconds;
            lowTimer += seconds;
        }
    }
}
