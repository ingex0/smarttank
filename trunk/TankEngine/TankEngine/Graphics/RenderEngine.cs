using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TankEngine2D.Graphics;
using Microsoft.Xna.Framework.Graphics;
using TankEngine2D.Helpers;
using Microsoft.Xna.Framework.Content;

namespace TankEngine2D
{
    /// <summary>
    /// 绘制模块，支持以下功能：
    ///     多视口的绘制；
    ///     自由平移缩放旋转的摄像机；
    ///     像素级的精灵冲突检测；
    ///     精灵动画绘制与切帧方案；
    ///     中文字体绘制。
    /// </summary>
    public class RenderEngine
    {
        internal static readonly string contentPath =
            Path.Combine( System.Environment.CurrentDirectory, "EngineContents" );

        GraphicsDevice device;

        CoordinMgr coordinMgr;
        SpriteMgr spriteMgr;
        BasicGraphics basicGraphics;
        FontMgr fontMgr;
        AnimatedMgr animatedMgr;

        /// <summary>
        /// 获得图形设备
        /// </summary>
        public GraphicsDevice Device
        {
            get { return device; }
        }
        /// <summary>
        /// 获得坐标管理者
        /// </summary>
        public CoordinMgr CoordinMgr
        {
            get { return coordinMgr; }
        }
        /// <summary>
        /// 获得基础图形绘制者
        /// </summary>
        public BasicGraphics BasicGrahpics
        {
            get { return basicGraphics; }
        }
        /// <summary>
        /// 获得贴图管理者
        /// </summary>
        public SpriteMgr SpriteMgr
        {
            get { return spriteMgr; }
        }
        /// <summary>
        /// 获得文字管理者
        /// </summary>
        public FontMgr FontMgr
        {
            get { return fontMgr; }
        }
        /// <summary>
        /// 获得动画管理者
        /// </summary>
        public AnimatedMgr AnimatedMgr
        {
            get { return animatedMgr; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device">图形设备</param>
        /// <param name="contentMgr">素材管理者</param>
        public RenderEngine ( GraphicsDevice device, ContentManager contentMgr )
        {
            if (device == null || contentMgr == null)
                throw new NullReferenceException();

            this.device = device;
            this.coordinMgr = new CoordinMgr();
            this.spriteMgr = new SpriteMgr( this );
            this.basicGraphics = new BasicGraphics( this );
            this.fontMgr = new FontMgr( this, contentMgr );
            this.animatedMgr = new AnimatedMgr();

            Log.Initialize();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device">图形设备</param>
        public RenderEngine ( GraphicsDevice device )
            : this( device, null )
        {
        }

        /// <summary>
        /// 开始绘制
        /// </summary>
        public void BeginRender ()
        {
            this.spriteMgr.SpriteBatchBegin();
        }

        /// <summary>
        /// 结束绘制
        /// </summary>
        public void EndRender ()
        {
            this.spriteMgr.SpriteBatchEnd();
        }
    }
}
