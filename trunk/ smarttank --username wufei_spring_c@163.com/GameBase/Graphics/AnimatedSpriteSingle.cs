using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Helpers;

namespace GameBase.Graphics
{
    public class AnimatedSpriteSingle : AnimatedSprite, IDisposable
    {
        public static void LoadResources ( string assetName )
        {
            try
            {
                BaseGame.Content.Load<Texture2D>( assetName );
            }
            catch (Exception)
            {
                Log.Write( "导入动画资源错误，请检查图片资源路径: " + assetName );
                throw new Exception( "导入动画资源错误，请检查图片资源路径" );
            }
        }

        Texture2D tex;

        Rectangle[] sourceRectangles;

        bool alreadyLoad = false;

        public Vector2 Origin;
        public Vector2 Pos;
        public float Width;
        public float Height;
        public float Rata;
        public Color Color;
        public float LayerDepth;
        public SpriteBlendMode BlendMode;

        Rectangle mDestiRect;

        int cellWidth;
        int cellHeight;


        public void LoadFromContent ( string assetName, int cellWidth, int cellHeight, int cellInterval )
        {
            if (alreadyLoad)
                throw new Exception( "重复导入动画资源。" );

            alreadyLoad = true;

            try
            {
                tex = BaseGame.Content.Load<Texture2D>( assetName );
            }
            catch (Exception)
            {
                tex = null;
                alreadyLoad = false;

                throw new Exception( "导入动画资源错误，请检查图片资源路径" );
            }

            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;

            BuildSourceRect( cellInterval );

            AnimatedManager.Add( this );
        }

        private void BuildSourceRect ( int cellInterval )
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
        /// 设置参数
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="pos"></param>
        /// <param name="scale">逻辑大小/原图大小</param>
        /// <param name="rata"></param>
        /// <param name="color"></param>
        /// <param name="layerDepth"></param>
        /// <param name="blendMode"></param>
        public void SetParameters ( Vector2 origin, Vector2 pos, float scale, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            SetParameters( origin, pos, (float)(tex.Width) * scale, (float)(tex.Height) * scale, rata, color, layerDepth, blendMode );
        }

        protected override void Draw ()
        {

            CalDestinRect();

            if (BlendMode == SpriteBlendMode.Additive)
            {
                Sprite.additiveSprite.Draw( tex, mDestiRect, sourceRectangles[mCurFrameIndex], Color, Rata, Origin, SpriteEffects.None, LayerDepth );
            }
            else if (BlendMode == SpriteBlendMode.AlphaBlend)
            {
                Sprite.alphaSprite.Draw( tex, mDestiRect, sourceRectangles[mCurFrameIndex], Color, Rata, Origin, SpriteEffects.None, LayerDepth );
            }
        }

        private void CalDestinRect ()
        {
            Vector2 scrnPos = Coordin.ScreenPos( new Vector2( Pos.X, Pos.Y ) );
            mDestiRect.X = (int)scrnPos.X;
            mDestiRect.Y = (int)scrnPos.Y;
            mDestiRect.Width = Coordin.ScrnLength( cellWidth );
            mDestiRect.Height = Coordin.ScrnLength( cellHeight );
        }

        #region IDisposable 成员

        public void Dispose ()
        {
            tex.Dispose();
        }

        #endregion


    }
}
