using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Graphics;

namespace TankEngine2D.PhiCol
{
    /// <summary>
    /// 为精灵提供的碰撞检测方法
    /// </summary>
    public class SpriteColMethod : IColMethod
    {
        Sprite[] colSprites;

        /// <summary>
        /// 获得用于冲突检测的精灵
        /// </summary>
        public Sprite[] ColSprites
        {
            get { return colSprites; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colSprites">用于冲突检测的精灵</param>
        public SpriteColMethod ( Sprite[] colSprites )
        {
            this.colSprites = colSprites;
        }

        #region IColMethod 成员

        /// <summary>
        /// 检测与另一对象是否冲突
        /// </summary>
        /// <param name="colB"></param>
        /// <returns></returns>
        public CollisionResult CheckCollision ( IColMethod colB )
        {
            return colB.CheckCollisionWithSprites( this );
        }

        /// <summary>
        /// 检测与精灵对象是否冲突
        /// </summary>
        /// <param name="spriteChecker"></param>
        /// <returns></returns>
        public CollisionResult CheckCollisionWithSprites ( SpriteColMethod spriteChecker )
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

        /// <summary>
        /// 检测与边界对象是否冲突
        /// </summary>
        /// <param name="Border"></param>
        /// <returns></returns>
        public CollisionResult CheckCollisionWithBorder ( BorderMethod Border )
        {
            CollisionResult temp = Border.CheckCollisionWithSprites( this );
            return new CollisionResult( temp.IsCollided, temp.InterPos, -temp.NormalVector );
        }

        #endregion


    }

}
