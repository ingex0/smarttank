//=================================================
// xWinForms
// Copyright ?2007 by Eric Grossinger
// http://psycad007.spaces.live.com/
//=================================================
// edit by wufei_spring
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Common.Helpers;
using System.IO;
using GameEngine.Graphics;
using GameEngine.Draw;


namespace GameEngine.UI
{
    public class Combo : Control
    {
        #region Variables
        public List<string> Items;
        public int width;

        Texture2D[] listboxParts;
        Rectangle listboxRect;

        Texture2D listBackground;
        Texture2D selectionArea;

        Color color = Color.White;
        Color textColor = Color.Black;
        Color listColor = Color.Snow;

        Button listBoxButton;

        int charHeight = 15;

        State state = State.Closed;
        public enum State
        {
            Opened,
            Closed
        }

        MouseState ms;


        public event EventHandler OnChangeSelection;
        private void Combo_onChangeSelection ( Object obj, EventArgs e ) { Close(); }

        public int currentIndex = -1;

        #endregion

        #region Construction
        public Combo ( string name, Vector2 position, int width )//, Font font, Form.Style style)
        {
            this.Type = ControlType.Combo;
            this.width = width;
            this.position = position;
            this.name = name;
            //this.font = font;
            //this.style = style;
            Init();//this.style);

            OnChangeSelection += new EventHandler( Combo_onChangeSelection );

            Items = new List<string>();
        }

        #endregion

        #region Initial
        private void Init ()
        {
            listboxParts = new Texture2D[4];
            listboxParts[0] = BaseGame.ContentMgr.Load<Texture2D>( Path.Combine( Directories.UIContent, "textbox_left" ) );
            listboxParts[1] = BaseGame.ContentMgr.Load<Texture2D>( Path.Combine( Directories.UIContent, "textbox_middle" ) );
            listboxParts[2] = BaseGame.ContentMgr.Load<Texture2D>( Path.Combine( Directories.UIContent, "textbox_right" ) );
            listboxRect = new Rectangle( 0, 0, (width - listboxParts[0].Width) - listboxParts[1].Width, listboxParts[0].Height );

            listBoxButton = new Button( "bt_Combo", "combo_button", position + new Vector2( listboxParts[0].Width + listboxParts[1].Width + listboxParts[2].Width, 0f ), Color.White );
            listBoxButton.OnMousePress += new EventHandler( listBoxButton_onMousePress );
        }
        #endregion

        #region Items Methods
        public void AddItem ( string item )
        {
            Items.Add( item );
            if (Items.Count == 1)
                selectedItem = Items[0];
        }

        public void RemoveItem ( string item )
        {
            Items.Remove( item );
        }

        public void RemoveItem ( int index )
        {
            Items.RemoveAt( index );
        }

        public void Sort ()
        {
            Items.Sort();
        }

        public void Clear ()
        {
            Items.Clear();
        }

        public bool Contains ( string item )
        {
            return Items.Contains( item );
        }

        private void SelectItem ( int index )
        {
            if (currentIndex == index)
                return;

            currentIndex = index;
            selectedItem = Items[index];
            OnChangeSelection( this, EventArgs.Empty );
        }
        #endregion

        #region Create Textures
        public void CreateTextures ()
        {
            int listHeight = Items.Count * charHeight + 8;
            listBackground = new Texture2D( BaseGame.Device, listboxParts[0].Width + listboxRect.Width + listboxParts[2].Width, listHeight, 1, Control.resourceUsage, SurfaceFormat.Color );

            Color[] pixels = new Color[listBackground.Width * listBackground.Height];
            for (int y = 0; y < listBackground.Height; y++)
            {
                for (int x = 0; x < listBackground.Width; x++)
                {
                    if (x < 3 || y < 3 || x > listBackground.Width - 3 || y > listBackground.Height - 3)
                    {
                        if (x == 0 || y == 0 || x == listBackground.Width - 1 || y == listBackground.Height - 1)
                            pixels[x + y * listBackground.Width] = Color.Black;
                        if (x == 1 || y == 1 || x == listBackground.Width - 2 || y == listBackground.Height - 2)
                            pixels[x + y * listBackground.Width] = Color.LightGray;
                        if (x == 2 || y == 2 || x == listBackground.Width - 3 || y == listBackground.Height - 3)
                            pixels[x + y * listBackground.Width] = Color.Gray;
                    }
                    else
                    {
                        float cX = listColor.ToVector3().X;
                        float cY = listColor.ToVector3().Y;
                        float cZ = listColor.ToVector3().Z;

                        cX *= 1.0f - ((float)y / (float)listBackground.Height) * 0.4f;
                        cY *= 1.0f - ((float)y / (float)listBackground.Height) * 0.4f;
                        cZ *= 1.0f - ((float)y / (float)listBackground.Height) * 0.4f;

                        pixels[x + y * listBackground.Width] = new Color( new Vector4( cX, cY, cZ, listColor.ToVector4().W ) );
                    }

                    float currentX = pixels[x + y * listBackground.Width].ToVector3().X;
                    float currentY = pixels[x + y * listBackground.Width].ToVector3().Y;
                    float currentZ = pixels[x + y * listBackground.Width].ToVector3().Z;

                    pixels[x + y * listBackground.Width] = new Color( new Vector4( currentX, currentY, currentZ, 0.85f ) );
                }
            }

            listBackground.SetData<Color>( pixels );

            selectionArea = new Texture2D( BaseGame.Device, listBackground.Width - 8, charHeight, 1, Control.resourceUsage, SurfaceFormat.Color );
            pixels = new Color[selectionArea.Width * selectionArea.Height];
            for (int y = 0; y < selectionArea.Height; y++)
            {
                for (int x = 0; x < selectionArea.Width; x++)
                {
                    pixels[x + y * selectionArea.Width] = new Color( new Vector4( 0f, 0f, 0f, 0.3f ) );
                }
            }

            selectionArea.SetData<Color>( pixels );
        }
        #endregion

