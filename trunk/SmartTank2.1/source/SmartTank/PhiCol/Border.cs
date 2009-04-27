using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Graphics;
using Microsoft.Xna.Framework;
using TankEngine2D.DataStructure;
using Microsoft.Xna.Framework.Graphics;
using SmartTank.GameObjs;

namespace SmartTank.PhiCol
{
    /// <summary>
    /// �����߽�
    /// </summary>
    public class Border : ICollideObj, IGameObj
    {
        Rectanglef borderRect;
        BorderChecker colChecker;

        GameObjInfo objInfo;

        /// <summary>
        /// ��ó����߽�ķ�Χ
        /// </summary>
        public Rectanglef BorderRect
        {
            get { return borderRect; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minX">�߽����СX����</param>
        /// <param name="minY">�߽����СY����</param>
        /// <param name="maxX">�߽�����X����</param>
        /// <param name="maxY">�߽�����Y����</param>
        public Border( float minX, float minY, float maxX, float maxY )
            : this( new Rectanglef( minX, minY, maxX - minX, maxY - minY ) )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borderRect">�߽����</param>
        public Border( Rectanglef borderRect )
        {
            this.borderRect = borderRect;
            colChecker = new BorderChecker( borderRect );
            objInfo = new GameObjInfo( "Border", "" );
        }

        #region ICollider ��Ա

        /// <summary>
        /// ��ó�ͻ�����
        /// </summary>
        public IColChecker ColChecker
        {
            get { return (IColChecker)colChecker; }
        }

        #endregion

        #region IGameObj ��Ա

        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        public Vector2 Pos
        {
            get
            {
                return new Vector2( borderRect.X + borderRect.Width * 0.5f, borderRect.Y + borderRect.Height * 0.5f );
            }
            set
            {

            }
        }

        public float Azi
        {
            get { return 0; }
        }

        #endregion

        #region IUpdater ��Ա

        public void Update( float seconds )
        {

        }

        #endregion

        #region IDrawableObj ��Ա

        public void Draw()
        {

        }

        #endregion



        #region IGameObj ��Ա

        public string Name
        {
            get { return "Border"; }
        }

        #endregion
    }

    
}
