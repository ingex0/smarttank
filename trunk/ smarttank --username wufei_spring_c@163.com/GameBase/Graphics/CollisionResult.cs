using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameBase.Graphics
{
    public class CollisionResult
    {
        public bool IsCollided;
        public Vector2 InterPos;
        public Vector2 NormalVector;

        public CollisionResult ()
        {

        }
        public CollisionResult ( bool isCollided )
        {
            this.IsCollided = isCollided;
        }
        public CollisionResult ( Vector2 interPos, Vector2 normalVector )
        {
            IsCollided = true;
            InterPos = interPos;
            NormalVector = normalVector;
        }
        public CollisionResult ( bool isCollided, Vector2 interPos, Vector2 nornalVector )
        {
            this.IsCollided = isCollided;
            this.InterPos = interPos;
            this.NormalVector = nornalVector;
        }
    }
}
