using System;
using System.Collections.Generic;
using System.Text;
using GameBase;
using Microsoft.Xna.Framework;
using GameBase.Graphics;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Helpers;
using GameBase.Effects;

namespace SmartTank.Test
{
    class TextEffectTest : BaseGame
    {
        int tick = -1;
        int aimtick = RandomHelper.GetRandomInt( 100 );

        protected override void Initialize ()
        {
            base.Initialize();
            Coordin.SetScreenViewRect( new Rectangle( 0, 0, ClientRec.Width, ClientRec.Height ) );
            //Coordin.SetLogicViewRect( new Rectangle( 0, 0, 100, 100 ) );
            Coordin.SetCamera( 2, new Vector2( 200, 150 ), 0 );
        }

        protected override void Update ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            if (tick == aimtick)
            {
                tick = -1;
                aimtick = RandomHelper.GetRandomInt(20, 100 );
                TextEffect.AddRiseFade( "Hello RiseFade Effect!", 
                    new Vector2( 50, 50 ), 1f,
                    new Color( (byte)RandomHelper.GetRandomInt( 255 ), 
                    (byte)RandomHelper.GetRandomInt( 255 ),
                    (byte)RandomHelper.GetRandomInt( 255 ), 255 ),
                    0, FontType.Comic, 500, 0.5f );
                
            }
            tick++;
            base.Update( gameTime );
        }

        protected override void GameDraw ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            TextEffect.Draw();
        }
    }
}
