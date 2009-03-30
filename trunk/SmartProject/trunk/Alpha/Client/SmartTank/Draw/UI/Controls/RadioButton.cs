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
using TankEngine2D.Helpers;
using TankEngine2D.Graphics;
using SmartTank.Helpers;

namespace SmartTank.Draw.UI.Controls
{
    public class RadioButton : Control
    {
        #region Fields
        Texture2D[] texture;

        //int charWidth = 9;

        MouseState ms;

        public bool bPressed = false;
        public bool bMouseOver = false;

        public event EventHandler OnPress;
        public void checkbox_onPress( Object obj, EventArgs e ) { bPressed = true; bChecked = !bChecked; }
        #endregion

        #region Consturtion
        public RadioButton( string name, Vector2 position, string text, bool bChecked )//, Font font, Form.Style style)
        {
            //Console.WriteLine("New Radio Button");

            this.Type = ControlType.RadioButton;
            this.name = name;
            this.position = position;
            this.text = text;
            this.bChecked = bChecked;

            texture = new Texture2D[2];
            texture[0] = BaseGame.ContentMgr.Load<Texture2D>( Path.Combine( Directories.UIContent, "radio_button" ) );
            texture[1] = BaseGame.ContentMgr.Load<Texture2D>( Path.Combine( Directories.UIContent, "radio_button_checked" ) );

            OnPress += new EventHandler( checkbox_onPress );
        }
        #endregion

        #region Update
        public override void Update()//Vector2 formPosition, Vector2 formSize )
        {
            //position = origin + formPosition;

            //CheckVisibility( formPosition, formSize );

            //if (!Form.isInUse)
            CheckSelection();
        }

        //private void CheckVisibility ( Vector2 formPosition, Vector2 formSize )
        //{
        //    if (position.X + texture[0].Width + charWidth * text.Length > formPosition.X + formSize.X - 15f)
        //        bVisible = false;
        //    else if (position.Y + texture[0].Height > formPosition.Y + formSize.Y - 25f)
        //        bVisible = false;
        //    else
        //        bVisible = true;
        //}

        private void CheckSelection()
        {
            ms = Mouse.GetState();
            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (!bPressed && bIsOverButton())
                    OnPress( this, EventArgs.Empty );
            }
            else if (bPressed)
                bPressed = false;
            else
            {
                if (!bMouseOver && bIsOverButton())
                    bMouseOver = true;
                else if (bMouseOver && !bIsOverButton())
                    bMouseOver = false;
            }
        }

        private bool bIsOverButton()
        {
            if (ms.X > position.X && ms.X < position.X + texture[0].Width)
                if (ms.Y > position.Y && ms.Y < position.Y + texture[0].Height)
                    return true;
                else
                    return false;
            else
                return false;
        }
        #endregion

        #region Draw
        public override void Draw( SpriteBatch spriteBatch, float alpha )
        {
            Color dynamicColor = new Color( new Vector4( 1f, 1f, 1f, alpha ) );
            Color dynamicTextColor = new Color( new Vector4( 0f, 0f, 0f, 1f ) * alpha );

            if (bChecked)
                spriteBatch.Draw( texture[1], position, dynamicColor );
            else
                spriteBatch.Draw( texture[0], position, dynamicColor );

            //font.Draw( text, position + new Vector2( texture[0].Width + 5f, 0f ), 1f, dynamicTextColor, spriteBatch );
            BaseGame.FontMgr.DrawInScrnCoord( text, position + new Vector2( texture[0].Width + 5f, 0f ), Control.fontScale, dynamicTextColor, 0f, Control.fontName );
        }
        #endregion
    }
}
