using System;
using System.Collections.Generic;
using System.Text;
using Platform.AIHelper;
using Platform.GameObjects.Tank.TankAIs;
using Microsoft.Xna.Framework;
using Platform.Senses.Vision;
using GameBase.Helpers;
using GameBase.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Platform.GameObjects;
using Platform.GameObjects.Tank.Tanks;

namespace InterRules.Duel
{
    class ConsiderSearchEnemy : IConsider
    {
        #region Constants

        const float initialGridTime = -10;
        const float defaultDirWeightFactor = 5f;
        const float defaultDistanceWeightFactor = 0.2f;

        #endregion

        #region Variables

        IAIOrderServer orderServer;

        ConsiderPriority curPriority;

        Vector2 mapSize;

        float[,] grids;

        float gridWidth, gridHeight;

        Point LastGrid;

        float curTime;

        AIActionHelper action;

        public float forwardSpd = 0;

        public float dirWeightFactor = defaultDirWeightFactor;

        public float distanceWeightFactor = defaultDistanceWeightFactor;

        bool enterNewGrid = false;


        #endregion

        #region Constructions

        public ConsiderSearchEnemy ( Vector2 mapSize, float raderRadius )
        {
            this.mapSize = mapSize;
            curPriority = ConsiderPriority.Vacancy;

            int gridMaxX = (int)(mapSize.X / raderRadius) + 1;
            int gridMaxY = (int)(mapSize.Y / raderRadius) + 1;

            grids = new float[gridMaxX, gridMaxY];
            for (int i = 0; i < gridMaxX; i++)
            {
                for (int j = 0; j < gridMaxY; j++)
                {
                    grids[i, j] = initialGridTime;
                }
            }

            gridWidth = mapSize.X / gridMaxX;
            gridHeight = mapSize.Y / gridMaxY;


            curTime = 0;
        }

        public ConsiderSearchEnemy ( Vector2 mapSize, float raderRadius, float dirWeight )
        {
            this.mapSize = mapSize;
            curPriority = ConsiderPriority.Vacancy;

            int gridMaxX = (int)(mapSize.X / raderRadius) + 1;
            int gridMaxY = (int)(mapSize.Y / raderRadius) + 1;

            grids = new float[gridMaxX, gridMaxY];
            for (int i = 0; i < gridMaxX; i++)
            {
                for (int j = 0; j < gridMaxY; j++)
                {
                    grids[i, j] = initialGridTime;
                }
            }

            gridWidth = mapSize.X / gridMaxX;
            gridHeight = mapSize.Y / gridMaxY;

            curTime = 0;

            this.dirWeightFactor = dirWeight;
        }

        #endregion

        #region IConsider 成员

        public void Authorize ()
        {
            LastGrid = FindCurGrid();
            UpdateCurGridTime( LastGrid );

            Vector2 curDir = orderServer.Direction;
            Vector2 curPos = orderServer.Pos;
            float mapDistance = mapSize.LengthSquared();

            Vector2 aimGrid = Vector2.Zero;
            float maxWeight = 0;
            for (int x = 0; x < grids.GetLength( 0 ); x++)
            {
                for (int y = 0; y < grids.GetLength( 1 ); y++)
                {
                    if (LastGrid.X == x && LastGrid.Y == y)
                        continue;

                    Vector2 gridPos = new Vector2( (x + 0.5f) * gridWidth, (y + 0.5f) * gridHeight );
                    Vector2 gridDir = gridPos - curPos;
                    float timeSpan = curTime - grids[x, y];
                    float dirWeight = Vector2.Dot( curDir, Vector2.Normalize( gridDir ) ) + 1;
                    dirWeight *= 0.5f * dirWeightFactor;

                    float distance = gridDir.LengthSquared();
                    float distanceWeight = mapDistance / distance * distanceWeightFactor;

                    float sumWeight = timeSpan + dirWeight + distanceWeight;

                    if (sumWeight > maxWeight)
                    {
                        maxWeight = sumWeight;
                        aimGrid = gridPos;
                    }
                }
            }


            action.AddOrder( new OrderMoveCircle( aimGrid, forwardSpd, orderServer,
                delegate( IActionOrder order )
                {
                    Authorize();
                } , false ) );

            //TextEffect.AddRiseFade( "aim", aimGrid, 2f, Color.Black, LayerDepth.Text, FontType.Lucida, 300, 0 );
        }

        public void Bereave ()
        {
        }

        public ConsiderPriority CurPriority
        {
            get { return curPriority; }
        }

