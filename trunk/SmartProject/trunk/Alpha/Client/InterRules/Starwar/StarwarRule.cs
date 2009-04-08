using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Rule;
using SmartTank.Screens;
using SmartTank.net;
using SmartTank.GameObjs;
using SmartTank.PhiCol;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankEngine2D.Graphics;
using SmartTank;
using SmartTank.Draw;
using TankEngine2D.Helpers;
using TankEngine2D.Input;
using Microsoft.Xna.Framework.Input;

namespace InterRules.Starwar
{
    class StarwarRule : IGameRule
    {
        #region IGameRule ��Ա

        public string RuleIntroduction
        {
            get { return "20090328~20090417:��д���Ա��..."; }
        }

        public string RuleName
        {
            get { return "Starwar"; }
        }

        #endregion

        #region IGameScreen ��Ա

        public void OnClose()
        {
        }

        public void Render()
        {

        }

        public bool Update(float second)
        {
            return false;
        }

        #endregion
    }


    class StarwarLogic : RuleSupNet, IGameScreen
    {



        public StarwarLogic()
        {

        }


        #region IGameScreen ��Ա

        void IGameScreen.OnClose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IGameScreen.Render()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool IGameScreen.Update(float second)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }


    class WarShip : IGameObj, ICollideObj, IPhisicalObj
    {

        public WarShip(string name, Vector2 pos, float azi, bool openControl)
        {
            this.name = name;
            this.objInfo = new GameObjInfo("WarShip", name);
            this.openControl = openControl;
            InitializeTex(pos, azi);
            InitializePhisical(pos, azi);
            
        }

        #region Logic

        public const float cSpeedMax = 128;
        public const float cSpeedAccel = 100;       // ���ٶ�
        public const float cSpeedDecay = 0.5f;       // ��������״̬��ÿ�뽵�͵��ٶȰٷֱȣ�ʵ���ǻ��ֹ��� 
        public const float cStillSpeedScale = 0.0f; // Ӳֱ״̬���ƶ��ٶȵ�����ϵ��
        public readonly float cRootTwoDivideTwo = (float)(Math.Sqrt(2) * 0.5f);

        public const int cIniHP = 1000;
        public const float cShootCD = 0.25f;
        public const int cHitbyShellDamage = 100;
        public const int cHitbyObjExceptShell = 100;// û���߻�ȷ��
        public const float cStillTime = 0.3f;
        public const float cWTFTime = 3f;
        public const int cScoreByHit = 6;

        public const int cShootEndDest = 10;

        protected Vector2 curSpeed = new Vector2(0, 0);

        protected int curHP = cIniHP;
        protected bool isDead = false;

        /* ��ʱ����С��0ʱ��Ч��ʱ״̬
         * */
        protected float stillTimer = 0;
        protected float wtfTimer = 0;
        protected float shootTimer = 0;



        public delegate void WarShipShootEventHandler(WarShip firer, Vector2 endPoint, float azi);
        public delegate void WarShipDeadEventHandler(WarShip sender);

        public WarShipShootEventHandler OnShoot;
        public WarShipDeadEventHandler OnDead;

        protected bool Shoot(float ShootAzi)
        {
            if (shootTimer > 0)
            {
                if (OnShoot != null)
                    OnShoot(this, Pos + MathTools.NormalVectorFromAzi(Azi) * cShootEndDest, Azi);
                shootTimer -= cShootCD;
                return true;
            }
            return false;
        }

        public void BeginStill()
        {
            stillTimer = -cStillTime;
        }

        public void BeginWTF()
        {
            wtfTimer = -cWTFTime;
        }

        public void HitByShell()
        {
            LooseHP(cHitbyShellDamage);
        }

