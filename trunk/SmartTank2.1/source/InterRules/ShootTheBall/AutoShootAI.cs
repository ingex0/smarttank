using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.GameObjs.Tank;
using SmartTank.GameObjs.Tank.SinTur;
using TankEngine2D.Input;
using SmartTank.Senses.Vision;
using TankEngine2D.Helpers;
using InterRules.ShootTheBall;
using SmartTank.AI;
using Microsoft.Xna.Framework;
using SmartTank.AI.AIHelper;

namespace InterRules.ShootTheBall
{
    [AIAttribute( "AutoShootAI", "SmartTank Team", "A Simple Linear Aimming AI", 2007, 10, 31 )]
    class AutoShootAI : IAISinTur
    {
        IAIOrderServerSinTur orderServer;

        AIActionHelper action;

        Vector2 lastItemPos;

        bool aimming = false;
        float aimmingTime;

        Vector2 aimPos;
        Vector2 vel;

        bool lastRaderRotaWise = true;

        #region IAI 成员

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = (IAIOrderServerSinTur)value;
                action = new AIActionHelper( orderServer );
            }
        }

        public IAICommonServer CommonServer
        {
            set { }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            #region OLDCODE
            //if (InputHandler.IsKeyDown( Keys.W ))
            //{
            //    orderServer.ForwardSpeed = 1000;
            //}
            //else if (InputHandler.IsKeyDown( Keys.S ))
            //{
            //    orderServer.ForwardSpeed = -1000;
            //}
            //else
            //{
            //    orderServer.ForwardSpeed = 0;
            //}

            //if (InputHandler.IsKeyDown( Keys.D ))
            //{
            //    orderServer.TurnRightSpeed = 20;
            //}
            //else if (InputHandler.IsKeyDown( Keys.A ))
            //{
            //    orderServer.TurnRightSpeed = -20;
            //}
            //else
            //{
            //    orderServer.TurnRightSpeed = 0;
            //}
            #endregion

            aimmingTime -= seconds;

            List<IEyeableInfo> eyeableInfos = orderServer.GetEyeableInfo();
            if (eyeableInfos.Count != 0)
            {


                orderServer.TurnRaderWiseSpeed = 0;

                if (eyeableInfos[0] is ItemEyeableInfo)
                {
                    lastRaderRotaWise = MathTools.AziFromRefPos( ((ItemEyeableInfo)eyeableInfos[0]).Pos - orderServer.Pos ) > 0;
                    action.AddOrder( new OrderRotaRaderToPos( ((ItemEyeableInfo)eyeableInfos[0]).Pos ) );
                }
                else if (eyeableInfos[0] is TankSinTur.TankCommonEyeableInfo)
                {
                    lastRaderRotaWise = MathTools.AziFromRefPos( ((TankSinTur.TankCommonEyeableInfo)eyeableInfos[0]).Pos - orderServer.Pos ) > 0;
                    action.AddOrder( new OrderRotaRaderToPos( ((TankSinTur.TankCommonEyeableInfo)eyeableInfos[0]).Pos ) );
                }

                if (eyeableInfos[0] is ItemEyeableInfo)
                {
                    lastItemPos = ((ItemEyeableInfo)eyeableInfos[0]).Pos;
                    vel = ((ItemEyeableInfo)eyeableInfos[0]).Vel;
                }
                else if (eyeableInfos[0] is TankSinTur.TankCommonEyeableInfo)
                {
                    lastItemPos = ((TankSinTur.TankCommonEyeableInfo)eyeableInfos[0]).Pos;
                    vel = ((TankSinTur.TankCommonEyeableInfo)eyeableInfos[0]).Vel;
                }


                if (!aimming)
                {

                    float curTurretAzi = orderServer.TurretAimAzi;

                    float t = 0;

                    float maxt = 30;
                    bool canShoot = false;
                    while (t < maxt)
                    {
                        aimPos = lastItemPos + vel * t;
                        float timeRota = Math.Abs( MathTools.AngTransInPI( MathTools.AziFromRefPos( aimPos - orderServer.TurretAxePos ) - curTurretAzi ) / orderServer.MaxRotaTurretSpeed );
                        float timeShell = (Vector2.Distance( aimPos, orderServer.TurretAxePos ) - orderServer.TurretLength) / orderServer.ShellSpeed;
                        if (MathTools.FloatEqual( timeRota + timeShell, t, 0.03f ))
                        {
                            canShoot = true;
                            break;
                        }
                        t += 0.03f;
                    }

                    if (canShoot)
                    {
                        aimming = true;
                        aimmingTime = t;
                        action.AddOrder( new OrderRotaTurretToPos( aimPos, 0,
                            delegate( IActionOrder order )
                            {
                                orderServer.Fire();
                                aimming = false;
                            }, false ) );
                    }
                    else
                    {
                        action.AddOrder( new OrderRotaTurretToPos( lastItemPos ) );
                    }
                }
                else
                {
                    if ((lastItemPos + aimmingTime * vel - aimPos).Length() > 4)
                    {
                        aimming = false;
                    }
                }
            }
            else
            {
                orderServer.TurnRaderWiseSpeed = orderServer.MaxRotaRaderSpeed * (lastRaderRotaWise ? 1 : -1);
                
            }

            action.Update( seconds );
        }

        #endregion

        public void Draw ()
        {
            
        }
    }
}
