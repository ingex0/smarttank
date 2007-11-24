using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Input;
using GameBase.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameBase.DataStructure.test
{
    public class TestRectanglef : BaseGame
    {
        Rectanglef rectA = new Rectanglef( 130, 80, 40, 40 );
        Rectanglef rectB = new Rectanglef( 0, 0, 20, 20 );
        bool isInterset;

        protected override void Initialize ()
        {
            base.Initialize();
            Coordin.SetScreenViewRect( new Rectangle( 0, 0, ClientRec.Width, ClientRec.Height ) );
            //Coordin.SetLogicViewRect( new Rectanglef( 0, 0, 300, 200 ) );
            Coordin.SetCamera( 2, new Vector2( 200, 150 ), 0 );
            IsMouseVisible = true;
        }

        protected override void Update ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            base.Update( gameTime );
            if (InputHandler.CurMouseLeftBtnPressed)
            {
                rectB.X = InputHandler.CurMousePosInLogic.X;
                rectB.Y = InputHandler.CurMousePosInLogic.Y;
            }

            if (rectA.Intersects( rectB ))
            {
                isInterset = true;
            }
            else
            {
                isInterset = false;
            }
        }

        protected override void GameDraw ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            if (isInterset)
            {
                BaseGame.Device.Clear( Color.Red );
            }

            BasicGraphics.DrawRectangle( rectA, 4, Color.Black, 0f );
            BasicGraphics.DrawRectangle( rectB, 4, Color.Black, 0f );

        }
    }
}
