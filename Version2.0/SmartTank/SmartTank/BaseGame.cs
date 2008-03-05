using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TankEngine2D.Helpers;
using TankEngine2D.Graphics;
using TankEngine2D.Input;
using TankEngine2D;
using SmartTank.Helpers;
using SmartTank.Draw;

namespace SmartTank
{
    public class BaseGame : Game
    {
        #region Variables

        protected static GraphicsDeviceManager graphics;
        protected static ContentManager content;

        protected static bool showFps = true;
        protected static int Fps = 0;

        protected static GameTime curTime;

        protected static Rectangle clientRec;

        protected static BaseGame curInstance;

        protected static RenderEngine renderEngine;

        #endregion

        #region Properties

        public static GraphicsDevice Device
        {
            get { return graphics.GraphicsDevice; }
        }

        public static ContentManager ContentMgr
        {
            get { return content; }
        }

        public static RenderEngine RenderEngine
        {
            get { return renderEngine; }
            set { renderEngine = value; }
        }

        public static CoordinMgr CoordinMgr
        {
            get
            {
                if (renderEngine != null)
                    return renderEngine.CoordinMgr;
                return null;
            }
        }

        public static FontMgr FontMgr
        {
            get
            {
                if (renderEngine != null)
                    return renderEngine.FontMgr;
                return null;
            }
        }

        public static SpriteMgr SpriteMgr
        {
            get
            {
                if (renderEngine != null)
                    return renderEngine.SpriteMgr;
                return null;
            }
        }

        public static AnimatedMgr AnimatedMgr
        {
            get
            {
                if (renderEngine != null)
                    return renderEngine.AnimatedMgr;
                return null;
            }
        }

        public static BasicGraphics BasicGraphics
        {
            get
            {
                if (renderEngine != null)
                    return renderEngine.BasicGrahpics;
                return null;
            }
        }

        public static float CurTime
        {
            get { return (float)curTime.TotalGameTime.TotalSeconds; }
        }

        public static Rectangle ClientRect
        {
            get { return clientRec; }
        }

        public static bool ShowFPS
        {
            get { return showFps; }
            set { showFps = value; }
        }

        public static bool ShowMouse
        {
            get
            {
                if (curInstance != null)
                    return curInstance.IsMouseVisible;
                else
                    return false;
            }
            set
            {
                if (curInstance != null)
                    curInstance.IsMouseVisible = value;
            }
        }

        public Rectangle ClientRec
        {
            get { return Window.ClientBounds; }
        }

        public int Width
        {
            get { return ClientRec.Width; }
        }

        public int Height
        {
            get { return ClientRec.Height; }
        }

        #endregion

        #region Constructor

        public BaseGame()
        {
            graphics = new GraphicsDeviceManager( this );
            //graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>( graphics_PreparingDeviceSettings );
            clientRec = Window.ClientBounds;
            curInstance = this;
            //graphics.ToggleFullScreen();
        }

        //void graphics_PreparingDeviceSettings( object sender, PreparingDeviceSettingsEventArgs e )
        //{
        //    e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PlatformContents;
        //    e.GraphicsDeviceInformation.PresentationParameters.MultiSampleType = MultiSampleType.None;
        //    e.GraphicsDeviceInformation.PresentationParameters.MultiSampleQuality = 0;
        //}

        void HandleDeviceReset( object sender, EventArgs e )
        {
            //Sprite.HandleDeviceReset();
            //FontManager.HandleDeviceReset();
        }

        #endregion

        #region Initialize

        protected override void Initialize()
        {
            content = new ContentManager( Services );
            renderEngine = new RenderEngine( Device, content, "Content\\EngineContent" );
            RandomHelper.GenerateNewRandomGenerator();
            Log.Initialize();
            base.Initialize();
        }
        #endregion

        #region Update

        protected override void Update( GameTime gameTime )
        {
            curTime = gameTime;

            InputHandler.Update();
            if (showFps)
                CalulateFPS( gameTime );

            //waitUpdate = false;

            base.Update( gameTime );
        }

        private void CalulateFPS( GameTime gameTime )
        {
            Fps = (int)((double)1000 / gameTime.ElapsedRealTime.TotalMilliseconds);
        }

        #endregion

        #region Draw

        protected override sealed void Draw( GameTime gameTime )
        {
            try
            {
                Device.Clear( Color.Blue );

                //Quake.Update();

                renderEngine.BeginRender();

                GameDraw( gameTime );

                if (showFps)
                    FontMgr.DrawInScrnCoord( "FPS = " + Fps.ToString(), new Vector2( 30, 15 ), 0.5f, Color.White, 0f, GameFonts.Comic );

                //TextEffect.Draw();

                renderEngine.EndRender();
                base.Draw( gameTime );
            }
            catch (Exception ex)
            {
                Log.Write( ex.Message );
            }
        }

        protected virtual void GameDraw( GameTime gameTime )
        {

        }
        #endregion

    }
}
