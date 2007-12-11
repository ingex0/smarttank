#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using TankEngine2D;
using TankEngine2D.Graphics;
using TankEngine2D.PhiCol;
using TankEngine2D.DataStructure;
using TankEngine2D.Helpers;
#endregion

namespace TestTankEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;

        RenderEngine engine;

        Sprite sprite;

        public Game1 ()
        {
            graphics = new GraphicsDeviceManager( this );
            content = new ContentManager( Services );
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize ()
        {
            // TODO: Add your initialization logic here
            engine = new RenderEngine( graphics.GraphicsDevice, content );

            engine.CoordinMgr.SetScreenViewRect( new Rectangle( 0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height ) );
            engine.CoordinMgr.SetCamera( 2f, new Vector2( 200, 150 ), 0 );

            sprite = new Sprite( engine );
            sprite.LoadTextureFromContent( content, "scorpion", true );
            sprite.SetParameters( new Vector2( 0, 0 ), new Vector2( 200, 150 ), 1f, 0f, Color.Yellow, 0.4f, SpriteBlendMode.AlphaBlend );

            base.Initialize();
        }


        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent ( bool loadAllContent )
        {
            if (loadAllContent)
            {
                // TODO: Load any ResourceManagementMode.Automatic content
            }

            // TODO: Load any ResourceManagementMode.Manual content
        }


        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadGraphicsContent ( bool unloadAllContent )
        {
            if (unloadAllContent)
            {
                // TODO: Unload any ResourceManagementMode.Automatic content
                content.Unload();
            }

            // TODO: Unload any ResourceManagementMode.Manual content
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update ( GameTime gameTime )
        {
            // Allows the game to exit
            if (GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            KeyboardState KS = Keyboard.GetState();
            if (KS.IsKeyDown( Keys.R ))
                engine.CoordinMgr.Rota += 0.1f;
            if (KS.IsKeyDown( Keys.Right ))
                engine.CoordinMgr.MoveCamera( new Vector2( 5f, 0 ) );
            if (KS.IsKeyDown( Keys.Up ))
                engine.CoordinMgr.MoveCamera( new Vector2( 0, -5f ) );
            if (KS.IsKeyDown( Keys.Left ))
                engine.CoordinMgr.MoveCamera( new Vector2( -5f, 0 ) );
            if (KS.IsKeyDown( Keys.Down ))
                engine.CoordinMgr.MoveCamera( new Vector2( 0, 5f ) );

            base.Update( gameTime );
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw ( GameTime gameTime )
        {
            graphics.GraphicsDevice.Clear( Color.CornflowerBlue );

            // TODO: Add your drawing code here
            engine.BeginRender();
            engine.BasicGrahpics.DrawLine( new Vector2( 100, 50 ), new Vector2( 200, 150 ), 3f, Color.Red, 0.2f );
            engine.BasicGrahpics.DrawRectangle( new Rectangle( 200, 150, 100, 100 ), 1f, Color.Yellow, 0.2f );
            engine.BasicGrahpics.FillRectangle( new Rectangle( 200, 150, -100, -100 ), Color.White, 0.3f );
            sprite.Draw();

            engine.FontMgr.Draw( "Œ‚∑…", new Vector2( 100, 100 ), 1f, Color.Black, 0f, "HDZB" );
            engine.FontMgr.DrawInScrnCoord( "wufei", new Vector2( 200, 100 ), 1f, Color.Brown, 0f, "Comic" );

            engine.EndRender();
            base.Draw( gameTime );
        }
    }
}
