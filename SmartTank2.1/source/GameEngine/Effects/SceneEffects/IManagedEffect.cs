using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameEngine.Draw;


namespace GameEngine.Effects.SceneEffects
{
    public interface IManagedEffect:IDrawableObj
    {
        bool IsEnd { get; }

        void Update( float seconds );
    }
}