        public ActionRights NeedRights
        {
            get { return ActionRights.Move | ActionRights.Rota; }
        }

        public void Observe ()
        {
            List<IEyeableInfo> eyeableInfo = orderServer.GetEyeableInfo();
            if (eyeableInfo.Count == 0)
            {
                curPriority = ConsiderPriority.Low;
            }


            Point curGrid = FindCurGrid();
            if (curGrid != LastGrid)
            {
                LastGrid = curGrid;
                UpdateCurGridTime( curGrid );
                enterNewGrid = true;
            }
        }

        public Platform.GameObjects.Tank.TankAIs.IAIOrderServer OrderServer
        {
            set
            {
                orderServer = value;
                action = new AIActionHelper( orderServer );
                if (forwardSpd == 0)
                    forwardSpd = orderServer.MaxForwardSpeed;

                //averageTime = mapSize.X / orderServer.MaxForwardSpeed;

            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            curTime += seconds;

            if (enterNewGrid)
            {
                Authorize();
                enterNewGrid = false;
            }

            List<IEyeableInfo> eyeableInfo = orderServer.GetEyeableInfo();
            if (eyeableInfo.Count != 0)
            {
                curPriority = ConsiderPriority.Vacancy;
                action.StopMove();
                action.StopRota();
            }

            action.Update( seconds );
        }

        #endregion

        #region Private Functions

        private void UpdateCurGridTime ( Point curGrid )
        {
            grids[curGrid.X, curGrid.Y] = curTime;
        }

        private Point FindCurGrid ()
        {
            Vector2 pos = orderServer.Pos;
            return new Point( (int)(pos.X / gridWidth), (int)(pos.Y / gridHeight) );
        }

        #endregion

    }

    class ConsiderRaderScan : IConsider
    {
        #region Type Define

        enum State
        {
            Start,
            Scan,
            Circle,
            Border,
        }

        enum Area
        {
            Middle = 0,
            Left = 1,
            Right = 2,
            Up = 4,
            Dowm = 8,
            UpLeft = 5,
            UpRight = 6,
            DownLeft = 9,
            DownRight = 10,
        }

        #endregion

        #region Constances

        const float defaultGuardDestance = 30;
        const float defaultCircleSpeedFactor = 0.3f;

        #endregion

        #region Variables

        IAIOrderServer orderServer;

        AIActionHelper action;

        ConsiderPriority curPriority;

        Vector2 mapSize;

        float guardDestance = defaultGuardDestance;

        public float circleSpeedFactor = defaultCircleSpeedFactor;

        State lastState;

        bool firstCircleFinished;

        #endregion

        #region Constructions

        public ConsiderRaderScan ( Vector2 mapSize )
        {
            this.mapSize = mapSize;
            curPriority = ConsiderPriority.Vacancy;
            firstCircleFinished = false;
            lastState = State.Start;
        }

        #endregion

        #region IConsider 成员

        public void Authorize ()
        {
            if (lastState == State.Start)
            {
                float curRaderAzi = orderServer.RaderAzi;
                action.AddOrder( new OrderRotaRaderToAng( curRaderAzi + 0.66f * MathHelper.Pi, 0,
                    delegate( IActionOrder order )
                    {
                        action.AddOrder( new OrderRotaRaderToAng( curRaderAzi + 1.23f * MathHelper.Pi, 0,
                            delegate( IActionOrder order1 )
                            {
                                action.AddOrder( new OrderRotaRaderToAng( curRaderAzi + 2 * MathHelper.Pi, 0,
                                    delegate( IActionOrder order2 )
                                    {
                                        firstCircleFinished = true;
                                    }, false ) );
                            }, false ) );
                    }, false ) );

            }
            else if (lastState == State.Scan)
            {
                float maxScanAng = MathHelper.PiOver2 - orderServer.RaderAng;
                action.AddOrder( new OrderScanRader( maxScanAng, 0, true ) );
            }
            else if (lastState == State.Circle)
            {
                orderServer.TurnRaderWiseSpeed = orderServer.MaxRotaRaderSpeed;
            }
            else if (lastState == State.Border)
            {
                float maxRaderAng;
                float interDir = FindInterDir( out maxRaderAng );
                action.AddOrder( new OrderScanRaderAzi( interDir + maxRaderAng, interDir - maxRaderAng, 0, true ) );
            }

        }

        public void Bereave ()
        {
            action.StopRotaRader();
            //orderServer.TurnRaderWiseSpeed = 0;
        }

