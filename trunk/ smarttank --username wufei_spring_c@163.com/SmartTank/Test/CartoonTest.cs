using System;
using System.Collections.Generic;
using System.Text;
using GameBase;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameBase.Graphics;

namespace SmartTank.Test
{
    class CartoonTest : BaseGame
    {
        AnimatedSpriteSeries animatedSprite;

        protected override void Initialize ()
        {
            Sprite.Intial();
            Coordin.SetScreenViewRect( new Rectangle( 0, 0, ClientRec.Width, ClientRec.Height ) );
            //Coordin.SetLogicViewRect( new Rectangle( 0, 0, 100, 100 ) );
            Coordin.SetCamera( 2, new Vector2( 200, 150 ), 0 );
            animatedSprite = new AnimatedSpriteSeries();
            animatedSprite.OnStop += new EventHandler( cartoon_OnStop );

            base.Initialize();
        }

        void cartoon_OnStop ( object sender, EventArgs e )
        {
            animatedSprite.Start();
        }

        protected override void LoadGraphicsContent ( bool loadAllContent )
        {
            base.LoadGraphicsContent( loadAllContent );
            animatedSprite.LoadSeriesFormContent( "Test\\jump", 1, 6, false );
            animatedSprite.SetSpritesParameters( new Vector2( 56, 45 ), new Vector2( 30, 30 ), 33, 27, 0, Color.White, 0, SpriteBlendMode.AlphaBlend );
            animatedSprite.Sprites[1].Pos.X += 5;
            animatedSprite.Sprites[2].Pos.X += 10;
            animatedSprite.Sprites[3].Pos.X += 20;
            animatedSprite.Sprites[4].Pos.X += 35;
            animatedSprite.Sprites[5].Pos.X += 45;
            animatedSprite.Interval = 10;

            animatedSprite.Start( 3, 30, false );
        }

        protected override void Update ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            base.Update( gameTime );
        }

        protected override void GameDraw ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            AnimatedManager.Draw();
        }
    }
}
