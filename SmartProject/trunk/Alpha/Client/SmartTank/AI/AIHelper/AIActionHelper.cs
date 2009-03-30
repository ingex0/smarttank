using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SmartTank.Update;
using TankEngine2D.Helpers;
using TankEngine2D.DataStructure;

namespace SmartTank.AI.AIHelper
{
    public class AIActionHelper : IUpdater
    {
        #region Type Def
        class EmptyActionOrder : IActionOrder
        {
            #region IActionOrder 成员

            public ActionRights NeedRights
            {
                get { return 0; }
            }

            public IAIOrderServer OrderServer
            {
                set { ; }
            }

            public bool IsEnd
            {
                get { return false; }
            }

            public void Stop ()
            {
            }

            #endregion

            #region IUpdater 成员

            public void Update ( float seconds )
            {

            }

            #endregion
        }
        #endregion

        #region Variables

        IAIOrderServer orderServer;

        IActionOrder RotaRightOwner = new EmptyActionOrder();
        IActionOrder RotaTurretRightOwner = new EmptyActionOrder();
        IActionOrder RotaRaderRightOwner = new EmptyActionOrder();
        IActionOrder MoveRightOwner = new EmptyActionOrder();

        List<IActionOrder> curOrder = new List<IActionOrder>();

        #endregion

        #region Construction

        public AIActionHelper ( IAIOrderServer orderServer )
        {
            this.orderServer = orderServer;
        }

        #endregion

        #region AddOrder

        public void AddOrder ( IActionOrder order )
        {
            ActionRights needRights = order.NeedRights;
            if ((needRights & ActionRights.Rota) == ActionRights.Rota)
            {
                RotaRightOwner.Stop();
                DeleteAllRightOfOrder( RotaRightOwner );
                RotaRightOwner = order;
            }
            if ((needRights & ActionRights.RotaTurret) == ActionRights.RotaTurret)
            {
                RotaTurretRightOwner.Stop();
                DeleteAllRightOfOrder( RotaTurretRightOwner );
                RotaTurretRightOwner = order;
            }
            if ((needRights & ActionRights.RotaRader) == ActionRights.RotaRader)
            {
                RotaRaderRightOwner.Stop();
                DeleteAllRightOfOrder( RotaRaderRightOwner );
                RotaRaderRightOwner = order;
            }
            if ((needRights & ActionRights.Move) == ActionRights.Move)
            {
                MoveRightOwner.Stop();
                DeleteAllRightOfOrder( MoveRightOwner );
                MoveRightOwner = order;
            }

            order.OrderServer = orderServer;
            curOrder.Add( order );
        }

        private void DeleteAllRightOfOrder ( IActionOrder order )
        {
            if (RotaRightOwner == order)
            {
                RotaRightOwner = new EmptyActionOrder();
                orderServer.TurnRightSpeed = 0;
            }
            if (RotaTurretRightOwner == order)
            {
                RotaTurretRightOwner = new EmptyActionOrder();
                ((IAIOrderServerSinTur)orderServer).TurnTurretWiseSpeed = 0;
            }
            if (RotaRaderRightOwner == order)
            {
                RotaRaderRightOwner = new EmptyActionOrder();
                orderServer.TurnRaderWiseSpeed = 0;
            }
            if (MoveRightOwner == order)
            {
                MoveRightOwner = new EmptyActionOrder();
                orderServer.ForwardSpeed = 0;
            }

            curOrder.Remove( order );
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            List<IActionOrder> deleteOrder = new List<IActionOrder>();
            for (int i = 0; i < curOrder.Count; i++)
            {
                curOrder[i].Update( seconds );
                if (curOrder[i].IsEnd)
                    deleteOrder.Add( curOrder[i] );
            }
            foreach (IActionOrder order in deleteOrder)
            {
                DeleteAllRightOfOrder( order );
            }
        }

        #endregion

        #region StopOrder

        public void StopRota ()
        {
            RotaRightOwner.Stop();
            //orderServer.TurnRightSpeed = 0;
            DeleteAllRightOfOrder( RotaRightOwner );
        }

        public void StopRotaTurret ()
        {
            RotaTurretRightOwner.Stop();
            //orderServer.TurnTurretWiseSpeed = 0;
            DeleteAllRightOfOrder( RotaTurretRightOwner );
        }

        public void StopRotaRader ()
        {
            RotaRaderRightOwner.Stop();
            //orderServer.TurnRaderWiseSpeed = 0;
            DeleteAllRightOfOrder( RotaRaderRightOwner );
        }

        public void StopMove ()
        {
            MoveRightOwner.Stop();
            //orderServer.ForwardSpeed = 0;
            DeleteAllRightOfOrder( MoveRightOwner );
        }

        #endregion
    }

    [Flags]
    public enum ActionRights
    {
        Rota = 0x01,
        RotaTurret = 0x02,
        RotaRader = 0x04,
        Move = 0x08,
    }

    public delegate void ActionFinishHandler ( IActionOrder order );

    public interface IActionOrder : IUpdater
    {
        ActionRights NeedRights { get;}
        IAIOrderServer OrderServer { set;}
        bool IsEnd { get;}
        void Stop ();
    }

    #region OrderRotaToAzi
    /// <summary>
    /// 使坦克转向目标方向
    /// </summary>
    public class OrderRotaToAzi : IActionOrder
    {
        float aimAzi;
        bool wiseClock;
        float angSpeed = 0;
        bool isEnd = false;
        ActionFinishHandler finishHandler;
        bool stopCallFinish = false;

