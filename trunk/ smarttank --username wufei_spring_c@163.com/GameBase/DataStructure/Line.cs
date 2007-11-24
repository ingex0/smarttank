using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameBase.DataStructure
{
    public struct Line
    {
        public Vector2 pos;
        public Vector2 direction;

        public Line ( Vector2 pos, Vector2 direction )
        {
            this.pos = pos;
            this.direction = direction;
        }

        public override string ToString ()
        {
            return pos.ToString() + " " + direction.ToString();
        }
    }
}
