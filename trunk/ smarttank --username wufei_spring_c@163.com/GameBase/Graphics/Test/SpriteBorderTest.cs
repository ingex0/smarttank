using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace GameBase.Graphics.Test
{
    public class SpriteBorderTest:BaseGame
    {
        protected override void Initialize ()
        {
            base.Initialize();
            Texture2D tex = Content.Load<Texture2D>( "Test\\turret" );
            SpriteBorder border = new SpriteBorder( tex );
            border.ShowDataToConsole();
        }
    }
}