        IAIOrderServer orderServer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimAzi">目标方位角</param>
        public OrderRotaToAzi ( float aimAzi )
        {
            this.aimAzi = aimAzi;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimAzi">目标方位角</param>
        /// <param name="angSpeed">转向速度。如果是0，则使用最大旋转角速度</param>
        public OrderRotaToAzi ( float aimAzi, float angSpeed )
        {
            this.aimAzi = aimAzi;
            this.angSpeed = Math.Abs( angSpeed );
        }

        /// <summary>
        /// 允许指定结束后调用处理函数的构造函数。
        /// </summary>
        /// <param name="aimAzi">目标方位角</param>
        /// <param name="angSpeed">转向速度。如果是0，则使用最大旋转角速度</param>
        /// <param name="finishHandler">命令结束后的处理函数</param>
        /// <param name="stopCallFinish">指定当命令被终止时是否调用结束处理函数</param>
        public OrderRotaToAzi ( float aimAzi, float angSpeed, ActionFinishHandler finishHandler, bool stopCallFinish )
        {
            this.aimAzi = aimAzi;
            this.angSpeed = Math.Abs( angSpeed );
            this.finishHandler = finishHandler;
            this.stopCallFinish = stopCallFinish;
        }

        #region IActionOrder 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.Rota; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                this.orderServer = value;
                if (MathTools.FloatEqualZero( angSpeed, 0.001f ))
                    angSpeed = orderServer.MaxRotaSpeed;
                else
                    angSpeed = Math.Min( orderServer.MaxRotaSpeed, angSpeed );

                float DeltaAng = MathTools.AngTransInPI( aimAzi - orderServer.Azi );
                if (DeltaAng > 0)
                    wiseClock = true;
                else
                    wiseClock = false;
                orderServer.TurnRightSpeed = angSpeed * Math.Sign( DeltaAng );
            }
        }

        public bool IsEnd
        {
            get { return isEnd; }
        }

        public void Stop ()
        {
            if (stopCallFinish && finishHandler != null)
            {
                finishHandler( this );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            float DeltaAng = MathTools.AngTransInPI( aimAzi - orderServer.Azi );
            if (wiseClock ? DeltaAng > 0 : DeltaAng < 0)
            {

                if (Math.Abs( DeltaAng ) < angSpeed * seconds)
                {
                    orderServer.TurnRightSpeed = 0.5f * angSpeed * (wiseClock ? 1 : -1);
                }

            }
            else
            {
                isEnd = true;
                if (finishHandler != null)
                {
                    finishHandler( this );
                }
            }
        }

        #endregion
    }
    #endregion

    #region OrderRotaTurretToAzi
    /// <summary>
    /// 使炮塔指向目标方位。需要IAIOrderServerSinTur
    /// </summary>
    public class OrderRotaTurretToAzi : IActionOrder
    {
        float aimAzi;
        bool wiseClock;
        float angSpeed = 0;
        bool isEnd = false;
        ActionFinishHandler finishHandler;
        bool stopCallFinish = false;

        IAIOrderServerSinTur orderServer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimAzi">目标方位角</param>
        public OrderRotaTurretToAzi ( float aimAzi )
        {
            this.aimAzi = aimAzi;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimAzi">目标方位角</param>
        /// <param name="angSpeed">转向速度。如果是0，则使用最大旋转角速度</param>
        public OrderRotaTurretToAzi ( float aimAzi, float angSpeed )
        {
            this.aimAzi = aimAzi;
            this.angSpeed = Math.Abs( angSpeed );
        }

        /// <summary>
        /// 允许指定结束后调用处理函数的构造函数。
        /// </summary>
        /// <param name="aimAzi">目标方位角</param>
        /// <param name="angSpeed">转向速度。如果是0，则使用最大旋转角速度</param>
        /// <param name="finishHandler">命令结束后的处理函数</param>
        /// <param name="stopCallFinish">指定当命令被终止时是否调用结束处理函数</param>
        public OrderRotaTurretToAzi ( float aimAzi, float angSpeed, ActionFinishHandler finishHandler, bool stopCallFinish )
        {
            this.aimAzi = aimAzi;
            this.angSpeed = Math.Abs( angSpeed );
            this.finishHandler = finishHandler;
            this.stopCallFinish = stopCallFinish;
        }

        #region IActionOrder 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.RotaTurret; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                this.orderServer = (IAIOrderServerSinTur)value;
                if (MathTools.FloatEqualZero( angSpeed, 0.001f ))
                    angSpeed = orderServer.MaxRotaTurretSpeed;
                else
                    angSpeed = Math.Min( orderServer.MaxRotaTurretSpeed, angSpeed );

                float DeltaAng = MathTools.AngTransInPI( aimAzi - orderServer.TurretAimAzi );
                if (DeltaAng > 0)
                    wiseClock = true;
                else
                    wiseClock = false;
                orderServer.TurnTurretWiseSpeed = angSpeed * Math.Sign( DeltaAng );
            }
        }

        public bool IsEnd
        {
            get { return isEnd; }
        }

        public void Stop ()
        {
            if (stopCallFinish && finishHandler != null)
            {
                finishHandler( this );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            float DeltaAng = MathTools.AngTransInPI( aimAzi - orderServer.TurretAimAzi );
            if (wiseClock ? DeltaAng > 0 : DeltaAng < 0)
            {

                if (Math.Abs( DeltaAng ) < angSpeed * seconds)
                {
                    orderServer.TurnTurretWiseSpeed = 0.5f * angSpeed * (wiseClock ? 1 : -1);
                }

            }
            else
            {
                isEnd = true;
                if (finishHandler != null)
                {
                    finishHandler( this );
                }
            }
        }

        #endregion
    }
    #endregion

