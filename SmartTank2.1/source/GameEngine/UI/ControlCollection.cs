//=================================================
// xWinForms
// Copyright ?2007 by Eric Grossinger
// http://psycad007.spaces.live.com/
//=================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace GameEngine.UI
{
    public class ControlCollection
    {
        #region Properties
        public List<Control> controls;

        public Control this[string name]
        {
            get { return getElement( name ); }
        }
        public Control this[int index]
        {
            get { return controls[index]; }
        }

        public int Count
        {
            get { return controls.Count; }
        } 
        #endregion

        #region Construction
        public ControlCollection ()
        {
            controls = new List<Control>();
        } 
        #endregion

        #region Element Methods
        public void Add ( Control element ) { this.controls.Add( element ); }
        public void Remove ( Control element ) { this.controls.Remove( element ); }
        public Control getElement ( string name )
        {
            return this.controls.Find( delegate( Control returnElement ) { return returnElement.name == name; } );
        } 
        #endregion

        #region Update

        #region Old Code
        //public void Update ( Vector2 formPosition, Vector2 formSize )
        //{
        //    foreach (Control thisControl in controls)
        //    {
        //        if (thisControl != null)
        //            thisControl.Update( formPosition, formSize );
        //    }
        //} 
        #endregion

        public void Update ()
        {
            foreach (Control control in controls)
            {
                control.Update();
            }
        }

        #endregion

        #region Draw
        public void Draw ( SpriteBatch windowBatch, float alpha )
        {
            foreach (Control thisControl in controls)
            {
                if (thisControl != null && thisControl.bvisible)
                    thisControl.Draw( windowBatch, alpha );
            }
        }     
        #endregion
    }
}
