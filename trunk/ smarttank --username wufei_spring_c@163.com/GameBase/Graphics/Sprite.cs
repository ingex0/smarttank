using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using GameBase.Helpers;
using GameBase.DataStructure;

namespace GameBase.Graphics
{
    /// <summary>
    /// this is the class which manages all the sprite items in the gameScene.
    /// it can draw itself according to the position, rotation, scale, ect as you like.
    /// it's also able to detect collision between two sprite using pix-detection method.
    /// Notice: sourceRectangle is't supported, so you cann't load a picture with several items in it.
    /// 这个类管理游戏画面中的所有精灵。
    /// 它保存绘制的地点，旋转角，缩放比等参数，并能够按照这些参数绘制自身。
    /// 并且它具有判断两精灵是否重叠，以及该精灵是否出界的方法。使用的是像素检测的方法。
    /// 注意，该类并不支持sourceRectangle。所以不能用它导入一个拥有多个子图的图片。
    /// </summary>
    public class Sprite : IDisposable
    {
        #region Statics
        public static SpriteBatch alphaSprite;
        public static SpriteBatch additiveSprite;

        internal static void Intial ()
        {
            alphaSprite = new SpriteBatch( BaseGame.Device );
            additiveSprite = new SpriteBatch( BaseGame.Device );
        }

        internal static void HandleDeviceReset ()
        {
            Intial();
        }

        /// <summary>
        /// alphaSprite.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None );
        /// additiveSprite.Begin( SpriteBlendMode.Additive, SpriteSortMode.BackToFront, SaveStateMode.None );
        /// </summary>
        internal static void SpriteBatchBegin ()
        {
            alphaSprite.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None );
            additiveSprite.Begin( SpriteBlendMode.Additive, SpriteSortMode.BackToFront, SaveStateMode.None );
        }

        /// <summary>
        /// Sprite.alphaSprite.End();
        /// Sprite.additiveSprite.End();
        /// </summary>
        internal static void SpriteBatchEnd ()
        {
            Sprite.alphaSprite.End();
            Sprite.additiveSprite.End();
        }

        /// <summary>
        /// 一个计算碰撞法向量时用到的参数，表示计算法向量时将取碰撞处附近的多少点进行效果的平均
        /// </summary>
        public static readonly int DefaultAverageSum = 5;

        #endregion

        #region Variables

        Texture2D mTexture;

        /*
         * 逻辑上的位置和大小
         * */
        /// <summary>
        /// 贴图的中心，以贴图坐标表示
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// 逻辑位置
        /// </summary>
        public Vector2 Pos;

        /// <summary>
        /// 逻辑宽度
        /// </summary>
        public float Width;

        /// <summary>
        /// 逻辑高度
        /// </summary>
        public float Height;
        //public float Scale;

        /// <summary>
        /// 逻辑方位角
        /// </summary>
        public float Rata;

        /// <summary>
        /// 绘制颜色
        /// </summary>
        public Color Color;

        /// <summary>
        /// 深度，1为最低层，0为最表层。
        /// </summary>
        public float LayerDepth;

        /// <summary>
        /// 采用的混合模式
        /// </summary>
        public SpriteBlendMode BlendMode;

        Rectanglef mBounding;
        /// <summary>
        /// 获得包围盒，以逻辑坐标。
        /// 该属性中进行了除法运算。
        /// </summary>
        public Rectanglef BoundRect
        {
            get
            {
                //Vector2 logicPos = Coordin.LogicPos( new Vector2( mBounding.X, mBounding.Y ) );
                //return new Rectanglef( logicPos.X, logicPos.Y, Coordin.LogicLength( mBounding.Width ), Coordin.LogicLength( mBounding.Height ) );
                return mBounding;
            }
        }

        Matrix mTransform;
        /// <summary>
        /// 世界转换矩阵
        /// 每一帧需要先调用UpdateTransformBounding函数。
        /// </summary>
        public Matrix Transform
        {
            get { return mTransform; }
        }

        Rectangle mDestiRect;

        bool[] mTextureData;

        SpriteBorder mBorder;

        /// <summary>
        /// 计算遮挡所需原数据
        /// </summary>
        public CircleList<Border> BorderData
        {
            get { return mBorder.BorderCircle; }
        }

        bool mloaded = false;

        bool mSupportIntersectDect = false;

        int mAverageSum = DefaultAverageSum;

