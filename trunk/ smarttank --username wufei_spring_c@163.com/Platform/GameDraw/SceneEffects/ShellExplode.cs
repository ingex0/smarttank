using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Graphics;
using GameBase.Helpers;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platform.GameObjects;

namespace Platform.GameDraw.SceneEffects
{
    public class ShellExplode
    {
        AnimatedSpriteSeries shellTexs;

        public static void LoadResources ()
        {
            AnimatedSpriteSeries.LoadResource(
                Path.Combine( Directories.ContentDirectory, "SceneEffects\\ShellExplode\\MulEffect" ),
                0, 32 );
        }

        private static AnimatedSpriteSeries CreateTexs ()
        {
            AnimatedSpriteSeries result = new AnimatedSpriteSeries();
            result.LoadSeriesFormContent( Path.Combine( Directories.ContentDirectory, "SceneEffects\\ShellExplode\\MulEffect" ),
                0, 32, false );

            return result;
        }

        public ShellExplode ( Vector2 pos, float rota )
        {
            shellTexs = CreateTexs();
            shellTexs.SetSpritesParameters( new Vector2( 49, 49 ), pos, 30, 30, rota + MathHelper.PiOver2, Color.White, LayerDepth.Shell, SpriteBlendMode.AlphaBlend );
            shellTexs.Interval = 1;
            shellTexs.Start( 0, 32, true );
        }

    }
}
