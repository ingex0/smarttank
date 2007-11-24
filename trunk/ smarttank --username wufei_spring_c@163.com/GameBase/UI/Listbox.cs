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
using GameBase.Graphics;
namespace GameBase.UI
{
    public class Listbox : Control
    {
        #region Variables
        Scrollbar scrollbar;

        public List<string> Items;
        public Point size;

        Texture2D listBackground;
        Texture2D selectionArea;

        public Color backcolor = Color.Snow;
        public Color forecolor = Color.Black;

        int charHeight = 15;
        int visibleItems = 0;
        int startIndex = 0;
        int endIndex = 0;
        public int selectedIndex = -1;

        MouseState ms;
        KeyboardState ks;
        bool bHasFocus = false;

        //int scrollWheelValue = 0; 
        #endregion

        #region Event
        public event EventHandler OnChangeSelection;
        public void listbox_onChangeSelection ( Object obj, EventArgs e ) { }
        #endregion

        #region Consturtion
        public Listbox ( string name, Vector2 position, Point size, Color backcolor, Color forecolor )//, Font font)
        {
            this.Type = ControlType.Listbox;
            this.name = name;
            this.position = position;
            this.size = size;
            this.backcolor = backcolor;
            this.forecolor = forecolor;

            OnChangeSelection += new EventHandler( listbox_onChangeSelection );

            Items = new List<string>();

            CreateTextures();

            visibleItems = (int)System.Math.Round( (float)size.Y / charHeight, MidpointRounding.AwayFromZero ) - 1;
        }
        #endregion

