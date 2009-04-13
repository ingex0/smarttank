using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Timers;
using System.IO;
using TankEngine2D.Helpers;
using TankEngine2D.Graphics;
using SmartTank.Helpers;

namespace SmartTank.Draw.UI.Controls
{
    public class Textbox : Control
    {
        #region Variables
        public Texture2D[] parts;
        public float width = 30f;


        Color color = Color.White;
        Color textColor = Color.Black;

        Rectangle middlePartRect = Rectangle.Empty;

        bool bHasFocus = false;

        public event EventHandler OnMouseDown;
        public event EventHandler OnKeyPress;

        MouseState ms;
        KeyboardState ks;
        Keys[] textKeys = { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.Space, Keys.Enter};
        Keys[] numericKeys = { Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.Enter };
        List<Keys> pressedKeys;
        public bool bNumerical = false;
        

        Timer keyTimerLong, keyTimerShort;

        int cursorPos = 0;
        bool bInputKeyDown = false;
        bool bBackspaceDown = false;
        bool bLeftKey = false;
        bool bRightKey = false;
        bool bDeleteKey = false;
        bool bHomeKey = false;
        bool bEndKey = false;
        bool bInputEnable = true;
        public bool bStar = false;
        float textLen;
        string starText;

        Timer cursorTimer;


        bool bCursorVisible = false;

        int characterWidth = 9;

        public Point size = Point.Zero;

        //public Form.Style style = Form.Style.Default;

        int startIndex = 0;
        int endIndex = 0;
        int visibleChar = 0;
        public int maxLen = 1000;
        #endregion

        #region Construction
        public Textbox( string name, Vector2 position, float width, string text, bool bNumerical)//, Font font, Form.Style style)        
        {
            this.Type = ControlType.Textbox;
            this.name = name;
            this.position = position;
            this.text = text;
            this.width = width;
            this.bNumerical = bNumerical;

            parts = new Texture2D[3];
            parts[0] = BaseGame.ContentMgr.Load<Texture2D>( Path.Combine( Directories.UIContent, "textbox_left" ) );
            parts[1] = BaseGame.ContentMgr.Load<Texture2D>( Path.Combine( Directories.UIContent, "textbox_middle" ) );
            parts[2] = BaseGame.ContentMgr.Load<Texture2D>( Path.Combine( Directories.UIContent, "textbox_right" ) );

            middlePartRect = new Rectangle( (int)position.X + parts[0].Width, 0, (int)width - parts[0].Width - parts[1].Width, parts[1].Height );

            this.size = new Point( parts[0].Width + middlePartRect.Width + parts[2].Width, parts[0].Height );

            if (width < parts[0].Width + parts[1].Width + parts[2].Width)
                width = parts[0].Width + parts[1].Width + parts[2].Width;

            middlePartRect.Width = (int)width - parts[0].Width - parts[2].Width;
            middlePartRect.X = (int)position.X + parts[0].Width;
            middlePartRect.Y = (int)position.Y;

            visibleChar = middlePartRect.Width / characterWidth + 1;

            OnMouseDown += new EventHandler( Select );
            OnKeyPress += new EventHandler( KeyDown );
            pressedKeys = new List<Keys>();

            cursorTimer = new Timer( 500 );
            cursorTimer.Elapsed += new ElapsedEventHandler( cursorTimer_Tick );

            keyTimerLong = new Timer( 500 );
            keyTimerLong.Elapsed += new ElapsedEventHandler( keyTimerLong_Tick );

            keyTimerShort = new Timer(50);
            keyTimerShort.Elapsed += new ElapsedEventHandler(keyTimerShort_Tick);
            bInputEnable = true;
        }
        #endregion

        #region Update
        private void cursorTimer_Tick( Object obj, ElapsedEventArgs e )
        {
            bCursorVisible = !bCursorVisible;
        }

        private void keyTimerLong_Tick( Object obj, ElapsedEventArgs e )
        {

            bInputEnable = true;
            keyTimerLong.Stop();
        }

