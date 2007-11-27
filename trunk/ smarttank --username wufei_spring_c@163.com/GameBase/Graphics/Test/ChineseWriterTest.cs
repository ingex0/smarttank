using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameBase.Graphics.Test
{
    public class ChineseWriterTest : BaseGame
    {
        protected override void Initialize ()
        {
            base.Initialize();
            ChineseWriter.Intitial();
        }

        protected override void GameDraw ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            ChineseWriter.WriteText( "Œ‚∑…", new Vector2( 100, 100 ), 0, 1f, Color.White, 0f );
        }
    }
}
