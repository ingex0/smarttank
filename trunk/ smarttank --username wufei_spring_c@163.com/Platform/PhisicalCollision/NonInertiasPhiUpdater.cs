using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Platform.GameObjects;

namespace Platform.PhisicalCollision
{
    public class NonInertiasPhiUpdater : IPhisicalUpdater
    {
        GameObjInfo objInfo;

        public Vector2 Vel;
        public Vector2 Pos;

        public float AngVel;
        public float Azi;

        protected Vector2 nextPos;
        protected float nextAzi;

        public NonInertiasPhiUpdater ( GameObjInfo objInfo )
        {
            this.objInfo = objInfo;
        }

        public NonInertiasPhiUpdater ( GameObjInfo objInfo, Vector2 pos, Vector2 vel, float azi, float angVel )
        {
            this.objInfo = objInfo;
            this.Pos = pos;
            this.Vel = vel;
            this.Azi = azi;
            this.AngVel = angVel;
            this.nextPos = pos;
            this.nextAzi = azi;
        }

        #region IPhisicalUpdater ≥…‘±

        public virtual void CalNextStatus ( float seconds )
        {
            nextPos = Pos + Vel * seconds;
            nextAzi = Azi + AngVel * seconds;
        }

        public virtual void Validated ()
        {
            Pos = nextPos;
            Azi = nextAzi;
        }

        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        #endregion
    }
}
