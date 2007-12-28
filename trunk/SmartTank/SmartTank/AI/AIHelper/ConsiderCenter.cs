using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SmartTank.Update;
using TankEngine2D.DataStructure;
using TankEngine2D.Helpers;

namespace SmartTank.AI.AIHelper
{
    public class ConsiderCenter : IUpdater
    {
        #region Type Define

        struct ConsiderWithPRI
        {
            public IConsider consider;
            public int PRI;

            public ConsiderWithPRI ( IConsider consider, int PRI )
            {
                this.consider = consider;
                this.PRI = PRI;
            }
        }

        class PRIHelper
        {
            class Comparision : IComparer<ConsiderWithPRI>
            {

                #region IComparer<ConsiderWithPRI> 成员

                public int Compare ( ConsiderWithPRI x, ConsiderWithPRI y )
                {
                    if ((int)x.consider.CurPriority > (int)y.consider.CurPriority)
                        return -1;
                    else if ((int)x.consider.CurPriority < (int)y.consider.CurPriority)
                        return 1;
                    else
                    {
                        if (x.PRI > y.PRI)
                            return -1;
                        else if (x.PRI < y.PRI)
                            return 1;
                        else
                            return 0;
                    }

                }

                #endregion
            }

            bool RotaAuthorized = false;
            bool RotaTurretAuthorized = false;
            bool RotaRaderAuthorized = false;
            bool MoveAuthorized = false;

            List<IConsider> authorizeList;

            public PRIHelper ( List<ConsiderWithPRI> ConsiderWithPRIs )
            {
                authorizeList = new List<IConsider>();

                List<ConsiderWithPRI> PriorityList = new List<ConsiderWithPRI>();

                foreach (ConsiderWithPRI considerWithPRI in ConsiderWithPRIs)
                {
                    if (considerWithPRI.consider.CurPriority != ConsiderPriority.Vacancy)
                        PriorityList.Add( considerWithPRI );
                }

                PriorityList.Sort( new Comparision() );

                foreach (ConsiderWithPRI considerWithPRI in PriorityList)
                {
                    ActionRights needRights = considerWithPRI.consider.NeedRights;

                    bool canAuthorize = true;

                    if ((needRights & ActionRights.Rota) == ActionRights.Rota)
                    {
                        if (RotaAuthorized)
                            canAuthorize = false;

                    }
                    if ((needRights & ActionRights.RotaTurret) == ActionRights.RotaTurret)
                    {
                        if (RotaTurretAuthorized)
                            canAuthorize = false;
                    }
                    if ((needRights & ActionRights.RotaRader) == ActionRights.RotaRader)
                    {
                        if (RotaRaderAuthorized)
                            canAuthorize = false;
                    }
                    if ((needRights & ActionRights.Move) == ActionRights.Move)
                    {
                        if (MoveAuthorized)
                            canAuthorize = false;
                    }

                    if (canAuthorize)
                        AddConsiderAuthorized( considerWithPRI.consider );
                }
            }

            public List<IConsider> AuthorizeList
            {
                get { return authorizeList; }
            }

            private void AddConsiderAuthorized ( IConsider consider )
            {
                ActionRights needRights = consider.NeedRights;
                if ((needRights & ActionRights.Rota) == ActionRights.Rota)
                {
                    RotaAuthorized = true;
                }
                if ((needRights & ActionRights.RotaTurret) == ActionRights.RotaTurret)
                {
                    RotaTurretAuthorized = true;
                }
                if ((needRights & ActionRights.RotaRader) == ActionRights.RotaRader)
                {
                    RotaRaderAuthorized = true;
                }
                if ((needRights & ActionRights.Move) == ActionRights.Move)
                {
                    MoveAuthorized = true;
                }

                authorizeList.Add( consider );
            }
        }

        #endregion

        IAIOrderServer orderServer;

        List<ConsiderWithPRI> considers;

