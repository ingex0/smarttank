using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Platform.GameDraw
{
    public interface IDrawableObj
    {
        void Draw ();
        Vector2 Pos { get;}
    }
}
