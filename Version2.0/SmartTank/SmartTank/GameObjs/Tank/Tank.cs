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
         * ̹���ཫ��Ƴ�������������¼�������Ϸ������������
         * 
         * ����������̹����;�����Ϸ�������ϣ����̹����������ԡ�
         * 
         * ������¼��д���ӡ�
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

        #region IGameObj ��Ա

        public virtual GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        public abstract Vector2 Pos { get;set;}

        public abstract float Azi { get;set;}

        #endregion

        #region IUpdater ��Ա

        public virtual void Update( float seconds )
        {
            if (!isDead && tankAI != null)
                tankAI.Update( seconds );
        }

        #endregion

        #region IDrawable ��Ա

        public virtual void Draw()
        {
            tankAI.Draw();
        }

        #endregion

        #region ICollideObj ��Ա


        public IColChecker ColChecker
        {
            get { return colChecker; }
        }

        #endregion

        #region IPhisicalObj ��Ա

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

        #region IHasBorderObj ��Ա

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



        #region IGameObj ��Ա

        public string Name
        {
            get { return name; }
        }

        #endregion
    }
}
