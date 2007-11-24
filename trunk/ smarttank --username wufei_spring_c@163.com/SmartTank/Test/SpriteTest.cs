using System;
using System.Collections.Generic;
using System.Text;
using GameBase;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameBase.Graphics;
using GameBase.DataStructure;
namespace SmartTank.Test
{
    class SpriteTest : BaseGame
    {
        Sprite sprite1;
        Sprite sprite2;
        Sprite point;

        Rectanglef Border = new Rectanglef( 50, 50, 300, 200 );

        Vector2 NormalVector;

        bool ScreenRed = false;

        static SpriteTest test = new SpriteTest();

        protected override void Initialize ()
        {
            this.IsMouseVisible = true;
            Coordin.SetScreenViewRect( new Rectangle( 0, 0, ClientRec.Width, ClientRec.Height ) );
            //Coordin.SetLogicViewRect( new Rectangle( 0, 0, 400, 300 ) );
            Coordin.SetCamera( 2, new Vector2( 200, 150 ), MathHelper.PiOver4 );
            sprite1 = new Sprite();
            sprite2 = new Sprite();
            point = new Sprite();
            base.Initialize();
        }

        protected override void LoadGraphicsContent ( bool loadAllContent )
        {
            base.LoadGraphicsContent( loadAllContent );

            sprite1.LoadTextureFromContent( "Test\\turret", 10 );
            sprite1.SetParameters( new Vector2( 41, 50 ), new Vector2( 100, 100 ), 1f, 0f, Color.White, 0.5f, SpriteBlendMode.AlphaBlend );

            sprite2.LoadTextureFromContent( "Test\\hucSmart", true );
            sprite2.SetParameters( new Vector2( 41, 50 ), new Vector2( 200, 150 ), 1f, 0f, Color.White, 1f, SpriteBlendMode.AlphaBlend );

            point.LoadTextureFromContent( "Test\\point", false );
            point.SetParameters( new Vector2( 5, 5 ), new Vector2( 0, 0 ), 2, 2, 0, Color.White, 0, SpriteBlendMode.AlphaBlend );

        }

        protected override void Update ( GameTime gameTime )
        {
            MouseState MS = Mouse.GetState();
            if (MS.LeftButton == ButtonState.Pressed)
            {
                //sprite1.Pos.X = Coordin.LogicX( MS.X );
                //sprite1.Pos.Y = Coordin.LogicY( MS.Y );
                sprite1.Pos = Coordin.LogicPos( new Vector2( MS.X, MS.Y ) );
            }
            if (MS.RightButton == ButtonState.Pressed)
            {
                sprite1.Rata += 0.01f;
            }
            //for (int i = 0; i < 500; i++)
            //{
            sprite1.UpdateTransformBounding();
            sprite2.UpdateTransformBounding();

            CollisionResult result = Sprite.IntersectPixels( sprite1, sprite2 );
            if (result.IsCollided)
            {
                ScreenRed = true;
                point.Pos = result.InterPos;
                NormalVector = result.NormalVector * 10;
            }
            else
            {
                result = sprite1.CheckOutBorder( Border );
                if (result.IsCollided)
                {
                    ScreenRed = true;
                    point.Pos = result.InterPos;
                    NormalVector = result.NormalVector * 10;
                }
                else
                    ScreenRed = false;
            }
            //}



            base.Update( gameTime );
        }

        protected override void GameDraw ( GameTime gameTime )
        {
            if (ScreenRed)
                Device.Clear( Color.Red );
            else
                Device.Clear( Color.Blue );

            sprite2.Draw();
            sprite1.Draw();
            point.Draw();
            BasicGraphics.DrawLine( point.Pos, point.Pos + NormalVector * 5, 3f, Color.Blue, 0f );


            BasicGraphics.DrawRectangle( Border, 3, Color.Yellow, 0f );
        }
    }
}