    #region OrderRotaTurretToPos
    /// <summary>
    /// 转动炮塔，使指向目标位置。需要IAIOrderServerSinTur
    /// </summary>
    public class OrderRotaTurretToPos : IActionOrder
    {
        Vector2 aimPos;
        bool wiseClock;
        float angSpeed = 0;
        bool isEnd = false;
        ActionFinishHandler finishHandler;
        bool stopCallFinish = false;

        IAIOrderServerSinTur orderServer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimPos">目标位置</param>
        public OrderRotaTurretToPos ( Vector2 aimPos )
        {
            this.aimPos = aimPos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimAzi">目标位置</param>
        /// <param name="angSpeed">转向速度。如果是0，则使用最大旋转角速度</param>
        public OrderRotaTurretToPos ( Vector2 aimPos, float angSpeed )
        {
            this.aimPos = aimPos;
            this.angSpeed = Math.Abs( angSpeed );
        }

        /// <summary>
        /// 允许指定结束后调用处理函数的构造函数。
        /// </summary>
        /// <param name="aimAzi">目标位置</param>
        /// <param name="angSpeed">转向速度。如果是0，则使用最大旋转角速度</param>
        /// <param name="finishHandler">命令结束后的处理函数</param>
        /// <param name="stopCallFinish">指定当命令被终止时是否调用结束处理函数</param>
        public OrderRotaTurretToPos ( Vector2 aimPos, float angSpeed, ActionFinishHandler finishHandler, bool stopCallFinish )
        {
            this.aimPos = aimPos;
            this.angSpeed = Math.Abs( angSpeed );
            this.finishHandler = finishHandler;
            this.stopCallFinish = stopCallFinish;
        }

        #region IActionOrder 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.RotaTurret; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                this.orderServer = (IAIOrderServerSinTur)value;
                if (MathTools.FloatEqualZero( angSpeed, 0.001f ))
                    angSpeed = orderServer.MaxRotaTurretSpeed;
                else
                    angSpeed = Math.Min( orderServer.MaxRotaTurretSpeed, angSpeed );

                float aimAzi = MathTools.AziFromRefPos( aimPos - orderServer.TurretAxePos );
                float DeltaAng = MathTools.AngTransInPI( aimAzi - orderServer.TurretAimAzi );
                if (DeltaAng > 0)
                    wiseClock = true;
                else
                    wiseClock = false;
                orderServer.TurnTurretWiseSpeed = angSpeed * Math.Sign( DeltaAng );
            }
        }

        public bool IsEnd
        {
            get { return isEnd; }
        }

        public void Stop ()
        {
            if (stopCallFinish && finishHandler != null)
            {
                finishHandler( this );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            float aimAzi = MathTools.AziFromRefPos( aimPos - orderServer.TurretAxePos );
            float DeltaAng = MathTools.AngTransInPI( aimAzi - orderServer.TurretAimAzi );
            if (wiseClock ? DeltaAng > 0 : DeltaAng < 0)
            {

                if (Math.Abs( DeltaAng ) < angSpeed * seconds)
                {
                    orderServer.TurnTurretWiseSpeed = 0.5f * angSpeed * (wiseClock ? 1 : -1);
                }

            }
            else
            {
                isEnd = true;
                if (finishHandler != null)
                {
                    finishHandler( this );
                }
            }
        }

        #endregion
    }
    #endregion

    #region OrderRotaRaderToAng
    /// <summary>
    /// 转动雷达，使其指向相对车身的角度
    /// </summary>
    public class OrderRotaRaderToAng : IActionOrder
    {
        float aimAng;
        bool wiseClock;
        float angSpeed = 0;
        bool isEnd = false;
        ActionFinishHandler finishHandler;
        bool stopCallFinish = false;

        IAIOrderServer orderServer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimAzi">目标角</param>
        public OrderRotaRaderToAng ( float aimAng )
        {
            this.aimAng = aimAng;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimAzi">目标角</param>
        /// <param name="angSpeed">转向速度。如果是0，则使用最大旋转角速度</param>
        public OrderRotaRaderToAng ( float aimAng, float angSpeed )
        {
            this.aimAng = aimAng;
            this.angSpeed = Math.Abs( angSpeed );
        }

        /// <summary>
        /// 允许指定结束后调用处理函数的构造函数。
        /// </summary>
        /// <param name="aimAzi">目标角</param>
        /// <param name="angSpeed">转向速度。如果是0，则使用最大旋转角速度</param>
        /// <param name="finishHandler">命令结束后的处理函数</param>
        /// <param name="stopCallFinish">指定当命令被终止时是否调用结束处理函数</param>
        public OrderRotaRaderToAng ( float aimAng, float angSpeed, ActionFinishHandler finishHandler, bool stopCallFinish )
        {
            this.aimAng = aimAng;
            this.angSpeed = Math.Abs( angSpeed );
            this.finishHandler = finishHandler;
            this.stopCallFinish = stopCallFinish;
        }

        #region IActionOrder 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.RotaRader; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                this.orderServer = value;
                if (MathTools.FloatEqualZero( angSpeed, 0.001f ))
                    angSpeed = orderServer.MaxRotaRaderSpeed;
                else
                    angSpeed = Math.Min( orderServer.MaxRotaRaderSpeed, angSpeed );

                float DeltaAng = MathTools.AngTransInPI( aimAng - orderServer.RaderAzi );
                if (DeltaAng > 0)
                    wiseClock = true;
                else
                    wiseClock = false;
                orderServer.TurnRaderWiseSpeed = angSpeed * Math.Sign( DeltaAng );
            }
        }

