using System;
using System.Collections.Generic;
using System.Text;
using GameEngine.PhiCol;
using GameEngine.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.PhiCol
{
    public class NonInertiasColUpdater : NonInertiasPhiUpdater, IColChecker
    {
        Sprite[] collideSprites;
        SpriteColMethod colMethod;

        public NonInertiasColUpdater( Sprite[] collideSprites )
            : base()
        {
            this.collideSprites = collideSprites;
            colMethod = new SpriteColMethod( collideSprites );
        }

        public NonInertiasColUpdater( Vector2 pos, Vector2 vel, float azi, float angVel, Sprite[] collideSprites )
            : base(pos, vel, azi, angVel )
        {
            this.collideSprites = collideSprites;
            colMethod = new SpriteColMethod( collideSprites );
        }

        #region IColChecker ≥…‘±

        public virtual void HandleCollision( CollisionResult result,ICollideObj objA, ICollideObj objB )
        {
            Pos += result.NormalVector * BaseGame.CoordinMgr.LogicLength( 0.5f );

            if (OnCollied != null)
                OnCollied( result, objA, objB );
        }

        public virtual void HandleOverlap( CollisionResult result, ICollideObj objA, ICollideObj objB )
        {
            if (OnOverlap != null)
                OnOverlap( result, objA, objB );
        }

        public virtual void ClearNextStatus()
        {
            nextPos = Pos;
            nextAzi = Azi;
        }

        public IColMethod CollideMethod
        {
            get
            {
                foreach (Sprite sprite in collideSprites)
                {
                    sprite.Pos = nextPos;
                    sprite.Rata = nextAzi;
                }
                return colMethod;
            }
        }

        #endregion

        #region Event

        public event OnCollidedEventHandler OnCollied;

        public event OnCollidedEventHandler OnOverlap;

        #endregion




    }
}