        public ConsiderPriority CurPriority
        {
            get { return curPriority; }
        }

        public ActionRights NeedRights
        {
            get { return ActionRights.RotaRader; }
        }

        public void Observe ()
        {
            List<IEyeableInfo> eyeableInfo = orderServer.GetEyeableInfo();
            if (eyeableInfo.Count == 0)
            {
                curPriority = ConsiderPriority.Low;
            }
            else
            {
                curPriority = ConsiderPriority.Vacancy;
            }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = value;
                action = new AIActionHelper( orderServer );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            State curState = CheckState();
            if (curState != lastState)
            {
                lastState = curState;
                Authorize();
            }

            action.Update( seconds );


        }

        #endregion

        #region Private Functions

        private State CheckState ()
        {
            if (!firstCircleFinished)
                return State.Start;

            Vector2 curPos = orderServer.Pos;

            if (curPos.X < guardDestance || curPos.X > mapSize.X - guardDestance ||
                curPos.Y < guardDestance || curPos.Y > mapSize.Y - guardDestance)
                return State.Border;

            float curSpeed = orderServer.ForwardSpeed;
            if (curSpeed < orderServer.MaxForwardSpeed * circleSpeedFactor)
            {
                return State.Circle;
            }

            return State.Scan;
        }

        private float FindInterDir ( out float maxRaderAng )
        {
            int sum = 0;
            Vector2 pos = orderServer.Pos;
            if (pos.X < guardDestance)
                sum += 1;
            if (pos.X > mapSize.X - guardDestance)
                sum += 2;
            if (pos.Y < guardDestance)
                sum += 4;
            if (pos.Y > mapSize.Y - guardDestance)
                sum += 8;

            Area curArea = (Area)sum;
            if (curArea == Area.Dowm)
            {
                maxRaderAng = MathHelper.PiOver2 - orderServer.RaderAng;
                return 0;
            }
            else if (curArea == Area.Up)
            {
                maxRaderAng = MathHelper.PiOver2 - orderServer.RaderAng;
                return MathHelper.Pi;
            }
            else if (curArea == Area.Left)
            {
                maxRaderAng = MathHelper.PiOver2 - orderServer.RaderAng;
                return MathHelper.PiOver2;
            }
            else if (curArea == Area.Right)
            {
                maxRaderAng = MathHelper.PiOver2 - orderServer.RaderAng;
                return MathHelper.Pi + MathHelper.PiOver2;
            }
            else if (curArea == Area.UpLeft)
            {
                maxRaderAng = MathHelper.PiOver4 - orderServer.RaderAng;
                return MathHelper.PiOver2 + MathHelper.PiOver4;
            }
            else if (curArea == Area.UpRight)
            {
                maxRaderAng = MathHelper.PiOver4 - orderServer.RaderAng;
                return MathHelper.Pi + MathHelper.PiOver4;
            }
            else if (curArea == Area.DownRight)
            {
                maxRaderAng = MathHelper.PiOver4 - orderServer.RaderAng;
                return -MathHelper.PiOver4;
            }
            else if (curArea == Area.DownLeft)
            {
                maxRaderAng = MathHelper.PiOver4 - orderServer.RaderAng;
                return MathHelper.PiOver4;
            }
            maxRaderAng = 0;
            return 0;
        }

        #endregion
    }

    class ConsiderAwayFromEnemyTurret : IConsider
    {
        #region Type Define
        enum Area
        {
            Middle = 0,
            Left = 1,
            Right = 2,
            Up = 4,
            Dowm = 8,
            UpLeft = 5,
            UpRight = 6,
            DownLeft = 9,
            DownRight = 10,
        }
        #endregion

        #region Constants
        const float defaultGuardAng = MathHelper.Pi / 6;
        const float guardDestance = 50;
        #endregion

        #region Variables
        IAIOrderServer orderServer;

        AIActionHelper action;

        ConsiderPriority curPriority;

        public float guardAng = defaultGuardAng;

        TankSinTur.TankCommonEyeableInfo enemyInfo;

        bool lastDeltaAngWise;

        Vector2 mapSize;

        #endregion

        #region Constrution
        public ConsiderAwayFromEnemyTurret ( Vector2 mapSize )
        {
            curPriority = ConsiderPriority.Vacancy;
            this.mapSize = mapSize;
        }
        #endregion

        #region IConsider 成员

