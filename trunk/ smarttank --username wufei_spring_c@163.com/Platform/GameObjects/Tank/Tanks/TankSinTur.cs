using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Platform.GameDraw;
using IDrawable = Platform.GameDraw.IDrawableObj;
using Platform.GameObjects.Tank.TankControls;
using Platform.GameObjects.Tank.TankSkins;
using GameBase.Graphics;
using Platform.Shelter;
using Microsoft.Xna.Framework.Graphics;
using Platform.Senses.Vision;
using GameBase.Helpers;
using Platform.GameObjects.Tank.TankAIs;
using Platform.PhisicalCollision;
using Platform.Senses.Memory;
using GameBase.DataStructure;
using System.IO;

namespace Platform.GameObjects.Tank.Tanks
{
    public class TankSinTur : Tank, IRaderOwner, IAIOrderServerSinTur, IEyeableObj
    {
        #region Statics

        static public string M1A2TexPath = Path.Combine( Directories.ItemDirectory, "Internal\\M1A2" );
        static public GameObjData M1A2Data = GameObjData.Load( File.OpenRead( Path.Combine( M1A2TexPath, "M1A2.xml" ) ) );

        static public string M60TexPath = Path.Combine( Directories.ItemDirectory, "Internal\\M60" );
        static public GameObjData M60Data = GameObjData.Load( File.OpenRead( Path.Combine( M60TexPath, "M60.xml" ) ) );

        static public string TigerTexPath = Path.Combine( Directories.ItemDirectory, "Internal\\Tiger" );
        static public GameObjData TigerData = GameObjData.Load( File.OpenRead( Path.Combine( TigerTexPath, "Tiger.xml" ) ) );

        #endregion

        #region Events

        public event ShootEventHandler onShoot;

        public event OnCollidedEventHandler onCollide;

        public event OnCollidedEventHandler onOverLap;

        public event BorderObjUpdatedEventHandler onBorderObjUpdated;

        #endregion

        #region Variables

        TankContrSinTur controller;

        TankSkinSinTur skin;

        Rader rader;

        float shellSpeed;

        #endregion

        #region Properties

        public float FireCDTime
        {
            get { return controller.fireCDTime; }
            set
            {
                controller.fireCDTime = Math.Max( 0, value );
            }
        }

        public override Vector2 Pos
        {
            get { return controller.Pos; }
            set { controller.Pos = value; }
        }

        public Vector2 TurretAxePos
        {
            get { return skin.Sprites[1].Pos; }
        }

        public override float Azi
        {
            get { return controller.Azi; }
            set { controller.Azi = value; }
        }

        public Vector2 Direction
        {
            get { return MathTools.NormalVectorFromAzi( Azi ); }
        }

        public float TurretAimAzi
        {
            get { return controller.Azi + controller.turretAzi; }
        }

        public float TurretAzi
        {
            get { return controller.turretAzi; }
        }

        public float RaderRadius
        {
            get
            {
                return rader.R;
            }
        }

        public float RaderAng
        {
            get
            {
                return rader.Ang;
            }
        }

        public float RaderAzi
        {
            get { return controller.raderAzi; }
        }

        public float RaderAimAzi
        {
            get { return controller.Azi + controller.raderAzi; }
        }

        public float MaxForwardSpeed
        {
            get { return controller.limit.MaxForwardSpeed; }
        }

        public float MaxBackwardSpeed
        {
            get { return controller.limit.MaxBackwardSpeed; }
        }

        public float MaxRotaSpeed
        {
            get { return controller.limit.MaxTurnAngleSpeed; }
        }

        public float MaxRotaTurretSpeed
        {
            get { return controller.limit.MaxTurretAngleSpeed; }
        }

        public float MaxRotaRaderSpeed
        {
            get { return controller.limit.MaxRaderAngleSpeed; }
        }

        public float ShellSpeed
        {
            get { return shellSpeed; }
            set { shellSpeed = value; }
        }

        public float TurretLength
        {
            get { return skin.TurretLength; }
        }

        public override GameBase.DataStructure.CircleList<GameBase.Graphics.Border> BorderData
        {
            get { return skin.Sprites[0].BorderData; }
        }

        public override Matrix WorldTrans
        {
            get { return skin.Sprites[0].Transform; }
        }

