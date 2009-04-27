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
    /// 场景边界
    /// </summary>
    public class Border : ICollideObj, IGameObj
    {
        Rectanglef borderRect;
        BorderChecker colChecker;

        GameObjInfo objInfo;

        /// <summary>
        /// 获得场景边界的范围
        /// </summary>
        public Rectanglef BorderRect
        {
            get { return borderRect; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="minX">边界的最小X坐标</param>
        /// <param name="minY">边界的最小Y坐标</param>
        /// <param name="maxX">边界的最大X坐标</param>
        /// <param name="maxY">边界的最大Y坐标</param>
        public Border( float minX, float minY, float maxX, float maxY )
            : this( new Rectanglef( minX, minY, maxX - minX, maxY - minY ) )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borderRect">边界矩形</param>
        public Border( Rectanglef borderRect )
        {
            this.borderRect = borderRect;
            colChecker = new BorderChecker( borderRect );
            objInfo = new GameObjInfo( "Border", "" );
        }

        #region ICollider 成员

        /// <summary>
        /// 获得冲突检查者
        /// </summary>
        public IColChecker ColChecker
        {
            get { return (IColChecker)colChecker; }
        }

        #endregion

        #region IGameObj 成员

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

        #region IUpdater 成员

        public void Update( float seconds )
        {

        }

        #endregion

        #region IDrawableObj 成员

        public void Draw()
        {

        }

        #endregion



        #region IGameObj 成员

        public string Name
        {
            get { return "Border"; }
        }

        #endregion
    }

    
}