        PRIHelper curPRI, lastPRI;

        public ConsiderCenter ( IAIOrderServer orderServer )
        {
            considers = new List<ConsiderWithPRI>();
            this.orderServer = orderServer;
        }

        public void AddConsider ( IConsider consider, int PRI )
        {
            consider.OrderServer = orderServer;
            considers.Add( new ConsiderWithPRI( consider, PRI ) );
        }

        public void ClearAllConsider ()
        {
            considers.Clear();
        }

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            foreach (ConsiderWithPRI considerWithPRI in considers)
            {
                considerWithPRI.consider.Observe();
            }

            curPRI = new PRIHelper( considers );

            if (lastPRI != null)
            {
                foreach (IConsider last in lastPRI.AuthorizeList)
                {
                    if (curPRI.AuthorizeList.IndexOf( last ) == -1)
                    {
                        last.Bereave();
                    }
                }
                foreach (IConsider cur in curPRI.AuthorizeList)
                {
                    if (lastPRI.AuthorizeList.IndexOf( cur ) == -1)
                    {
                        cur.Authorize();
                    }
                }
            }
            else
            {
                foreach (IConsider cur in curPRI.AuthorizeList)
                {
                    cur.Authorize();
                }
            }

            foreach (IConsider consider in curPRI.AuthorizeList)
            {
                consider.Update( seconds );
            }

            lastPRI = curPRI;
        }

