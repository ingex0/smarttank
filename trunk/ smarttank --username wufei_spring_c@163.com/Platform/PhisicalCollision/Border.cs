using System;
using System.Collections.Generic;
using System.Text;
using Platform.PhisicalCollision;
using Platform.GameObjects;
using GameBase.Graphics;
using Microsoft.Xna.Framework;
using GameBase;
using GameBase.DataStructure;
using Microsoft.Xna.Framework.Graphics;

namespace Platform.PhisicalCollision
{
    [Serializable]
    public class Border : ICollideObj
    {
        Rectanglef borderRect;
        BorderChecker colChecker;

        GameObjInfo objInfo = new GameObjInfo( "Border", "Border" );

        public Rectanglef BorderRect
        {
            get { return borderRect; }
        }

        public Border ( float minX, float maxX, float minY, float maxY )
        {
            borderRect = new Rectanglef( minX, minY, maxX - minX, maxY - minY );
            colChecker = new BorderChecker( borderRect );
        }

        #region ICollider 成员

        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        public IColChecker ColChecker
        {
            get { return (IColChecker)colChecker; }
        }

        #endregion

        #region IDrawable 成员

        public void Draw ( Color color )
        {
            BasicGraphics.DrawRectangle( borderRect, 3, color, 0f );
        }

        #endregion

        #region IHasBorderObj 成员

        public Matrix WorldTrans
        {
            get { throw new Exception( "The method or operation is not implemented." ); }
        }

        public Rectanglef BoundingBox
        {
            get { throw new Exception( "The method or operation is not implemented." ); }
        }

        CircleList<GameBase.Graphics.Border> IHasBorderObj.BorderData
        {
            get { throw new Exception( "The method or operation is not implemented." ); }
        }

        #endregion
    }

    [Serializable]
    public class BorderMethod : IColMethod
    {
        Rectanglef borderRect;

        #region IColMethod 成员

        public CollisionResult CheckCollision ( IColMethod colB )
        {
            return colB.CheckCollisionWithBorder( this );
        }

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

        public CollisionResult CheckCollisionWithBorder ( BorderMethod Border )
        {
            throw new Exception( "The method or operation is not implemented." );
        }

        #endregion

        public BorderMethod ( Rectanglef borderRect )
        {
            this.borderRect = borderRect;
        }
    }

    [Serializable]
    public class BorderChecker : IColChecker
    {
        BorderMethod method;

        #region IColChecker 成员

        public IColMethod CollideMethod
        {
            get { return method; }
        }

        public void HandleCollision ( CollisionResult result, GameObjInfo objB )
        {
            
        }

        public void HandleOverlap ( CollisionResult result, GameObjInfo objB )
        {

        }

        public void ClearNextStatus ()
        {
            
        }

        #endregion

        public BorderChecker ( Rectanglef borderRect )
        {
            method = new BorderMethod( borderRect );
        }

        #region IColChecker 成员


        

        #endregion
    }
}
