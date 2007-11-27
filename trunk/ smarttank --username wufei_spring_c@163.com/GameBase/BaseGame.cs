using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameBase;
using GameBase.Helpers;
using GameBase.Graphics;
using GameBase.Input;
using GameBase.Effects;

namespace GameBase
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

        #endregion

        #region Properties

        public static GraphicsDevice Device
        {
            get { return graphics.GraphicsDevice; }
        }

        public static ContentManager Content
        {
            get { return content; }
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

        public BaseGame ()
        {
            graphics = new GraphicsDeviceManager( this );
            clientRec = Window.ClientBounds;
            curInstance = this;
            //graphics.ToggleFullScreen();
        }

        void HandleDeviceReset ( object sender, EventArgs e )
        {
            Sprite.HandleDeviceReset();
            FontManager.HandleDeviceReset();
        }

        #endregion

        #region Initialize

        protected override void Initialize ()
        {
            content = new ContentManager( Services );
            Sprite.Intial();
            FontManager.Initial();
            ChineseWriter.Intitial();
            BasicGraphics.Initial();
            RandomHelper.GenerateNewRandomGenerator();
            Log.Initialize();
            base.Initialize();
        }
        #endregion

        #region Update

        protected override void Update ( GameTime gameTime )
        {
            curTime = gameTime;

            InputHandler.Update();
            if (showFps)
                CalulateFPS( gameTime );

            //waitUpdate = false;

            base.Update( gameTime );
        }

        private void CalulateFPS ( GameTime gameTime )
        {
            Fps = (int)((double)1000 / gameTime.ElapsedRealTime.TotalMilliseconds);
        }

        #endregion

        #region Draw

        protected override sealed void Draw ( GameTime gameTime )
        {
            try
            {
                Device.Clear( Color.Blue );

                Quake.Update();

                Sprite.SpriteBatchBegin();
                FontManager.SpriteBatchBegin();

                GameDraw( gameTime );

                if (showFps)
                    FontManager.DrawInScrnCoord( "FPS = " + Fps.ToString(), new Vector2( 30, 15 ), 0.5f, Color.White, 0f, FontType.Comic );

                TextEffect.Draw();

                Sprite.SpriteBatchEnd();
                FontManager.SpriteBatchEnd();


                base.Draw( gameTime );
            }
            catch (Exception)
            {
                HandleDeviceReset( this, EventArgs.Empty );
            }
        }

        protected virtual void GameDraw ( GameTime gameTime )
        {

        }
        #endregion

    }
}
