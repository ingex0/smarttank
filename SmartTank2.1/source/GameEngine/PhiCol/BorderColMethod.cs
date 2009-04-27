using System;
using System.Collections.Generic;
using System.Text;
using GameEngine.Graphics;
using Common.DataStructure;
using Microsoft.Xna.Framework;

namespace GameEngine.PhiCol
{
    /// <summary>
    /// 边界对象的冲突检测方法
    /// </summary>
    public class BorderColMethod : IColMethod
    {
        Rectanglef borderRect;

        #region IColMethod 成员

        /// <summary>
        /// 检测与另一对象是否冲突
        /// </summary>
        /// <param name="colB"></param>
        /// <returns></returns>
        public CollisionResult CheckCollision( IColMethod colB )
        {
            return colB.CheckCollisionWithBorder( this );
        }

        /// <summary>
        /// 检测与精灵对象是否冲突
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
        /// 检测与边界对象是否冲突，该方法无效
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
        /// <param name="borderRect">边界矩形</param>
        public BorderColMethod( Rectanglef borderRect )
        {
            this.borderRect = borderRect;
        }

    }

    /// <summary>
    /// 边界对象的冲突检查者
    /// </summary>
    public class BorderColChecker : IColChecker
    {
        BorderColMethod method;

        #region IColChecker 成员

        /// <summary>
        /// 
        /// </summary>
        public IColMethod CollideMethod
        {
            get { return method; }
        }

        /// <summary>
        /// 处理碰撞，空函数
        /// </summary>
        /// <param name="result"></param>
        /// <param name="objB"></param>
        public void HandleCollision( CollisionResult result, ICollideObj objA, ICollideObj objB )
        {
        }

        
        /// <summary>
        /// 处理重叠，空函数
        /// </summary>
        /// <param name="result"></param>
        /// <param name="objB"></param>
        public void HandleOverlap( CollisionResult result, ICollideObj objA, ICollideObj objB )
        {
        }

        /// <summary>
        /// 撤销下一个物理状态，空函数
        /// </summary>
        public void ClearNextStatus()
        {

        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borderRect">边界矩形</param>
        public BorderColChecker( Rectanglef borderRect )
        {
            method = new BorderColMethod( borderRect );
        }


    }
}
