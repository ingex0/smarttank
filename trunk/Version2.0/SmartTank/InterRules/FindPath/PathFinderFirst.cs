using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SmartTank.AI;
using SmartTank.AI.AIHelper;
using TankEngine2D.Input;
using TankEngine2D.Helpers;
using SmartTank;
using SmartTank.Senses.Memory;
using SmartTank.Scene;

namespace InterRules.FindPath
{
    /// <summary>
    /// 拥有一个很简陋的寻路逻辑的AI。
    /// </summary>
    [AIAttribute( "PathFinder No.1", "SmartTank编写组", "测试AI的寻路能力", 2007, 11, 21 )]
    class PathFinderFirst : IAISinTur
    {
        IAIOrderServerSinTur orderServer;

        AICommonServer commonServer;

        AIActionHelper action;

        #region IAI 成员

        public IAICommonServer CommonServer
        {
            set { commonServer = (AICommonServer)value; }
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

        Vector2 aimPos;

        bool waitOrder = true;

        float tempAzi;

        bool rotaing = false;

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            Vector2 curPos = orderServer.Pos;

            if (InputHandler.MouseJustPressRight)
            {
                aimPos = InputHandler.GetCurMousePosInLogic( BaseGame.RenderEngine );
                float aimAzi = MathTools.AziFromRefPos( aimPos - curPos );
                action.StopMove();
                action.StopRota();

                rotaing = true;
                action.AddOrder( new OrderRotaToAzi( aimAzi, 0,
                    delegate( IActionOrder order )
                    {
                        rotaing = false;
                        SearchPath();
                    }, false ) );

                waitOrder = false;
            }

            if (waitOrder)
                return;

            if (!rotaing)
                SearchPath();

            if (Vector2.Distance( curPos, aimPos ) < 1)
            {
                action.StopMove();
                action.StopRota();
                waitOrder = true;
            }


            action.Update( seconds );
        }

        private void SearchPath ()
        {
            Vector2 curPos = orderServer.Pos;
            float curAzi = MathTools.AngTransInPI( orderServer.Azi );
            float aimAzi = MathTools.AziFromRefPos( aimPos - curPos );

            bool aimObstruct = false;
            bool curObstruct = false;
            bool crossPi = false;
            bool crossZero = false;
            float minAziMinus = 0;
            float maxAziMinus = -MathHelper.Pi;
            float minAziPlus = MathHelper.Pi;
            float maxAziPlus = 0;

            foreach (EyeableBorderObjInfo borderObjInfo in orderServer.EyeableBorderObjInfos)
            {
                if (!((SceneCommonObjInfo)borderObjInfo.EyeableInfo.ObjInfo.SceneInfo).isTankObstacle)
                    continue;

                if (InputHandler.JustPressKey( Microsoft.Xna.Framework.Input.Keys.B ))
                    //borderObjInfo.UpdateConvexHall();

                    foreach (BordPoint bordP in borderObjInfo.Border.VisiBorder)
                    {


                        Vector2 logicP = Vector2.Transform( ConvertHelper.PointToVector2( bordP.p ), borderObjInfo.EyeableInfo.CurTransMatrix );
                        float azi = MathTools.AziFromRefPos( logicP - curPos );

                        if (azi < 0)
                        {
                            minAziMinus = Math.Min( minAziMinus, azi );
                            maxAziMinus = Math.Max( maxAziMinus, azi );
                        }
                        else
                        {
                            minAziPlus = Math.Min( minAziPlus, azi );
                            maxAziPlus = Math.Max( maxAziPlus, azi );
                        }

                        if (MathTools.FloatEqualZero( MathTools.AngTransInPI( azi - MathHelper.Pi ), 0.1f ))
                        {
                            crossPi = true;
                        }

                        if (MathTools.FloatEqualZero( azi, 0.1f ))
                        {
                            crossZero = true;
                        }

                        if (MathTools.FloatEqual( azi, aimAzi, 0.1f ) && Vector2.Distance( logicP, curPos ) < Vector2.Distance( aimPos, curPos ))
                        {
                            aimObstruct = true;
                        }

                        if (MathTools.FloatEqual( curAzi, azi, 0.1f ) && Vector2.Distance( logicP, curPos ) < Vector2.Distance( aimPos, curPos ))
                        {
                            curObstruct = true;
                        }
                    }
            }

            if (!aimObstruct)
            {
                if (!MathTools.FloatEqual( curAzi, aimAzi, 0.1f ))
                    action.AddOrder( new OrderMoveToPosDirect( aimPos ) );
                else
                    orderServer.ForwardSpeed = orderServer.MaxForwardSpeed;
            }
            else if (!curObstruct)
            {
                orderServer.ForwardSpeed = orderServer.MaxForwardSpeed;
            }
            else
            {
                orderServer.ForwardSpeed = 0;

                float aziEage1 = 0;
                float aziEage2 = 0;

                if (!crossZero && !crossPi)
                {
                    if (minAziMinus == 0)
                    {
                        aziEage1 = minAziPlus;
                        aziEage2 = maxAziPlus;
                    }
                    else
                    {
                        aziEage1 = minAziMinus;
                        aziEage2 = maxAziMinus;
                    }
                }
                else if (crossZero)
                {
                    aziEage1 = minAziMinus;
                    aziEage2 = maxAziPlus;
                }
                else if (crossPi)
                {
                    aziEage1 = minAziPlus;
                    aziEage2 = maxAziMinus;
                }
                else
                {

                }

                float curAimAzi = 0;

                if (Math.Abs( MathTools.AngTransInPI( curAzi - aziEage1 ) ) < Math.Abs( MathTools.AngTransInPI( curAzi - aziEage2 ) ))
                {
                    curAimAzi = aziEage1 - 0.1f;
                }
                else
                {
                    curAimAzi = aziEage2 + 0.1f;
                }

                rotaing = true;
                action.AddOrder( new OrderRotaToAzi( curAimAzi, 0,
                    delegate( IActionOrder order )
                    {
                        rotaing = false;
                        orderServer.ForwardSpeed = orderServer.MaxForwardSpeed;
                    }, false ) );
            }
        }

        #endregion

        #region IAI 成员


        public void Draw ()
        {

        }

        #endregion
    }
}