        public bool IsEnd
        {
            get { return isEnd; }
        }

        public void Stop ()
        {
            if (stopCallFinish && finishHandler != null)
            {
                finishHandler( this );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            float DeltaAng = MathTools.AngTransInPI( aimAng - orderServer.RaderAzi );
            if (wiseClock ? DeltaAng > 0 : DeltaAng < 0)
            {

                if (Math.Abs( DeltaAng ) < angSpeed * seconds)
                {
                    orderServer.TurnRaderWiseSpeed = 0.5f * angSpeed * (wiseClock ? 1 : -1);
                }

            }
            else
            {
                isEnd = true;
                if (finishHandler != null)
                {
                    finishHandler( this );
                }
            }
        }

        #endregion
    }
    #endregion

    #region OrderRotaRaderToAzi
    /// <summary>
    /// 转动雷达到指定方位
    /// </summary>
    public class OrderRotaRaderToAzi : IActionOrder
    {
        float aimAzi;
        bool wiseClock;
        float angSpeed = 0;
        bool isEnd = false;
        ActionFinishHandler finishHandler;
        bool stopCallFinish = false;

        IAIOrderServer orderServer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimAzi">目标方位角</param>
        public OrderRotaRaderToAzi ( float aimAzi )
        {
            this.aimAzi = aimAzi;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimAzi">目标方位角</param>
        /// <param name="angSpeed">转向速度。如果是0，则使用最大旋转角速度</param>
        public OrderRotaRaderToAzi ( float aimAzi, float angSpeed )
        {
            this.aimAzi = aimAzi;
            this.angSpeed = Math.Abs( angSpeed );
        }

        /// <summary>
        /// 允许指定结束后调用处理函数的构造函数。
        /// </summary>
        /// <param name="aimAzi">目标方位角</param>
        /// <param name="angSpeed">转向速度。如果是0，则使用最大旋转角速度</param>
        /// <param name="finishHandler">命令结束后的处理函数</param>
        /// <param name="stopCallFinish">指定当命令被终止时是否调用结束处理函数</param>
        public OrderRotaRaderToAzi ( float aimAzi, float angSpeed, ActionFinishHandler finishHandler, bool stopCallFinish )
        {
            this.aimAzi = aimAzi;
            this.angSpeed = Math.Abs( angSpeed );
            this.finishHandler = finishHandler;
            this.stopCallFinish = stopCallFinish;
        }

        #region IActionOrder 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.RotaRader; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                this.orderServer = value;
                if (MathTools.FloatEqualZero( angSpeed, 0.001f ))
                    angSpeed = orderServer.MaxRotaRaderSpeed;
                else
                    angSpeed = Math.Min( orderServer.MaxRotaRaderSpeed, angSpeed );

                float DeltaAng = MathTools.AngTransInPI( aimAzi - orderServer.RaderAimAzi );
                if (DeltaAng > 0)
                    wiseClock = true;
                else
                    wiseClock = false;
                orderServer.TurnRaderWiseSpeed = angSpeed * Math.Sign( DeltaAng );
            }
        }

        public bool IsEnd
        {
            get { return isEnd; }
        }

        public void Stop ()
        {
            if (stopCallFinish && finishHandler != null)
            {
                finishHandler( this );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            float DeltaAng = MathTools.AngTransInPI( aimAzi - orderServer.RaderAimAzi );
            if (wiseClock ? DeltaAng > 0 : DeltaAng < 0)
            {

                if (Math.Abs( DeltaAng ) < angSpeed * seconds)
                {
                    orderServer.TurnRaderWiseSpeed = 0.5f * angSpeed * (wiseClock ? 1 : -1);
                }

            }
            else
            {
                isEnd = true;
                if (finishHandler != null)
                {
                    finishHandler( this );
                }
            }
        }

        #endregion
    }
    #endregion

    #region OrderRotaRaderToPos
    /// <summary>
    /// 转动雷达，使指向目标位置
    /// </summary>
    public class OrderRotaRaderToPos : IActionOrder
    {
        Vector2 aimPos;
        bool wiseClock;
        float angSpeed = 0;
        bool isEnd = false;
        ActionFinishHandler finishHandler;
        bool stopCallFinish = false;

        IAIOrderServer orderServer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimAzi">目标位置</param>
        public OrderRotaRaderToPos ( Vector2 aimPos )
        {
            this.aimPos = aimPos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aimAzi">目标位置</param>
        /// <param name="angSpeed">转向速度。如果是0，则使用最大旋转角速度</param>
        public OrderRotaRaderToPos ( Vector2 aimPos, float angSpeed )
        {
            this.aimPos = aimPos;
            this.angSpeed = Math.Abs( angSpeed );
        }

        /// <summary>
        /// 允许指定结束后调用处理函数的构造函数。
        /// </summary>
        /// <param name="aimAzi">目标位置</param>
        /// <param name="angSpeed">转向速度。如果是0，则使用最大旋转角速度</param>
        /// <param name="finishHandler">命令结束后的处理函数</param>
        /// <param name="stopCallFinish">指定当命令被终止时是否调用结束处理函数</param>
        public OrderRotaRaderToPos ( Vector2 aimPos, float angSpeed, ActionFinishHandler finishHandler, bool stopCallFinish )
        {
            this.aimPos = aimPos;
            this.angSpeed = Math.Abs( angSpeed );
            this.finishHandler = finishHandler;
            this.stopCallFinish = stopCallFinish;
        }

        #region IActionOrder 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.RotaRader; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                this.orderServer = value;
                if (MathTools.FloatEqualZero( angSpeed, 0.001f ))
                    angSpeed = orderServer.MaxRotaRaderSpeed;
                else
                    angSpeed = Math.Min( orderServer.MaxRotaRaderSpeed, angSpeed );

                float aimAzi = MathTools.AziFromRefPos( aimPos - orderServer.Pos );
                float DeltaAng = MathTools.AngTransInPI( aimAzi - orderServer.RaderAimAzi );
                if (DeltaAng > 0)
                    wiseClock = true;
                else
                    wiseClock = false;
                orderServer.TurnRaderWiseSpeed = angSpeed * Math.Sign( DeltaAng );
            }
        }

