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
    /// ����ģ�飬֧�����¹��ܣ�
    ///     ���ӿڵĻ��ƣ�
    ///     ����ƽ��������ת���������
    ///     ���ؼ��ľ����ͻ��⣻
    ///     ���鶯����������֡������
    ///     ����������ơ�
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
        /// ���ͼ���豸
        /// </summary>
        public GraphicsDevice Device
        {
            get { return device; }
        }
        /// <summary>
        /// ������������
        /// </summary>
        public CoordinMgr CoordinMgr
        {
            get { return coordinMgr; }
        }
        /// <summary>
        /// ��û���ͼ�λ�����
        /// </summary>
        public BasicGraphics BasicGrahpics
        {
            get { return basicGraphics; }
        }
        /// <summary>
        /// �����ͼ������
        /// </summary>
        public SpriteMgr SpriteMgr
        {
            get { return spriteMgr; }
        }
        /// <summary>
        /// ������ֹ�����
        /// </summary>
        public FontMgr FontMgr
        {
            get { return fontMgr; }
        }
        /// <summary>
        /// ��ö���������
        /// </summary>
        public AnimatedMgr AnimatedMgr
        {
            get { return animatedMgr; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device">ͼ���豸</param>
        /// <param name="contentMgr">�زĹ�����</param>
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
        /// <param name="device">ͼ���豸</param>
        public RenderEngine ( GraphicsDevice device )
            : this( device, null )
        {
        }

        /// <summary>
        /// ��ʼ����
        /// </summary>
        public void BeginRender ()
        {
            this.spriteMgr.SpriteBatchBegin();
        }

        /// <summary>
        /// ��������
        /// </summary>
        public void EndRender ()
        {
            this.spriteMgr.SpriteBatchEnd();
        }
    }
}
