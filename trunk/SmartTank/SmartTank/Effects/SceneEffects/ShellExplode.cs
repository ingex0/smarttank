using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Graphics;
using TankEngine2D.Helpers;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SmartTank.Helpers;
using SmartTank.Draw;

namespace SmartTank.Effects.SceneEffects
{
    public class ShellExplode
    {
        AnimatedSpriteSeries shellTexs;

        public static void LoadResources()
        {
            AnimatedSpriteSeries.LoadResource( BaseGame.ContentMgr,
                Path.Combine( Directories.ContentDirectory, "SceneEffects\\ShellExplode\\MulEffect" ),
                0, 32 );
        }

        private static AnimatedSpriteSeries CreateTexs()
        {
            AnimatedSpriteSeries result = new AnimatedSpriteSeries( BaseGame.RenderEngine );
            result.LoadSeriesFormContent( BaseGame.RenderEngine, BaseGame.ContentMgr, Path.Combine( Directories.ContentDirectory, "SceneEffects\\ShellExplode\\MulEffect" ),
                0, 32, false );

            return result;
        }

        public ShellExplode( Vector2 pos, float rota )
        {
            shellTexs = CreateTexs();
            shellTexs.SetSpritesParameters( new Vector2( 49, 49 ), pos, 30, 30, rota + MathHelper.PiOver2, Color.White, LayerDepth.Shell, SpriteBlendMode.AlphaBlend );
            shellTexs.Interval = 1;
            shellTexs.Start( 0, 32, true );
        }

    }
}
