using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameEngine.Graphics;

namespace GameEngine.Effects
{
    /// <summary>
    /// 从多幅图中建立动画的动画类
    /// </summary>
    public class AnimatedSpriteSeries : AnimatedSprite, IDisposable
    {
        Vector2 pos;

        /// <summary>
        /// 提前将贴图文件导入到Content当中，以避免读取造成游戏的停滞。
        /// 资源名由固定的头名字和名字后的阿拉伯数字索引组成
        /// </summary>
        /// <param name="contentMgr">资源管理者</param>
        /// <param name="assetHead">资源的路径以及头名字</param>
        /// <param name="firstNo">索引开始的数字</param>
        /// <param name="sumFrame">系列贴图文件的数量</param>
        static public void LoadResource( ContentManager contentMgr, string assetHead, int firstNo, int sumFrame )
        {
            try
            {
                for (int i = 0; i < sumFrame; i++)
                {
                    contentMgr.Load<Texture2D>( assetHead + (firstNo + i) );
                }
            }
            catch (Exception)
            {
                throw new Exception( "导入动画资源错误，请检查图片资源是否完整" );
            }
        }

        #region Variables

        Sprite[] mSprites;

        bool alreadyLoad = false;

        /// <summary>
        /// 获取当前帧的Sprite对象
        /// </summary>
        public Sprite CurSprite
        {
            get { return mSprites[mCurFrameIndex]; }
        }

        /// <summary>
        /// 获取动画每一帧的Sprite对象
        /// </summary>
        public Sprite[] Sprites
        {
            get { return mSprites; }
        }

        #endregion

        /// <summary>
        /// 从多幅图中建立动画的动画类
        /// </summary>
        /// <param name="engine">渲染组件</param>
        public AnimatedSpriteSeries ( RenderEngine engine )
        {

        }

        #region Load Textures

        /// <summary>
        /// 从文件中导入系列贴图资源。
        /// 资源名由固定的头名字和名字后的阿拉伯数字索引组成。
        /// </summary>
        /// <param name="engine">渲染组件</param>
        /// <param name="path">贴图资源的路径</param>
        /// <param name="fileHeadName">头名字</param>
        /// <param name="extension">扩展名</param>
        /// <param name="firstNo">第一个数字索引</param>
        /// <param name="sumFrame">索引总数</param>
        /// <param name="supportInterDect">是否添加冲突检测的支持</param>
        public void LoadSeriesFromFiles ( RenderEngine engine, string path, string fileHeadName, string extension, int firstNo, int sumFrame, bool supportInterDect )
        {
            if (alreadyLoad)
                throw new Exception( "重复导入动画资源。" );

            alreadyLoad = true;

            mSprites = new Sprite[sumFrame];
            try
            {
                for (int i = 0; i < sumFrame; i++)
                {
                    mSprites[i] = new Sprite( engine );
                    mSprites[i].LoadTextureFromFile( Path.Combine( path, fileHeadName + (i + firstNo) + extension ), supportInterDect );
                }
            }
            catch (Exception)
            {
                mSprites = null;
                alreadyLoad = false;

                throw new Exception( "导入动画资源错误，请检查图片资源是否完整" );
            }

            mSumFrame = sumFrame;
            mCurFrameIndex = 0;
        }

        /// <summary>
        /// 使用素材管道导入贴图文件。
        /// 资源名由固定的头名字和名字后的阿拉伯数字索引组成
        /// </summary>
        /// <param name="engine">渲染组件</param>
        /// <param name="contentMgr">素材管理者</param>
        /// <param name="assetHead">资源的路径以及头名字</param>
        /// <param name="firstNo">索引开始的数字</param>
        /// <param name="sumFrame">系列贴图文件的数量</param>
        /// <param name="supportInterDect">是否提供冲突检测的支持</param>
        public void LoadSeriesFormContent ( RenderEngine engine, ContentManager contentMgr, string assetHead, int firstNo, int sumFrame, bool supportInterDect )
        {
            if (alreadyLoad)
                throw new Exception( "重复导入动画资源。" );

            alreadyLoad = true;

            mSprites = new Sprite[sumFrame];

            try
            {
                for (int i = 0; i < sumFrame; i++)
                {
                    mSprites[i] = new Sprite( engine );
                    mSprites[i].LoadTextureFromContent( contentMgr, assetHead + (firstNo + i), supportInterDect );
                }
            }
            catch (Exception)
            {
                mSprites = null;
                alreadyLoad = false;

                throw new Exception( "导入动画资源错误，请检查图片资源是否完整" );
            }

            mSumFrame = sumFrame;
            mCurFrameIndex = 0;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose ()
        {
            foreach (Sprite sprite in mSprites)
            {
                sprite.Dispose();
            }
        }

        #endregion

        #region Set Sprites Parameters

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="pos"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="rata"></param>
        /// <param name="color"></param>
        /// <param name="layerDepth"></param>
        /// <param name="blendMode"></param>
        public void SetSpritesParameters ( Vector2 origin, Vector2 pos, float width, float height, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            this.pos = pos;
            foreach (Sprite sprite in mSprites)
            {
                sprite.SetParameters( origin, pos, width, height, rata, color, layerDepth, blendMode );
            }
        }

        /// <summary>
        /// 设置所有Sprite的参数
        /// </summary>
        /// <param name="origin">贴图的中心，以贴图坐标表示</param>
        /// <param name="pos">逻辑位置</param>
        /// <param name="scale">贴图的缩放比（= 逻辑长度/贴图长度）</param>
        /// <param name="rata">逻辑方位角</param>
        /// <param name="color">绘制颜色</param>
        /// <param name="layerDepth">深度，1为最低层，0为最表层</param>
        /// <param name="blendMode">采用的混合模式</param>
        public void SetSpritesParameters ( Vector2 origin, Vector2 pos, float scale, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            foreach (Sprite sprite in mSprites)
            {
                sprite.SetParameters( origin, pos, scale, rata, color, layerDepth, blendMode );
            }
        }

        #endregion

        #region Draw Current Frame

        /// <summary>
        /// 绘制当前的贴图
        /// </summary>
        protected override void DrawCurFrame()
        {
            CurSprite.Draw();
        }

        #endregion



        public override Vector2 Pos
        {
            get { return pos; }
        }
    }
}
