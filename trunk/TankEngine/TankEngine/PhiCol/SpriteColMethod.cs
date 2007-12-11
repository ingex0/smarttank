using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Graphics;

namespace TankEngine2D.PhiCol
{
    /// <summary>
    /// Ϊ�����ṩ����ײ��ⷽ��
    /// </summary>
    public class SpriteColMethod : IColMethod
    {
        Sprite[] colSprites;

        /// <summary>
        /// ������ڳ�ͻ���ľ���
        /// </summary>
        public Sprite[] ColSprites
        {
            get { return colSprites; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="colSprites">���ڳ�ͻ���ľ���</param>
        public SpriteColMethod ( Sprite[] colSprites )
        {
            this.colSprites = colSprites;
        }

        #region IColMethod ��Ա

        /// <summary>
        /// �������һ�����Ƿ��ͻ
        /// </summary>
        /// <param name="colB"></param>
        /// <returns></returns>
        public CollisionResult CheckCollision ( IColMethod colB )
        {
            return colB.CheckCollisionWithSprites( this );
        }

        /// <summary>
        /// ����뾫������Ƿ��ͻ
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
        /// �����߽�����Ƿ��ͻ
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