        public bool IsEnd
        {
            get { return isEnd; }
        }

        public void Stop ()
        {
            if (stopCallFinish && finishHandler != null)
            {
                finishHandler( this );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            float aimAzi = MathTools.AziFromRefPos( aimPos - orderServer.Pos );
            float DeltaAng = MathTools.AngTransInPI( aimAzi - orderServer.RaderAimAzi );
            if (wiseClock ? DeltaAng > 0 : DeltaAng < 0)
            {

                if (Math.Abs( DeltaAng ) < angSpeed * seconds)
                {
                    orderServer.TurnRaderWiseSpeed = 0.5f * angSpeed * (wiseClock ? 1 : -1);
                }

            }
            else
            {
                isEnd = true;
                if (finishHandler != null)
                {
                    finishHandler( this );
                }
            }
        }

        #endregion
    }
    #endregion

    #region OrderMove
    /// <summary>
    /// 前进或后退
    /// </summary>
    public class OrderMove : IActionOrder
    {
        float length;
        float movedLength = 0;
        float speed = 0;
        bool isEnd = false;
        ActionFinishHandler finishHandler;
        bool stopCallFinish = false;

        IAIOrderServer orderServer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length">距离。大于0表示前进，小于0表示后退</param>
        public OrderMove ( float length )
        {
            this.length = length;
        }

        /// <summary>
        /// 允许设置移动速度
        /// </summary>
        /// <param name="length">距离。大于0表示前进，小于0表示后退</param>
        /// <param name="speed">移动速度。无论前进或后退都输入正数。输入零表示最大速度</param>
        public OrderMove ( float length, float speed )
        {
            this.length = length;
            this.speed = speed;
        }

        /// <summary>
        /// 允许指定结束后调用处理函数的构造函数。
        /// </summary>
        /// <param name="length">距离。大于0表示前进，小于0表示后退</param>
        /// <param name="speed">移动速度。无论前进或后退都输入正数。输入零表示最大速度</param>
        /// <param name="finishHandler">命令结束后的处理函数</param>
        /// <param name="stopCallFinish">指定当命令被终止时是否调用结束处理函数</param>
        public OrderMove ( float length, float speed, ActionFinishHandler finishHandler, bool stopCallFinish )
        {
            this.length = length;
            this.speed = Math.Abs( speed );
            this.finishHandler = finishHandler;
            this.stopCallFinish = stopCallFinish;
        }

        #region IActionOrder 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.Move; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = value;

