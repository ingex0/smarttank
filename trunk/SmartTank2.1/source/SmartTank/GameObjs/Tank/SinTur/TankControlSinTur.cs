using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Graphics;
using Microsoft.Xna.Framework;
using SmartTank.PhiCol;
using SmartTank.Update;
using SmartTank.Scene;
using SmartTank.GameObjs.Shell;
using TankEngine2D.Helpers;

namespace SmartTank.GameObjs.Tank.SinTur
{
    public class TankContrSinTur : NonInertiasColUpdater, ITankController, IUpdater
    {
        #region Events

        public event EventHandler onShoot;

        #endregion

        #region Variables

        public TankLimitSinTur limit;

        Sprite[] collideSprites;

        #region Additional StateParameter

        public float forwardVel;

        public float turretAzi;
        public float turretAngVel;

        public float raderAzi;
        public float raderAngVel;

        public float fireCDTime;
        public float fireLeftCDTime = 0;
        public bool fireOnNextSpare;



        #endregion

        bool enable = true;

        #endregion

        #region Properties

        Vector2 ForwardVector
        {
            get
            {
                return new Vector2( (float)Math.Sin( Azi ), -(float)Math.Cos( Azi ) );
            }
        }

        public bool Enable
        {
            get { return enable; }
            set
            {
                enable = value;
                this.Vel = Vector2.Zero;
                this.AngVel = 0;
                this.forwardVel = 0;
                this.turretAngVel = 0;
                this.raderAngVel = 0;
            }
        }

        #endregion

        #region Construction

        public TankContrSinTur ( GameObjInfo objInfo,
            Sprite[] collideSprites,
            Vector2 initialPos, float initialRota,
            float maxForwardSpeed, float maxBackwardSpeed, float maxTurnAngleSpeed,
            float maxTurretAngleSpeed, float maxRaderAngleSpeed,
            float fireCDTime )
            : base( objInfo, initialPos, Vector2.Zero, initialRota, 0f, collideSprites )
        {
            this.collideSprites = collideSprites;
            limit = new TankLimitSinTur( maxForwardSpeed, maxBackwardSpeed, maxTurnAngleSpeed, maxTurretAngleSpeed, maxRaderAngleSpeed );
            this.fireCDTime = fireCDTime;
        }

        #endregion

        #region IAIOrderServerSinTur 成员

        public float ForwardSpeed
        {
            get
            {
                return forwardVel;
            }
            set
            {
                forwardVel = limit.LimitSpeed( value );
            }
        }

        public float TurnRightSpeed
        {
            get
            {
                return AngVel;
            }
            set
            {
                AngVel = limit.LimitAngleSpeed( value );
            }
        }

        public float TurnTurretWiseSpeed
        {
            get
            {
                return turretAngVel;
            }
            set
            {
                turretAngVel = limit.LimitTurretAngleSpeed( value );
            }
        }

        public float FireLeftCDTime
        {
            get { return fireLeftCDTime; }
        }

        public float TurnRaderWiseSpeed
        {
            get
            {
                return raderAngVel;
            }
            set
            {
                raderAngVel = limit.LimitRaderAngleSpeed( value );
            }
        }

        public void Fire ()
        {
            // 接收到开火命令后，如果不在冷却中，就开火
            if (fireLeftCDTime <= 0)
                Shoot();
        }

        //public List<Platform.Senses.Vision.IEyeableInfo> GetEyeableInfo ()
        //{
        //    throw new Exception( "The method or operation is not implemented." );
        //}



        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            if (!enable) 
                return;

            turretAzi += turretAngVel * seconds;
            raderAzi += raderAngVel*seconds;
            fireLeftCDTime = Math.Max( 0, fireLeftCDTime - seconds );

            if (fireOnNextSpare && fireLeftCDTime == 0 )
            {
                fireOnNextSpare = false;
                Shoot();
            }
        }

        #endregion

        #region Override Functions

        public override void ClearNextStatus ()
        {
            base.ClearNextStatus();
        }

        public override void CalNextStatus ( float seconds )
        {
            if (!enable)
                return;

            Vel = forwardVel * ForwardVector;
            base.CalNextStatus( seconds );
        }

        public override void Validated ()
        {
            if (!enable)
                return;

            base.Validated();
        }

        #endregion

        private void Shoot ()
        {
            if (onShoot != null)
                onShoot( this, EventArgs.Empty );
            fireLeftCDTime = fireCDTime;
        }

    }
}