        public void Authorize ()
        {
            if (enemyInfo == null)
                return;

            Vector2 selfToEnemyVec = orderServer.Pos - enemyInfo.Pos;
            float selfToEnemyAzi = MathTools.AziFromRefPos( selfToEnemyVec );
            float deltaAzi = MathTools.AngTransInPI( selfToEnemyAzi - enemyInfo.TurretAimAzi );
            lastDeltaAngWise = deltaAzi > 0;

            Vector2 aimDir = Vector2.Transform( selfToEnemyVec,
                Matrix.CreateRotationZ( (deltaAzi > 0 ? MathHelper.PiOver2 : -MathHelper.PiOver2) + deltaAzi ) );

            Area curArea = FindArea();
            if (curArea != Area.Middle)
            {
                Vector2 borderDir = FindBorderDir( curArea );
                if (Vector2.Dot( aimDir, borderDir ) < 0)
                    aimDir = -aimDir;
            }

            float aimAzi = MathTools.AziFromRefPos( aimDir );

            Vector2 selfDir = orderServer.Direction;

            if (Vector2.Dot( selfDir, aimDir ) > 0)
            {
                action.AddOrder( new OrderRotaToAzi( aimAzi ) );
                action.AddOrder( new OrderMove( 100 ) );
            }
            else
            {
                action.AddOrder( new OrderRotaToAzi( aimAzi + MathHelper.Pi ) );
                action.AddOrder( new OrderMove( -100 ) );
            }
        }

        public void Bereave ()
        {

        }

        public ConsiderPriority CurPriority
        {
            get { return curPriority; }
        }

        public ActionRights NeedRights
        {
            get { return ActionRights.Move | ActionRights.Rota; }
        }

        public void Observe ()
        {
            List<IEyeableInfo> eyeableInfo = orderServer.GetEyeableInfo();
            if (eyeableInfo.Count != 0)
            {
                IEyeableInfo first = eyeableInfo[0];
                if (first is TankSinTur.TankCommonEyeableInfo)
                {
                    enemyInfo = (TankSinTur.TankCommonEyeableInfo)first;
                    float selfToEnemyAzi = MathTools.AziFromRefPos( orderServer.Pos - enemyInfo.Pos );
                    float enemyTurretAzi = enemyInfo.TurretAimAzi;
                    if (Math.Abs( MathTools.AngTransInPI( enemyTurretAzi - selfToEnemyAzi ) ) < guardAng)
                    {
                        curPriority = ConsiderPriority.High;
                        return;
                    }
                }
            }

            curPriority = ConsiderPriority.Vacancy;
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = value;
                action = new AIActionHelper( orderServer );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            if (enemyInfo == null)
                return;

            Vector2 selfToEnemyVec = orderServer.Pos - enemyInfo.Pos;
            float selfToEnemyAzi = MathTools.AziFromRefPos( selfToEnemyVec );
            float deltaAzi = selfToEnemyAzi - enemyInfo.TurretAimAzi;
            bool deltaAngWise = deltaAzi > 0;
            if (deltaAngWise != lastDeltaAngWise)
            {
                lastDeltaAngWise = deltaAngWise;
                Authorize();
            }
        }

        #endregion

        #region Private Functions

        private Area FindArea ()
        {
            int sum = 0;
            Vector2 pos = orderServer.Pos;
            if (pos.X < guardDestance)
                sum += 1;
            if (pos.X > mapSize.X - guardDestance)
                sum += 2;
            if (pos.Y < guardDestance)
                sum += 4;
            if (pos.Y > mapSize.Y - guardDestance)
                sum += 8;

            return (Area)sum;
        }

        private Vector2 FindBorderDir ( Area area )
        {
            if (area == Area.Left)
                return new Vector2( 1, 0 );
            else if (area == Area.Right)
                return new Vector2( -1, 0 );
            else if (area == Area.Up)
                return new Vector2( 0, 1 );
            else if (area == Area.Dowm)
                return new Vector2( 0, -1 );
            else if (area == Area.UpLeft)
                return Vector2.Normalize( mapSize );
            else if (area == Area.UpRight)
                return Vector2.Normalize( new Vector2( -mapSize.X, mapSize.Y ) );
            else if (area == Area.DownLeft)
                return Vector2.Normalize( new Vector2( mapSize.X, -mapSize.Y ) );
            else if (area == Area.DownRight)
                return Vector2.Normalize( new Vector2( -mapSize.X, -mapSize.Y ) );
            else
                return Vector2.Zero;
        }

        #endregion

    }

    class ConsiderRaderLockEnemy : IConsider
    {
        #region Variables

        IAIOrderServer orderServer;

        AIActionHelper action;

