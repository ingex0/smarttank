using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using TankEngine2D.DataStructure;
using TankEngine2D.Helpers;


namespace TankEngine2D.Graphics
{
    /// <summary>
    /// 逻辑坐标与屏幕坐标之间转换的管理类
    /// </summary>
    public class CoordinMgr
    {
        #region Variables

        private Vector2 logicCenter;

        private Vector2 scrnCenter;

        private float rota;

        private Matrix rotaMatrix;
        private Matrix rotaMatrixInvert;


        private Rectangle gameViewRect;

        private float scale;

        #endregion

        #region Properties

        /// <summary>
        /// 获得绘制区在视口中的区域
        /// </summary>
        public Rectangle ScrnViewRect
        {
            get { return gameViewRect; }
        }

        /// <summary>
        /// 获得绘制区的宽度
        /// </summary>
        public int ViewWidth
        {
            get { return gameViewRect.Width; }
        }
        /// <summary>
        /// 获得绘制区的高度
        /// </summary>
        public int ViewHeight
        {
            get { return gameViewRect.Height; }
        }

        /// <summary>
        /// 获得一个屏幕像素的逻辑大小
        /// </summary>
        public Vector2 TexelSize
        {
            get { return new Vector2( 1 / scale, 1 / scale ); }
        }

        /// <summary>
        /// 获得或设置摄像机的方位角
        /// </summary>
        public float Rota
        {
            get { return rota; }
            set
            {
                this.rota = value;
                rotaMatrix = Matrix.CreateRotationZ( rota );
                rotaMatrixInvert = Matrix.CreateRotationZ( -rota );
            }
        }

        /// <summary>
        /// 获得或设置摄像机的缩放率（屏幕坐标/逻辑坐标）
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// 获得或设置摄像机焦点的逻辑位置
        /// </summary>
        public Vector2 LogicCenter
        {
            get { return logicCenter; }
            set { logicCenter = value; }
        }

        /// <summary>
        /// 获得从逻辑坐标到屏幕坐标的旋转转换矩阵
        /// </summary>
        public Matrix RotaMatrixFromLogicToScrn
        {
            get { return rotaMatrixInvert; }
        }
        /// <summary>
        /// 获得从屏幕坐标到逻辑坐标的旋转转换矩阵
        /// </summary>
        public Matrix RotaMatrixFromScrnToLogic
        {
            get { return rotaMatrix; }
        }


        #endregion

        #region SetFunctions Called By Platform

        /// <summary>
        /// 设置绘制区域
        /// </summary>
        /// <param name="rect"></param>
        public void SetScreenViewRect ( Rectangle rect )
        {
            gameViewRect = rect;
            scrnCenter = new Vector2( rect.X + 0.5f * rect.Width, rect.Y + 0.5f * rect.Height );
        }

        /// <summary>
        /// 设置摄像机
        /// </summary>
        /// <param name="setScale">缩放率(屏幕坐标/逻辑坐标)</param>
        /// <param name="centerLogicPos">摄像机焦点所在逻辑位置</param>
        /// <param name="setRota">设置摄像机的旋转角</param>
        public void SetCamera ( float setScale, Vector2 centerLogicPos, float setRota )
        {
            scale = setScale;
            rota = setRota;
            rotaMatrix = Matrix.CreateRotationZ( rota );
            rotaMatrixInvert = Matrix.CreateRotationZ( -rota );
            logicCenter = centerLogicPos;
        }

        #endregion

        #region HelpFunctions

        /// <summary>
        /// 将屏幕长度转换到逻辑长度
        /// </summary>
        /// <param name="scrnLength"></param>
        /// <returns></returns>
        public float LogicLength ( int scrnLength )
        {
            return scrnLength / scale;
        }
        /// <summary>
        /// 将屏幕长度转换到逻辑长度
        /// </summary>
        /// <param name="scrnLength"></param>
        /// <returns></returns>
        public float LogicLength ( float scrnLength )
        {
            return scrnLength / scale;
        }
        /// <summary>
        /// 将逻辑长度转换到屏幕长度
        /// </summary>
        /// <param name="logicLength"></param>
        /// <returns></returns>
        public int ScrnLength ( float logicLength )
        {
            return MathTools.Round( logicLength * scale );
        }
        /// <summary>
        /// 将逻辑长度转换到屏幕长度
        /// </summary>
        /// <param name="logicLength"></param>
        /// <returns></returns>
        public float ScrnLengthf ( float logicLength )
        {
            return logicLength * scale;
        }

        /// <summary>
        /// 将屏幕位置转换到逻辑位置
        /// </summary>
        /// <param name="screenPos"></param>
        /// <returns></returns>
        public Vector2 LogicPos ( Vector2 screenPos )
        {
            return Vector2.Transform( screenPos - scrnCenter, rotaMatrix ) / scale + logicCenter;
        }
        /// <summary>
        /// 将逻辑位置转换到屏幕位置
        /// </summary>
        /// <param name="logicPos"></param>
        /// <returns></returns>
        public Vector2 ScreenPos ( Vector2 logicPos )
        {
            return Vector2.Transform( logicPos - logicCenter, rotaMatrixInvert ) * scale + scrnCenter;
        }

        /// <summary>
        /// 将屏幕向量转换到逻辑向量
        /// </summary>
        /// <param name="screenVector"></param>
        /// <returns></returns>
        public Vector2 LogicVector ( Vector2 screenVector )
        {
            return new Vector2( LogicLength( screenVector.X ), LogicLength( screenVector.Y ) );
        }

        #endregion

        /// <summary>
        /// 在逻辑坐标中平移摄像机
        /// </summary>
        /// <param name="delta"></param>
        public void MoveCamera ( Vector2 delta )
        {
            logicCenter += delta;
        }
    }
}
