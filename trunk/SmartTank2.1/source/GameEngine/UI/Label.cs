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
using GameEngine.Graphics;
namespace GameEngine.UI
{
    public class Label : Control
    {

        public Color color = Color.Black;
        public float scale = 1f;

        public Label( string name, Vector2 position, string text, Color color, float scale )//, Font font)
        {
            this.Type = ControlType.Label;
            this.name = name;
            this.position = position;
            this.text = text;
            this.scale = scale;
            this.color = new Color( new Vector4( color.ToVector3(), 1f ) );
        }

        #region Old Code
        //public override void Update(Vector2 formPosition, Vector2 formSize)
        //{
        //    base.Update(formPosition, formSize);
        //    CheckVisibility(formPosition, formSize);
        //} 
        #endregion

        private void CheckVisibility( Vector2 formPosition, Vector2 formSize )
        {
            if (position.X + name.Length * 9f > formPosition.X + formSize.X - 15f)
                bVisible = false;
            else if (position.Y + 15f > formPosition.Y + formSize.Y - 25f)
                bVisible = false;
            else
                bVisible = true;
        }

        public override void Draw( SpriteBatch spriteBatch, float alpha )
        {
            Color dynamicColor = new Color( new Vector4( color.ToVector3(), alpha ) );
            //font.Draw(text, position, scale, dynamicColor, spriteBatch);
            BaseGame.FontMgr.DrawInScrnCoord( text, position, scale, dynamicColor, 0f, fontName );
        }
    }
}