        /// <summary>
        /// 获得或设置贴图的缩放比（= 逻辑长度/贴图长度）
        /// </summary>
        public float Scale
        {
            get
            {
                return Width / (float)mTexture.Width;
            }
            set
            {
                Width = (float)mTexture.Width * Math.Max( 0, value );
                Height = (float)mTexture.Height * Math.Max( 0, value );
                CalDestinRect();
            }
        }

        #endregion

        #region Constructions

        /// <summary>
        /// 构造未导入贴图的Sprite对象
        /// </summary>
        public Sprite ()
        {
            Rata = 0f;
            Color = Color.White;
            LayerDepth = 0f;
        }

        /// <summary>
        /// 构造Sprite对象并导入贴图
        /// </summary>
        /// <param name="fromContent"></param>
        /// <param name="texturePath"></param>
        /// <param name="SupportIntersectDect"></param>
        public Sprite ( bool fromContent, string texturePath, bool SupportIntersectDect )
        {
            Rata = 0f;
            Color = Color.White;
            LayerDepth = 0f;
            if (fromContent)
            {
                LoadTextureFromContent( texturePath, SupportIntersectDect );
            }
            else
            {
                LoadTextureFromFile( texturePath, SupportIntersectDect );
            }
        }
        #endregion

        #region LoadTexture Functions

        /// <summary>
        /// Load Texture From File
        /// 从文件中导入贴图。
        /// 由于导入过程可能会导致游戏的停顿，尽量从素材管道中导入贴图
        /// </summary>
        /// <param name="texturePath">the texture Directory and File Name, without GameBaseDirectory Path.
        /// 贴图文件的相对于游戏运行文件的路径</param>
        /// <param name="SupportIntersectDect">true to add intersect dectection support.
        /// 为true时，为贴图添加冲突检测的支持</param>
        public void LoadTextureFromFile ( string texturePath, bool SupportIntersectDect )
        {
            mloaded = true;
            try
            {
                mTexture = Texture2D.FromFile( BaseGame.Device, Path.Combine( Directories.GameBaseDirectory, texturePath ) );
                if (SupportIntersectDect)
                {
                    AddIntersectSupport();
                }
            }
            catch (Exception)
            {
                mloaded = false;
                mTexture = null;
                mTextureData = null;
                //mTextureBoundData = null;
                throw new Exception( "导入贴图文件失败！" );
            }
        }

        /// <summary>
        /// Load Texture From Content
        /// 从素材管道中导入贴图
        /// </summary>
        /// <param name="assetName">the assetName.素材路径</param>
        /// <param name="SupportIntersectDect">true to add intersect Dectection support.
        /// 为true时为贴图添加冲突检测的支持</param>
        public void LoadTextureFromContent ( string assetName, bool SupportIntersectDect )
        {
            mloaded = true;
            try
            {
                mTexture = BaseGame.Content.Load<Texture2D>( assetName );
                if (SupportIntersectDect)
                {
                    AddIntersectSupport();
                }
            }
            catch (Exception)
            {
                mloaded = false;
                mTexture = null;
                mTextureData = null;
                //mTextureBoundData = null;
                throw new Exception( "导入贴图文件失败！" );
            }
        }

        /// <summary>
        /// Load Texture From File, support Intersect dectect,
        /// the AverageSum is use to modify the default averageSum,
        /// which is used in calulate the Normal vector,
        /// biger the number is, more border points will be added to the result of Normal Vector. 
        /// 从文件中导入贴图，并添加冲突检测的支持
        /// 由于导入过程可能会导致游戏的停顿，尽量从素材管道中导入贴图
        /// </summary>
        /// <param name="texturePath">贴图文件的相对于游戏运行文件的路径</param>
        /// <param name="AverageSum">使用该传入值作为AverageSum。
        /// 这是一个计算碰撞法向量时用到的参数，表示计算法向量时将取碰撞处附近的多少点进行效果的平均</param>
        public void LoadTextureFromFile ( string texturePath, int AverageSum )
        {
            mAverageSum = AverageSum;
            LoadTextureFromFile( texturePath, true );
        }

        /// <summary>
        /// Load Texture From Content, support Intersect dectect,
        /// the AverageSum is use to modify the default averageSum,
        /// which is used in calulate the Normal vector,
        /// biger the number is, more border points will be added to the result of Normal Vector. 
        /// 从素材管道中导入贴图，并添加冲突检测的支持
        /// </summary>
        /// <param name="assetName">素材路径</param>
        /// <param name="AverageSum">使用该传入值作为AverageSum。
        /// 这是一个计算碰撞法向量时用到的参数，表示计算法向量时将取碰撞处附近的多少点进行效果的平均</param>
        public void LoadTextureFromContent ( string assetName, int AverageSum )
        {
            mAverageSum = AverageSum;
            LoadTextureFromContent( assetName, true );
        }

