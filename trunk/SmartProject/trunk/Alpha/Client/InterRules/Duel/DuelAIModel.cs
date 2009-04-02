using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SmartTank.AI;
using SmartTank.AI.AIHelper;
using TankEngine2D.DataStructure;
using TankEngine2D.Graphics;
using SmartTank.GameObjs;
using SmartTank.Senses.Vision;
using SmartTank.GameObjs.Tank.SinTur;

namespace InterRules.Duel
{
    [AIAttribute( "DuelAIModel", "SmartTank Team", "A Model for DuelAI", 2007, 11, 6 )]
    public class DuelAIModel : IDuelAI
    {
        IDuelAIOrderServer orderServer;
        AICommonServer commonServer;

        // 可以使用该类提供的高层接口
        AIActionHelper action;

        Rectanglef mapBorder;

        public DuelAIModel ()
        {

        }

        #region IAI 成员

        public IAICommonServer CommonServer
        {
            set
            {
                commonServer = (AICommonServer)value;

                // 可以从CommonServer中获得游戏中与当前坦克无关的一些参数。如：
                mapBorder = commonServer.MapBorder;
            }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = (IDuelAIOrderServer)value;
                // 可以添加事件处理
                orderServer.OnCollide += new OnCollidedEventHandlerAI( CollideHandler );

                // 如果使用了AIActionHelper，则需要在此进行初始化。
                action = new AIActionHelper( orderServer );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            // 可以从OrderServer中获得坦克当前的状态，并发出命令。如下：
            Vector2 curPos = orderServer.Pos;

            if (orderServer.FireLeftCDTime <= 0)
                orderServer.Fire();

            orderServer.ForwardSpeed = orderServer.MaxForwardSpeed;

            // 调用orderServer的GetEyeableInfo函数来获得视野中物体（敌对坦克的信息）。
            List<IEyeableInfo> eyeableInfos = orderServer.GetEyeableInfo();
            if (eyeableInfos.Count != 0)
            {
                if (eyeableInfos[0].ObjInfo.ObjClass == "DuelTank" && eyeableInfos[0] is TankSinTur.TankCommonEyeableInfo)
                {
                    // 可获得目标坦克的信息，如下：
                    Vector2 enemyPos = ((TankSinTur.TankCommonEyeableInfo)eyeableInfos[0]).Pos;
                    Vector2 enemyDirection = ((TankSinTur.TankCommonEyeableInfo)eyeableInfos[0]).Direction;

                }
            }


            /* 用AIActionHelper可以使用不少高级的运动指令
             * 
             * 列表如下：
             * RotaToAziOrder           旋转坦克到制定方位角
             * RotaTurretToAziOrder
             * RotaTurretToPosOrder     旋转坦克炮塔直到瞄准制定位置
             * RotaRaderToAziOrder
             * RotaRaderToPosOrder
             * MoveOrder                让坦克向前或向后移动一段距离
             * MoveToPosDirectOrder     直线移动到制定点
             * MoveCircleOrder          沿着一个圆弧的路径走到目标点
             * 
             * 也可以自己从IActionOrder接口继承自己的高级运动指令。
             * */
            action.AddOrder( new OrderRotaRaderToPos( new Vector2( 100, 100 ) ) );
            action.AddOrder( new OrderMoveCircle( new Vector2( 100, 100 ), orderServer.MaxForwardSpeed, orderServer,
                delegate( IActionOrder order )
                {
                    orderServer.Fire();
                }, false ) );

            // 终止一个命令
            action.StopRota();

            // 如果使用AIActionHelper，必须调用他的Update函数
            action.Update( seconds );
        }

        #endregion

        void CollideHandler ( CollisionResult result, GameObjInfo objB )
        {
            // 通过objB的信息确定碰撞物的种类。
            if (objB.ObjClass == "Border")
            {
                // 添加自己的处理函数
            }
            else if (objB.ObjClass == "ShellNormal")
            {
                // 添加自己的处理函数
            }
            else if (objB.ObjClass == "DuelTank")
            {
                // 添加自己的处理函数
            }

        }



        #region IAI 成员


        public void Draw ()
        {
            
        }

        #endregion
    }
}
