using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankEngine2D.Helpers;
using Microsoft.Xna.Framework.Content;

namespace TankEngine2D.Graphics
{
    /// <summary>
    /// ��һ�����������ͼ��ͼƬ�н�������
    /// </summary>
    public class AnimatedSpriteSingle : AnimatedSprite, IDisposable
    {
        /// <summary>
        ///  ��ǰ����ͼ�ļ����뵽Content���У��Ա����ȡ�����Ϸ��ͣ�͡�
        /// </summary>
        /// <param name="contentMgr">��Դ������</param>
        /// <param name="assetName">��Դ������</param>
        public static void LoadResources( ContentManager contentMgr, string assetName )
        {
            try
            {
                contentMgr.Load<Texture2D>( assetName );
            }
            catch (Exception)
            {
                Log.Write( "���붯����Դ��������ͼƬ��Դ·��: " + assetName );
                throw new Exception( "���붯����Դ��������ͼƬ��Դ·��" );
            }
        }


        #region Fields

        RenderEngine engine;

        Texture2D tex;

        Rectangle[] sourceRectangles;

        bool alreadyLoad = false;

        /// <summary>
        /// ��ͼ�����ģ�����ͼ�����ʾ
        /// </summary>
        public Vector2 Origin;
        /// <summary>
        /// �߼�λ��
        /// </summary>
        public Vector2 Pos;
        /// <summary>
        /// �߼����
        /// </summary>
        public float Width;
        /// <summary>
        /// �߼��߶�
        /// </summary>
        public float Height;
        /// <summary>
        /// ��λ��
        /// </summary>
        public float Rata;
        /// <summary>
        /// ��ɫ
        /// </summary>
        public Color Color;
        /// <summary>
        /// �������
        /// </summary>
        public float LayerDepth;
        /// <summary>
        /// ���ģʽ
        /// </summary>
        public SpriteBlendMode BlendMode;

        Rectangle mDestiRect;

        int cellWidth;
        int cellHeight;

        #endregion

        /// <summary>
        /// ��һ�����������ͼ��ͼƬ�н�������
        /// </summary>
        /// <param name="engine">��Ⱦ���</param>
        public AnimatedSpriteSingle( RenderEngine engine )
        {
            this.engine = engine;
        }

        /// <summary>
        /// ͨ���زĹܵ�����ͼƬ��Դ
        /// </summary>
        /// <param name="contentMgr">�زĹ�����</param>
        /// <param name="assetName">�ز�����</param>
        /// <param name="cellWidth">һ����ͼ�Ŀ��</param>
        /// <param name="cellHeight">һ����ͼ�ĸ߶�</param>
        /// <param name="cellInterval">��ʾ����֮֡����ͼ�ļ����ֻʹ���ܱ�cellInterval��������������ͼ</param>
        public void LoadFromContent( ContentManager contentMgr, string assetName, int cellWidth, int cellHeight, int cellInterval )
        {
            if (alreadyLoad)
                throw new Exception( "�ظ����붯����Դ��" );

            alreadyLoad = true;

            try
            {
                tex = contentMgr.Load<Texture2D>( assetName );
            }
            catch (Exception)
            {
                tex = null;
                alreadyLoad = false;

                throw new Exception( "���붯����Դ��������ͼƬ��Դ·��" );
            }

            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;

            BuildSourceRect( cellInterval );
        }

        /// <summary>
        /// ���ļ��е�����ͼ
        /// </summary>
        /// <param name="filePath">��ͼ�ļ�·��</param>
        /// <param name="cellWidth">��ͼ���</param>
        /// <param name="cellHeight">��ͼ�߶�</param>
        /// <param name="cellInterval">��ʾ����֮֡����ͼ�ļ����ֻʹ���ܱ�cellInterval��������������ͼ</param>
        public void LoadFromFile( string filePath, int cellWidth, int cellHeight, int cellInterval )
        {
            if (alreadyLoad)
                throw new Exception( "�ظ����붯����Դ��" );

            alreadyLoad = true;

            try
            {
                tex = Texture2D.FromFile( engine.Device, filePath );
            }
            catch (Exception)
            {
                tex = null;
                alreadyLoad = false;

                throw new Exception( "���붯����Դ��������ͼƬ��Դ·��" );
            }
        }

        private void BuildSourceRect( int cellInterval )
        {
            int curX = 0;
            int curY = 0;



            List<Rectangle> result = new List<Rectangle>();

            int curCell = 0;

            while (curY + cellHeight <= tex.Height)
            {
                while (curX + cellWidth <= tex.Width)
                {
                    if (curCell % cellInterval == 0)
                        result.Add( new Rectangle( curX, curY, cellWidth, cellHeight ) );

                    curCell++;
                    curX += cellWidth;
                }

                curY += cellHeight;
                curX = 0;
            }

            sourceRectangles = result.ToArray();

            mSumFrame = result.Count;
        }

        /// <summary>
        /// ���û��Ʋ���
        /// </summary>
        /// <param name="origin">��ͼ�����ģ�����ͼ�����ʾ</param>
        /// <param name="pos">�߼�λ��</param>
        /// <param name="width">�߼����</param>
        /// <param name="height">�߼��߶�</param>
        /// <param name="rata">��λ��</param>
        /// <param name="color">������ɫ</param>
        /// <param name="layerDepth">�������</param>
        /// <param name="blendMode">���ģʽ</param>
        public void SetParameters( Vector2 origin, Vector2 pos, float width, float height, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
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
        /// ���û��Ʋ���
        /// </summary>
        /// <param name="origin">��ͼ�����ģ�����ͼ�����ʾ</param>
        /// <param name="pos">�߼�λ��</param>
        /// <param name="scale">�߼���С/ԭͼ��С</param>
        /// <param name="rata">��λ��</param>
        /// <param name="color">������ɫ</param>
        /// <param name="layerDepth">�������</param>
        /// <param name="blendMode">���ģʽ</param>
        public void SetParameters( Vector2 origin, Vector2 pos, float scale, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            SetParameters( origin, pos, (float)(cellWidth) * scale, (float)(cellHeight) * scale, rata, color, layerDepth, blendMode );
        }

        /// <summary>
        /// ���Ƶ�ǰ֡
        /// </summary>
        protected override void Draw()
        {

            CalDestinRect();

            if (BlendMode == SpriteBlendMode.Additive)
            {
                engine.SpriteMgr.additiveSprite.Draw( tex, mDestiRect, sourceRectangles[mCurFrameIndex], Color, Rata, Origin, SpriteEffects.None, LayerDepth );
            }
            else if (BlendMode == SpriteBlendMode.AlphaBlend)
            {
                engine.SpriteMgr.alphaSprite.Draw( tex, mDestiRect, sourceRectangles[mCurFrameIndex], Color, Rata, Origin, SpriteEffects.None, LayerDepth );
            }
        }

        private void CalDestinRect()
        {
            Vector2 scrnPos = engine.CoordinMgr.ScreenPos( new Vector2( Pos.X, Pos.Y ) );
            mDestiRect.X = (int)scrnPos.X;
            mDestiRect.Y = (int)scrnPos.Y;
            mDestiRect.Width = engine.CoordinMgr.ScrnLength( Width );
            mDestiRect.Height = engine.CoordinMgr.ScrnLength( Height );
        }

        #region IDisposable ��Ա

        /// <summary>
        /// �ͷ�ͼƬ��Դ
        /// </summary>
        public void Dispose()
        {
            tex.Dispose();
        }

        #endregion


    }
}