        ConsiderPriority curPriority;

        Vector2 enemyPos;

        #endregion

        #region IConsider 成员

        public void Authorize ()
        {

        }

        public void Bereave ()
        {
            action.StopRotaRader();
        }

        public ConsiderPriority CurPriority
        {
            get { return curPriority; }
        }

        public ActionRights NeedRights
        {
            get { return ActionRights.RotaRader; }
        }

        public void Observe ()
        {
            List<IEyeableInfo> eyeableInfo = orderServer.GetEyeableInfo();
            if (eyeableInfo.Count != 0)
            {
                curPriority = ConsiderPriority.High;
                IEyeableInfo first = eyeableInfo[0];
                if (first is TankSinTur.TankCommonEyeableInfo)
                {
                    TankSinTur.TankCommonEyeableInfo enemyInfo = (TankSinTur.TankCommonEyeableInfo)first;
                    enemyPos = enemyInfo.Pos;
                }
            }
            else
                curPriority = ConsiderPriority.Vacancy;
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = value;
                action = new AIActionHelper( orderServer );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            action.AddOrder( new OrderRotaRaderToPos( enemyPos ) );
            action.Update( seconds );
        }

        #endregion
    }

    class ConsiderKeepDistanceFromEnemy : IConsider
    {
        #region Type Define
        enum State
        {
            Near,
            Mid,
            Far,
        }
        #endregion

        #region Constants

        const float defaultMaxGuardFactor = 0.6f;
        const float defaultMinGuardFactor = 0.4f;
        const float defaultDepartFactor = 1f;
        const float maxDistance = 1000;
        #endregion

        #region Variables
        IAIOrderServer orderServer;

        AIActionHelper action;

        ConsiderPriority curPriority;

        float raderRadius;

        float maxGuardFactor = defaultMaxGuardFactor;

        float minGuardFactor = defaultMinGuardFactor;

        Vector2 enemyPos;

        State lastState;

        float departFactor = defaultDepartFactor;
        #endregion

        #region Construction

        public ConsiderKeepDistanceFromEnemy ()
        {
            curPriority = ConsiderPriority.Vacancy;
        }
        #endregion

        #region IConsider 成员

        public void Authorize ()
        {
            Vector2 curPos = orderServer.Pos;
            Vector2 enemyVec = Vector2.Normalize( curPos - enemyPos );
            Vector2 curDir = orderServer.Direction;

            Vector2 lineVec = new Vector2( enemyVec.Y, -enemyVec.X );
            if (Vector2.Dot( lineVec, curDir ) < 0)
                lineVec = -lineVec;

            Vector2 departVec = Vector2.Zero;
            if (lastState == State.Far)
                departVec = -enemyVec;
            else
                departVec = enemyVec;

            Vector2 aimVec = Vector2.Normalize( lineVec * departFactor + departVec );

            action.AddOrder( new OrderRotaToAzi( MathTools.AziFromRefPos( aimVec ) ) );
            if (Vector2.Dot( departVec, curDir ) < 0 && lastState == State.Near)
                action.AddOrder( new OrderMove( -maxDistance ) );
            else if (RandomHelper.GetRandomFloat( -1, 1 ) > -0.3f)
                action.AddOrder( new OrderMove( maxDistance ) );
            else
                action.AddOrder( new OrderMove( -maxDistance ) );
        }

        public void Bereave ()
        {
            action.StopMove();
            action.StopRota();
        }

        public ConsiderPriority CurPriority
        {
            get { return curPriority; }
        }

        public ActionRights NeedRights
        {
            get { return ActionRights.Move | ActionRights.Rota; }
        }

        public void Observe ()
        {

            List<IEyeableInfo> eyeableInfo = orderServer.GetEyeableInfo();
            if (eyeableInfo.Count != 0)
            {
                IEyeableInfo first = eyeableInfo[0];
                if (first is TankSinTur.TankCommonEyeableInfo)
                {
                    TankSinTur.TankCommonEyeableInfo enemyInfo = (TankSinTur.TankCommonEyeableInfo)first;
                    enemyPos = enemyInfo.Pos;
                    float Distance = Vector2.Distance( enemyPos, orderServer.Pos );

                    Vector2 curPos = orderServer.Pos;
                    Vector2 enemyVec = Vector2.Normalize( curPos - enemyPos );
                    Vector2 curDir = orderServer.Direction;

                    Vector2 lineVec = new Vector2( enemyVec.Y, -enemyVec.X );
                    if (Vector2.Dot( lineVec, curDir ) < 0)
                        lineVec = -lineVec;
                    if (Distance > maxGuardFactor * raderRadius || Distance < minGuardFactor * raderRadius ||
                        Vector2.Dot( curDir, lineVec ) < 0.8)
                    {
                        curPriority = ConsiderPriority.Urgency;
                        return;
                    }
                }
            }

            curPriority = ConsiderPriority.Vacancy;

        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = value;
                action = new AIActionHelper( orderServer );
                this.raderRadius = orderServer.RaderRadius;
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            lastState = FindCurState();

            Authorize();

            //else
            //{
            //    curPriority = ConsiderPriority.Vacancy;
            //}

            action.Update( seconds );
        }