        private void keyTimerShort_Tick(Object obj, ElapsedEventArgs e)
        {

            bInputEnable = true;
            keyTimerShort.Stop();
        }

        public override void Update()// Vector2 formPosition, Vector2 formSize )
        {
            //position = origin + formPosition;
            //middlePartRect.X = (int)position.X + parts[0].Width;
            //middlePartRect.Y = (int)position.Y;

            this.size = new Point( parts[0].Width + middlePartRect.Width + parts[2].Width, parts[0].Height );

            //CheckVisibility( formPosition, formSize );

            if (bVisible)// && !Form.isInUse)
                CheckInput();
        }

        //private void CheckVisibility ( Vector2 formPosition, Vector2 formSize )
        //{
        //    if (position.X + middlePartRect.Width + parts[2].Width > formPosition.X + formSize.X - 15f)
        //        bVisible = false;
        //    else if (position.Y + parts[1].Height > formPosition.Y + formSize.Y - 25f)
        //        bVisible = false;
        //    else
        //        bVisible = true;
        //}

        private void CheckInput()
        {
            ms = Mouse.GetState();

            if (ms.LeftButton == ButtonState.Pressed)
            {
                if (ms.X > position.X && ms.X < position.X + width)
                {
                    if (ms.Y > position.Y && ms.Y < position.Y + parts[0].Height)
                    {
                        if (!bHasFocus)
                        {
                            Select( null, EventArgs.Empty );
                            bCursorVisible = true;
                        }
                    }
                    else if (bHasFocus)
                        Unselect( null, EventArgs.Empty );
                }
                else if (bHasFocus)
                    Unselect( null, EventArgs.Empty );
            }

            if (bHasFocus)
            {
                int PressNum = 0;
                ks = Keyboard.GetState();
                //


                foreach (Keys thisKey in textKeys)
                {
                    if (ks.IsKeyDown(thisKey))
                        PressNum++;
                }

                if (PressNum > 1)
                {
                    if (!ks.IsKeyDown(Keys.RightShift) && !ks.IsKeyDown(Keys.LeftShift))
                    {
                        pressedKeys.Clear();
                        return;
                    }
                    else if (PressNum > 2)
                    {
                        pressedKeys.Clear();
                        return;
                    }
                }


                //Text keys
                bool bFoundKey = false;
                bInputKeyDown = false;
                bool NoPress = true;
                if (text.Length < maxLen)
                {
                    foreach (Keys thisKey in textKeys)
                    {
                        bFoundKey = false;
                        //for (int i = 0; i < pressedKeys.Count; i++)
                        for (int i = pressedKeys.Count - 1; i >= 0; i--)
                        {
                            if (pressedKeys[i] == thisKey)
                                bFoundKey = true;
                            break;
                        }

                        if (ks.IsKeyDown(thisKey))
                        {
                            if (!bFoundKey)
                            {

                                bInputKeyDown = true;
                                OnKeyPress(thisKey, null);

                                keyTimerShort.Stop();
                                keyTimerLong.Stop();
                                bInputEnable = false;
                                keyTimerLong.Start();
                            }
                            else
                            {
                                if (bInputEnable)
                                {

                                    bInputKeyDown = true;
                                    OnKeyPress(thisKey, null);

                                    keyTimerShort.Stop();
                                    keyTimerLong.Stop();
                                    bInputEnable = false;
                                    keyTimerShort.Start();
                                }
                            }
                            NoPress = false;
                            break;
                        }
                        else
                        {
                            if (bFoundKey)
                            {
                                keyTimerShort.Stop();
                                keyTimerLong.Stop();
                                bInputEnable = false;
                                keyTimerLong.Start();
                            }
                            for (int i = 0; i < pressedKeys.Count; i++)
                            {
                                if (thisKey == pressedKeys[i])
                                    pressedKeys.RemoveAt(i);
                            }


                        }
                    }
                }

                if (NoPress)
                {
                    pressedKeys.Clear();
                    bInputKeyDown = false;
                }
                else
                {
                    return;
                }

                if (!bBackspaceDown && !bDeleteKey && !bLeftKey && !bRightKey)
                {
                        bInputEnable = true;
                }

                //Backspace
                if (ks.IsKeyDown( Keys.Back ))
                {

                    if (!bBackspaceDown)
                    {
                        bBackspaceDown = true;

                        keyTimerShort.Stop();
                        keyTimerLong.Stop();
                        bInputEnable = false;
                        keyTimerLong.Start();
                        if (cursorPos > 0 && cursorPos < text.Length + 1)
                        {
                            Backspace();
                        }
                    }
                    else
                    {
                        if (bInputEnable)
                        {
                            keyTimerShort.Stop();
                            keyTimerLong.Stop();
                            bInputEnable = false;
                            keyTimerShort.Start();
                            if (cursorPos > 0 && cursorPos < text.Length + 1)
                            {
                                Backspace();
                            }
                        }
                    }
                }
                else if (ks.IsKeyUp(Keys.Back) && bBackspaceDown)
                {
                    bBackspaceDown = false;
                    bInputEnable = true;
                }

                //Delete
                if (ks.IsKeyDown( Keys.Delete ))
                {
                    if (!bDeleteKey)
                    {
                        keyTimerShort.Stop();
                        keyTimerLong.Stop();
                        bInputEnable = false;
                        keyTimerLong.Start();
                        bDeleteKey = true;
                        if (cursorPos < text.Length)
                            Remove();
                    }
                    else
                    {
                        if (bInputEnable)
                        {
                            keyTimerShort.Stop();
                            keyTimerLong.Stop();
                            bInputEnable = false;
                            keyTimerShort.Start();
                            if (cursorPos < text.Length)
                                Remove();                            
                        }
                    }
                }
                else if (bDeleteKey)
                {
                    bInputEnable = true;
                    bDeleteKey = false;
                }

                if (ks.IsKeyDown( Keys.Left ))
                {
                    if (!bLeftKey)
                    {
                        keyTimerShort.Stop();
                        keyTimerLong.Stop();
                        bInputEnable = false;
                        keyTimerLong.Start();
                        bLeftKey = true;
                        if (cursorPos > 0)
                        {
                            cursorPos -= 1;
                            if (cursorPos == startIndex - 1 && startIndex > 0)
                                startIndex -= 1;
                            bCursorVisible = true;
                        }
                    }
                    else
                    {
                        if (bInputEnable)
                        {
                            keyTimerShort.Stop();
                            keyTimerLong.Stop();
                            bInputEnable = false;
                            keyTimerShort.Start();
                            if (cursorPos > 0)
                            {
                                cursorPos -= 1;
                                if (cursorPos == startIndex - 1 && startIndex > 0)
                                    startIndex -= 1;
                                bCursorVisible = true;
                            }
                        }
                    
                    }
                }
                else if (bLeftKey)
                {
                    bInputEnable = true;
                    bLeftKey = false;
                }

                if (ks.IsKeyDown( Keys.Right ))
                {
                    if (!bRightKey)
                    {
                        keyTimerShort.Stop();
                        keyTimerLong.Stop();
                        bInputEnable = false;
                        keyTimerLong.Start();
                        bRightKey = true;
                        if (cursorPos < text.Length)
                        {
                            cursorPos += 1;
                            if (cursorPos > startIndex + visibleChar)
                                startIndex += 1;
                            bCursorVisible = true;
                        }
                    }
                    else
                    {
                        if (bInputEnable)
                        {
                            keyTimerShort.Stop();
                            keyTimerLong.Stop();
                            bInputEnable = false;
                            keyTimerShort.Start();
                            if (cursorPos < text.Length)
                            {
                                cursorPos += 1;
                                if (cursorPos > startIndex + visibleChar)
                                    startIndex += 1;
                                bCursorVisible = true;
                            }
                        }
                    }
                }
                else if (bRightKey)
                {
                    bInputEnable = true;
                    bRightKey = false;
                }

                if (ks.IsKeyDown( Keys.Home ))
                {
                    if (!bHomeKey)
                    {
                        bHomeKey = true;
                        Home();
                    }
                }
                else if (bHomeKey)
                    bHomeKey = false;

                if (ks.IsKeyDown( Keys.End ))
                {
                    if (!bEndKey)
                    {
                        bEndKey = true;
                        End();
                    }
                }
                else if (bEndKey)
                    bEndKey = false;
            }
        }