                if (length > 0)
                {
                    if (speed == 0)
                    {
                        speed = orderServer.MaxForwardSpeed;
                    }

                    speed = Math.Min( orderServer.MaxForwardSpeed, speed );
                    orderServer.ForwardSpeed = speed;
                }
                else
                {
                    if (speed == 0)
                    {
                        speed = orderServer.MaxBackwardSpeed;
                    }
                    speed = Math.Min( orderServer.MaxBackwardSpeed, speed );
                    orderServer.ForwardSpeed = -speed;
                }
            }
        }

        public bool IsEnd
        {
            get { return isEnd; }
        }

        public void Stop ()
        {
            if (stopCallFinish && finishHandler != null)
            {
                finishHandler( this );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            if (movedLength < Math.Abs( length ))
            {
                if (Math.Abs( length ) - movedLength < speed * seconds)
                {
                    orderServer.ForwardSpeed = speed * 0.5f;
                    movedLength += speed * 0.5f * seconds;
                }
                else
                    movedLength += speed * seconds;
            }
            else
            {
                isEnd = true;
                if (finishHandler != null)
                {
                    finishHandler( this );
                }
            }
        }

        #endregion
    }
    #endregion

    #region OrderMoveToPosDirect
    /// <summary>
    /// 走直线到指定地点
    /// </summary>
    public class OrderMoveToPosDirect : IActionOrder
    {
        IAIOrderServer orderServer;

        Vector2 aimPos;

        float rotaVel;
        float forwardVel;

        bool isEnd = false;
        IActionOrder curOrder;

        ActionFinishHandler finishHandler;
        bool stopCallFinish = false;

        public OrderMoveToPosDirect ( Vector2 aimPos )
        {
            this.aimPos = aimPos;
        }

        public OrderMoveToPosDirect ( Vector2 aimPos, float rotaVel, float forwardVel )
        {
            this.aimPos = aimPos;
            this.rotaVel = rotaVel;
            this.forwardVel = forwardVel;
        }

        public OrderMoveToPosDirect ( Vector2 aimPos, float rotaVel, float forwardVel, ActionFinishHandler finishHandler, bool stopCallFinish )
        {
            this.aimPos = aimPos;
            this.rotaVel = rotaVel;
            this.forwardVel = forwardVel;
            this.finishHandler = finishHandler;
            this.stopCallFinish = stopCallFinish;
        }

        #region IActionOrder 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.Move | ActionRights.Rota; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = value;
                if (rotaVel == 0)
                    rotaVel = orderServer.MaxRotaSpeed;
                else rotaVel = Math.Min( orderServer.MaxRotaSpeed, Math.Abs( rotaVel ) );
                curOrder = new OrderRotaToAzi( MathTools.AziFromRefPos( aimPos - orderServer.Pos ), rotaVel );
                curOrder.OrderServer = orderServer;

                if (forwardVel == 0)
                    forwardVel = orderServer.MaxForwardSpeed;
                else
                    forwardVel = Math.Min( orderServer.MaxForwardSpeed, Math.Abs( forwardVel ) );
            }
        }

        public bool IsEnd
        {
            get { return isEnd; }
        }

        public void Stop ()
        {
            if (stopCallFinish && finishHandler != null)
            {
                finishHandler( this );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            if (curOrder.IsEnd)
            {
                if (curOrder is OrderRotaToAzi)
                {
                    orderServer.TurnRightSpeed = 0;
                    curOrder = new OrderMove( Vector2.Distance( aimPos, orderServer.Pos ), forwardVel );
                    curOrder.OrderServer = orderServer;
                }
                else
                {
                    isEnd = true;
                    if (finishHandler != null)
                    {
                        finishHandler( this );
                    }
                }
            }
            curOrder.Update( seconds );
        }

        #endregion
    }
    #endregion

    #region OrderMoveToPosSmooth
    /// <summary>
    /// 光滑地移动到目标点。
    /// 与OrderMoveToPosDirect命令的不同之处在于，当目标方向与当前方向的夹角在某个阀值之内时，将在转弯的同时向前移动。
    /// 这使该命令能够适用于可能会不间歇地调用的场合。
    /// </summary>
    public class OrderMoveToPosSmooth : IActionOrder
    {
        const float fixFactor = 0.9f;

        IAIOrderServer orderServer;

        bool isEnd = false;

        ActionFinishHandler finishHandler;
        bool stopCallFinish = false;

        Vector2 aimPos;
        float smoothAng = MathHelper.PiOver2;

        float vel;
        float rotaVel;

        bool turnWise;

        bool rotaWForward = false;

        /// <summary>
        /// 移动到目标点。
        /// 与OrderMoveToPosDirect命令的不同之处在于，当目标方向与当前方向的夹角在某个阀值之内时，将在转弯的同时向前移动。
        /// </summary>
        /// <param name="aimPos">目标点</param>
        /// <param name="smoothAng">允许在转弯的同时向前移动的最大角度</param>
        /// <param name="vel">向前移动的速度。如果为0，将取最大速度</param>
        /// <param name="rotaVel">旋转的速度。如果为0，将取最大速度</param>
        /// <param name="finishHandler">命令结束后的处理函数</param>
        /// <param name="stopCallFinish">指定当命令被终止时是否调用结束处理函数</param>
        public OrderMoveToPosSmooth ( Vector2 aimPos, float smoothAng, float vel, float rotaVel, ActionFinishHandler finishHandler, bool stopCallFinish )
        {
            this.aimPos = aimPos;
            this.finishHandler = finishHandler;
            this.stopCallFinish = stopCallFinish;
            this.vel = Math.Abs( vel );
            this.rotaVel = Math.Abs( rotaVel );
            this.smoothAng = smoothAng;
        }


        #region IActionOrder 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.Rota | ActionRights.Move; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                this.orderServer = value;

                if (vel == 0)
                    vel = orderServer.MaxForwardSpeed;
                if (rotaVel == 0)
                    rotaVel = orderServer.MaxRotaSpeed;

                Vector2 curPos = orderServer.Pos;
                Vector2 refPos = aimPos - curPos;
                float aimAzi = MathTools.AziFromRefPos( refPos );
                float curAzi = orderServer.Azi;
                float deltaAzi = MathTools.AngTransInPI( aimAzi - curAzi );
                if (deltaAzi > 0)
                    turnWise = true;
                else
                    turnWise = false;

                orderServer.TurnRightSpeed = Math.Sign( deltaAzi ) * rotaVel;

                if (Math.Abs( deltaAzi ) < smoothAng)
                {
                    Line refLine = new Line( curPos, orderServer.Direction );
                    Line vertice = MathTools.VerticeLine( refLine, curPos );
                    Line midVert = MathTools.MidVerLine( curPos, aimPos );
                    Vector2 interPoint;

                    bool canFor = false;
                    if (MathTools.InterPoint( vertice, midVert, out interPoint ))
                    {
                        float radius = Vector2.Distance( interPoint, curPos );

                        if (radius * fixFactor > vel / rotaVel)
                            canFor = true;
                    }
                    else
                    {
                        canFor = true;
                    }

                    if (canFor)
                    {
                        rotaWForward = true;
                        orderServer.ForwardSpeed = vel;
                    }
                }
            }
        }

        public bool IsEnd
        {
            get { return isEnd; }
        }

        public void Stop ()
        {
            if (stopCallFinish && finishHandler != null)
            {
                finishHandler( this );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            Vector2 curPos = orderServer.Pos;
            Vector2 refPos = aimPos - curPos;
            float aimAzi = MathTools.AziFromRefPos( refPos );
            float curAzi = orderServer.Azi;
            float deltaAzi = MathTools.AngTransInPI( aimAzi - curAzi );

            if (turnWise && deltaAzi < 0)
                orderServer.TurnRightSpeed = 0;
            else if (!turnWise && deltaAzi > 0)
                orderServer.TurnRightSpeed = 0;

            if (!rotaWForward && Math.Abs( deltaAzi ) < smoothAng)
            {
                Line refLine = new Line( curPos, orderServer.Direction );
                Line vertice = MathTools.VerticeLine( refLine, curPos );
                Line midVert = MathTools.MidVerLine( curPos, aimPos );
                Vector2 interPoint;

                bool canFor = false;
                if (MathTools.InterPoint( vertice, midVert, out interPoint ))
                {
                    float radius = Vector2.Distance( interPoint, curPos );
                    if (radius * fixFactor > vel / rotaVel)
                        canFor = true;
                }
                else
                {
                    canFor = true;
                }

                if (canFor)
                {
                    rotaWForward = true;
                    orderServer.ForwardSpeed = vel;
                }
            }

            if (!rotaWForward && orderServer.TurnRightSpeed == 0)
            {
                orderServer.ForwardSpeed = vel;
            }

            if (orderServer.TurnRightSpeed == 0 && Math.Abs( MathTools.AngTransInPI( curAzi - aimAzi ) ) > MathHelper.PiOver4)
            {
                isEnd = true;
                orderServer.TurnRightSpeed = 0;
                orderServer.ForwardSpeed = 0;

                if (finishHandler != null)
                {
                    finishHandler( this );
                }
            }
        }

        #endregion
    }
    #endregion

    #region OrderMoveCircle
    public class OrderMoveCircle : IActionOrder
    {
        float length;
        float rotaVel;
        float moveVel;

        OrderMove moveOrder;

        bool isEnd = false;
        IAIOrderServer orderServer;

        ActionFinishHandler finishHandler;
        bool stopCallFinish = false;

        public OrderMoveCircle ( float length, float rotaVel, float moveVel )
        {
            this.length = length;
            this.rotaVel = rotaVel;
            this.moveVel = moveVel;
        }

        public OrderMoveCircle ( float length, float rotaVel, float moveVel, ActionFinishHandler finishHandler, bool stopCallFinish )
        {
            this.length = length;
            this.rotaVel = rotaVel;
            this.moveVel = moveVel;
            this.finishHandler = finishHandler;
            this.stopCallFinish = stopCallFinish;
        }

        public OrderMoveCircle ( Vector2 aimPos, float moveVel, IAIOrderServer orderServer )
        {
            this.orderServer = orderServer;
            moveVel = Math.Min( Math.Max( -orderServer.MaxBackwardSpeed, moveVel ), orderServer.MaxForwardSpeed );
            Vector2 direction = orderServer.Direction;
            Line curVertice = MathTools.VerticeLine( new Line( orderServer.Pos, direction ), orderServer.Pos );
            Line midVertice = MathTools.MidVerLine( orderServer.Pos, aimPos );
            Vector2 center;
            if (!MathTools.InterPoint( curVertice, midVertice, out center ))
            {
                this.moveVel = moveVel;
                this.rotaVel = 0;
                float forward = Vector2.Dot( direction, aimPos - orderServer.Pos );
                this.length = Vector2.Distance( aimPos, orderServer.Pos ) * Math.Sign( forward );
            }
            else
            {
                float radius = Vector2.Distance( center, orderServer.Pos );

                float turnRight = MathTools.Vector2Cross( direction, aimPos - orderServer.Pos );

                if (orderServer.MaxRotaSpeed * radius < Math.Abs( moveVel ))
                {
                    this.moveVel = orderServer.MaxRotaSpeed * radius;
                    this.rotaVel = orderServer.MaxRotaSpeed * Math.Sign( turnRight );

                }
                else
                {
                    this.moveVel = moveVel;
                    this.rotaVel = moveVel / radius * Math.Sign( turnRight );
                }
                float ang = MathTools.AngBetweenVectors( orderServer.Pos - center, aimPos - center );
                bool inFront = Vector2.Dot( direction, aimPos - orderServer.Pos ) >= 0;
                if (moveVel > 0)
                {
                    if (inFront)
                    {
                        this.length = ang * radius;
                    }
                    else
                    {
                        this.length = (2 * MathHelper.Pi - ang) * radius;
                    }
                }
                else
                {
                    if (inFront)
                    {
                        this.length = -(2 * MathHelper.Pi - ang) * radius;
                    }
                    else
                    {
                        this.length = -ang * radius;
                    }
                }
            }
        }

        public OrderMoveCircle ( Vector2 aimPos, float moveVel, IAIOrderServer orderServer, ActionFinishHandler finishHandler, bool stopCallFinish )
        {
            this.orderServer = orderServer;
            moveVel = Math.Min( Math.Max( -orderServer.MaxBackwardSpeed, moveVel ), orderServer.MaxForwardSpeed );
            Vector2 direction = orderServer.Direction;
            Line curVertice = MathTools.VerticeLine( new Line( orderServer.Pos, direction ), orderServer.Pos );
            Line midVertice = MathTools.MidVerLine( orderServer.Pos, aimPos );
            Vector2 center;
            if (!MathTools.InterPoint( curVertice, midVertice, out center ))
            {
                this.moveVel = moveVel;
                this.rotaVel = 0;
                float forward = Vector2.Dot( direction, aimPos - orderServer.Pos );
                this.length = Vector2.Distance( aimPos, orderServer.Pos ) * Math.Sign( forward );
            }
            else
            {
                float radius = Vector2.Distance( center, orderServer.Pos );

                float turnRight = MathTools.Vector2Cross( direction, aimPos - orderServer.Pos );

                if (orderServer.MaxRotaSpeed * radius < Math.Abs( moveVel ))
                {
                    this.moveVel = orderServer.MaxRotaSpeed * radius;
                    this.rotaVel = orderServer.MaxRotaSpeed * Math.Sign( turnRight );

                }
                else
                {
                    this.moveVel = moveVel;
                    this.rotaVel = moveVel / radius * Math.Sign( turnRight );
                }
                float ang = MathTools.AngBetweenVectors( orderServer.Pos - center, aimPos - center );
                bool inFront = Vector2.Dot( direction, aimPos - orderServer.Pos ) >= 0;
                if (moveVel > 0)
                {
                    if (inFront)
                    {
                        this.length = ang * radius;
                    }
                    else
                    {
                        this.length = (2 * MathHelper.Pi - ang) * radius;
                    }
                }
                else
                {
                    if (inFront)
                    {
                        this.length = -(2 * MathHelper.Pi - ang) * radius;
                    }
                    else
                    {
                        this.length = -ang * radius;
                    }
                }
            }

            this.finishHandler = finishHandler;
            this.stopCallFinish = stopCallFinish;
        }

        #region IActionOrder 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.Move | ActionRights.Rota; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = value;
                moveOrder = new OrderMove( length, moveVel,
                    delegate( IActionOrder action )
                    {
                        isEnd = true;
                    }, false );
                moveOrder.OrderServer = orderServer;
                orderServer.TurnRightSpeed = rotaVel;
            }
        }

        public bool IsEnd
        {
            get { return isEnd; }
        }

        public void Stop ()
        {
            if (stopCallFinish && finishHandler != null)
            {
                finishHandler( this );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            moveOrder.Update( seconds );
            if (isEnd)
            {
                if (finishHandler != null)
                {
                    finishHandler( this );
                }
            }
        }

        #endregion
    }
    #endregion

    #region OrderScanRader
    /// <summary>
    /// 雷达以坦克方向为基准在一定夹角范围内来回扫描
    /// </summary>
    public class OrderScanRader : IActionOrder
    {
        float rotaAngLeft;
        float rotaAngRight;
        float rotaSpeed;

        bool startAtRight;

        IAIOrderServer orderServer;

        AIActionHelper action;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotaAng">最大扫描角</param>
        /// <param name="rotaSpeed">扫描速度,如果为零，则为最大旋转速度</param>
        /// <param name="startAtRight">是否先向右扫描</param>
        public OrderScanRader ( float rotaAng, float rotaSpeed, bool startAtRight )
        {
            this.rotaAngLeft = rotaAng;
            this.rotaAngRight = rotaAng;
            this.rotaSpeed = rotaSpeed;
            this.startAtRight = startAtRight;
        }

        public OrderScanRader ( float rotaAngRight, float rotaAngLeft, float rotaSpeed, bool startAtRight )
        {
            this.rotaAngRight = rotaAngRight;
            this.rotaAngLeft = rotaAngLeft;
            this.rotaSpeed = rotaSpeed;
            this.startAtRight = startAtRight;
        }

        #region IActionOrder 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.RotaRader; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = value;
                action = new AIActionHelper( orderServer );

                if (rotaSpeed == 0)
                    rotaSpeed = orderServer.MaxRotaRaderSpeed;

                if (startAtRight)
                    TurnRight( null );
                else
                    TurnLeft( null );
            }
        }

        private void TurnRight ( IActionOrder order )
        {
            action.AddOrder( new OrderRotaRaderToAng( rotaAngRight, rotaSpeed, TurnLeft, false ) );
        }

        private void TurnLeft ( IActionOrder order )
        {
            action.AddOrder( new OrderRotaRaderToAng( -rotaAngLeft, rotaSpeed, TurnRight, false ) );
        }

        public bool IsEnd
        {
            get { return false; }
        }

        public void Stop ()
        {

        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            action.Update( seconds );
        }

        #endregion
    }
    #endregion

    #region OrderScanRaderAzi
    /// <summary>
    /// 雷达在一定方位角范围内来回扫描
    /// </summary>
    public class OrderScanRaderAzi : IActionOrder
    {
        float rotaAziLeft;
        float rotaAziRight;
        float rotaSpeed;

        bool startAtRight;

        IAIOrderServer orderServer;

        AIActionHelper action;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rotaAzi">最大扫描角</param>
        /// <param name="rotaSpeed">扫描速度,如果为零，则为最大旋转速度</param>
        /// <param name="startAtRight">是否先向右扫描</param>
        public OrderScanRaderAzi ( float rotaAzi, float rotaSpeed, bool startAtRight )
        {
            this.rotaAziLeft = rotaAzi;
            this.rotaAziRight = rotaAzi;
            this.rotaSpeed = rotaSpeed;
            this.startAtRight = startAtRight;
        }

        public OrderScanRaderAzi ( float rotaAziRight, float rotaAziLeft, float rotaSpeed, bool startAtRight )
        {
            this.rotaAziRight = rotaAziRight;
            this.rotaAziLeft = rotaAziLeft;
            this.rotaSpeed = rotaSpeed;
            this.startAtRight = startAtRight;
        }

        #region IActionOrder 成员

        public ActionRights NeedRights
        {
            get { return ActionRights.RotaRader; }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = value;
                action = new AIActionHelper( orderServer );

                if (rotaSpeed == 0)
                    rotaSpeed = orderServer.MaxRotaRaderSpeed;

                if (startAtRight)
                    TurnRight( null );
                else
                    TurnLeft( null );
            }
        }

        private void TurnRight ( IActionOrder order )
        {
            action.AddOrder( new OrderRotaRaderToAzi( rotaAziRight, rotaSpeed, TurnLeft, false ) );
        }

        private void TurnLeft ( IActionOrder order )
        {
            action.AddOrder( new OrderRotaRaderToAzi( rotaAziLeft, rotaSpeed, TurnRight, false ) );
        }

        public bool IsEnd
        {
            get { return false; }
        }

        public void Stop ()
        {

        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            action.Update( seconds );
        }

        #endregion
    }
    #endregion
}

