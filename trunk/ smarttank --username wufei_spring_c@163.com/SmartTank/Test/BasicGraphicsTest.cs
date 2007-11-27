using System;
using System.Collections.Generic;
using System.Text;
using GameBase;
using GameBase.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Helpers;
using GameBase.Effects;
using GameBase.Input;

namespace SmartTank.Test
{
    class BasicGraphicsTest : BaseGame
    {
        int tick = -1;
        int aimtick = RandomHelper.GetRandomInt( 100 );

        protected override void Initialize ()
        {
            base.Initialize();
            Coordin.SetScreenViewRect( new Rectangle( 10, 10, ClientRec.Width - 20, ClientRec.Height - 20 ) );
            //Coordin.SetLogicViewRect( new Rectangle( 0, 0, 100, 100 ) );
            Coordin.SetCamera( 2, new Vector2( 50, 50 ), MathHelper.PiOver4 );
        }

        protected override void Update ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            base.Update( gameTime );
            if (InputHandler.MouseJustClickLeft)
            {
                Quake.BeginQuake( 30, 50 );
            }
            if (tick == aimtick)
            {
                tick = -1;
                aimtick = RandomHelper.GetRandomInt( 20, 100 );
                TextEffect.AddRiseFade( "Hello RiseFade Effect!",
                    new Vector2( 50, 50 ), 1f,
                    new Color( (byte)RandomHelper.GetRandomInt( 255 ),
                    (byte)RandomHelper.GetRandomInt( 255 ),
                    (byte)RandomHelper.GetRandomInt( 255 ), 255 ),
                    0, FontType.Comic, 500, 0.5f );

            }
            tick++;
        }

        protected override void GameDraw ( Microsoft.Xna.Framework.GameTime gameTime )
        {

            BasicGraphics.DrawPoint( new Vector2( 20, 20 ), 1f, Color.Red, 0 );
            BasicGraphics.DrawLine( new Vector2( 20, 20 ), new Vector2( 50, 50 ), 10, Color.Gold, 1 );
            BasicGraphics.DrawRectangle( new Rectangle( 0, 0, 100, 100 ), 3, Color.Red, 1, SpriteBlendMode.Additive );
            BasicGraphics.DrawPoint( new Vector2( 60, 80 ), 1f, Color.Red, 1 );
            BasicGraphics.DrawPoint( new Vector2( 90, 95 ), 1f, Color.Red, 0 );
            BasicGraphics.FillRectangle( new Rectangle( 60, 80, 30, 15 ), ColorHelper.ApplyAlphaToColor( Color.Green, 0.7f ), 0.5f, SpriteBlendMode.AlphaBlend );

            FontManager.Draw( "Press The Mouse Left Button, I'll Show You An EarthQuake!", new Vector2( 20, 10 ), 0.6f, Color.Gold, 0f, FontType.Lucida );
            TextEffect.Draw();
        }
    }
}