        private State FindCurState ()
        {
            List<IEyeableInfo> eyeableInfo = orderServer.GetEyeableInfo();
            if (eyeableInfo.Count != 0)
            {
                IEyeableInfo first = eyeableInfo[0];
                if (first is TankSinTur.TankCommonEyeableInfo)
                {
                    TankSinTur.TankCommonEyeableInfo enemyInfo = (TankSinTur.TankCommonEyeableInfo)first;
                    enemyPos = enemyInfo.Pos;
                    float Distance = Vector2.Distance( enemyPos, orderServer.Pos );
                    if (Distance > maxGuardFactor * raderRadius)
                        return State.Far;
                    else if (Distance < minGuardFactor * raderRadius)
                        return State.Near;
                    else
                        return State.Mid;
                }
            }
            return State.Mid;
        }

        #endregion
    }

    class ConsiderShootEnemy : IConsider
    {
        #region Variables

        IAIOrderServerSinTur orderServer;

        ConsiderPriority curPriority;

        AIActionHelper action;

        Vector2 enemyPos;

        Vector2 enemyVel;

        bool aimming;

        float aimmingTime;

        Vector2 aimmingPos;
        #endregion

        #region Consturtions
        public ConsiderShootEnemy ()
        {

        }
        #endregion

        #region IConsider 成员

        public void Authorize ()
        {

            float curTurretAzi = orderServer.TurretAimAzi;

            float t = 0;

            float maxt = 30;
            Vector2 aimPos = Vector2.Zero;
            bool canShoot = false;
            while (t < maxt)
            {
                aimPos = enemyPos + enemyVel * t;
                float timeRota = Math.Abs( MathTools.AngTransInPI( MathTools.AziFromRefPos( aimPos - orderServer.TurretAxePos ) - curTurretAzi ) / orderServer.MaxRotaTurretSpeed );
                float timeShell = (Vector2.Distance( aimPos, orderServer.TurretAxePos ) - orderServer.TurretLength) / orderServer.ShellSpeed;
                if (MathTools.FloatEqual( timeRota + timeShell, t, 0.05f ))
                {
                    canShoot = true;
                    break;
                }
                t += 0.05f;
            }

            if (canShoot)
            {
                aimmingTime = t;
                aimmingPos = aimPos;
                aimming = true;
                action.AddOrder( new OrderRotaTurretToPos( aimPos, 0,
                    delegate( IActionOrder order )
                    {
                        orderServer.Fire();
                        aimming = false;
                    }, true ) );
            }
        }

        public void Bereave ()
        {
            aimming = false;
        }

        public ConsiderPriority CurPriority
        {
            get { return curPriority; }
        }

        public ActionRights NeedRights
        {
            get { return ActionRights.RotaTurret; }
        }

        public void Observe ()
        {
            List<IEyeableInfo> eyeableInfo = orderServer.GetEyeableInfo();
            if (eyeableInfo.Count != 0)
            {
                IEyeableInfo first = eyeableInfo[0];
                if (first is TankSinTur.TankCommonEyeableInfo)
                {
                    TankSinTur.TankCommonEyeableInfo enemyInfo = (TankSinTur.TankCommonEyeableInfo)first;
                    enemyPos = enemyInfo.Pos;
                    enemyVel = enemyInfo.Vel;
                }
                curPriority = ConsiderPriority.Urgency;
            }
            else
                curPriority = ConsiderPriority.Vacancy;
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = (IAIOrderServerSinTur)value;
                action = new AIActionHelper( orderServer );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            if (aimming)
            {
                aimmingTime -= seconds;
                if ((enemyPos + aimmingTime * enemyVel - aimmingPos).Length() > 4)
                {
                    Authorize();
                }
            }
            else
            {
                Authorize();
            }

            action.Update( seconds );
        }

        #endregion
    }
}