        #endregion
    }

    public enum ConsiderPriority
    {
        Vacancy,
        Low,
        Middle,
        High,
        Urgency,
    }

    public interface IConsider : IUpdater
    {
        ActionRights NeedRights { get;}
        IAIOrderServer OrderServer { set;}
        ConsiderPriority CurPriority { get;}
        void Observe ();
        void Authorize ();
        void Bereave ();
    }

    /// <summary>
    /// 当离边界的距离小于警戒距离时将优先级设为Urgency，并转向。
    /// </summary>
    public class ConsiderAwayFromBorder : IConsider
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

        const float defaultGuardDestance = 20;
        const float defaultSpaceForTankWidth = 10;

        #endregion

        #region Variables

        IAIOrderServer orderServer;

        ConsiderPriority curPriority;

        Rectanglef mapSize;

        Area lastArea;

        float rotaAng;

        bool turning;

        float turnRightSpeed;

        float forwardSpeed;

        public float guardDestance = defaultGuardDestance;

        public float spaceForTankWidth = defaultSpaceForTankWidth;

        #endregion

        #region Construction

        public ConsiderAwayFromBorder ( Rectanglef mapSize )
        {
            this.mapSize = mapSize;
            curPriority = ConsiderPriority.Vacancy;
        }

        public ConsiderAwayFromBorder ( Rectanglef mapSize, float guardDestance, float spaceForTankWidth )
        {
            this.mapSize = mapSize;
            this.guardDestance = guardDestance;
            this.spaceForTankWidth = spaceForTankWidth;

            curPriority = ConsiderPriority.Vacancy;
        }

        #endregion

        #region IConsider 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.Rota | ActionRights.Move; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = value;
            }
        }

        public ConsiderPriority CurPriority
        {
            get { return curPriority; }
        }

        public void Observe ()
        {
            Vector2 curPos = orderServer.Pos;
            if (curPos.X < mapSize.X + guardDestance || curPos.X > mapSize.X + mapSize.Width - guardDestance ||
                curPos.Y < mapSize.Y + guardDestance || curPos.Y > mapSize.Y + mapSize.Height - guardDestance)
            {
                curPriority = ConsiderPriority.Urgency;
            }
        }

        public void Authorize ()
        {
            lastArea = FindCurArea();

            Vector2 curDir = orderServer.Direction;
            Vector2 aimDir = Vector2.Zero;

            if (lastArea == Area.UpLeft || lastArea == Area.UpRight ||
                lastArea == Area.DownLeft || lastArea == Area.DownRight)
                aimDir = FindMirrorNormal( lastArea );

            else
            {
                Vector2 mirrorNormal = FindMirrorNormal( lastArea );
                if (Vector2.Dot( mirrorNormal, curDir ) > -0.5)
                    aimDir = mirrorNormal;
                else
                    aimDir = MathTools.ReflectVector( curDir, mirrorNormal );
            }

            rotaAng = MathTools.AngBetweenVectors( curDir, aimDir );
            float turnRight = MathTools.Vector2Cross( curDir, aimDir );

            Vector2 curPos = orderServer.Pos;

            float r = CalTurnRadius( curPos, curDir, turnRight );

            turnRightSpeed = orderServer.MaxRotaSpeed * Math.Sign( turnRight == 0 ? 1 : turnRight );
            forwardSpeed = orderServer.MaxRotaSpeed * r;

            orderServer.TurnRightSpeed = turnRightSpeed;
            orderServer.ForwardSpeed = forwardSpeed;

            turning = true;
        }


        public void Bereave ()
        {

        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            Area curArea = FindCurArea();
            if (curArea != lastArea && curArea != Area.Middle)
            {
                Authorize();
                return;
            }

            if (turning)
            {
                orderServer.TurnRightSpeed = turnRightSpeed;
                orderServer.ForwardSpeed = forwardSpeed;
                rotaAng -= orderServer.MaxRotaSpeed * seconds;
                if (rotaAng <= 0)
                {
                    turning = false;
                    orderServer.TurnRightSpeed = 0;
                    orderServer.ForwardSpeed = orderServer.MaxForwardSpeed;
                }
            }
            if (curArea == Area.Middle)
            {
                curPriority = ConsiderPriority.Vacancy;
                orderServer.ForwardSpeed = 0;
                orderServer.TurnRightSpeed = 0;
            }
        }

        #endregion

        #region Private Functions

        private Area FindCurArea ()
        {
            int sum = 0;
            Vector2 pos = orderServer.Pos;
            if (pos.X < mapSize.X + guardDestance)
                sum += 1;
            if (pos.X > mapSize.X + mapSize.Width - guardDestance)
                sum += 2;
            if (pos.Y < mapSize.Y + guardDestance)
                sum += 4;
            if (pos.Y > mapSize.Y + mapSize.Height - guardDestance)
                sum += 8;

            return (Area)sum;
        }

        private Vector2 FindMirrorNormal ( Area area )
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
                return Vector2.Normalize( new Vector2( mapSize.Width, mapSize.Height ) );
            else if (area == Area.UpRight)
                return Vector2.Normalize( new Vector2( -mapSize.Width, mapSize.Height ) );
            else if (area == Area.DownLeft)
                return Vector2.Normalize( new Vector2( mapSize.Width, -mapSize.Height ) );
            else if (area == Area.DownRight)
                return Vector2.Normalize( new Vector2( -mapSize.Width, -mapSize.Height ) );
            else
                return Vector2.Zero;
        }

        private float CalTurnRadius ( Vector2 curPos, Vector2 curDir, float turnRight )
        {
            Vector2 vertiDir = turnRight > 0 ? new Vector2( -curDir.Y, curDir.X ) : new Vector2( curDir.Y, -curDir.X );
            vertiDir.Normalize();

            float defaultRadius = orderServer.MaxForwardSpeed / orderServer.MaxRotaSpeed;

            float r = defaultRadius;

            for (; r > 0; r--)
            {
                Vector2 center = curPos + vertiDir * r;
                if (center.X > r + mapSize.X + spaceForTankWidth && mapSize.X + mapSize.Width - center.X > r + spaceForTankWidth &&
                    center.Y > r + mapSize.Y + spaceForTankWidth && mapSize.Y + mapSize.Height - center.Y > r + spaceForTankWidth)
                    break;
            }

            return r;
        }

        #endregion

    }


}
