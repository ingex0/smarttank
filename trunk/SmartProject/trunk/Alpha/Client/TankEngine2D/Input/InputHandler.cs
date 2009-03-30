using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TankEngine2D.Helpers;

namespace TankEngine2D.Input
{
    /// <summary>
    /// XNA环境下的输入处理类
    /// </summary>
    /// test comment
    public static class InputHandler
    {
        #region Mouse

        #region Variables

        static MouseState curMouseState;
        static MouseState lastMouseState;

        #endregion

        #region Properties

        /// <summary>
        /// 上一帧鼠标的视口X坐标
        /// </summary>
        static public int LastMouseX
        {
            get { return lastMouseState.X; }
        }
        /// <summary>
        /// 上一帧鼠标的视口Y坐标
        /// </summary>
        static public int LastMouseY
        {
            get { return lastMouseState.Y; }
        }
        /// <summary>
        /// 上一帧鼠标的视口位置
        /// </summary>
        static public Point LastMousePos
        {
            get { return new Point( LastMouseX, LastMouseY ); }
        }
        /// <summary>
        /// 当前帧鼠标的视口X坐标
        /// </summary>
        static public int CurMouseX
        {
            get { return curMouseState.X; }
        }
        /// <summary>
        /// 当前帧鼠标的视口Y坐标
        /// </summary>
        static public int CurMouseY
        {
            get { return curMouseState.Y; }
        }
        /// <summary>
        /// 当前帧的鼠标视口位置
        /// </summary>
        static public Point CurMousePos
        {
            get { return new Point( CurMouseX, CurMouseY ); }
        }
        /// <summary>
        /// 获得当前帧鼠标的逻辑位置
        /// </summary>
        static public Vector2 GetCurMousePosInLogic ( RenderEngine engine )
        {
            return engine.CoordinMgr.LogicPos( ConvertHelper.PointToVector2( CurMousePos ) );
        }
        /// <summary>
        /// 当前帧与上一帧之间鼠标是否移动
        /// </summary>
        static public bool MouseMoved
        {
            get { return CurMouseX != LastMouseX || CurMouseY != LastMouseY; }
        }
        /// <summary>
        /// 当前帧与上一帧之间鼠标视口位置的X增量
        /// </summary>
        static public int MouseXDelta
        {
            get { return CurMouseX - LastMouseX; }
        }
        /// <summary>
        /// 当前帧与上一帧之间鼠标视口位置的Y增量
        /// </summary>
        static public int MouseYDelta
        {
            get { return CurMouseY - LastMouseY; }
        }
        /// <summary>
        /// 当前帧与上一帧之间鼠标滚轮的增量
        /// </summary>
        static public int MouseWheelDelta
        {
            get { return curMouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue; }
        }
        /// <summary>
        /// 当前帧鼠标左键是否按下
        /// </summary>
        static public bool CurMouseLeftDown
        {
            get { return curMouseState.LeftButton == ButtonState.Pressed; }
        }
        /// <summary>
        /// 当前帧鼠标右键是否按下
        /// </summary>
        static public bool CurMouseRightDown
        {
            get { return curMouseState.RightButton == ButtonState.Pressed; }
        }
        /// <summary>
        /// 当前帧鼠标中键是否按下
        /// </summary>
        static public bool CurMouseMidDown
        {
            get { return curMouseState.MiddleButton == ButtonState.Pressed; }
        }
        /// <summary>
        /// 上一帧鼠标左键是否按下
        /// </summary>
        static public bool LastMouseLeftDown
        {
            get { return lastMouseState.LeftButton == ButtonState.Pressed; }
        }
        /// <summary>
        /// 上一帧鼠标右键是否按下
        /// </summary>
        static public bool LastMouseRightDown
        {
            get { return lastMouseState.RightButton == ButtonState.Pressed; }
        }
        /// <summary>
        /// 上一帧鼠标中键是否按下
        /// </summary>
        static public bool LastMouseMidDown
        {
            get { return lastMouseState.MiddleButton == ButtonState.Pressed; }
        }
        /// <summary>
        /// 鼠标是否在当前帧释放了左键
        /// </summary>
        static public bool MouseJustReleaseLeft
        {
            get { return LastMouseLeftDown && !CurMouseLeftDown; }
        }
        /// <summary>
        /// 鼠标是否在当前帧释放了右键
        /// </summary>
        static public bool MouseJustReleaseRight
        {
            get { return LastMouseRightDown && !CurMouseRightDown; }
        }
        /// <summary>
        /// 鼠标是否在当前帧释放了中键
        /// </summary>
        static public bool MouseJustReleaseMid
        {
            get { return LastMouseMidDown && !CurMouseMidDown; }
        }
        /// <summary>
        /// 鼠标是否在当前帧按下了左键
        /// </summary>
        static public bool MouseJustPressLeft
        {
            get { return !LastMouseLeftDown && CurMouseLeftDown; }
        }
        /// <summary>
        /// 鼠标是否在当前帧按下了右键
        /// </summary>
        static public bool MouseJustPressRight
        {
            get { return !LastMouseRightDown && CurMouseRightDown; }
        }
        /// <summary>
        /// 鼠标是否在当前帧按下了中键
        /// </summary>
        static public bool MouseJusePressMid
        {
            get { return !LastMouseMidDown && CurMouseMidDown; }
        }

        #endregion

        #region Help Functions

        /// <summary>
        /// 判断鼠标是否在矩形中
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        static public bool MouseInRect ( Rectangle rect )
        {
            return rect.Contains( new Point( CurMouseX, CurMouseY ) );
        }

        #endregion

        #endregion

        #region KeyBoard

        static KeyboardState curKeyboardState;
        static KeyboardState lastKeyboardState;

        /// <summary>
        /// 判断某按键当前是否处于按下状态
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static public bool IsKeyDown ( Keys key )
        {
            return curKeyboardState.IsKeyDown( key );
        }
        /// <summary>
        /// 判断某按键是否在当前帧被按下
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static public bool JustPressKey ( Keys key )
        {
            return curKeyboardState.IsKeyDown( key ) && lastKeyboardState.IsKeyUp( key );
        }
        /// <summary>
        /// 判断某按键是否在当前帧被释放
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static public bool JustReleaseKey ( Keys key )
        {
            return curKeyboardState.IsKeyUp( key ) && lastKeyboardState.IsKeyDown( key );
        }

        #endregion

        #region Update

        /// <summary>
        /// 更新输入处理类
        /// </summary>
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
