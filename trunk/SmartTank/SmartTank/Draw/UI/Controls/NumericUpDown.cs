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

namespace SmartTank.Draw.UI.Controls
{
    public class NumericUpDown : Control
    {
        #region Variables
        Button buttonUp, buttonDown;
        Textbox textbox;

        public int width = 50;
        public int increment = 1; 

        #endregion

        #region Construction
        public NumericUpDown ( string name, Vector2 position, int width, int min, int max, int value, int increment )
        {
            Type = ControlType.NumericUpDown;
            this.name = name;
            this.position = position;
            this.width = width;
            this.min = min;
            this.max = max;
            this.value = value;
            this.increment = increment;

            if (this.increment == 0)
                this.increment = 1;

            textbox = new Textbox( "txt_numericUpDown", position, width, value.ToString(), true );//, HUD.TextFont, Form.Style.Default);
            buttonUp = new Button( "bt_numericUp", "numeric_up", new Vector2( position.X + width, position.Y - 2 ), new Color( new Vector4( 0.9f, 0.9f, 0.9f, 1f ) ) );//, Form.Style.Default, true);
            buttonDown = new Button( "bt_numericDown", "numeric_down", new Vector2( position.X + width, position.Y + buttonUp.size.Y ), new Color( new Vector4( 0.9f, 0.9f, 0.9f, 1f ) ) );//, Form.Style.Default, true);

            buttonUp.OnMouseRelease += new EventHandler( onButtonUp );
            buttonDown.OnMouseRelease += new EventHandler( onButtonDown );
            textbox.OnKeyPress += new EventHandler( onKeyPress );
        } 
        #endregion

        #region Update
        private void onButtonUp ( object obj, EventArgs e )
        {
            if (value.ToString() != textbox.text)
                SetValue( System.Convert.ToInt32( textbox.text ) );

            SetValue( value + increment );
            textbox.text = value.ToString();
        }

        private void onButtonDown ( object obj, EventArgs e )
        {
            if (value.ToString() != textbox.text)
                SetValue( System.Convert.ToInt32( textbox.text ) );

            SetValue( value - increment );
            textbox.text = value.ToString();
        }

        private void onKeyPress ( object obj, EventArgs e )
        {
            //Console.WriteLine("NumericUpDown_OnKeyPress");
            Keys key = (Keys)obj;

            if (key == Keys.Enter)
                SetValue( System.Convert.ToInt32( textbox.text ) );
        }

        public void SetValue ( int value )
        {
            this.value = value;

            if (this.value < min)
                this.value = min;
            else if (this.value > max)
                this.value = max;

            textbox.text = this.value.ToString();
        }

        public override void Update ()// Vector2 formPosition, Vector2 formSize )
        {
            textbox.Update();// formPosition, formSize );

            //base.Update( formPosition, formSize );

            buttonUp.position.X = position.X + width;
            buttonUp.position.Y = position.Y - 1;
            buttonUp.Update();

            buttonDown.position.X = buttonUp.position.X;
            buttonDown.position.Y = buttonUp.position.Y + buttonUp.size.Y - 1;
            buttonDown.Update();

            //CheckVisibility( formPosition, formSize );
        }

        //private void CheckVisibility ( Vector2 formPosition, Vector2 formSize )
        //{
        //    if (position.X + textbox.size.X + buttonUp.size.X > formPosition.X + formSize.X - 15f)
        //        bVisible = false;
        //    else if (position.Y + textbox.size.Y + 15f > formPosition.Y + formSize.Y - 25f)
        //        bVisible = false;
        //    else
        //        bVisible = true;
        //}

        #endregion

        #region Draw
        public override void Draw ( SpriteBatch spriteBatch, float alpha )
        {
            textbox.Draw( spriteBatch, alpha );
            buttonUp.Draw( spriteBatch, alpha );
            buttonDown.Draw( spriteBatch, alpha );

            base.Draw( spriteBatch, alpha );
        } 
        #endregion
    }
}
