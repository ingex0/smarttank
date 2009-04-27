using System;
using System.Collections.Generic;
using System.Text;
using GameEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Common.Helpers;
using GameEngine.Draw;


namespace GameEngine.Effects.SceneEffects
{
    public class Explode
    {
        AnimatedSpriteSingle animatefire;
        AnimatedSpriteSingle animateBright;

        public static void LoadResources()
        {
            AnimatedSpriteSingle.LoadResources( BaseGame.ContentMgr, Path.Combine( Directories.ContentDirectory, "SceneEffects\\fire" ) );
            AnimatedSpriteSingle.LoadResources( BaseGame.ContentMgr, Path.Combine( Directories.ContentDirectory, "SceneEffects\\bright" ) );
        }

        public Explode( Vector2 pos, float rata )
        {
            animatefire = new AnimatedSpriteSingle( BaseGame.RenderEngine );
            animatefire.LoadFromContent( BaseGame.ContentMgr, Path.Combine( Directories.ContentDirectory, "SceneEffects\\fire" ), 32, 32, 2 );
            animatefire.SetParameters( new Vector2( 16, 16 ), pos, 2f, rata, Color.White, LayerDepth.EffectLow, SpriteBlendMode.AlphaBlend );
            animatefire.Start( 0, 64, true );

            animateBright = new AnimatedSpriteSingle( BaseGame.RenderEngine );
            animateBright.LoadFromContent( BaseGame.ContentMgr, Path.Combine( Directories.ContentDirectory, "SceneEffects\\bright" ), 32, 32, 2 );
            animateBright.SetParameters( new Vector2( 16, 16 ), pos, 2f, rata, Color.White, LayerDepth.EffectLow, SpriteBlendMode.AlphaBlend );
            animateBright.Start( 0, 64, true );
        }
    }
}
