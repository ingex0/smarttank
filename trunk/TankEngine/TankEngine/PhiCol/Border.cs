using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Graphics;
using Microsoft.Xna.Framework;
using TankEngine2D.DataStructure;
using Microsoft.Xna.Framework.Graphics;

namespace TankEngine2D.PhiCol
{
    /// <summary>
    /// 场景边界
    /// </summary>
    public class Border : ICollideObj
    {
        Rectanglef borderRect;
        BorderChecker colChecker;

        /// <summary>
        /// 获得场景边界的范围
        /// </summary>
        public Rectanglef BorderRect
        {
            get { return borderRect; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minX">边界的最小X坐标</param>
        /// <param name="minY">边界的最小Y坐标</param>
        /// <param name="maxX">边界的最大X坐标</param>
        /// <param name="maxY">边界的最大Y坐标</param>
        public Border ( float minX, float minY, float maxX, float maxY )
        {
            borderRect = new Rectanglef( minX, minY, maxX - minX, maxY - minY );
            colChecker = new BorderChecker( borderRect );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borderRect">边界矩形</param>
        public Border ( Rectanglef borderRect )
        {
            this.borderRect = borderRect;
            colChecker = new BorderChecker( borderRect );
        }

        #region ICollider 成员

        /// <summary>
        /// 获得冲突检查者
        /// </summary>
        public IColChecker ColChecker
        {
            get { return (IColChecker)colChecker; }
        }

        #endregion
    }

    /// <summary>
    /// 边界对象的冲突检测方法
    /// </summary>
    public class BorderMethod : IColMethod
    {
        Rectanglef borderRect;

        #region IColMethod 成员

        /// <summary>
        /// 检测与另一对象是否冲突
        /// </summary>
        /// <param name="colB"></param>
        /// <returns></returns>
        public CollisionResult CheckCollision ( IColMethod colB )
        {
            return colB.CheckCollisionWithBorder( this );
        }

        /// <summary>
        /// 检测与精灵对象是否冲突
        /// </summary>
        /// <param name="spriteChecker"></param>
        /// <returns></returns>
        public CollisionResult CheckCollisionWithSprites ( SpriteColMethod spriteChecker )
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
        public CollisionResult CheckCollisionWithBorder ( BorderMethod Border )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borderRect">边界矩形</param>
        public BorderMethod ( Rectanglef borderRect )
        {
            this.borderRect = borderRect;
        }
    }

    /// <summary>
    /// 边界对象的冲突检查者
    /// </summary>
    public class BorderChecker : IColChecker
    {
        BorderMethod method;

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
        public void HandleCollision ( CollisionResult result, ICollideObj objB )
        {

        }

        /// <summary>
        /// 处理重叠，空函数
        /// </summary>
        /// <param name="result"></param>
        /// <param name="objB"></param>
        public void HandleOverlap ( CollisionResult result, ICollideObj objB )
        {

        }

        /// <summary>
        /// 撤销下一个物理状态，空函数
        /// </summary>
        public void ClearNextStatus ()
        {

        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borderRect">边界矩形</param>
        public BorderChecker ( Rectanglef borderRect )
        {
            method = new BorderMethod( borderRect );
        }

    }
}
