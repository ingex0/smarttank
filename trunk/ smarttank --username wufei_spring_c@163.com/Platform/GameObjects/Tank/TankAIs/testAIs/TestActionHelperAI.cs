using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameObjects.Tank.TankControls;
using GameBase.Input;
using GameBase.Helpers;
using Microsoft.Xna.Framework;
using Platform.AIHelper;

namespace Platform.GameObjects.Tank.TankAIs.testAIs
{
    class TestActionHelperAI : IAISinTur
    {
        AIActionHelper action;

        IAIOrderServerSinTur orderServer;

        #region IAI 成员

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = (IAIOrderServerSinTur)value;
                action = new AIActionHelper( (IAIOrderServerSinTur)value );
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
            if (InputHandler.MouseJustClickLeft)
            {
                //action.AddOrder( new RotaToAziOrder( MathTools.AziFromRefPos( InputHandler.CurMousePosInLogic - orderServer.Pos ), 0,
                //    delegate( IActionOrder order )
                //    {
                //        orderServer.Fire();
                //    } , true ) );
                //action.AddOrder( new RotaTurretToPosOrder( InputHandler.CurMousePosInLogic, 0,
                //    delegate( IActionOrder order )
                //    {
                //        orderServer.Fire();
                //    } , true ) );
                action.AddOrder( new OrderRotaRaderToPos( InputHandler.CurMousePosInLogic, 0,
                    delegate( IActionOrder order )
                    {
                        orderServer.Fire();
                    } , true ) );
            }

            if (InputHandler.MouseJustClickRight)
            {
                //action.AddOrder( new RotaTurretToAziOrder( MathTools.AziFromRefPos( InputHandler.CurMousePosInLogic - orderServer.TurretAxePos ), 0,
                //    delegate( IActionOrder order )
                //    {
                //        orderServer.Fire();
                //    } , true ) );
                //action.AddOrder( new RotaRaderToAziOrder( MathTools.AziFromRefPos( InputHandler.CurMousePosInLogic - orderServer.Pos ), 0,
                //    delegate( IActionOrder order )
                //    {
                //        orderServer.Fire();
                //    } , true ) );
                //action.AddOrder( new MoveToPosDirectOrder( InputHandler.CurMousePosInLogic, orderServer.MaxRotaSpeed * 0.5f, orderServer.MaxForwardSpeed * 0.5f ) );
                //action.AddOrder( new MoveCircleOrder( InputHandler.CurMousePosInLogic, orderServer.MaxForwardSpeed, orderServer ) );
                action.AddOrder( new OrderRotaTurretToPos( InputHandler.CurMousePosInLogic ) );
            }

            if (InputHandler.JustPressKey( Microsoft.Xna.Framework.Input.Keys.S ))
            {
                //action.AddOrder( new MoveOrder( -60, 0,
                //    delegate( IActionOrder order )
                //    {
                //        action.AddOrder( new MoveOrder( 60 ) );
                //    }, false ) );
                action.AddOrder( new OrderMove( -60 ) );
            }
            if (InputHandler.JustPressKey( Microsoft.Xna.Framework.Input.Keys.W ))
            {
                //action.AddOrder( new MoveOrder( 60 ) );
                action.AddOrder( new OrderMoveCircle( 300, orderServer.MaxRotaSpeed, orderServer.MaxForwardSpeed,
                    delegate( IActionOrder order )
                    {
                        orderServer.Fire();
                    } , true ) );
            }

            if (InputHandler.JustPressKey( Microsoft.Xna.Framework.Input.Keys.G ))
            {
                action.StopRota();
                action.StopRotaTurret();
                action.StopRotaRader();
            }

            action.Update( seconds );

            //if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.S ))
            //{
            //    orderServer.ForwardSpeed = -100;
            //}
            //else
            //{
            //    orderServer.ForwardSpeed = 0;
            //}

            if (InputHandler.JustPressKey( Microsoft.Xna.Framework.Input.Keys.Space ))
            {

            }
        }

        #endregion

        public void Draw ()
        {

        }

    }
}
