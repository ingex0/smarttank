using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Graphics;
using Microsoft.Xna.Framework;
using TankEngine2D.DataStructure;
using Microsoft.Xna.Framework.Graphics;
using SmartTank.GameObjs;

namespace SmartTank.PhiCol
{
    /// <summary>
    /// �����߽�
    /// </summary>
    public class Border : ICollideObj, IGameObj
    {
        Rectanglef borderRect;
        BorderChecker colChecker;

        GameObjInfo objInfo;

        /// <summary>
        /// ��ó����߽�ķ�Χ
        /// </summary>
        public Rectanglef BorderRect
        {
            get { return borderRect; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minX">�߽����СX����</param>
        /// <param name="minY">�߽����СY����</param>
        /// <param name="maxX">�߽�����X����</param>
        /// <param name="maxY">�߽�����Y����</param>
        public Border( float minX, float minY, float maxX, float maxY )
            : this( new Rectanglef( minX, minY, maxX - minX, maxY - minY ) )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borderRect">�߽����</param>
        public Border( Rectanglef borderRect )
        {
            this.borderRect = borderRect;
            colChecker = new BorderChecker( borderRect );
            objInfo = new GameObjInfo( "Border", "" );
        }

        #region ICollider ��Ա

        /// <summary>
        /// ��ó�ͻ�����
        /// </summary>
        public IColChecker ColChecker
        {
            get { return (IColChecker)colChecker; }
        }

        #endregion

        #region IGameObj ��Ա

        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        public Vector2 Pos
        {
            get
            {
                return new Vector2( borderRect.X + borderRect.Width * 0.5f, borderRect.Y + borderRect.Height * 0.5f );
            }
            set
            {

            }
        }

        public float Azi
        {
            get { return 0; }
        }

        #endregion

        #region IUpdater ��Ա

        public void Update( float seconds )
        {

        }

        #endregion

        #region IDrawableObj ��Ա

        public void Draw()
        {

        }

        #endregion



        #region IGameObj ��Ա

        public string Name
        {
            get { return "Border"; }
        }

        #endregion
    }

    /// <summary>
    /// �߽����ĳ�ͻ��ⷽ��
    /// </summary>
    public class BorderMethod : IColMethod
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
        public CollisionResult CheckCollisionWithBorder( BorderMethod Border )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borderRect">�߽����</param>
        public BorderMethod( Rectanglef borderRect )
        {
            this.borderRect = borderRect;
        }

    }

    /// <summary>
    /// �߽����ĳ�ͻ�����
    /// </summary>
    public class BorderChecker : IColChecker
    {
        BorderMethod method;

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
        public void HandleCollision( CollisionResult result, ICollideObj objB )
        {

        }

        /// <summary>
        /// �����ص����պ���
        /// </summary>
        /// <param name="result"></param>
        /// <param name="objB"></param>
        public void HandleOverlap( CollisionResult result, ICollideObj objB )
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
        public BorderChecker( Rectanglef borderRect )
        {
            method = new BorderMethod( borderRect );
        }

    }
}