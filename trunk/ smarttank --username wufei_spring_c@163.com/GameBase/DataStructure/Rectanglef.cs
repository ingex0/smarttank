using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameBase.DataStructure
{
    [Serializable]
    public struct Rectanglef
    {
        public float X, Y;
        public float Width, Height;

        public float Top
        {
            get { return Y; }
        }
        public float Left
        {
            get { return X; }
        }
        public float Bottom
        {
            get { return Y + Height; }
        }
        public float Right
        {
            get { return X + Width; }
        }

        public Rectanglef ( float x, float y, float width, float height )
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public Rectanglef ( Vector2 min, Vector2 max )
        {
            this.X = min.X;
            this.Y = min.Y;
            this.Width = max.X - min.X;
            this.Height = max.Y - min.Y;
        }

        public bool Intersects ( Rectanglef rectB )
        {
            if (rectB.X < this.X + this.Width && rectB.X + rectB.Width > this.X &&
                rectB.Y + rectB.Height > this.Y && rectB.Y < this.Y + this.Height ||
                rectB.X + rectB.Width > this.X && rectB.X < this.X + this.Width &&
                rectB.Y + rectB.Height > this.Y && rectB.Y < this.Y + this.Height )
                return true;
            else
                return false;
        }

        public bool Contains ( Vector2 point )
        {
            if (X <= point.X && point.X <= X + Width && Y <= point.Y && point.Y <= Y + Height)
                return true;
            else
                return false;
        }
    }
}