        private void Select( Object obj, EventArgs e )
        {
            cursorPos = text.Length;
            bHasFocus = true;
            cursorTimer.Start();
        }

        private void Unselect( Object obj, EventArgs e )
        {
            bHasFocus = false;
            cursorTimer.Stop();
            bCursorVisible = false;
        }

        private void KeyDown( Object obj, EventArgs e )
        {
            if (obj != null)
            {
                bInputKeyDown = true;
                Keys currentKey = (Keys)obj;

                bool bNewKey = true;
                for (int i = 0; i < pressedKeys.Count; i++)
                    if (currentKey == pressedKeys[i])
                        bNewKey = false;

                //if (bNewKey)
                {
                    if (bNewKey)
                        pressedKeys.Add( currentKey );
                    string[] strSplit = new string[2];
                    if (this.cursorPos < this.text.Length + 1)
                    {
                        strSplit[0] = this.text.Substring( 0, this.cursorPos );
                        strSplit[1] = this.text.Substring( this.cursorPos, this.text.Length - this.cursorPos );
                    }

                    if (currentKey == Keys.Space && !bNumerical)
                        Add( " " );
                    else if (currentKey != Keys.Enter)
                    {
                        if (!bNumerical)
                        {
                            bool bNumericKey = false;
                            for (int i = 0; i < numericKeys.Length; i++)
                                if (currentKey == numericKeys[i])
                                    bNumericKey = true;

                            if (bNumericKey)
                                Add( currentKey.ToString().Substring( 6, 1 ) );
                            else if (ks.IsKeyDown( Keys.LeftShift ) || ks.IsKeyDown( Keys.RightShift ))
                                Add( currentKey.ToString() );
                            else
                                Add( currentKey.ToString().ToLower() );
                        }
                        else if (bNumerical)
                        {
                            bool bNumericKey = false;
                            for (int i = 0; i < numericKeys.Length; i++)
                                if (currentKey == numericKeys[i])
                                    bNumericKey = true;

                            if (bNumericKey)
                                Add( currentKey.ToString().Substring( 6, 1 ) );
                        }
                    }
                }
            }
        }