        public void HitByObjExceptShell()
        {
            LooseHP(cHitbyObjExceptShell);
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
                float maxSpeed = cSpeedMax;
                float accel = cSpeedAccel;
                if (stillTimer < 0)
                {
                    // Ӳֱ״̬
                    // maxSpeed *= cStillSpeedScale; 
                    accel *= cStillSpeedScale;
                }

                #region  ��ȡ��������

                bool Up = InputHandler.IsKeyDown(Keys.W);
                bool Left = InputHandler.IsKeyDown(Keys.A);
                bool Down = InputHandler.IsKeyDown(Keys.S);
                bool Right = InputHandler.IsKeyDown(Keys.D);

                Vector2 deltaSpeed = new Vector2();
                bool noInput = false;

                // ����
                if (Up && Left && !Right && !Down)
                {
                    deltaSpeed.X = cRootTwoDivideTwo;
                    deltaSpeed.Y = -cRootTwoDivideTwo;
                }
                // ����
                else if (Up && Right && !Left && !Down)
                {
                    deltaSpeed.X = -cRootTwoDivideTwo;
                    deltaSpeed.Y = -cRootTwoDivideTwo;
                }
                // ����
                else if (Down && Right && !Up && !Left)
                {
                    deltaSpeed.X = cRootTwoDivideTwo;
                    deltaSpeed.Y = cRootTwoDivideTwo;
                }
                // ����
                else if (Down && Left && !Up && !Right)
                {
                    deltaSpeed.X = -cRootTwoDivideTwo;
                    deltaSpeed.Y = cRootTwoDivideTwo;
                }
                // ��
                else if (Up && !Down)
                {
                    deltaSpeed.X = 0;
                    deltaSpeed.Y = -1.0f;
                }
                // ��
                else if (Down && !Up)
                {
                    deltaSpeed.X = 0;
                    deltaSpeed.Y = 1.0f;
                }
                // ��
                else if (Left && !Right)
                {
                    deltaSpeed.X = -1.0f;
                    deltaSpeed.Y = 0;
                }
                // ��
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

                #region ������״̬

                if (InputHandler.LastMouseLeftDown)
                {
                    Vector2 mousePos = InputHandler.GetCurMousePosInLogic(BaseGame.RenderEngine);
                    float shootAzi = MathTools.AziFromRefPos(mousePos - this.Pos);

                    this.Shoot(shootAzi);
                }

                #endregion


                if (!noInput)
                {
                    phisicalUpdater.Azi = MathTools.AziFromRefPos(curSpeed);
                    deltaSpeed *= seconds * accel;
                    curSpeed += deltaSpeed;
                    float SpeedAbs = curSpeed.Length();
                    if (SpeedAbs > cSpeedMax)
                    {
                        curSpeed *= cSpeedMax / SpeedAbs;
                    }
                }
                else // ˥��
                {
                    // ����ʱ�����Ǿ��ȵģ����Ҽ����С��
                    if (cSpeedDecay * seconds < 1)
                    {
                        curSpeed *= (1 - cSpeedDecay * seconds);
                    }
                }
                phisicalUpdater.Vel = curSpeed;
            }
        }

        #endregion

        #region IPhisicalObj ��Ա

        public event OnCollidedEventHandler OnCollied;
        public event OnCollidedEventHandler OnOverLap;


        private void InitializePhisical(Vector2 pos, float azi)
        {
            phisicalUpdater = new NonInertiasColUpdater(objInfo, new Sprite[] { texture });
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
                InfoRePath.CallEvent(this.mgPath, "OnOverlap", OnOverLap, Sender, result, objB);
        }

        void phisicalUpdater_OnCollied(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            if (OnCollied != null)
                InfoRePath.CallEvent(this.mgPath, "OnCollied", OnCollied, Sender, result, objB);
        }


        protected NonInertiasColUpdater phisicalUpdater;

        public IPhisicalUpdater PhisicalUpdater
        {
            get { return phisicalUpdater; }
        }

        #endregion

        #region ICollideObj ��Ա

        public IColChecker ColChecker
        {
            get { return phisicalUpdater; }
        }

        #endregion

        #region IGameObj ��Ա

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

        #region IUpdater ��Ա

        protected bool openControl;
        public void Update(float seconds)
        {
            if (openControl)
                HandlerControl(seconds);

            UpdateTimers(seconds);
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

        #region IDrawableObj ��Ա


        Sprite texture;
        private void InitializeTex(Vector2 pos, float azi)
        {
            texture = new Sprite(BaseGame.RenderEngine);
            texture.LoadTextureFromFile(Path.Combine(Directories.GameObjsDirectory, "Internal\\Ball\\scorpion"), true);
            texture.SetParameters(new Vector2(0, 0), pos, 1.0f, azi, Color.White, LayerDepth.TankBase, SpriteBlendMode.AlphaBlend);
        }

        public void Draw()
        {
            texture.Draw();
        }

        #endregion
    }
}