        /// <summary>
        /// 为Sprite对象添加冲突检测的支持
        /// </summary>
        public void AddIntersectSupport ()
        {
            mSupportIntersectDect = true;

            Color[] textureData = new Color[mTexture.Width * mTexture.Height];
            mTextureData = new bool[mTexture.Width * mTexture.Height];
            mTexture.GetData( textureData );
            for (int i = 0; i < textureData.Length; i++)
            {
                if (textureData[i].A >= SpriteBorder.minBlockAlpha)
                    mTextureData[i] = true;
                else
                    mTextureData[i] = false;
            }
            mBorder = new SpriteBorder( mTexture );
        }
        #endregion

        #region Set Parameters
        /// <summary>
        /// 设置该精灵的绘制参数
        /// </summary>
        /// <param name="origin">贴图的中心，以贴图坐标表示</param>
        /// <param name="pos">逻辑位置</param>
        /// <param name="width">逻辑宽度</param>
        /// <param name="height">逻辑高度</param>
        /// <param name="rata">逻辑方位角</param>
        /// <param name="color">绘制颜色</param>
        /// <param name="layerDepth">深度，1为最低层，0为最表层</param>
        /// <param name="blendMode">采用的混合模式</param>
        public void SetParameters ( Vector2 origin, Vector2 pos, float width, float height, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            Origin = origin;
            Pos = pos;
            Width = width;
            Height = height;
            Rata = rata;
            Color = color;
            LayerDepth = layerDepth;
            BlendMode = blendMode;
        }
        /// <summary>
        /// 设置该精灵的绘制参数
        /// </summary>
        /// <param name="origin">贴图的中心，以贴图坐标表示</param>
        /// <param name="pos">逻辑位置</param>
        /// <param name="scale">贴图的缩放比（= 逻辑长度/贴图长度）</param>
        /// <param name="rata">逻辑方位角</param>
        /// <param name="color">绘制颜色</param>
        /// <param name="layerDepth">深度，1为最低层，0为最表层</param>
        /// <param name="blendMode">采用的混合模式</param>
        public void SetParameters ( Vector2 origin, Vector2 pos, float scale, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            SetParameters( origin, pos, (float)(mTexture.Width) * scale, (float)(mTexture.Height) * scale, rata, color, layerDepth, blendMode );
        }
        #endregion

        #region Update Transform Matrix and Bounding Rectangle

        /// <summary>
        /// 更新精灵的转换矩阵和包围盒
        /// </summary>
        public void UpdateTransformBounding ()
        {
            CalDestinRect();

            mTransform =
                Matrix.CreateTranslation( new Vector3( -Origin, 0f ) ) *
                Matrix.CreateScale( new Vector3( Width / (float)mTexture.Width, Height / (float)mTexture.Height, 1 ) ) *
                Matrix.CreateRotationZ( Rata ) *
                Matrix.CreateTranslation( new Vector3( Pos, 0f ) );

            mBounding = CalculateBoundingRectangle( new Rectangle( 0, 0, mTexture.Width, mTexture.Height ), mTransform );
        }
        #endregion

        #region Calculate Destination Rectangle

        private void CalDestinRect ()
        {
            Vector2 scrnPos = Coordin.ScreenPos( new Vector2( Pos.X, Pos.Y ) );
            mDestiRect.X = (int)scrnPos.X;
            mDestiRect.Y = (int)scrnPos.Y;
            mDestiRect.Width = Coordin.ScrnLength( Width );
            mDestiRect.Height = Coordin.ScrnLength( Height );
        }

        #endregion

        #region Draw Functions

        /// <summary>
        /// 绘制该精灵
        /// </summary>
        public void Draw ()
        {
            if (!mloaded) return;

            CalDestinRect();

            if (BlendMode == SpriteBlendMode.Additive)
            {
                additiveSprite.Draw( mTexture, mDestiRect, null, Color, Rata - Coordin.Rota, Origin, SpriteEffects.None, LayerDepth );
            }
            else if (BlendMode == SpriteBlendMode.AlphaBlend)
            {
                alphaSprite.Draw( mTexture, mDestiRect, null, Color, Rata - Coordin.Rota, Origin, SpriteEffects.None, LayerDepth );
            }
        }
        #endregion