        #region Update
        private void listBoxButton_onMousePress ( Object obj, EventArgs e )
        {
            //Console.WriteLine("listBoxButton_OnMousePress");
            if (state == State.Closed)
                Open();
            else if (state == State.Opened)
                Close();
        }

        public void Open ()
        {
            CreateTextures();
            state = State.Opened;
        }

        public void Close ()
        {
            state = State.Closed;
        }

        public override void Update ()// Vector2 formPosition, Vector2 formSize )
        {
            if (selectedItem == string.Empty && Items.Count > 0)
                selectedItem = Items[0];

            //position = formPosition + origin;
            listboxRect.X = (int)position.X + listboxParts[0].Width;
            listboxRect.Y = (int)position.Y;

            listBoxButton.MoveTo( position + new Vector2( listboxParts[0].Width + listboxRect.Width + listboxParts[2].Width, 0f ) );

            listBoxButton.Update();

            //CheckVisibility( formPosition, formSize );

            if (state == State.Opened)//&& !Form.isInUse)
                CheckSelection();
        }

        //private void CheckVisibility ( Vector2 formPosition, Vector2 formSize )
        //{
        //    if (position.X + listboxRect.Width + listboxParts[2].Width + listBoxButton.size.X > formPosition.X + formSize.X - 15f)
        //        bVisible = false;
        //    else if (position.Y + listboxParts[1].Height > formPosition.Y + formSize.Y - 25f)
        //        bVisible = false;
        //    else
        //        bVisible = true;
        //}

        private void CheckSelection ()
        {
            ms = Mouse.GetState();

            if (ms.LeftButton == ButtonState.Pressed)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (bIsOverItem( i ))
                        SelectItem( i );
                }
            }
        }

        private bool bIsOverItem ( int i )
        {
            Vector2 itemPosition = position + new Vector2( 4f, listboxParts[0].Height + 4f );

            if (ms.X > itemPosition.X && ms.X < itemPosition.X + listBackground.Width - 8f)
            {
                if (ms.Y > itemPosition.Y + (charHeight * i) && ms.Y < itemPosition.Y + +(charHeight * (i + 1)))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        #endregion

        #region Draw
        public override void Draw ( SpriteBatch spriteBatch, float alpha )
        {
            //Text Area
            Color dynamicColor = new Color( new Vector4( color.ToVector3(), alpha ) );
            spriteBatch.Draw( listboxParts[0], position, null, dynamicColor, 0, Vector2.Zero, 1f, SpriteEffects.None, LayerDepth.UI );
            spriteBatch.Draw( listboxParts[1], listboxRect, null, dynamicColor, 0, Vector2.Zero, SpriteEffects.None, LayerDepth.UI );
            spriteBatch.Draw( listboxParts[2], position + new Vector2( listboxParts[0].Width + listboxRect.Width, 0f ), dynamicColor );
            listBoxButton.Draw( spriteBatch, alpha );

            //Text
            Color dynamicTextColor = new Color( new Vector4( textColor.ToVector3(), alpha ) );
            BaseGame.FontMgr.DrawInScrnCoord( selectedItem, position + new Vector2( 5f, 2f ), Control.fontScale, dynamicTextColor, LayerDepth.Text, Control.fontName );

            //Listbox
            if (state == State.Opened)
            {
                Vector2 listPosition = position + new Vector2( 0f, listboxParts[0].Height );

                //Background
                Color dynamicListboxColor = new Color( new Vector4( listColor.ToVector3(), alpha ) );
                spriteBatch.Draw( listBackground, listPosition, null, dynamicListboxColor, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth.UI );

                //Items
                for (int i = 0; i < Items.Count; i++)
                {
                    BaseGame.FontMgr.DrawInScrnCoord( Items[i], listPosition + new Vector2( 4f, 4f ) + new Vector2( 0f, charHeight * i ), Control.fontScale, dynamicTextColor, LayerDepth.Text, Control.fontName );

                    //Selection Area
                    if (ms.LeftButton == ButtonState.Released)
                    {
                        if (bIsOverItem( i ))
                        {
                            Color dynamicAreaColor = new Color( new Vector4( Vector3.One, alpha ) );
                            spriteBatch.Draw( selectionArea, listPosition + new Vector2( 4f, 4f ) + new Vector2( 0f, charHeight * i ), null, dynamicAreaColor, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth.UI );
                        }
                    }
                }
            }

        }
        #endregion
    }
}
