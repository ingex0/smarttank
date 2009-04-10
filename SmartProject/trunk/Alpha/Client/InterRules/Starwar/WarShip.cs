using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.GameObjs;
using TankEngine2D.Graphics;
using Microsoft.Xna.Framework;
using SmartTank.PhiCol;
using System.IO;
using SmartTank;
using SmartTank.Draw;
using Microsoft.Xna.Framework.Graphics;
using TankEngine2D.Input;
using TankEngine2D.Helpers;
using Microsoft.Xna.Framework.Input;
using SmartTank.net;
using SmartTank.Helpers;

namespace InterRules.Starwar
{
    class WarShip : IGameObj, ICollideObj, IPhisicalObj
    {

        public WarShip(string name, Vector2 pos, float azi, bool openControl)
        {
            this.name = name;
            this.objInfo = new GameObjInfo("WarShip", name);
            this.openControl = openControl;
            InitializeTex(pos, azi);
            InitializePhisical(pos, azi);
            this.Pos = pos;
            this.Azi = azi;
            this.OnDead += new WarShipDeadEventHandler(WarShip_OnDead);
        }

        void WarShip_OnDead(WarShip sender)
        {
            destoryAnimate.SetSpritesParameters(new Vector2(48, 48), this.Pos, 1f, this.Azi, Color.White, LayerDepth.TankBase, SpriteBlendMode.AlphaBlend);
            destoryAnimate.Interval = 5;
            destoryAnimate.Start(0, 6, true);
            GameManager.AnimatedMgr.Add(destoryAnimate);
        }

        #region Logic

        public readonly float cRootTwoDivideTwo = (float)(Math.Sqrt(2) * 0.5f);

        //protected Vector2 curSpeed = new Vector2(0, 0);

        protected int curHP = SpaceWarConfig.IniHP;
        protected bool isDead = false;

        /* 计时器在小于0时有效计时状态
         * */
        protected float stillTimer = 0;
        protected float wtfTimer = 0;
        protected float shootTimer = 0;
        protected int score = 0;

        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        public Vector2 Vel
        {
            get { return phisicalUpdater.Vel; }
            set { phisicalUpdater.Vel = value; }
        }

        public int HP
        {
            get { return curHP; }
        }

        public delegate void WarShipShootEventHandler(WarShip firer, Vector2 endPoint, float azi);
        public delegate void WarShipDeadEventHandler(WarShip sender);

        public event WarShipShootEventHandler OnShoot;
        public event WarShipDeadEventHandler OnDead;

        protected bool Shoot(float ShootAzi)
        {
            if (shootTimer >= 0)
            {
                if (OnShoot != null)
                    OnShoot(this, Pos + MathTools.NormalVectorFromAzi(ShootAzi) * SpaceWarConfig.ShootEndDest, ShootAzi);
                shootTimer -= SpaceWarConfig.ShootCD;
                return true;
            }
            return false;
        }

        internal void Born(Vector2 newPos)
        {
            BeginWTF();
            this.Pos = newPos;
            this.Vel = Vector2.Zero;
        }

        public void BeginStill()
        {
            stillTimer = -SpaceWarConfig.StillTime;
        }

        public void BeginWTF()
        {
            wtfTimer = -SpaceWarConfig.WTFTime;
        }

        public void HitByShell()
        {
            LooseHP(SpaceWarConfig.HitbyShellDamage);
        }

        public void HitByObjExceptShell()
        {
            LooseHP(SpaceWarConfig.HitbyObjExceptShell);
        }

        private void LooseHP(int Damage)
        {
            this.curHP -= Damage;

            if (curHP <= 0)
            {
                isDead = true;
                if (OnDead != null)
                    OnDead(this);
            }
        }

        private void HandlerControl(float seconds)
        {
            if (!isDead && openControl)
            {
                float maxSpeed = SpaceWarConfig.SpeedMax;
                float accel = SpaceWarConfig.SpeedAccel;
                if (stillTimer < 0)
                {
                    // 硬直状态
                    // maxSpeed *= cStillSpeedScale; 
                    accel *= SpaceWarConfig.StillSpeedScale;
                }

                #region  获取键盘输入

                bool Up = InputHandler.IsKeyDown(Keys.W);
                bool Left = InputHandler.IsKeyDown(Keys.A);
                bool Down = InputHandler.IsKeyDown(Keys.S);
                bool Right = InputHandler.IsKeyDown(Keys.D);

                Vector2 deltaSpeed = new Vector2();
                bool noInput = false;

                // 左上
                if (Up && Left && !Right && !Down)
                {
                    deltaSpeed.X = -cRootTwoDivideTwo;
                    deltaSpeed.Y = -cRootTwoDivideTwo;
                }
                // 右上
                else if (Up && Right && !Left && !Down)
                {
                    deltaSpeed.X = cRootTwoDivideTwo;
                    deltaSpeed.Y = -cRootTwoDivideTwo;
                }
                // 右下
                else if (Down && Right && !Up && !Left)
                {
                    deltaSpeed.X = cRootTwoDivideTwo;
                    deltaSpeed.Y = cRootTwoDivideTwo;
                }
                // 左下
                else if (Down && Left && !Up && !Right)
                {
                    deltaSpeed.X = -cRootTwoDivideTwo;
                    deltaSpeed.Y = cRootTwoDivideTwo;
                }
                // 上
                else if (Up && !Down)
                {
                    deltaSpeed.X = 0;
                    deltaSpeed.Y = -1.0f;
                }
                // 下
                else if (Down && !Up)
                {
                    deltaSpeed.X = 0;
                    deltaSpeed.Y = 1.0f;
                }
                // 左
                else if (Left && !Right)
                {
                    deltaSpeed.X = -1.0f;
                    deltaSpeed.Y = 0;
                }
                // 右
                else if (Right && !Left)
                {
                    deltaSpeed.X = 1.0f;
                    deltaSpeed.Y = 0;
                }
                else
                {
                    noInput = true;
                }

                #endregion

                #region 获得鼠标状态

                if (InputHandler.LastMouseLeftDown)
                {
                    Vector2 mousePos = InputHandler.GetCurMousePosInLogic(BaseGame.RenderEngine);
                    float shootAzi = MathTools.AziFromRefPos(mousePos - this.Pos);

                    this.Shoot(shootAzi);
                }

                #endregion


                if (!noInput)
                {
                    phisicalUpdater.Azi = MathTools.AziFromRefPos(Vel);
                    deltaSpeed *= seconds * accel;
                    Vel += deltaSpeed;
                    float SpeedAbs = Vel.Length();
                    if (SpeedAbs > maxSpeed)
                    {
                        Vel *= maxSpeed / SpeedAbs;
                    }
                }
                else // 衰减
                {
                    // 假设时间间隔是均匀的，并且间隔很小。
                    if (SpaceWarConfig.SpeedDecay * seconds < 1)
                    {
                        Vel *= (1 - SpaceWarConfig.SpeedDecay * seconds);
                    }
                }
                phisicalUpdater.Vel = Vel;
            }
        }

