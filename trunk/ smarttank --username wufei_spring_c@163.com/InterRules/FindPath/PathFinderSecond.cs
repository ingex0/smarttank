using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameObjects.Tank.TankAIs;
using Platform.AIHelper;
using GameBase.Input;
using Platform.Senses.Memory;
using Microsoft.Xna.Framework.Input;
using GameBase.DataStructure;
using GameBase.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Platform.Senses.Vision;
using Platform.Scene;

namespace InterRules.FindPath
{
    [AIAttribute( "PathFinder No.2", "SmartTank编写组", "测试AI的寻路能力", 2007, 11, 23 )]
    class PathFinderSecond : IAISinTur
    {
        IAIOrderServerSinTur orderServer;

        AICommonServer commonServer;

        AIActionHelper action;

        NavigateMap naviMap;

        bool seeItem = false;
        bool itemDisappeared = false;
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


            if (InputHandler.MouseJustClickRight)
            {
                action.AddOrder( new OrderRotaRaderToPos( InputHandler.CurMousePosInLogic ) );
            }

            action.AddOrder( new OrderRotaTurretToPos( InputHandler.CurMousePosInLogic ) );


            //if (InputHandler.JustPressKey( Keys.C ))
            //{
            foreach (EyeableBorderObjInfo borderObj in orderServer.EyeableBorderObjInfos)
            {
                borderObj.UpdateConvexHall( 10 );
            }
            //}

            //if (InputHandler.JustPressKey( Keys.N ))
            //{

            naviMap = orderServer.CalNavigateMap(
                delegate( EyeableBorderObjInfo obj )
                {
                    if (((SceneCommonObjInfo)(obj.EyeableInfo.ObjInfo.SceneInfo)).isTankObstacle)
                        return true;
                    else
                        return false;
                }, commonServer.MapBorder, 5 );
            //}

            action.Update( seconds );

            itemDisappeared = false;
            foreach (EyeableBorderObjInfo borderObjInfo in orderServer.EyeableBorderObjInfos)
            {
                if (borderObjInfo.EyeableInfo.ObjInfo.Name == "Item")
                {
                    if (borderObjInfo.IsDisappeared)
                        itemDisappeared = true;
                }
            }

            seeItem = false;
            foreach (IEyeableInfo eyeableInfo in orderServer.GetEyeableInfo())
            {
                if (eyeableInfo.ObjInfo.Name == "Item")
                {
                    seeItem = true;
                }
            }
        }

        #endregion

        #region IAI 成员


        public void Draw ()
        {
            if (naviMap != null)
            {
                foreach (GraphPoint<NaviPoint> naviPoint in naviMap.Map)
                {
                    BasicGraphics.DrawPoint( naviPoint.value.Pos, 1f, Color.Yellow, 0.1f );

                    foreach (GraphPath<NaviPoint> path in naviPoint.neighbors)
                    {
                        BasicGraphics.DrawLine( naviPoint.value.Pos, path.neighbor.value.Pos, 3f, Color.Yellow, 0.1f );
                        FontManager.Draw( path.weight.ToString(), 0.5f * (naviPoint.value.Pos + path.neighbor.value.Pos), 0.5f, Color.Black, 0f, FontType.Comic );
                    }
                }

                foreach (Segment guardLine in naviMap.GuardLines)
                {
                    BasicGraphics.DrawLine( guardLine.startPoint, guardLine.endPoint, 3f, Color.Red, 0f, SpriteBlendMode.AlphaBlend );
                }
            }

            if (seeItem)
                FontManager.Draw( "I see Item!", orderServer.Pos, 1f, Color.Black, 0f, FontType.Lucida );

            if (itemDisappeared)
                FontManager.Draw( "Item Disappeared!", orderServer.Pos - new Vector2( 0, 10 ), 1f, Color.Black, 0f, FontType.Lucida );
        }

        #endregion
    }
}
