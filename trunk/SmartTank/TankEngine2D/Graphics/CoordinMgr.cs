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
    /// �߼���������Ļ����֮��ת���Ĺ�����
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
        /// ��û��������ӿ��е�����
        /// </summary>
        public Rectangle ScrnViewRect
        {
            get { return gameViewRect; }
        }

        /// <summary>
        /// ��û������Ŀ��
        /// </summary>
        public int ViewWidth
        {
            get { return gameViewRect.Width; }
        }
        /// <summary>
        /// ��û������ĸ߶�
        /// </summary>
        public int ViewHeight
        {
            get { return gameViewRect.Height; }
        }

        /// <summary>
        /// ���һ����Ļ���ص��߼���С
        /// </summary>
        public Vector2 TexelSize
        {
            get { return new Vector2( 1 / scale, 1 / scale ); }
        }

        /// <summary>
        /// ��û�����������ķ�λ��
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
        /// ��û�����������������ʣ���Ļ����/�߼����꣩
        /// </summary>
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// ��û����������������߼�λ��
        /// </summary>
        public Vector2 LogicCenter
        {
            get { return logicCenter; }
            set { logicCenter = value; }
        }

        /// <summary>
        /// ��ô��߼����굽��Ļ�������תת������
        /// </summary>
        public Matrix RotaMatrixFromLogicToScrn
        {
            get { return rotaMatrixInvert; }
        }
        /// <summary>
        /// ��ô���Ļ���굽�߼��������תת������
        /// </summary>
        public Matrix RotaMatrixFromScrnToLogic
        {
            get { return rotaMatrix; }
        }


        #endregion

        #region SetFunctions Called By Platform

        /// <summary>
        /// ���û�������
        /// </summary>
        /// <param name="rect"></param>
        public void SetScreenViewRect ( Rectangle rect )
        {
            gameViewRect = rect;
            scrnCenter = new Vector2( rect.X + 0.5f * rect.Width, rect.Y + 0.5f * rect.Height );
        }

        /// <summary>
        /// ���������
        /// </summary>
        /// <param name="setScale">������(��Ļ����/�߼�����)</param>
        /// <param name="centerLogicPos">��������������߼�λ��</param>
        /// <param name="setRota">�������������ת��</param>
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
        /// ����Ļ����ת�����߼�����
        /// </summary>
        /// <param name="scrnLength"></param>
        /// <returns></returns>
        public float LogicLength ( int scrnLength )
        {
            return scrnLength / scale;
        }
        /// <summary>
        /// ����Ļ����ת�����߼�����
        /// </summary>
        /// <param name="scrnLength"></param>
        /// <returns></returns>
        public float LogicLength ( float scrnLength )
        {
            return scrnLength / scale;
        }
        /// <summary>
        /// ���߼�����ת������Ļ����
        /// </summary>
        /// <param name="logicLength"></param>
        /// <returns></returns>
        public int ScrnLength ( float logicLength )
        {
            return MathTools.Round( logicLength * scale );
        }
        /// <summary>
        /// ���߼�����ת������Ļ����
        /// </summary>
        /// <param name="logicLength"></param>
        /// <returns></returns>
        public float ScrnLengthf ( float logicLength )
        {
            return logicLength * scale;
        }

        /// <summary>
        /// ����Ļλ��ת�����߼�λ��
        /// </summary>
        /// <param name="screenPos"></param>
        /// <returns></returns>
        public Vector2 LogicPos ( Vector2 screenPos )
        {
            return Vector2.Transform( screenPos - scrnCenter, rotaMatrix ) / scale + logicCenter;
        }
        /// <summary>
        /// ���߼�λ��ת������Ļλ��
        /// </summary>
        /// <param name="logicPos"></param>
        /// <returns></returns>
        public Vector2 ScreenPos ( Vector2 logicPos )
        {
            return Vector2.Transform( logicPos - logicCenter, rotaMatrixInvert ) * scale + scrnCenter;
        }

        /// <summary>
        /// ����Ļ����ת�����߼�����
        /// </summary>
        /// <param name="screenVector"></param>
        /// <returns></returns>
        public Vector2 LogicVector ( Vector2 screenVector )
        {
            return new Vector2( LogicLength( screenVector.X ), LogicLength( screenVector.Y ) );
        }

        #endregion

        /// <summary>
        /// ���߼�������ƽ�������
        /// </summary>
        /// <param name="delta"></param>
        public void MoveCamera ( Vector2 delta )
        {
            logicCenter += delta;
        }
    }
}
