using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SmartTank.GameObjs;
using SmartTank.PhiCol;

namespace SmartTank.PhiCol
{
    public class NonInertiasPhiUpdater : IPhisicalUpdater, ISyncable
    {
        public delegate void PosAziChangedEventHandler();
        public event PosAziChangedEventHandler posAziChanged;

        GameObjInfo objInfo;

        public Vector2 Vel;
        public Vector2 Pos;

        public float AngVel;
        public float Azi;

        protected Vector2 nextPos;
        protected float nextAzi;

        public Vector2 serPos;
        public Vector2 serVel;
        public float serAzi;
        public float serAziVel;

        protected Vector2 localPos;
        protected Vector2 localVel;
        protected float localAzi;
        protected float localAziVel;

        protected float syncTotolTime = 0;
        protected bool startSync = false;
        protected bool syncing = false;
        protected float syncCurTime = 0;
        protected float mount = 0;


        public NonInertiasPhiUpdater(GameObjInfo objInfo)
        {
            this.objInfo = objInfo;
        }

        public NonInertiasPhiUpdater(GameObjInfo objInfo, Vector2 pos, Vector2 vel, float azi, float angVel)
        {
            this.objInfo = objInfo;
            this.Pos = pos;
            this.Vel = vel;
            this.Azi = azi;
            this.AngVel = angVel;
            this.nextPos = pos;
            this.nextAzi = azi;
        }

        #region IPhisicalUpdater 成员

        public virtual void CalNextStatus(float seconds)
        {
            if (startSync)
            {
                syncCurTime = 0;
                localPos = Pos;
                localVel = Vel;
                localAzi = Azi;
                localAziVel = AngVel;
                mount = 0;
                startSync = false;
            }

            if (syncing)
            {
                syncCurTime += seconds;

                float tScale = syncCurTime / syncTotolTime;

                float V0 = localVel.Length();
                float V1 = serVel.Length();

                float M = (V0 + V1) * syncTotolTime / 2;
                if (M != 0)
                {
                    float curV = MathHelper.Lerp(V0, V1, tScale);
                    float w = (V0 + curV) * syncCurTime / 2;
                    float mount = w / M;
                    if (mount > 1)
                    {
                        int x = 0;
                    }
                    nextPos = Vector2.Hermite(localPos, localVel, serPos, serVel, mount);
                }
                else
                {
                    nextPos = (serPos - localPos) * tScale + localPos;
                }

                float MAzi = (localAziVel + serAziVel) * syncCurTime / 2;
                if (MAzi != 0)
                {
                    float curAziVel = MathHelper.Lerp(localAziVel, serAziVel, tScale);
                    float wAzi = (localAziVel + curAziVel) * syncCurTime / 2;
                    float mountAzi = wAzi / MAzi;
                    nextAzi = MathHelper.Hermite(localAzi, localAziVel, serAzi, serAziVel, mountAzi);
                }
                else
                {
                    nextAzi = MathHelper.Lerp(localAzi, serAziVel, tScale);
                }

                if (syncCurTime > syncTotolTime)
                {
                    syncing = false;
                    Vel = serVel;
                    AngVel = serAziVel;
                }
            }
            else
            {
                nextPos = Pos + Vel * seconds;
                nextAzi = Azi + AngVel * seconds;
            }
        }

        public virtual void Validated()
        {
            Pos = nextPos;
            Azi = nextAzi;
            if (posAziChanged != null)
                posAziChanged();
        }

        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        #endregion

        #region ISyncable 成员

        public void SetServerStatue(Vector2 serPos, Vector2 serVel, float serAzi, float serAziVel, float syncTime)
        {
            if (syncTime == 0)
                return;

            this.serPos = serPos + serVel * syncTime;
            this.serVel = serVel;
            this.serAzi = serAzi + serAziVel * syncTime;
            this.serAziVel = serAziVel;
            this.syncTotolTime = syncTime;
            this.startSync = true;
            this.syncing = true;
        }

        #endregion

    }
}