        public override GameBase.DataStructure.Rectanglef BoundingBox
        {
            get { return skin.Sprites[0].BoundRect; }
        }

        #endregion

        #region Consturction

        public TankSinTur ( GameObjInfo objInfo, string texPath, GameObjData skinData,
            float raderLength, float raderAng, Color raderColor,
            float maxForwardSpeed, float maxBackwardSpeed, float maxRotaSpeed,
            float maxTurretRotaSpeed, float maxRaderRotaSpeed, float fireCDTime,
            Vector2 pos, float baseAzi )
        {
            this.objInfo = objInfo;
            this.skin = new TankSkinSinTur( new TankSkinSinTurData( texPath, skinData ) );
            skin.Initial( pos, baseAzi, 0 );
            controller = new TankContrSinTur( objInfo, new Sprite[] { skin.Sprites[0] } , pos, baseAzi, maxForwardSpeed, maxBackwardSpeed, maxRotaSpeed, maxTurretRotaSpeed, maxRaderRotaSpeed, Math.Max( 0, fireCDTime ) );
            colChecker = controller;
            phisicalUpdater = controller;
            controller.onShoot += new EventHandler( controller_onShoot );
            controller.OnCollied += new Platform.PhisicalCollision.OnCollidedEventHandler( controller_OnCollied );
            controller.OnOverlap += new Platform.PhisicalCollision.OnCollidedEventHandler( controller_OnOverlap );
            rader = new Rader( raderAng, raderLength, pos, baseAzi, raderColor );

        }

        public TankSinTur ( GameObjInfo objInfo, string texPath, GameObjData skinData,
            float raderLength, float raderAng, Color raderColor, float raderAzi,
            float maxForwardSpeed, float maxBackwardSpeed, float maxRotaSpeed,
            float maxTurretRotaSpeed, float maxRaderRotaSpeed, float fireCDTime,
            Vector2 pos, float baseRota, float turretRota )
        {
            this.objInfo = objInfo;
            this.skin = new TankSkinSinTur( new TankSkinSinTurData( texPath, skinData ) );
            skin.Initial( pos, baseRota, turretRota );
            controller = new TankContrSinTur( objInfo, new Sprite[] { skin.Sprites[0] } , pos, baseRota, maxForwardSpeed, maxBackwardSpeed, maxRotaSpeed, maxTurretRotaSpeed, maxRaderRotaSpeed, Math.Max( 0, fireCDTime ) );
            colChecker = controller;
            phisicalUpdater = controller;
            controller.onShoot += new EventHandler( controller_onShoot );
            rader = new Rader( raderAng, raderLength, pos, raderAzi + baseRota, raderColor );
        }

        #endregion

        #region Update

        public override void Update ( float seconds )
        {
            base.Update( seconds );
            controller.Update( seconds );
            skin.Update( seconds );
        }


        #endregion

        #region Dead
        public override void Dead ()
        {
            base.Dead();
            controller.Enable = false;
        }
        #endregion

        #region Draw

        public override void Draw ()
        {
            skin.ResetSprites( controller.Pos, controller.Azi, controller.turretAzi );
            skin.Draw();

            if (!isDead)
            {
                rader.Pos = Pos;
                rader.Azi = controller.raderAzi + controller.Azi;
                rader.Draw();
            }

            base.Draw();
        }

        #endregion

        #region EventHandler

        void controller_onShoot ( object sender, EventArgs e )
        {
            if (onShoot != null)
                onShoot( this, skin.GetTurretEndPos( controller.Pos, controller.Azi, controller.turretAzi ), controller.Azi + controller.turretAzi );

            skin.BeginRecoil();
        }

        void controller_OnOverlap ( IGameObj Sender, CollisionResult result, GameObjInfo objB )
        {
            if (onOverLap != null)
                onOverLap( this, result, objB );

            if (OnOverLap != null)
                OnOverLap( result, objB );
        }

        void controller_OnCollied ( IGameObj Sender, CollisionResult result, GameObjInfo objB )
        {
            if (onCollide != null)
                onCollide( this, result, objB );

            if (OnCollide != null)
                OnCollide( result, objB );
        }

        #endregion

        #region IRaderOwner 成员

        public Rader Rader
        {
            get { return rader; }
        }

