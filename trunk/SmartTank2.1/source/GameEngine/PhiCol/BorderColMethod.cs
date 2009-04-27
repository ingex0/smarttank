using System;
using System.Collections.Generic;
using System.Text;
using GameEngine.Graphics;
using Common.DataStructure;
using Microsoft.Xna.Framework;

namespace GameEngine.PhiCol
{
    /// <summary>
    /// �߽����ĳ�ͻ��ⷽ��
    /// </summary>
    public class BorderColMethod : IColMethod
    {
        Rectanglef borderRect;

        #region IColMethod ��Ա

        /// <summary>
        /// �������һ�����Ƿ��ͻ
        /// </summary>
        /// <param name="colB"></param>
        /// <returns></returns>
        public CollisionResult CheckCollision( IColMethod colB )
        {
            return colB.CheckCollisionWithBorder( this );
        }

        /// <summary>
        /// ����뾫������Ƿ��ͻ
        /// </summary>
        /// <param name="spriteChecker"></param>
        /// <returns></returns>
        public CollisionResult CheckCollisionWithSprites( SpriteColMethod spriteChecker )
        {

            foreach (Sprite sprite in spriteChecker.ColSprites)
            {
                CollisionResult result = sprite.CheckOutBorder( borderRect );
                if (result.IsCollided)
                {
                    float originX = result.NormalVector.X;
                    float originY = result.NormalVector.Y;

                    float x, y;

                    if (Math.Abs( originX ) > 0.5)
                        x = 1;
                    else
                        x = 0;
                    if (Math.Abs( originY ) > 0.5)
                        y = 1;
                    else
                        y = 0;

                    x *= Math.Sign( originX );
                    y *= Math.Sign( originY );


                    return new CollisionResult( result.InterPos, new Vector2( x, y ) );
                }
            }
            return new CollisionResult( false );
        }

        /// <summary>
        /// �����߽�����Ƿ��ͻ���÷�����Ч
        /// </summary>
        /// <param name="Border"></param>
        /// <returns></returns>
        public CollisionResult CheckCollisionWithBorder( BorderColMethod Border )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borderRect">�߽����</param>
        public BorderColMethod( Rectanglef borderRect )
        {
            this.borderRect = borderRect;
        }

    }

    /// <summary>
    /// �߽����ĳ�ͻ�����
    /// </summary>
    public class BorderColChecker : IColChecker
    {
        BorderColMethod method;

        #region IColChecker ��Ա

        /// <summary>
        /// 
        /// </summary>
        public IColMethod CollideMethod
        {
            get { return method; }
        }

        /// <summary>
        /// ������ײ���պ���
        /// </summary>
        /// <param name="result"></param>
        /// <param name="objB"></param>
        public void HandleCollision( CollisionResult result, ICollideObj objA, ICollideObj objB )
        {
        }

        
        /// <summary>
        /// �����ص����պ���
        /// </summary>
        /// <param name="result"></param>
        /// <param name="objB"></param>
        public void HandleOverlap( CollisionResult result, ICollideObj objA, ICollideObj objB )
        {
        }

        /// <summary>
        /// ������һ������״̬���պ���
        /// </summary>
        public void ClearNextStatus()
        {

        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borderRect">�߽����</param>
        public BorderColChecker( Rectanglef borderRect )
        {
            method = new BorderColMethod( borderRect );
        }


    }
}