        private void Add( string character )
        {
            if (cursorPos == text.Length)
                text = text + character;
            else
            {
                string[] strSplit = new string[2];
                strSplit[0] = text.Substring( 0, cursorPos );
                strSplit[1] = text.Substring( cursorPos, text.Length - cursorPos );
                text = strSplit[0] + character + strSplit[1];
            }

            cursorPos += 1;

            if (cursorPos > startIndex + visibleChar)
                startIndex += 1;

            //endIndex = cursorPos;
        }

        private void Remove()
        {
            string[] strSplit = new string[2];
            strSplit[0] = text.Substring( 0, cursorPos );
            strSplit[1] = text.Substring( cursorPos + 1, text.Length - cursorPos - 1 );
            text = strSplit[0] + strSplit[1];
        }

        private void Backspace()
        {
            text = text.Remove( cursorPos - 1, 1 );
            cursorPos -= 1;

            if (cursorPos < startIndex + 1)
            {
                startIndex -= visibleChar;
                endIndex = startIndex + 1;
            }
        }

        private void Home()
        {
            cursorPos = 0;
            startIndex = 0;
        }

        private void End()
        {
            cursorPos = text.Length;
            startIndex = cursorPos - visibleChar;
        }
        #endregion

        #region Draw
        public override void Draw( SpriteBatch spriteBatch, float alpha )
        {
            Color dynamicColor = new Color( new Vector4( color.ToVector3().X, color.ToVector3().Y, color.ToVector3().Z, alpha ) );
            Color dynamicTextColor = new Color( new Vector4( textColor.ToVector3().X, textColor.ToVector3().Y, textColor.ToVector3().Z, alpha ) );

            //spriteBatch.Draw( parts[0], position, dynamicColor );
            //spriteBatch.Draw( parts[1], middlePartRect, dynamicColor );
            //spriteBatch.Draw( parts[2], position + new Vector2( parts[0].Width + middlePartRect.Width, 0f ), dynamicColor );
            spriteBatch.Draw(parts[0], position, new Rectangle(0, 0, parts[0].Width, parts[0].Height), dynamicColor, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth.UI);
            spriteBatch.Draw(parts[1], position + new Vector2(parts[0].Width, 0), new Rectangle(0, 0, middlePartRect.Width, middlePartRect.Height), dynamicColor, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth.UI);
            spriteBatch.Draw(parts[2], position + new Vector2(parts[0].Width + middlePartRect.Width, 0f), new Rectangle(0, 0, parts[2].Width, parts[2].Height), dynamicColor, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth.UI);


            if (text.Length >= visibleChar)
            {
                endIndex = text.Length - startIndex;
                if (endIndex > visibleChar)
                    endIndex = visibleChar;
            }
            else
            {
                startIndex = 0;
                endIndex = text.Length;
            }
            //BaseGame.FontMgr.DrawInScrnCoord(
            string visibleText = text.Substring( startIndex, endIndex );
            if (BaseGame.FontMgr.LengthOfString( visibleText, Control.fontScale, Control.fontName ) > width)
                visibleText = text.Substring( startIndex, endIndex - 1 );

            //font.Draw( text.Substring( startIndex, endIndex ), position + new Vector2( 5f, 4f ), 1f, dynamicTextColor, spriteBatch );

            if (bStar)
            { 
                starText = "";
                for (int i = 0; i < text.Length; i++)
                {
                    starText += "*";
                }

                BaseGame.FontMgr.DrawInScrnCoord(starText.Substring(startIndex, endIndex), position + new Vector2(5f, 1f), Control.fontScale, dynamicTextColor, 0f, Control.fontName);
                textLen = BaseGame.FontMgr.LengthOfString(starText.Substring(startIndex, cursorPos - startIndex), Control.fontScale, Control.fontName);
            }
            else
            {
                BaseGame.FontMgr.DrawInScrnCoord(text.Substring(startIndex, endIndex), position + new Vector2(5f, 1f), Control.fontScale, dynamicTextColor, 0f, Control.fontName);
                textLen = BaseGame.FontMgr.LengthOfString(text.Substring(startIndex, cursorPos - startIndex), Control.fontScale, Control.fontName);
            }
            if (bHasFocus)
            {
                if (bCursorVisible)
                {
                    if (cursorPos - startIndex >= 0 && cursorPos - startIndex <= text.Length)
                        //    font.Draw( "|", position + new Vector2( font.Measure( text.Substring( startIndex, cursorPos - startIndex ), 1f ), 3f ), 1f, dynamicTextColor, spriteBatch );
                        BaseGame.FontMgr.DrawInScrnCoord( "|", position + new Vector2(textLen + 5f, -3f ), 0.7f, dynamicTextColor, 0f, Control.fontName );
                    else
                        //    font.Draw( "|", position + new Vector2( 0f, 3f ), 1f, dynamicTextColor, spriteBatch );
                        BaseGame.FontMgr.DrawInScrnCoord( "|", position + new Vector2( 0f, -3f ), Control.fontScale, dynamicTextColor, 0f, Control.fontName );
                }
            }

        }

        #endregion
    }
}
