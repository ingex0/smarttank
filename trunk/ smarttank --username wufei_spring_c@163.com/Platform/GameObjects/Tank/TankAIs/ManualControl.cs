using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameObjects.Tank.TankControls;
using GameBase.Input;
using Microsoft.Xna.Framework.Input;
using GameBase.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Platform.Senses.Vision;
using Microsoft.Xna.Framework;
using Platform.AIHelper;

namespace Platform.GameObjects.Tank.TankAIs
{
    [AIAttribute( "ManualControl", "SmartTank Team", "manualControl for test", 2007, 10, 25 )]
    public class ManualControl : IAISinTur
    {
        IAIOrderServerSinTur orderServer;

        AIActionHelper action;

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
            if (InputHandler.IsKeyDown( Keys.W ))
            {
                orderServer.ForwardSpeed = 1000;
            }
            else if (InputHandler.IsKeyDown( Keys.S ))
            {
                orderServer.ForwardSpeed = -1000;
            }
            else
            {
                orderServer.ForwardSpeed = 0;
            }

            if (InputHandler.IsKeyDown( Keys.D ))
            {
                orderServer.TurnRightSpeed = 20;
            }
            else if (InputHandler.IsKeyDown( Keys.A ))
            {
                orderServer.TurnRightSpeed = -20;
            }
            else
            {
                orderServer.TurnRightSpeed = 0;
            }

            action.AddOrder( new OrderRotaTurretToPos( InputHandler.CurMousePosInLogic ) );

            //if (InputHandler.IsKeyDown( Keys.K ))
            //{
            //    tankContr.TurnTurretWiseSpeed = 20;
            //}
            //else if (InputHandler.IsKeyDown( Keys.J ))
            //{
            //    tankContr.TurnTurretWiseSpeed = -20;
            //}
            //else
            //{
            //    tankContr.TurnTurretWiseSpeed = 0;
            //}



            if (InputHandler.JustPressKey( Keys.Space ) ||
                InputHandler.MouseJustClickLeft)
            {
                orderServer.Fire();
            }

            if (InputHandler.IsKeyDown( Keys.M ))
            {
                orderServer.TurnRaderWiseSpeed = 20;
            }

            if (InputHandler.IsKeyDown( Keys.N ))
            {
                orderServer.TurnRaderWiseSpeed = -20;
            }

            if (InputHandler.MouseJustClickRight)
            {
                action.AddOrder( new OrderRotaRaderToPos( InputHandler.CurMousePosInLogic ) );
            }

            if (InputHandler.MouseWheelDelta != 0)
            {
                action.AddOrder( new OrderRotaRaderToAzi( orderServer.RaderAimAzi - (float)InputHandler.MouseWheelDelta / 300 ) );
            }

            // test

            if (InputHandler.JustPressKey( Keys.Y ))
            {
                //action.AddOrder( new OrderRotaRaderToAng( MathHelper.PiOver4 ) );
                action.AddOrder( new OrderScanRaderAzi( MathHelper.PiOver4, -MathHelper.PiOver4, 0, true ) );
            }

            action.Update( seconds );
        }

        #endregion


        #region IAI 成员


        public void Draw ()
        {
            
        }

        #endregion
    }
}