        #endregion

        #region IPhisicalObj 成员

        public event OnCollidedEventHandler OnCollied;
        public event OnCollidedEventHandler OnOverLap;


        private void InitializePhisical(Vector2 pos, float azi)
        {
            phisicalUpdater = new NonInertiasColUpdater(objInfo, new Sprite[] { normalSprite });
            phisicalUpdater.OnCollied += new OnCollidedEventHandler(phisicalUpdater_OnCollied);
            phisicalUpdater.OnOverlap += new OnCollidedEventHandler(phisicalUpdater_OnOverlap);
        }

        public void CallOnOverlap(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            if (OnOverLap != null)
                OnOverLap(Sender, result, objB);
        }

        public void CallOnCollied(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            if (OnCollied != null)
                OnCollied(Sender, result, objB);
        }

        void phisicalUpdater_OnOverlap(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            if (OnOverLap != null)
                InfoRePath.CallEvent(this.mgPath, "OnOverlap", OnOverLap, this, result, objB);
        }

        void phisicalUpdater_OnCollied(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            if (OnCollied != null)
                InfoRePath.CallEvent(this.mgPath, "OnCollied", OnCollied, this, result, objB);
        }


        protected NonInertiasColUpdater phisicalUpdater;

        public IPhisicalUpdater PhisicalUpdater
        {
            get { return phisicalUpdater; }
        }

        #endregion

        #region ICollideObj 成员

        public IColChecker ColChecker
        {
            get { return phisicalUpdater; }
        }

        #endregion

        #region IGameObj 成员

        public float Azi
        {
            get { return phisicalUpdater.Azi; }
            set { phisicalUpdater.Azi = value; }
        }

        protected string mgPath;
        public string MgPath
        {
            get
            {
                return mgPath;
            }
            set
            {
                mgPath = value;
            }
        }

        protected string name;
        public string Name
        {
            get { return name; }
        }

        protected GameObjInfo objInfo;
        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        public Microsoft.Xna.Framework.Vector2 Pos
        {
            get
            {
                return phisicalUpdater.Pos;
            }
            set
            {
                phisicalUpdater.Pos = value;
            }
        }

        #endregion

        #region IUpdater 成员

        protected bool openControl;
        public void Update(float seconds)
        {
            if (openControl)
                HandlerControl(seconds);

            UpdateTimers(seconds);

            UpdateSprite();
        }

        private void UpdateSprite()
        {
            hitingSprite.Pos = this.Pos;
            hitingSprite.Rata = this.Azi;
        }

        private void UpdateTimers(float seconds)
        {
            if (shootTimer < 0)
                shootTimer += seconds;
            if (wtfTimer < 0)
                wtfTimer += seconds;
            if (stillTimer < 0)
                stillTimer += seconds;

        }


        #endregion

        #region IDrawableObj 成员


        Sprite normalSprite;
        Sprite hitingSprite;

        AnimatedSpriteSeries destoryAnimate;

        private void InitializeTex(Vector2 pos, float azi)
        {
            normalSprite = new Sprite(BaseGame.RenderEngine);
            normalSprite.LoadTextureFromFile(Path.Combine(Directories.ContentDirectory, "Rules\\SpaceWar\\image\\field_spaceship_001.png"), true);
            normalSprite.SetParameters(new Vector2(16, 16), pos, 1f, azi, Color.White, LayerDepth.TankBase, SpriteBlendMode.AlphaBlend);

            hitingSprite = new Sprite(BaseGame.RenderEngine);
            hitingSprite.LoadTextureFromFile(Path.Combine(Directories.ContentDirectory, "Rules\\SpaceWar\\image\\field_spaceship_002.png"), true);
            hitingSprite.SetParameters(new Vector2(16, 16), pos, 1f, azi, Color.White, LayerDepth.TankBase, SpriteBlendMode.AlphaBlend);

            destoryAnimate = new AnimatedSpriteSeries(BaseGame.RenderEngine);
            destoryAnimate.LoadSeriesFromFiles(BaseGame.RenderEngine, Path.Combine(Directories.ContentDirectory, "Rules\\SpaceWar\\image"), "field_burst_00", ".png", 1, 6, false);
        }

        public void Draw()
        {
            if (isDead)
            {

            }
            else if (stillTimer >= 0)
            {
                normalSprite.Draw();
            }
            else
            {
                hitingSprite.Draw();
            }
        }

        #endregion

        
    }
}
