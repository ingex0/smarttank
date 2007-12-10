using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WeifenLuo.WinFormsUI.Docking;
using Microsoft.Xna.Framework.Graphics;
using GameBase;
using GameBase.Graphics;

namespace MapEditer
{
    public partial class MapEditer : Form
    {
        ObjPropertyPanel objPropertyPanel;
        GroupPanel groupPanel;
        ObjDisplayPanel objDisplayPanel;
        BackGroundPanel backGroundPanel;

        MainScreen mainScreen;
        SpriteBatch mainScrnBatch;


        GraphicsDevice mainDevice;

        public MapEditer ()
        {
            InitializeComponent();
            InitializeChildForm();
            InitializeMainScrn();
        }

        private void InitializeChildForm ()
        {
            objPropertyPanel = new ObjPropertyPanel();
            groupPanel = new GroupPanel();
            objDisplayPanel = new ObjDisplayPanel();
            backGroundPanel = new BackGroundPanel();

            objPropertyPanel.Show( dockPanel, DockState.DockRightAutoHide );
            groupPanel.Show( dockPanel, DockState.DockLeftAutoHide );
            objDisplayPanel.Show( dockPanel, DockState.DockLeftAutoHide );
            backGroundPanel.Show( dockPanel, DockState.DockRightAutoHide );

        }

        private void toolStripButtonShowGroupPanel_Click ( object sender, EventArgs e )
        {
            groupPanel.Show( dockPanel, DockState.DockLeftAutoHide );
        }

        private void toolStripButtonShowBackGroundPanel_Click ( object sender, EventArgs e )
        {
            backGroundPanel.Show( dockPanel, DockState.DockRightAutoHide );
        }

        private void toolStripButtonShowObjCreatePanel_Click ( object sender, EventArgs e )
        {
            objDisplayPanel.Show( dockPanel, DockState.DockLeftAutoHide );
        }

        private void toolStripButtonShowObjPropertyPanel_Click ( object sender, EventArgs e )
        {
            objPropertyPanel.Show( dockPanel, DockState.DockRightAutoHide );
        }

        private void InitializeMainScrn ()
        {
            mainScreen = new MainScreen();

            PresentationParameters pp = new PresentationParameters();
            pp.BackBufferCount = 1;
            pp.IsFullScreen = false;
            pp.SwapEffect = SwapEffect.Discard;
            pp.BackBufferWidth = mainScreen.screen.Width;
            pp.BackBufferHeight = mainScreen.screen.Height;

            pp.AutoDepthStencilFormat = DepthFormat.Depth24Stencil8;
            pp.EnableAutoDepthStencil = true;
            pp.PresentationInterval = PresentInterval.Default;
            pp.BackBufferFormat = SurfaceFormat.Unknown;
            pp.MultiSampleType = MultiSampleType.None;

            mainDevice = new GraphicsDevice( GraphicsAdapter.DefaultAdapter,
                DeviceType.Hardware, this.mainScreen.screen.Handle,
                CreateOptions.HardwareVertexProcessing,
                pp );

            mainScreen.SizeChanged += new EventHandler( mainScreen_SizeChanged );
            mainScreen.screen.Paint += new EventHandler( mainScreen_Paint );

            mainDevice.Reset();

            mainScreen.Show( dockPanel, DockState.Document );

            mainScrnBatch = new SpriteBatch( mainDevice );

            BasicGraphics.Initial( mainDevice );
        }

        void mainScreen_Paint ( object sender, EventArgs e )
        {
            mainDevice.Clear( Microsoft.Xna.Framework.Graphics.Color.Blue );

            Coordin.SetScreenViewRect( new Microsoft.Xna.Framework.Rectangle( 0, 0, mainScreen.screen.Width, mainScreen.screen.Height ) );
            Coordin.SetCamera( 1, new Microsoft.Xna.Framework.Vector2( 100, 100 ), 0 );

            Sprite.alphaSprite = mainScrnBatch;
            mainScrnBatch.Begin();

            BasicGraphics.DrawLine( new Microsoft.Xna.Framework.Vector2( 100, 100 ), new Microsoft.Xna.Framework.Vector2( 0, 0 ), 3, Microsoft.Xna.Framework.Graphics.Color.Red, 0f );

            mainScrnBatch.End();
            mainDevice.Present();
        }

        void mainScreen_SizeChanged ( object sender, EventArgs e )
        {
            if (this.WindowState == FormWindowState.Minimized)
                return;

            MainDeviceReset();
        }

        private void MainDeviceReset ()
        {
            mainDevice.PresentationParameters.BackBufferWidth = this.mainScreen.screen.Width;
            mainDevice.PresentationParameters.BackBufferHeight = this.mainScreen.screen.Height;
            mainDevice.Reset();
        }
    }
}