using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SmartTank.Effects.SceneEffects.BaseEffects;
using System.IO;
using TankEngine2D.Helpers;
using SmartTank.Helpers;
using SmartTank.Draw;

namespace SmartTank.Effects.SceneEffects
{
    public class ShellExplodeBeta
    {
        ScaleUp scaleUp;

        public static void LoadResources ()
        {

        }

        public ShellExplodeBeta ( Vector2 pos, float rota )
        {
            scaleUp = new ScaleUp( Path.Combine( Directories.ContentDirectory, "SceneEffects\\star2" ), new Vector2( 64, 64 ), LayerDepth.EffectLow,
                pos,
                delegate( float curTime, float lastRadius )
                {
                    if (curTime == 0)
                        return 3f;
                    else
                        return lastRadius + 0.3f;
                },
                delegate( float curTime )
                {
                    return 0;
                }, 20 );
        }
    }
}
