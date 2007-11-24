using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Graphics;

namespace Platform.PhisicalCollision
{
    public class SpriteColMethod : IColMethod
    {
        Sprite[] colSprites;

        public Sprite[] ColSprites
        {
            get { return colSprites; }
        }

        #region IColMethod ≥…‘±

        public GameBase.Graphics.CollisionResult CheckCollision ( IColMethod colB )
        {
            return colB.CheckCollisionWithSprites( this );
        }

        public GameBase.Graphics.CollisionResult CheckCollisionWithSprites ( SpriteColMethod spriteChecker )
        {
            foreach (Sprite spriteA in spriteChecker.colSprites)
            {
                foreach (Sprite spriteB in this.colSprites)
                {
                    CollisionResult result = Sprite.IntersectPixels( spriteA, spriteB );
                    if (result.IsCollided)
                        return result;
                }
            }
            return new CollisionResult( false );
        }

        public GameBase.Graphics.CollisionResult CheckCollisionWithBorder ( BorderMethod Border )
        {
            CollisionResult temp = Border.CheckCollisionWithSprites( this );
            return new CollisionResult( temp.IsCollided, temp.InterPos, -temp.NormalVector );
        }

        #endregion

        public SpriteColMethod ( Sprite[] colSprites )
        {
            this.colSprites = colSprites;
        }
    }
   
}
