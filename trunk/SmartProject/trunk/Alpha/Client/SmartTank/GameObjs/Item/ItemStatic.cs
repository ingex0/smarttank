using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.PhiCol;
using TankEngine2D.Graphics;
using Microsoft.Xna.Framework;

namespace SmartTank.GameObjs.Item
{
    public abstract class ItemStatic : IGameObj, ICollideObj, IColChecker
    {
        public event OnCollidedEventHandler OnCollide;
        public event OnCollidedEventHandler OnOverLap;

        protected SpriteColMethod colMethod;
        protected Sprite sprite;

        #region ICollideObj 成员

        public IColChecker ColChecker
        {
            get { return this; }
        }

        #endregion

        #region IGameObj 成员

        protected float azi;
        public float Azi
        {
            get { return azi; }
        }

        protected string mgPath;
        public string MgPath
        {
            get
            {
                return mgPath;
            }
            set
            {
                mgPath = value;
            }
        }

        protected string name;
        public string Name
        {
            get { return name; }
        }

        protected GameObjInfo objInfo;
        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        protected Vector2 pos;
        public Microsoft.Xna.Framework.Vector2 Pos
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
            }
        }

        #endregion

        #region IUpdater 成员

        public abstract void Update(float seconds);

        #endregion

        #region IDrawableObj 成员

        public abstract void Draw();

        #endregion

        #region IColChecker 成员

        public void ClearNextStatus()
        {
        }

        public IColMethod CollideMethod
        {
            get { return colMethod; }
        }

        public void HandleCollision(TankEngine2D.Graphics.CollisionResult result, ICollideObj objB)
        {
            if (OnCollide != null)
                OnCollide(this, result, (objB as IGameObj).ObjInfo);
        }

        public void HandleOverlap(TankEngine2D.Graphics.CollisionResult result, ICollideObj objB)
        {
            if (OnOverLap != null)
                OnOverLap(this, result, (objB as IGameObj).ObjInfo);
        }

        #endregion
    }
}
