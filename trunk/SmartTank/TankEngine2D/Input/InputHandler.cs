using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TankEngine2D.Helpers;

namespace TankEngine2D.Input
{
    /// <summary>
    /// XNA�����µ����봦����
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
        /// ��һ֡�����ӿ�X����
        /// </summary>
        static public int LastMouseX
        {
            get { return lastMouseState.X; }
        }
        /// <summary>
        /// ��һ֡�����ӿ�Y����
        /// </summary>
        static public int LastMouseY
        {
            get { return lastMouseState.Y; }
        }
        /// <summary>
        /// ��һ֡�����ӿ�λ��
        /// </summary>
        static public Point LastMousePos
        {
            get { return new Point( LastMouseX, LastMouseY ); }
        }
        /// <summary>
        /// ��ǰ֡�����ӿ�X����
        /// </summary>
        static public int CurMouseX
        {
            get { return curMouseState.X; }
        }
        /// <summary>
        /// ��ǰ֡�����ӿ�Y����
        /// </summary>
        static public int CurMouseY
        {
            get { return curMouseState.Y; }
        }
        /// <summary>
        /// ��ǰ֡������ӿ�λ��
        /// </summary>
        static public Point CurMousePos
        {
            get { return new Point( CurMouseX, CurMouseY ); }
        }
        /// <summary>
        /// ��õ�ǰ֡�����߼�λ��
        /// </summary>
        static public Vector2 GetCurMousePosInLogic ( RenderEngine engine )
        {
            return engine.CoordinMgr.LogicPos( ConvertHelper.PointToVector2( CurMousePos ) );
        }
        /// <summary>
        /// ��ǰ֡����һ֮֡������Ƿ��ƶ�
        /// </summary>
        static public bool MouseMoved
        {
            get { return CurMouseX != LastMouseX || CurMouseY != LastMouseY; }
        }
        /// <summary>
        /// ��ǰ֡����һ֮֡������ӿ�λ�õ�X����
        /// </summary>
        static public int MouseXDelta
        {
            get { return CurMouseX - LastMouseX; }
        }
        /// <summary>
        /// ��ǰ֡����һ֮֡������ӿ�λ�õ�Y����
        /// </summary>
        static public int MouseYDelta
        {
            get { return CurMouseY - LastMouseY; }
        }
        /// <summary>
        /// ��ǰ֡����һ֮֡�������ֵ�����
        /// </summary>
        static public int MouseWheelDelta
        {
            get { return curMouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue; }
        }
        /// <summary>
        /// ��ǰ֡�������Ƿ���
        /// </summary>
        static public bool CurMouseLeftDown
        {
            get { return curMouseState.LeftButton == ButtonState.Pressed; }
        }
        /// <summary>
        /// ��ǰ֡����Ҽ��Ƿ���
        /// </summary>
        static public bool CurMouseRightDown
        {
            get { return curMouseState.RightButton == ButtonState.Pressed; }
        }
        /// <summary>
        /// ��ǰ֡����м��Ƿ���
        /// </summary>
        static public bool CurMouseMidDown
        {
            get { return curMouseState.MiddleButton == ButtonState.Pressed; }
        }
        /// <summary>
        /// ��һ֡�������Ƿ���
        /// </summary>
        static public bool LastMouseLeftDown
        {
            get { return lastMouseState.LeftButton == ButtonState.Pressed; }
        }
        /// <summary>
        /// ��һ֡����Ҽ��Ƿ���
        /// </summary>
        static public bool LastMouseRightDown
        {
            get { return lastMouseState.RightButton == ButtonState.Pressed; }
        }
        /// <summary>
        /// ��һ֡����м��Ƿ���
        /// </summary>
        static public bool LastMouseMidDown
        {
            get { return lastMouseState.MiddleButton == ButtonState.Pressed; }
        }
        /// <summary>
        /// ����Ƿ��ڵ�ǰ֡�ͷ������
        /// </summary>
        static public bool MouseJustReleaseLeft
        {
            get { return LastMouseLeftDown && !CurMouseLeftDown; }
        }
        /// <summary>
        /// ����Ƿ��ڵ�ǰ֡�ͷ����Ҽ�
        /// </summary>
        static public bool MouseJustReleaseRight
        {
            get { return LastMouseRightDown && !CurMouseRightDown; }
        }
        /// <summary>
        /// ����Ƿ��ڵ�ǰ֡�ͷ����м�
        /// </summary>
        static public bool MouseJustReleaseMid
        {
            get { return LastMouseMidDown && !CurMouseMidDown; }
        }
        /// <summary>
        /// ����Ƿ��ڵ�ǰ֡���������
        /// </summary>
        static public bool MouseJustPressLeft
        {
            get { return !LastMouseLeftDown && CurMouseLeftDown; }
        }
        /// <summary>
        /// ����Ƿ��ڵ�ǰ֡�������Ҽ�
        /// </summary>
        static public bool MouseJustPressRight
        {
            get { return !LastMouseRightDown && CurMouseRightDown; }
        }
        /// <summary>
        /// ����Ƿ��ڵ�ǰ֡�������м�
        /// </summary>
        static public bool MouseJusePressMid
        {
            get { return !LastMouseMidDown && CurMouseMidDown; }
        }

        #endregion

        #region Help Functions

        /// <summary>
        /// �ж�����Ƿ��ھ�����
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
        /// �ж�ĳ������ǰ�Ƿ��ڰ���״̬
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static public bool IsKeyDown ( Keys key )
        {
            return curKeyboardState.IsKeyDown( key );
        }
        /// <summary>
        /// �ж�ĳ�����Ƿ��ڵ�ǰ֡������
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        static public bool JustPressKey ( Keys key )
        {
            return curKeyboardState.IsKeyDown( key ) && lastKeyboardState.IsKeyUp( key );
        }
        /// <summary>
        /// �ж�ĳ�����Ƿ��ڵ�ǰ֡���ͷ�
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
        /// �������봦����
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
