using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameBase.Helpers;

namespace GameBase.Input
{
    public static class InputHandler
    {
        #region Mouse

        #region Variables
        static MouseState curMouseState;
        static MouseState lastMouseState;



        #endregion

        #region Properties

        static public int LastMouseX
        {
            get { return lastMouseState.X; }
        }
        static public int LastMouseY
        {
            get { return lastMouseState.Y; }
        }
        static public Point LastMousePos
        {
            get { return new Point( LastMouseX, LastMouseY ); }
        }

        static public int CurMouseX
        {
            get { return curMouseState.X; }
        }
        static public int CurMouseY
        {
            get { return curMouseState.Y; }
        }
        static public Point CurMousePos
        {
            get { return new Point( CurMouseX, CurMouseY ); }
        }

        static public Vector2 CurMousePosInLogic
        {
            get { return Coordin.LogicPos( ConvertHelper.PointToVector2( CurMousePos ) ); }
        }

        static public bool MouseMoved
        {
            get { return CurMouseX != LastMouseX || CurMouseY != LastMouseY; }
        }

        static public int MouseXDelta
        {
            get { return CurMouseX - LastMouseX; }
        }
        static public int MouseYDelta
        {
            get { return CurMouseY - LastMouseY; }
        }
        static public int MouseWheelDelta
        {
            get { return curMouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue; }
        }

        static public bool CurMouseLeftBtnPressed
        {
            get { return curMouseState.LeftButton == ButtonState.Pressed; }
        }
        static public bool CurMouseRightBtnPressed
        {
            get { return curMouseState.RightButton == ButtonState.Pressed; }
        }
        static public bool CurMouseMidBtnPressed
        {
            get { return curMouseState.MiddleButton == ButtonState.Pressed; }
        }

        static public bool LastMouseLeftPressed
        {
            get { return lastMouseState.LeftButton == ButtonState.Pressed; }
        }
        static public bool LastMouseRightPressed
        {
            get { return lastMouseState.RightButton == ButtonState.Pressed; }
        }
        static public bool LastMouseMidPressed
        {
            get { return lastMouseState.MiddleButton == ButtonState.Pressed; }
        }

        static public bool MouseJustClickLeft
        {
            get { return LastMouseLeftPressed && !CurMouseLeftBtnPressed; }
        }
        static public bool MouseJustClickRight
        {
            get { return LastMouseRightPressed && !CurMouseRightBtnPressed; }
        }
        static public bool MouseJustClickMid
        {
            get { return LastMouseMidPressed && !CurMouseMidBtnPressed; }
        }



        #endregion

        #region Help Functions

        static public bool MouseInRect ( Rectangle rect )
        {
            return rect.Contains( new Point( CurMouseX, CurMouseY ) );
        }

        #endregion

        #endregion

        #region KeyBoard

        static KeyboardState curKeyboardState;
        static KeyboardState lastKeyboardState;

        static public bool IsKeyDown ( Keys key )
        {
            return curKeyboardState.IsKeyDown( key );
        }

        static public bool JustPressKey ( Keys key )
        {
            return curKeyboardState.IsKeyDown( key ) && lastKeyboardState.IsKeyUp( key );
        }

        static public bool JustReleaseKey ( Keys key )
        {
            return curKeyboardState.IsKeyUp( key ) && lastKeyboardState.IsKeyDown( key );
        }

        #endregion

        #region Update

        static public void Update ()
        {
            MouseState MS = Mouse.GetState();
            KeyboardState KS = Keyboard.GetState();

            #region Update Mouse
            lastMouseState = curMouseState;
            curMouseState = MS;
            #endregion

            #region Update Keyboard

            lastKeyboardState = curKeyboardState;
            curKeyboardState = KS;

            #endregion
        }

        #endregion

    }
}