        public List<IEyeableInfo> CurEyeableObjs
        {
            set
            {
                rader.CurEyeableObjs = value;
            }
        }

        public void BorderObjUpdated ( EyeableBorderObjInfo[] borderObjInfo )
        {
            if (onBorderObjUpdated != null)
                onBorderObjUpdated( borderObjInfo );
        }

        #endregion

        #region IAIOrderServerSinTur 成员

        public List<IEyeableInfo> GetEyeableInfo ()
        {
            return rader.CurEyeableObjs;
        }

        public float ForwardSpeed
        {
            get
            {
                return controller.ForwardSpeed;
            }
            set
            {
                controller.ForwardSpeed = value;
            }
        }

        public float TurnRightSpeed
        {
            get
            {
                return controller.TurnRightSpeed;
            }
            set
            {
                controller.TurnRightSpeed = value;
            }
        }

        public float TurnTurretWiseSpeed
        {
            get
            {
                return controller.TurnTurretWiseSpeed;
            }
            set
            {
                controller.TurnTurretWiseSpeed = value;
            }
        }

        public float FireLeftCDTime
        {
            get { return controller.FireLeftCDTime; }
        }

        public float TurnRaderWiseSpeed
        {
            get
            {
                return controller.TurnRaderWiseSpeed;
            }
            set
            {
                controller.TurnRaderWiseSpeed = value;
            }
        }

        public void Fire ()
        {
            controller.Fire();
        }

        public float TankWidth
        {
            get { return skin.TankWidth; }
        }

        public float TankLength
        {
            get { return skin.TankLength; }
        }

        public event OnCollidedEventHandlerAI OnCollide;

        public event OnCollidedEventHandlerAI OnOverLap;

        public Platform.Senses.Memory.EyeableBorderObjInfo[] EyeableBorderObjInfos
        {
            get { return rader.ObjMemoryKeeper.GetEyeableBorderObjInfos(); }
        }

        public NavigateMap CalNavigateMap ( NaviMapConsiderObj selectFun, Rectanglef mapBorder, float spaceForTank )
        {
            return rader.ObjMemoryKeeper.CalNavigationMap( selectFun, mapBorder, spaceForTank );
        }

        #endregion

        #region IEyeableObj 成员

        static public IEyeableInfo GetCommonEyeInfoFun ( IRaderOwner raderOwner, IEyeableObj tank )
        {
            return new TankCommonEyeableInfo( (TankSinTur)tank );
        }

        public class TankCommonEyeableInfo : IEyeableInfo
        {
            EyeableInfo eyeableInfo;

            Vector2 vel;
            float azi;
            Vector2 direction;
            float turretAimAzi;

            public Vector2 Pos
            {
                get { return eyeableInfo.Pos; }
            }

            public Vector2 Vel
            {
                get { return vel; }
            }

            public float Azi
            {
                get { return azi; }
            }

            public Vector2 Direction
            {
                get { return direction; }
            }

            public float TurretAimAzi
            {
                get { return turretAimAzi; }
            }

            public TankCommonEyeableInfo ( TankSinTur tank )
            {
                this.eyeableInfo = new EyeableInfo( tank );

                this.azi = tank.Azi;
                this.vel = tank.Direction * tank.ForwardSpeed;
                this.direction = tank.Direction;
                this.turretAimAzi = tank.TurretAimAzi;
            }

            #region IEyeableInfo 成员

            public GameObjInfo ObjInfo
            {
                get { return eyeableInfo.ObjInfo; }
            }

            public Vector2[] CurKeyPoints
            {
                get { return eyeableInfo.CurKeyPoints; }
            }

            public Matrix CurTransMatrix
            {
                get { return eyeableInfo.CurTransMatrix; }
            }

            #endregion
        }

        public Vector2[] KeyPoints
        {
            get { return skin.VisiKeyPoints; }
        }

        public Matrix TransMatrix
        {
            get { return skin.Sprites[0].Transform; }
        }

        GetEyeableInfoHandler getEyeableInfoHandler;

        public GetEyeableInfoHandler GetEyeableInfoHandler
        {
            get
            {
                return getEyeableInfoHandler;
            }
            set
            {
                getEyeableInfoHandler = value;
            }
        }

        #endregion

        #region IAIOrderServer 成员




        #endregion

    }
}
