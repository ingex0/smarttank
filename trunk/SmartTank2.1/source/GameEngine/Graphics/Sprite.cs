using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using Common.Helpers;
using Common.DataStructure;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.Graphics
{
    /// <summary>
    /// this is the class which manages all the sprite items in the gameScene.
    /// it can draw itself according to the position, rotation, scale, ect as you like.
    /// it's also able to detect collision between two sprite using pix-detection method.
    /// Notice: sourceRectangle is't supported, so you cann't load a picture with several items in it.
    /// ����������Ϸ�����е����о��顣
    /// ��������Ƶĵص㣬��ת�ǣ����űȵȲ��������ܹ�������Щ��������������
    /// �����������ж��������Ƿ��ص����Լ��þ����Ƿ����ķ�����ʹ�õ������ؼ��ķ�����
    /// ע�⣬���ಢ��֧��sourceRectangle�����Բ�����������һ��ӵ�ж����ͼ��ͼƬ��
    /// </summary>
    public class Sprite : IDisposable
    {
        #region Statics

        /// <summary>
        /// һ�������ͻ������ʱ�õ��Ĳ�������ʾ���㷨����ʱ��ȡ��ͻ�������Ķ��ٵ����Ч����ƽ��
        /// </summary>
        public static readonly int DefaultAverageSum = 5;

        #endregion

        #region Variables

        RenderEngine engine;

        Texture2D mTexture;

        /*
         * �߼��ϵ�λ�úʹ�С
         * */
        /// <summary>
        /// ��ͼ�����ģ�����ͼ�����ʾ
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// �߼�λ��
        /// </summary>
        public Vector2 Pos;

        /// <summary>
        /// �߼�����
        /// </summary>
        public float Width;

        /// <summary>
        /// �߼��߶�
        /// </summary>
        public float Height;
        //public float Scale;

        /// <summary>
        /// �߼���λ��
        /// </summary>
        public float Rata;

        /// <summary>
        /// ������ɫ
        /// </summary>
        public Color Color;

        /// <summary>
        /// ��ȣ�1Ϊ��Ͳ㣬0Ϊ����㡣
        /// </summary>
        public float LayerDepth;

        /// <summary>
        /// ���õĻ��ģʽ
        /// </summary>
        public SpriteBlendMode BlendMode;

        Rectanglef mBounding;
        /// <summary>
        /// ��ð�Χ�У����߼����ꡣ
        /// �������н����˳������㡣
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
        /// ����ת������
        /// ÿһ֡��Ҫ�ȵ���UpdateTransformBounding������
        /// </summary>
        public Matrix Transform
        {
            get { return mTransform; }
        }

        Rectangle mDestiRect;

        bool[] mTextureData;

        SpriteBorder mBorder;

        /// <summary>
        /// �����ڵ�����ԭ����
        /// </summary>
        public CircleList<BorderPoint> BorderData
        {
            get { return mBorder.BorderCircle; }
        }

        bool mloaded = false;

        bool mSupportIntersectDect = false;

        int mAverageSum = DefaultAverageSum;

        /// <summary>
        /// ��û�������ͼ�����űȣ�= �߼�����/��ͼ���ȣ�
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
        /// ����δ������ͼ��Sprite����
        /// </summary>
        public Sprite ( RenderEngine engine )
        {
            this.engine = engine;
            Rata = 0f;
            Color = Color.White;
            LayerDepth = 0f;
        }

        /// <summary>
        /// ����Sprite���󲢵�����ͼ
        /// </summary>
        /// <param name="engine">��Ⱦ���</param>
        /// <param name="texturePath">��ͼ·��</param>
        /// <param name="SupportIntersectDect"></param>
        public Sprite ( RenderEngine engine, string texturePath, bool SupportIntersectDect )
            : this( engine )
        {
            LoadTextureFromFile( texturePath, SupportIntersectDect );
        }

        /// <summary>
        /// ����Sprite���󲢵�����ͼ
        /// </summary>
        /// <param name="engine">��Ⱦ���</param>
        /// <param name="contentMgr">�زĹ�����</param>
        /// <param name="assetPath">�ز�·��</param>
        /// <param name="SupportIntersectDect">�Ƿ����ӳ�ͻ���֧��</param>
        public Sprite ( RenderEngine engine, ContentManager contentMgr, string assetPath, bool SupportIntersectDect )
            : this( engine )
        {
            LoadTextureFromContent( contentMgr, assetPath, SupportIntersectDect );
        }

        #endregion

        #region LoadTexture Functions

        /// <summary>
        /// �����ͼ���Ƿ��ܽ����߽磬��������߽�ʧ�ܣ����׳��쳣���ɹ�ʱ�򷵻������ı߽硣
        /// �ڼ����ͼ�Ƿ��ܱ������Ľ����߽�������ʹ�á�
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="borderMap">�����߽�ɹ�ʱ���ؽ��</param>
        public static void CheckBorder ( Texture2D tex, out SpriteBorder.BorderMap borderMap )
        {
            SpriteBorder border = new SpriteBorder( tex, out borderMap );
        }

        /// <summary>
        /// Load Texture From File
        /// ���ļ��е�����ͼ��
        /// </summary>
        /// <param name="texturePath">the texture Directory and File Name, without GameBaseDirectory Path.
        /// ��ͼ�ļ����������Ϸ�����ļ���·��</param>
        /// <param name="SupportIntersectDect">true to add intersect dectection support.
        /// Ϊtrueʱ��Ϊ��ͼ���ӳ�ͻ����֧��</param>
        public void LoadTextureFromFile ( string texturePath, bool SupportIntersectDect )
        {
            mloaded = true;
            try
            {
                mTexture = Texture2D.FromFile( engine.Device, texturePath );
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
                throw new Exception( "������ͼ�ļ�ʧ�ܣ�" );
            }
        }

        /// <summary>
        /// Load Texture From Content
        /// ���زĹܵ��е�����ͼ
        /// </summary>
        /// <param name="contentMgr">�زĹ�����</param>
        /// <param name="assetName">the assetName.�ز�·��</param>
        /// <param name="SupportIntersectDect">true to add intersect Dectection support.
        /// ΪtrueʱΪ��ͼ���ӳ�ͻ����֧��</param>
        public void LoadTextureFromContent ( ContentManager contentMgr, string assetName, bool SupportIntersectDect )
        {
            mloaded = true;
            try
            {
                mTexture = contentMgr.Load<Texture2D>( assetName );
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
                throw new Exception( "������ͼ�ļ�ʧ�ܣ�" );
            }
        }

        /// <summary>
        /// Load Texture From File, support Intersect dectect,
        /// the AverageSum is use to modify the default averageSum,
        /// which is used in calulate the Normal vector,
        /// biger the number is, more border points will be added to the result of Normal Vector. 
        /// ���ļ��е�����ͼ�������ӳ�ͻ����֧��
        /// </summary>
        /// <param name="texturePath">��ͼ�ļ����������Ϸ�����ļ���·��</param>
        /// <param name="AverageSum">ʹ�øô���ֵ��ΪAverageSum��
        /// ����һ��������ײ������ʱ�õ��Ĳ�������ʾ���㷨����ʱ��ȡ��ײ�������Ķ��ٵ����Ч����ƽ��</param>
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
        /// ���زĹܵ��е�����ͼ�������ӳ�ͻ����֧��
        /// </summary>
        /// <param name="contentMgr">�زĹ�����</param>
        /// <param name="assetName">�ز�·��</param>
        /// <param name="AverageSum">ʹ�øô���ֵ��ΪAverageSum��
        /// ����һ��������ײ������ʱ�õ��Ĳ�������ʾ���㷨����ʱ��ȡ��ײ�������Ķ��ٵ����Ч����ƽ��</param>
        public void LoadTextureFromContent ( ContentManager contentMgr, string assetName, int AverageSum )
        {
            mAverageSum = AverageSum;
            LoadTextureFromContent( contentMgr, assetName, true );
        }

        /// <summary>
        /// ΪSprite�������ӳ�ͻ����֧��
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
        /// ���øþ���Ļ��Ʋ���
        /// </summary>
        /// <param name="origin">��ͼ�����ģ�����ͼ�����ʾ</param>
        /// <param name="pos">�߼�λ��</param>
        /// <param name="width">�߼�����</param>
        /// <param name="height">�߼��߶�</param>
        /// <param name="rata">�߼���λ��</param>
        /// <param name="color">������ɫ</param>
        /// <param name="layerDepth">��ȣ�1Ϊ��Ͳ㣬0Ϊ�����</param>
        /// <param name="blendMode">���õĻ��ģʽ</param>
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
        /// ���øþ���Ļ��Ʋ���
        /// </summary>
        /// <param name="origin">��ͼ�����ģ�����ͼ�����ʾ</param>
        /// <param name="pos">�߼�λ��</param>
        /// <param name="scale">��ͼ�����űȣ�= �߼�����/��ͼ���ȣ�</param>
        /// <param name="rata">�߼���λ��</param>
        /// <param name="color">������ɫ</param>
        /// <param name="layerDepth">��ȣ�1Ϊ��Ͳ㣬0Ϊ�����</param>
        /// <param name="blendMode">���õĻ��ģʽ</param>
        public void SetParameters ( Vector2 origin, Vector2 pos, float scale, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            SetParameters( origin, pos, (float)(mTexture.Width) * scale, (float)(mTexture.Height) * scale, rata, color, layerDepth, blendMode );
        }
        #endregion

        #region Update Transform Matrix and Bounding Rectangle

        /// <summary>
        /// ���¾����ת������Ͱ�Χ��
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
            Vector2 scrnPos = engine.CoordinMgr.ScreenPos( new Vector2( Pos.X, Pos.Y ) );
            mDestiRect.X = (int)scrnPos.X;
            mDestiRect.Y = (int)scrnPos.Y;
            mDestiRect.Width = engine.CoordinMgr.ScrnLength( Width );
            mDestiRect.Height = engine.CoordinMgr.ScrnLength( Height );
        }

        #endregion

        #region Draw Functions

        /// <summary>
        /// ���Ƹþ���
        /// </summary>
        public void Draw ()
        {
            if (!mloaded) return;

            CalDestinRect();

            if (BlendMode == SpriteBlendMode.Additive)
            {
                engine.SpriteMgr.additiveSprite.Draw( mTexture, mDestiRect, null, Color, Rata - engine.CoordinMgr.Rota, Origin, SpriteEffects.None, LayerDepth );
            }
            else if (BlendMode == SpriteBlendMode.AlphaBlend)
            {
                engine.SpriteMgr.alphaSprite.Draw( mTexture, mDestiRect, null, Color, Rata - engine.CoordinMgr.Rota, Origin, SpriteEffects.None, LayerDepth );
            }
        }
        #endregion

        #region Intersect Detect Functions

        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// ������������Ƿ�����ײ
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

            CircleList<BorderPoint> list = spriteA.mBorder.BorderCircle;
            CircleListNode<BorderPoint> cur = list.First;

            bool justStart = true;
            bool find = false;

            CircleListNode<BorderPoint> firstNode = cur;
            int length = 0;

            #region �ҳ���һ���ཻ��͸������ཻ�ߵĳ���
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
                        CircleListNode<BorderPoint> temp = cur.pre;
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
        /// ����Ƿ��ڱ߽������
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


            CircleList<BorderPoint> list = mBorder.BorderCircle;
            CircleListNode<BorderPoint> cur = list.First;

            bool justStart = true;
            bool find = false;

            CircleListNode<BorderPoint> firstNode = cur;
            int length = 0;

            #region �ҳ���һ���ཻ��͸������ཻ�ߵĳ���
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
                        CircleListNode<BorderPoint> temp = cur.pre;

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
        /// ������Դ
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