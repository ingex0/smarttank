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
using SmartTank.Scene;
using System.IO;

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

        GraphicsDevice displayDevice;
        SpriteBatch displayBatch;

        SceneMgr curScene;

        public MapEditer ()
        {
            InitializeComponent();
            InitializeChildForm();
            InitializeMainScrn();
            InitializeDisplayScrn();
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

        #region MainScrn

        private void InitializeMainScrn ()
        {
            mainScreen = new MainScreen();

            PresentationParameters pp = new PresentationParameters();
            pp.BackBufferCount = 1;
            pp.IsFullScreen = false;
            pp.SwapEffect = SwapEffect.Discard;
            pp.BackBufferWidth = mainScreen.canvas.Width;
            pp.BackBufferHeight = mainScreen.canvas.Height;

            pp.AutoDepthStencilFormat = DepthFormat.Depth24Stencil8;
            pp.EnableAutoDepthStencil = true;
            pp.PresentationInterval = PresentInterval.Default;
            pp.BackBufferFormat = SurfaceFormat.Unknown;
            pp.MultiSampleType = MultiSampleType.None;

            mainDevice = new GraphicsDevice( GraphicsAdapter.DefaultAdapter,
                DeviceType.Hardware, this.mainScreen.canvas.Handle,
                CreateOptions.HardwareVertexProcessing,
                pp );

            mainScreen.canvas.SizeChanged += new EventHandler( mainScreen_canvas_SizeChanged );
            mainScreen.canvas.Paint += new EventHandler( mainScreen_canvas_Paint );

            mainDevice.Reset();

            mainScreen.Show( dockPanel, DockState.Document );

            mainScrnBatch = new SpriteBatch( mainDevice );

            //BasicGraphics.Initial( mainDevice );
        }

        void mainScreen_canvas_Paint ( object sender, EventArgs e )
        {
            mainDevice.Clear( Microsoft.Xna.Framework.Graphics.Color.Blue );

            //Coordin.SetScreenViewRect( new Microsoft.Xna.Framework.Rectangle( 0, 0, mainScreen.canvas.Width, mainScreen.canvas.Height ) );
            //Coordin.SetCamera( 1, new Microsoft.Xna.Framework.Vector2( 100, 100 ), 0 );

            //Sprite.alphaSprite = mainScrnBatch;
            //mainScrnBatch.Begin();

            //BasicGraphics.DrawLine( new Microsoft.Xna.Framework.Vector2( 100, 100 ), new Microsoft.Xna.Framework.Vector2( 0, 0 ), 3, Microsoft.Xna.Framework.Graphics.Color.Red, 0f );

            //mainScrnBatch.End();
            mainDevice.Present();
        }

        void mainScreen_canvas_SizeChanged ( object sender, EventArgs e )
        {
            MainDeviceReset();
        }

        private void MainDeviceReset ()
        {
            if (this.WindowState == FormWindowState.Minimized ||
                this.mainScreen.canvas.Width == 0 ||
                this.mainScreen.canvas.Height == 0)
                return;
            mainDevice.PresentationParameters.BackBufferWidth = this.mainScreen.canvas.Width;
            mainDevice.PresentationParameters.BackBufferHeight = this.mainScreen.canvas.Height;
            mainDevice.Reset();
        }

        #endregion

        #region DisplayScrn

        private void InitializeDisplayScrn ()
        {
            PresentationParameters pp = new PresentationParameters();
            pp.BackBufferCount = 1;
            pp.IsFullScreen = false;
            pp.SwapEffect = SwapEffect.Discard;
            pp.BackBufferWidth = objDisplayPanel.canvas.Width;
            pp.BackBufferHeight = objDisplayPanel.canvas.Height;

            pp.AutoDepthStencilFormat = DepthFormat.Depth24Stencil8;
            pp.EnableAutoDepthStencil = true;
            pp.PresentationInterval = PresentInterval.Default;
            pp.BackBufferFormat = SurfaceFormat.Unknown;
            pp.MultiSampleType = MultiSampleType.None;

            displayDevice = new GraphicsDevice( GraphicsAdapter.DefaultAdapter,
                DeviceType.Hardware, objDisplayPanel.canvas.Handle,
                CreateOptions.HardwareVertexProcessing, pp );

            objDisplayPanel.canvas.SizeChanged += new EventHandler( objDisplay_canvas_SizeChanged );
            objDisplayPanel.canvas.Paint += new EventHandler( objDisplayPanel_canvas_Paint );

            displayBatch = new SpriteBatch( displayDevice );
        }

        void objDisplayPanel_canvas_Paint ( object sender, EventArgs e )
        {
            displayDevice.Clear( Microsoft.Xna.Framework.Graphics.Color.YellowGreen );

            //Sprite.alphaSprite = displayBatch;
            //displayBatch.Begin();

            //BasicGraphics.DrawLineInScrn( new Microsoft.Xna.Framework.Vector2( 0, 0 ), new Microsoft.Xna.Framework.Vector2( 50, 100 ), 3, Microsoft.Xna.Framework.Graphics.Color.White, 0f );

            //displayBatch.End();
            displayDevice.Present();
        }

        void objDisplay_canvas_SizeChanged ( object sender, EventArgs e )
        {
            DisplayDeviceReset();
        }

        private void DisplayDeviceReset ()
        {
            if (this.WindowState == FormWindowState.Minimized ||
                this.objDisplayPanel.canvas.Width == 0 ||
                this.objDisplayPanel.canvas.Height == 0)
                return;

            displayDevice.PresentationParameters.BackBufferWidth = this.objDisplayPanel.canvas.Width;
            displayDevice.PresentationParameters.BackBufferHeight = this.objDisplayPanel.canvas.Height;
            displayDevice.Reset();
        }


        #endregion

        private void MenuItemNewScene_Click ( object sender, EventArgs e )
        {
            //if (curScene != null) // —ØŒ  «∑Ò±£¥Ê
            curScene = new SceneMgr();
        }

        private void MenuItemOpenScene_Click ( object sender, EventArgs e )
        {
            DialogResult result = openSceneDialog.ShowDialog( this );
            if (result == DialogResult.OK)
            {
                curScene = SceneMgr.Load( openSceneDialog.FileName );
            }
        }

        private void MenuItemSaveScene_Click ( object sender, EventArgs e )
        {
            if (curScene == null)
                return;

            DialogResult result = saveSceneDialog.ShowDialog( this );
            if (result == DialogResult.OK)
            {
                Stream file =File.Create(saveSceneDialog.FileName);
                SceneMgr.Save( file, curScene );
            }
        }



    }
}