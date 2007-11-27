using System;
using System.Collections.Generic;
using System.Text;
using GameBase;
using GameBase.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Graphics;

namespace SmartTank.Test
{
    class FontTest : BaseGame
    {
        protected override void Initialize ()
        {
            Coordin.SetScreenViewRect( new Rectangle( 10, 10, ClientRec.Width - 20, ClientRec.Height - 20 ) );
            //Coordin.SetLogicViewRect( new Rectangle( 0, 0, 100, 100 ) );
            Coordin.SetCamera( 2, new Vector2( 200, 150 ), 0 );
            base.Initialize();
        }

        protected override void LoadGraphicsContent ( bool loadAllContent )
        {
            base.LoadGraphicsContent( loadAllContent );
        }

        protected override void Update ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            base.Update( gameTime );
        }

        protected override void GameDraw ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            FontManager.Draw( "the quick brown fox jumps over the lazy dog.1234567890", new Vector2( 0, 0 ), 1f, Color.Gold, 0, FontType.Comic );
            FontManager.Draw( "the quick brown fox jumps over the lazy dog.1234567890", new Vector2( 0, 50 ), 1f, Color.WhiteSmoke, 0, FontType.Lucida );
        }
    }
}
