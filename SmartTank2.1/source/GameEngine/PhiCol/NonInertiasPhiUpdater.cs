using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameEngine.PhiCol;

namespace GameEngine.PhiCol
{
    public class NonInertiasPhiUpdater : IPhisicalUpdater
    {
        public Vector2 Vel;
        public Vector2 Pos;

        public float AngVel;
        public float Azi;

        protected Vector2 nextPos;
        protected float nextAzi;

        public NonInertiasPhiUpdater ()
        {
        }

        public NonInertiasPhiUpdater (Vector2 pos, Vector2 vel, float azi, float angVel )
        {
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

        #endregion
    }
}
