using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.GameObjs.Shell;
using SmartTank.PhiCol;
using TankEngine2D.Helpers;
using SmartTank.GameObjs;
using Microsoft.Xna.Framework;
using TankEngine2D.Graphics;
using SmartTank.Helpers;
using SmartTank.net;

namespace InterRules.Starwar
{
    class WarShipShell : ShellNormal
    {
        public delegate void ShellOutDateEventHandler(WarShipShell sender, IGameObj shooter);
        public event ShellOutDateEventHandler OnOutDate;

        protected float liveTimer = -SpaceWarConfig.ShellLiveTime;

        public Vector2 Vel
        {
            get { return ((NonInertiasPhiUpdater)PhisicalUpdater).Vel; }
            set { ((NonInertiasPhiUpdater)PhisicalUpdater).Vel = value; }
        }

        public WarShipShell(string name, IGameObj firer, Vector2 startPos, float startAzi)
            : base(name, firer, startPos, startAzi, SpaceWarConfig.ShellSpeed, 
            Directories.ContentDirectory + "\\Rules\\SpaceWar\\image\\field_bullet_001.png", false, new Vector2(4, 4), 1f)
        {
            this.objInfo = new GameObjInfo("WarShipShell", "");
        }

        public override void Update(float seconds)
        {
            liveTimer += seconds;
            if (liveTimer > 0)
            {
                if (OnOutDate != null)
                    OnOutDate(this, Firer);
            }
            SyncCasheWriter.SubmitNewStatus(this.MgPath, "Pos", SyncImportant.HighFrequency, this.Pos);
            SyncCasheWriter.SubmitNewStatus(this.MgPath, "Vel", SyncImportant.HighFrequency, this.Vel);

        }



        internal void MirrorPath(CollisionResult result)
        {
            Vector2 curVel = ((NonInertiasPhiUpdater)this.PhisicalUpdater).Vel;
            float mirVecLength = Vector2.Dot(curVel, result.NormalVector);
            Vector2 horizVel = curVel - mirVecLength * result.NormalVector;
            Vector2 newVel = horizVel + Math.Abs(mirVecLength) * result.NormalVector;
            ((NonInertiasPhiUpdater)this.PhisicalUpdater).Vel = newVel;
            ((NonInertiasPhiUpdater)this.PhisicalUpdater).Azi = MathTools.AziFromRefPos(newVel);

        }
    }
}
