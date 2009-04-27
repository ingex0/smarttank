using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine.Draw
{
    public interface IDrawableObj
    {
        void Draw ();
        Vector2 Pos { get;}
    }
}
