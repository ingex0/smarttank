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
            ChineseWriter.WriteText( "∞¬£¨Œ‚∑…Spring1234567!@$^&*()?/.,:;'", new Vector2( 100, 100 ), 0, 2f, Color.Red, 0f, FontType.HanDinJianShu );
            FontManager.DrawInScrnCoord( "¡’¡’", new Vector2( 100, 200 ), 1f, Color.White, 0f, FontType.HanDinJianShu );
            float width = FontManager.LengthOfString( "¡’¡’", 1f, FontType.HanDinJianShu );
            FontManager.DrawInScrnCoord( "≈∂£¨π˜π˜", new Vector2( 100 + width, 200 ), 1f, Color.Yellow, 0f, FontType.HanDinJianShu );
        }
    }
}
