using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Graphics;
using SmartTank.Draw;
using SmartTank.PhiCol;
using Microsoft.Xna.Framework;
using SmartTank.AI;
using TankEngine2D.DataStructure;

namespace SmartTank.GameObjs.Tank
{
    public abstract class Tank : IGameObj, ICollideObj, IPhisicalObj
    {
        #region EventHandlers

        /*
         * 坦克类将设计场景交互方面的事件交给游戏规则类来处理。
         * 
         * 这样能消除坦克类和具体游戏规则的耦合，提高坦克类的重用性。
         * 
         * 更多的事件有待添加。
         * 
         * */

        public delegate void ShootEventHandler( Tank sender, Vector2 turretEnd, float azi );

        #endregion

        #region Variables

        protected string name;

        protected GameObjInfo objInfo;

        //protected ITankSkin skin;

        protected IColChecker colChecker;

        protected IPhisicalUpdater phisicalUpdater;

        protected IAI tankAI;

        protected bool isDead;

        #endregion

        #region Properties

        public bool IsDead
        {
            get { return isDead; }
        }
        #endregion

        #region IGameObj 成员

        public virtual GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        public abstract Vector2 Pos { get;set;}

        public abstract float Azi { get;set;}

        #endregion

        #region IUpdater 成员

        public virtual void Update( float seconds )
        {
            if (!isDead && tankAI != null)
                tankAI.Update( seconds );
        }

        #endregion

        #region IDrawable 成员

        public virtual void Draw()
        {
            tankAI.Draw();
        }

        #endregion

        #region ICollideObj 成员


        public IColChecker ColChecker
        {
            get { return colChecker; }
        }

        #endregion

        #region IPhisicalObj 成员

        public IPhisicalUpdater PhisicalUpdater
        {
            get { return phisicalUpdater; }
        }

        #endregion

        #region SetTankAI

        public void SetTankAI( IAI tankAI )
        {
            if (tankAI == null)
                throw new NullReferenceException( "tankAI is null!" );
            this.tankAI = tankAI;
        }

        #endregion

        #region Dead

        public virtual void Dead()
        {
            isDead = true;
        }

        #endregion

        #region IHasBorderObj 成员

        public abstract CircleList<BorderPoint> BorderData
        {
            get;
        }


        public abstract Matrix WorldTrans
        {
            get;
        }

        public abstract Rectanglef BoundingBox
        {
            get;
        }

        #endregion



        #region IGameObj 成员

        public string Name
        {
            get { return name; }
        }

        #endregion
    }
}
