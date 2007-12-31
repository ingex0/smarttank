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
    /// 从一副包含多个子图的图片中建立动画
    /// </summary>
    public class AnimatedSpriteSingle : AnimatedSprite, IDisposable
    {
        /// <summary>
        ///  提前将贴图文件导入到Content当中，以避免读取造成游戏的停滞。
        /// </summary>
        /// <param name="contentMgr">资源管理者</param>
        /// <param name="assetName">资源的名称</param>
        public static void LoadResources( ContentManager contentMgr, string assetName )
        {
            try
            {
                contentMgr.Load<Texture2D>( assetName );
            }
            catch (Exception)
            {
                Log.Write( "导入动画资源错误，请检查图片资源路径: " + assetName );
                throw new Exception( "导入动画资源错误，请检查图片资源路径" );
            }
        }


        #region Fields

        RenderEngine engine;

        Texture2D tex;

        Rectangle[] sourceRectangles;

        bool alreadyLoad = false;

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
        /// <summary>
        /// 方位角
        /// </summary>
        public float Rata;
        /// <summary>
        /// 颜色
        /// </summary>
        public Color Color;
        /// <summary>
        /// 绘制深度
        /// </summary>
        public float LayerDepth;
        /// <summary>
        /// 混合模式
        /// </summary>
        public SpriteBlendMode BlendMode;

        Rectangle mDestiRect;

        int cellWidth;
        int cellHeight;

        #endregion

        /// <summary>
        /// 从一副包含多个子图的图片中建立动画
        /// </summary>
        /// <param name="engine">渲染组件</param>
        public AnimatedSpriteSingle( RenderEngine engine )
        {
            this.engine = engine;
        }

        /// <summary>
        /// 通过素材管道导入图片资源
        /// </summary>
        /// <param name="contentMgr">素材管理者</param>
        /// <param name="assetName">素材名称</param>
        /// <param name="cellWidth">一个子图的宽度</param>
        /// <param name="cellHeight">一个子图的高度</param>
        /// <param name="cellInterval">表示相邻帧之间子图的间隔，只使用能被cellInterval整除的索引的子图</param>
        public void LoadFromContent( ContentManager contentMgr, string assetName, int cellWidth, int cellHeight, int cellInterval )
        {
            if (alreadyLoad)
                throw new Exception( "重复导入动画资源。" );

            alreadyLoad = true;

            try
            {
                tex = contentMgr.Load<Texture2D>( assetName );
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
        }

        /// <summary>
        /// 从文件中导入贴图
        /// </summary>
        /// <param name="filePath">贴图文件路径</param>
        /// <param name="cellWidth">子图宽度</param>
        /// <param name="cellHeight">子图高度</param>
        /// <param name="cellInterval">表示相邻帧之间子图的间隔，只使用能被cellInterval整除的索引的子图</param>
        public void LoadFromFile( string filePath, int cellWidth, int cellHeight, int cellInterval )
        {
            if (alreadyLoad)
                throw new Exception( "重复导入动画资源。" );

            alreadyLoad = true;

            try
            {
                tex = Texture2D.FromFile( engine.Device, filePath );
            }
            catch (Exception)
            {
                tex = null;
                alreadyLoad = false;

                throw new Exception( "导入动画资源错误，请检查图片资源路径" );
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
        /// 设置绘制参数
        /// </summary>
        /// <param name="origin">贴图的中心，以贴图坐标表示</param>
        /// <param name="pos">逻辑位置</param>
        /// <param name="width">逻辑宽度</param>
        /// <param name="height">逻辑高度</param>
        /// <param name="rata">方位角</param>
        /// <param name="color">绘制颜色</param>
        /// <param name="layerDepth">绘制深度</param>
        /// <param name="blendMode">混合模式</param>
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
        /// 设置绘制参数
        /// </summary>
        /// <param name="origin">贴图的中心，以贴图坐标表示</param>
        /// <param name="pos">逻辑位置</param>
        /// <param name="scale">逻辑大小/原图大小</param>
        /// <param name="rata">方位角</param>
        /// <param name="color">绘制颜色</param>
        /// <param name="layerDepth">绘制深度</param>
        /// <param name="blendMode">混合模式</param>
        public void SetParameters( Vector2 origin, Vector2 pos, float scale, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            SetParameters( origin, pos, (float)(cellWidth) * scale, (float)(cellHeight) * scale, rata, color, layerDepth, blendMode );
        }

        /// <summary>
        /// 绘制当前帧
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

        #region IDisposable 成员

        /// <summary>
        /// 释放图片资源
        /// </summary>
        public void Dispose()
        {
            tex.Dispose();
        }

        #endregion


    }
}
