using System;
using System.Collections.Generic;
using System.Text;
using Platform.PhisicalCollision;
using GameBase.Graphics;
using Microsoft.Xna.Framework;
using Platform.GameObjects;
using GameBase;

namespace Platform.PhisicalCollision
{
    public class NonInertiasColUpdater : NonInertiasPhiUpdater, IColChecker
    {
        Sprite[] collideSprites;
        SpriteColMethod colMethod;

        public NonInertiasColUpdater ( GameObjInfo objInfo, Sprite[] collideSprites )
            : base( objInfo )
        {
            this.collideSprites = collideSprites;
            colMethod = new SpriteColMethod( collideSprites );
        }

        public NonInertiasColUpdater ( GameObjInfo objInfo, Vector2 pos, Vector2 vel, float azi, float angVel, Sprite[] collideSprites )
            : base( objInfo, pos, vel, azi, angVel )
        {
            this.collideSprites = collideSprites;
            colMethod = new SpriteColMethod( collideSprites );
        }

        #region IColChecker ≥…‘±

        public virtual void HandleCollision ( CollisionResult result, GameObjInfo objB )
        {
            Pos += result.NormalVector * Coordin.LogicLength( 0.5f );

            if (OnCollied != null)
                OnCollied( null, result, objB );
        }

        public virtual void HandleOverlap ( CollisionResult result, GameObjInfo objB )
        {
            if (OnOverlap != null)
                OnOverlap( null, result, objB );
        }

        public virtual void ClearNextStatus ()
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
