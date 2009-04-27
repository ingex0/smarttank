using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using GameEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Common.Helpers;
using Microsoft.Xna.Framework.Content;

namespace GameEngine
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
        internal readonly string contentPath;

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
        /// <param name="contentPath">����������Դ·��</param>
        public RenderEngine( GraphicsDevice device, ContentManager contentMgr, string contentPath )
        {
            if (device == null)
                throw new NullReferenceException();

            this.contentPath = contentPath;
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
        /// <param name="contentPath">����������Դ·��</param>
        public RenderEngine( GraphicsDevice device, string contentPath )
            : this( device, null, contentPath )
        {
        }

        /// <summary>
        /// ��ʼ����
        /// </summary>
        public void BeginRender()
        {
            this.spriteMgr.SpriteBatchBegin();
        }

        /// <summary>
        /// ��������
        /// </summary>
        public void EndRender()
        {
            this.spriteMgr.SpriteBatchEnd();
        }
    }
}
