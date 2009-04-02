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

        static public void SubmitNewStatus(string objName, string statueName, SyncImportant improtant, params object[] values)
        {
            switch (improtant)
            {
                case SyncImportant.Immediate:
                    PushNewStatus(objName, statueName, values);
                    break;
                case SyncImportant.HighFrequency:
                    if (highTimer > 0)
                    {
                        PushNewStatus(objName, statueName, values);
                        highTimer = -highSpace;
                    }
                    break;
                case SyncImportant.MidFrequency:
                    if (midTimer > 0)
                    {
                        PushNewStatus(objName, statueName, values);
                        highTimer = -midSpace;
                    }
                    break;
                case SyncImportant.LowFrequency:
                    if (lowTimer > 0)
                    {
                        PushNewStatus(objName, statueName, values);
                        lowTimer = -lowSpace;
                    }
                    break;
                default:
                    break;
            }
        }

        static void PushNewStatus(string objName, string statueName, object[] values)
        {
            outPutCashe.AddObjStatusSyncInfo(objName, statueName, values);
        }

        static public void Update(float seconds)
        {
            highTimer += seconds;
            midTimer += seconds;
            lowTimer += seconds;
        }
    }
}