        #region Initial
        public void CreateTextures ()
        {
            listBackground = new Texture2D( BaseGame.Device, size.X, size.Y, 1, Control.resourceUsage, SurfaceFormat.Color, Control.resourceMode );

            Color[] pixels = new Color[size.X * size.Y];
            for (int y = 0; y < size.Y; y++)
            {
                for (int x = 0; x < size.X; x++)
                {
                    if (x < 3 || y < 3 || x > size.X - 3 || y > size.Y - 3)
                    {
                        if (x == 0 || y == 0 || x == size.X - 1 || y == size.Y - 1)
                            pixels[x + y * size.X] = Color.Black;
                        if (x == 1 || y == 1 || x == size.X - 2 || y == size.Y - 2)
                            pixels[x + y * size.X] = Color.LightGray;
                        if (x == 2 || y == 2 || x == size.X - 3 || y == size.Y - 3)
                            pixels[x + y * size.X] = Color.Gray;
                    }
                    else
                    {
                        float cX = backcolor.ToVector3().X;
                        float cY = backcolor.ToVector3().X;
                        float cZ = backcolor.ToVector3().X;

                        cX *= 1.0f - ((float)y / (float)size.Y) * 0.4f;
                        cY *= 1.0f - ((float)y / (float)size.Y) * 0.4f;
                        cZ *= 1.0f - ((float)y / (float)size.Y) * 0.4f;

                        pixels[x + y * size.X] = new Color( new Vector4( cX, cY, cZ, backcolor.ToVector4().W ) );
                    }

                    float currentX = pixels[x + y * size.X].ToVector3().X;
                    float currentY = pixels[x + y * size.X].ToVector3().X;
                    float currentZ = pixels[x + y * size.X].ToVector3().X;
                    pixels[x + y * size.X] = new Color( new Vector4( currentX, currentY, currentZ, 0.85f ) );
                }
            }

            pixels[0] = Color.TransparentBlack;
            pixels[size.X] = Color.TransparentBlack;
            pixels[(size.Y - 1) * size.X] = Color.TransparentBlack;
            pixels[size.X - 1 + (size.Y - 1) * size.X] = Color.TransparentBlack;

            listBackground.SetData<Color>( pixels );

            selectionArea = new Texture2D( BaseGame.Device, size.X - 8, charHeight, 1, Control.resourceUsage, SurfaceFormat.Color, Control.resourceMode );
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

        private void InitScrollbar ()
        {
            scrollbar = new Scrollbar( name + "scrollbar", position + new Vector2( size.X, 0f ), Scrollbar.Axis.Vertical, null, size.Y, Items.Count - 1, 0 );//, Style.Default);
            scrollbar.OnValueChange += new EventHandler( scrollbar_onChangeValue );
        }
        #endregion

        #region Item Methods
        public void AddItem ( string item )
        {
            if (Contains( item ))
                return;

            Items.Add( item );
            if (selectedIndex != -1)
            {
                selectedIndex++;
                selectedItem = Items[selectedIndex];
            }
            if (Items.Count > visibleItems && scrollbar == null)
                InitScrollbar();

            if (scrollbar != null)
                scrollbar.max = Items.Count - visibleItems;

            if (Items.Count < visibleItems)
                endIndex = Items.Count;
            else
                endIndex = startIndex + visibleItems;
        }

        public void RemoveItem ( string item )
        {
            Items.Remove( item );
            if (Items.Count <= visibleItems && scrollbar != null)
                scrollbar = null;
            if (selectedItem == item)
            {
                selectedIndex = -1;
                selectedItem = string.Empty;
            }
            if (scrollbar != null)
                scrollbar.max = Items.Count - visibleItems;

            if (Items.Count < visibleItems)
                endIndex = Items.Count;
            else
                endIndex = startIndex + visibleItems;
        }

        public void RemoveItem ( int index )
        {
            Items.RemoveAt( index );
            if (Items.Count <= visibleItems && scrollbar != null)
                scrollbar = null;
            if (selectedIndex == index)
            {
                selectedIndex = -1;
                selectedItem = string.Empty;
            }
            if (scrollbar != null)
                scrollbar.max = Items.Count - visibleItems;

            if (Items.Count < visibleItems)
                endIndex = Items.Count;
            else
                endIndex = startIndex + visibleItems;
        }

        public void Clear ()
        {
            Items.Clear();
            if (scrollbar != null)
                scrollbar = null;
            selectedIndex = -1;
            selectedItem = string.Empty;
            startIndex = 0;
            endIndex = 0;
        }

        public void Sort ()
        {
            Items.Sort();
            selectedIndex = -1;
            selectedItem = string.Empty;
        }

        public bool Contains ( string item )
        {
            return Items.Contains( item );
        }

        public int IndexOf ( string item )
        {
            for (int i = 0; i < Items.Count; i++)
                if (Items[i] == item)
                    return i;

            return -1;
        }

        public void selectItem ( string item )
        {
            if (selectedItem != item)
            {
                this.selectedItem = item;
                this.selectedIndex = IndexOf( item );
                OnChangeSelection( this, EventArgs.Empty );
            }
        }

        public void selectItem ( int index )
        {
            if (index >= Items.Count)
                return;

            if (selectedItem != Items[index])
            {
                this.selectedItem = Items[index];
                this.selectedIndex = index;
                OnChangeSelection( this, EventArgs.Empty );
            }
            else
                this.selectedItem = string.Empty;
        }

        public void unselect ()
        {
            selectedIndex = -1;
            selectedItem = string.Empty;
        }

        #endregion

        #region Update
        public void scrollbar_onChangeValue ( object obj, EventArgs e )
        {
            startIndex = scrollbar.value;
        }

        public override void Update ()// Vector2 formPosition, Vector2 formSize )
        {
            //position = origin + formPosition;
            //CheckVisibility( formPosition, formSize );

            if (bVisible)
            {
                CheckSelection();
                if (bHasFocus && selectedIndex != -1)
                    CheckKeyboardState();
            }

            if (scrollbar != null)
                scrollbar.Update();// formPosition, formSize );

            if (startIndex < 0)
                startIndex = 0;
        }

        private void CheckVisibility ( Vector2 formPosition, Vector2 formSize )
        {
            if (scrollbar != null)
            {
                if (position.X + size.X + scrollbar.size.X > formPosition.X + formSize.X - 15f)
                    bVisible = false;
                else if (position.Y + size.Y > formPosition.Y + formSize.Y - 25f)
                    bVisible = false;
                else
                    bVisible = true;
            }
            else
            {
                if (position.X + size.X > formPosition.X + formSize.X - 15f)
                    bVisible = false;
                else if (position.Y + size.Y > formPosition.Y + formSize.Y - 25f)
                    bVisible = false;
                else
                    bVisible = true;
            }
        }

        bool bItemPressed = false;
        private void CheckSelection ()
        {
            ms = Mouse.GetState();

            if (ms.LeftButton == ButtonState.Pressed)
            {
                Rectangle listArea;
                if (scrollbar == null)
                    listArea = new Rectangle( (int)position.X, (int)position.Y, size.X, size.Y );
                else
                    listArea = new Rectangle( (int)position.X, (int)position.Y, size.X + scrollbar.size.X, size.Y );

                //if (Math.isInRectangle(new Point(ms.X, ms.Y), listArea))
                if (listArea.Contains( new Point( ms.X, ms.Y ) ))
                    bHasFocus = true;
                else if (bHasFocus)
                {
                    bHasFocus = false;
                    //unselect();
                }

                if (!bItemPressed)
                {
                    int lastItem = Items.Count;
                    if (lastItem > visibleItems)
                        lastItem = visibleItems;
                    for (int i = 0; i < visibleItems; i++)
                    {
                        if (bIsOverItem( i ))
                        {
                            selectItem( i + startIndex );
                            bItemPressed = true;
                        }
                    }
                }
            }
            else if (bItemPressed)
            {
                bItemPressed = false;
            }
        }

        bool bKeyPressed = false;
        private void CheckKeyboardState ()
        {
            ks = Keyboard.GetState();

            if (ks.IsKeyDown( Keys.Up ) && !bKeyPressed)
            {
                bKeyPressed = true;
                if (selectedIndex > 0)
                    selectItem( selectedIndex - 1 );
                if (selectedIndex < startIndex)
                    startIndex--;
                if (scrollbar != null)
                    scrollbar.value = startIndex;
            }
            else if (ks.IsKeyDown( Keys.Down ) && !bKeyPressed)
            {
                bKeyPressed = true;
                if (selectedIndex < Items.Count - 1)
                    selectItem( selectedIndex + 1 );
                if (selectedIndex > startIndex + visibleItems - 1)
                    startIndex++;
                if (scrollbar != null)
                    scrollbar.value = startIndex;
            }
            else if (!ks.IsKeyDown( Keys.Up ) && !ks.IsKeyDown( Keys.Down ) && bKeyPressed)
                bKeyPressed = false;
        }

        private bool bIsOverItem ( int i )
        {
            Vector2 itemPosition = position + new Vector2( 4f, 4f );

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
            Color dynamicListColor = new Color( new Vector4( backcolor.ToVector3().X, backcolor.ToVector3().Y, backcolor.ToVector3().Z, alpha ) );
            spriteBatch.Draw( listBackground, position, dynamicListColor );

            //Items
            int endIndex = Items.Count;
            if (startIndex + visibleItems < Items.Count)
                endIndex = startIndex + visibleItems;

            int lastItem = Items.Count;
            if (lastItem > visibleItems)
                lastItem = visibleItems;

            for (int i = 0; i < lastItem; i++)
            {
                Color dynamicTextColor = new Color( new Vector4( forecolor.ToVector3().X, forecolor.ToVector3().Y, forecolor.ToVector3().Z, alpha ) );
                //font.Draw(Items[i + startIndex], position + new Vector2(4f, 4f) + new Vector2(0f, charHeight * i), 1f, dynamicTextColor, spriteBatch);
                FontManager.DrawLucidaInScrnCoord( Items[i + startIndex], position + new Vector2( 4f, 4f ) + new Vector2( 0f, charHeight * i ), 0.7f, dynamicTextColor, 0f );


                //Selected Area
                Color dynamicAreaColor = new Color( new Vector4( Color.White.ToVector3().X, Color.White.ToVector3().Y, Color.White.ToVector3().Z, alpha ) );

                if (Items[i + startIndex] == selectedItem)
                    spriteBatch.Draw( selectionArea, position + new Vector2( 4f, 4f ) + new Vector2( 0f, charHeight * i ), dynamicAreaColor );

                //Selection Area                    
                if (ms.LeftButton == ButtonState.Released /*&& !Form.isInUse*/)
                {
                    if (bIsOverItem( i ))
                    {
                        spriteBatch.Draw( selectionArea, position + new Vector2( 4f, 4f ) + new Vector2( 0f, charHeight * i ), dynamicAreaColor );
                    }
                }
            }

            if (scrollbar != null)
                scrollbar.Draw( spriteBatch, alpha );

        }
        #endregion
    }
}
