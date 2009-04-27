//=================================================
// xWinForms
// Copyright ?2007 by Eric Grossinger
// http://psycad007.spaces.live.com/
//=================================================
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Common.Helpers;
using GameEngine.Graphics;
using GameEngine.Input;
using GameEngine.Draw;

namespace GameEngine.UI
{
    public class TextButton : Control
    {
        #region Variables
        Point fontSize = new Point( 8, 15 );

        Texture2D buttonTexture;
        Rectangle[] sourceRect = new Rectangle[3];
        Rectangle[] destRect = new Rectangle[3];

        Rectangle buttonArea = Rectangle.Empty;

        public event EventHandler OnClick;
        private void onPress( object obj, EventArgs e ) { }

        bool bMouseOver = false;
        bool bPressed = false;

        public Color color = Color.White;
        public int width = 0;

        #endregion

        #region Construction

        public TextButton( string name, Vector2 position, string text, Nullable<int> width, /*Font font,*/ Color color )//, Style style)
        {
            this.Type = ControlType.TextButton;
            this.name = name;
            this.position = position;
            this.text = text;
            //this.font = font;
            this.color = color;
            if (width.HasValue)
                this.width = width.Value;
            OnClick += new EventHandler( onPress );

            Init();//style );
        }

        #endregion

        #region Initial
        private void Init( /*Style style*/ )
        {
            buttonTexture = BaseGame.ContentMgr.Load<Texture2D>( Path.Combine( Directories.UIContent, "button" ) );

            sourceRect[0] = new Rectangle( 0, 0, (int)(buttonTexture.Width * 0.1f), buttonTexture.Height );
            sourceRect[1] = new Rectangle( (int)(buttonTexture.Width * 0.1f), 0, buttonTexture.Width - (int)(buttonTexture.Width * 0.2f), buttonTexture.Height );
            sourceRect[2] = new Rectangle( buttonTexture.Width - (int)(buttonTexture.Width * 0.1f), 0, (int)(buttonTexture.Width * 0.1f), buttonTexture.Height );

            destRect[0] = sourceRect[0];
            destRect[1] = sourceRect[1];
            destRect[2] = sourceRect[2];

            if (width == 0)
                destRect[1].Width = text.Length * fontSize.X + 10;
            else if (width > destRect[0].Width + destRect[1].Width + destRect[2].Width)
                destRect[1].Width = width - (destRect[0].Width + destRect[2].Width);

            buttonArea.Width = destRect[0].Width + destRect[1].Width + destRect[2].Width;
            buttonArea.Height = destRect[0].Height;
        }
        #endregion

        #region Update
        public override void Update()//Vector2 formPosition, Vector2 formSize )
        {
            //base.Update( formPosition, formSize );

            destRect[0].X = (int)position.X;
            destRect[0].Y = (int)position.Y;

            destRect[1].X = (int)position.X + destRect[0].Width;
            destRect[1].Y = (int)position.Y;

            destRect[2].X = (int)position.X + destRect[0].Width + destRect[1].Width;
            destRect[2].Y = (int)position.Y;

            if (benable)
                CheckMouseState();

            //CheckVisibility( formPosition, formSize );
        }

        private void CheckMouseState()
        {

            buttonArea.X = (int)position.X;
            buttonArea.Y = (int)position.Y;

            if (buttonArea.Contains( InputHandler.CurMousePos ))
            {
                bMouseOver = true;
                if (InputHandler.MouseJustReleaseLeft)
                {
                    if (!bPressed)
                    {
                        bPressed = true;
                        OnClick( this, EventArgs.Empty );
                    }
                }
                else if (bPressed)
                    bPressed = false;
            }
            else if (bMouseOver)
                bMouseOver = false;

        }

        //private void CheckVisibility ( Vector2 formPosition, Vector2 formSize )
        //{
        //    if (position.X + buttonArea.Width > formPosition.X + formSize.X - 15f)
        //        bVisible = false;
        //    else if (position.Y + buttonArea.Height > formPosition.Y + formSize.Y - 25f)
        //        bVisible = false;
        //    else
        //        bVisible = true;
        //} 
        #endregion

        #region Draw

        public override void Draw( SpriteBatch spriteBatch, float alpha )
        {
            Color dynamicColor = Color.White;

            if (!bMouseOver && !bPressed)
                dynamicColor = new Color( new Vector4( color.ToVector3().X * 0.95f, color.ToVector3().Y * 0.95f, color.ToVector3().Z * 0.95f, alpha ) );
            else if (bPressed)
                dynamicColor = new Color( new Vector4( color.ToVector3().X * 0.9f, color.ToVector3().Y * 0.9f, color.ToVector3().Z * 0.9f, alpha ) );
            else if (bMouseOver)
                dynamicColor = new Color( new Vector4( color.ToVector3().X * 1.5f, color.ToVector3().Y * 1.5f, color.ToVector3().Z * 1.5f, alpha ) );

            spriteBatch.Draw( buttonTexture, destRect[0], sourceRect[0], dynamicColor, 0, Vector2.Zero, SpriteEffects.None, LayerDepth.UI );
            spriteBatch.Draw( buttonTexture, destRect[1], sourceRect[1], dynamicColor, 0, Vector2.Zero, SpriteEffects.None, LayerDepth.UI );
            spriteBatch.Draw( buttonTexture, destRect[2], sourceRect[2], dynamicColor, 0, Vector2.Zero, SpriteEffects.None, LayerDepth.UI );

            Color dynamicTextColor = new Color( new Vector4( 0f, 0f, 0f, alpha ) );

            int spacing = (destRect[1].Width - text.Length * fontSize.X) / 2;
            BaseGame.FontMgr.DrawInScrnCoord( text, new Vector2( destRect[0].X + destRect[0].Width + spacing, destRect[0].Y + 3f ), Control.fontScale, dynamicTextColor, LayerDepth.Text, Control.fontName );
        }
        #endregion
    }
}