        #region Intersect Detect Functions

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// 检查两个精灵是否发生碰撞
        /// </summary>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static CollisionResult IntersectPixels ( Sprite spriteA, Sprite spriteB )
        {
            if (!spriteA.mSupportIntersectDect || !spriteB.mSupportIntersectDect)
                throw new Exception( "At lest one of the two sprite doesn't support IntersectDect!" );

            spriteA.UpdateTransformBounding();
            spriteB.UpdateTransformBounding();

            CollisionResult result = new CollisionResult();

            if (!spriteA.mBounding.Intersects( spriteB.mBounding ))
            {
                result.IsCollided = false;
                return result;
            }

            int widthA = spriteA.mTexture.Width;
            int heightA = spriteA.mTexture.Height;
            int widthB = spriteB.mTexture.Width;
            int heightB = spriteB.mTexture.Height;

            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = spriteA.mTransform * Matrix.Invert( spriteB.mTransform );

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal( Vector2.UnitX, transformAToB );
            Vector2 stepY = Vector2.TransformNormal( Vector2.UnitY, transformAToB );

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 oriPosInB = Vector2.Transform( Vector2.Zero, transformAToB );

            CircleList<Border> list = spriteA.mBorder.BorderCircle;
            CircleListNode<Border> cur = list.First;

            bool justStart = true;
            bool find = false;

            CircleListNode<Border> firstNode = cur;
            int length = 0;

            #region 找出第一个相交点和该连续相交线的长度
            for (int i = 0; i < list.Length; i++)
            {
                Point bordPointA = cur.value.p;

                if (SpriteBBlockAtPos( spriteB, oriPosInB, stepX, stepY, bordPointA ))
                {
                    if (!justStart)
                    {
                        if (!find)
                        {
                            find = true;
                            firstNode = cur;
                        }
                        else
                        {
                            length++;
                        }
                    }
                    else
                    {
                        CircleListNode<Border> temp = cur.pre;
                        while (SpriteBBlockAtPos( spriteB, oriPosInB, stepX, stepY, temp.value.p ))
                        {
                            temp = temp.pre;
                        }
                        cur = temp;
                        i--;
                        justStart = false;
                    }
                }
                else
                {
                    justStart = false;

                    if (find)
                    {
                        break;
                    }
                }

                cur = cur.next;
            }
            #endregion

            if (find)
            {
                cur = firstNode;

                for (int i = 0; i < Math.Round( (float)length / 2 ); i++)
                {
                    cur = cur.next;
                }

                Point bordPointA = cur.value.p;
                result.IsCollided = true;
                Vector2 InterPos = Vector2.Transform( new Vector2( bordPointA.X, bordPointA.Y ), spriteA.mTransform );
                result.NormalVector = Vector2.Transform( spriteA.mBorder.GetNormalVector( cur, spriteA.mAverageSum ), spriteA.mTransform )
                    - Vector2.Transform( Vector2.Zero, spriteA.mTransform );
                result.NormalVector.Normalize();
                result.InterPos = InterPos;
                return result;
            }


            // No intersection found
            result.IsCollided = false;
            return result;
        }

        /// <summary>
        /// 检测是否在边界矩形外
        /// </summary>
        /// <param name="BorderRect"></param>
        /// <returns></returns>
        public CollisionResult CheckOutBorder ( Rectanglef BorderRect )
        {
            if (!this.mSupportIntersectDect)
                throw new Exception( "the sprite doesn't support IntersectDect!" );

            UpdateTransformBounding();

            CollisionResult result = new CollisionResult();
            Rectanglef screenRect = BorderRect;


            if (!this.mBounding.Intersects( screenRect ))
            {
                result.IsCollided = false;
                return result;
            }

            int widthA = this.mTexture.Width;
            int heightA = this.mTexture.Height;

            // Calculate a matrix which transforms from A's local space into
            // world space
            Matrix transformAToWorld = mTransform;

            Vector2 stepX = Vector2.TransformNormal( Vector2.UnitX, transformAToWorld );
            Vector2 stepY = Vector2.TransformNormal( Vector2.UnitY, transformAToWorld );

            Vector2 oriPosInWorld = Vector2.Transform( Vector2.Zero, transformAToWorld );


            CircleList<Border> list = mBorder.BorderCircle;
            CircleListNode<Border> cur = list.First;

            bool justStart = true;
            bool find = false;

            CircleListNode<Border> firstNode = cur;
            int length = 0;

            #region 找出第一个相交点和该连续相交线的长度
            for (int i = 0; i < list.Length; i++)
            {
                Point bordPointA = cur.value.p;

                if (PointOutBorder( oriPosInWorld, stepX, stepY, bordPointA, screenRect ))
                {
                    if (!justStart)
                    {
                        if (!find)
                        {
                            find = true;
                            firstNode = cur;
                        }
                        else
                        {
                            length++;
                        }
                    }
                    else
                    {
                        CircleListNode<Border> temp = cur.pre;

                        int leftLength = list.Length;
                        while (PointOutBorder( oriPosInWorld, stepX, stepY, temp.value.p, screenRect ) && leftLength >= 0)
                        {
                            temp = temp.pre;
                            leftLength--;
                        }
                        cur = temp;
                        i--;
                        justStart = false;
                    }
                }
                else
                {
                    justStart = false;

                    if (find)
                    {
                        break;
                    }
                }

                cur = cur.next;
            }
            #endregion

            if (find)
            {
                cur = firstNode;

                for (int i = 0; i < Math.Round( (float)length / 2 ); i++)
                {
                    cur = cur.next;
                }

                Point bordPointA = cur.value.p;
                result.IsCollided = true;
                Vector2 InterPos = Vector2.Transform( new Vector2( bordPointA.X, bordPointA.Y ), mTransform );
                result.NormalVector = Vector2.Transform( mBorder.GetNormalVector( cur, mAverageSum ), mTransform )
                    - Vector2.Transform( Vector2.Zero, mTransform );
                result.NormalVector.Normalize();
                result.InterPos = InterPos;
                return result;
            }


            // No intersection found
            result.IsCollided = false;
            return result;
        }

        private bool PointOutBorder ( Vector2 oriPosInWorld, Vector2 stepX, Vector2 stepY, Point bordPointA, Rectanglef screenRect )
        {
            Vector2 PInWorld = oriPosInWorld + bordPointA.X * stepX + bordPointA.Y * stepY;

            int xW = (int)(PInWorld.X);
            int yW = (int)(PInWorld.Y);

            if (screenRect.Contains( new Vector2( xW, yW ) ))
                return false;
            else
                return true;
        }

        private static bool SpriteBBlockAtPos ( Sprite spriteB, Vector2 oriPosInB, Vector2 stepX, Vector2 stepY, Point bordPointA )
        {
            Vector2 bPInB = oriPosInB + bordPointA.X * stepX + bordPointA.Y * stepY;

            int xB = (int)(bPInB.X);
            int yB = (int)(bPInB.Y);

            if (xB < 0 || xB >= spriteB.mTexture.Width || yB < 0 || yB >= spriteB.mTexture.Height)
                return false;
            else
                return spriteB.mTextureData[xB + yB * spriteB.mTexture.Width];
        }

        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        private static Rectanglef CalculateBoundingRectangle ( Rectangle rectangle, Matrix transform )
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2( rectangle.Left, rectangle.Top );
            Vector2 rightTop = new Vector2( rectangle.Right, rectangle.Top );
            Vector2 leftBottom = new Vector2( rectangle.Left, rectangle.Bottom );
            Vector2 rightBottom = new Vector2( rectangle.Right, rectangle.Bottom );

            // Transform all four corners into work space
            Vector2.Transform( ref leftTop, ref transform, out leftTop );
            Vector2.Transform( ref rightTop, ref transform, out rightTop );
            Vector2.Transform( ref leftBottom, ref transform, out leftBottom );
            Vector2.Transform( ref rightBottom, ref transform, out rightBottom );

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min( Vector2.Min( leftTop, rightTop ),
                                      Vector2.Min( leftBottom, rightBottom ) );
            Vector2 max = Vector2.Max( Vector2.Max( leftTop, rightTop ),
                                      Vector2.Max( leftBottom, rightBottom ) );

            // Return that as a rectangle
            return new Rectanglef( min.X, min.Y, (max.X - min.X), (max.Y - min.Y) );
        }

        #endregion

        #region Dispose

        /// <summary>
        /// 清理资源
        /// </summary>
        public void Dispose ()
        {
            if (mTexture != null)
                mTexture.Dispose();
            mTexture = null;
            mTextureData = null;
            //mTextureBoundData = null;
            mloaded = false;
        }

        #endregion
    }


}
