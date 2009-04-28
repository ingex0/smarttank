using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Effects.Particles
{
    class PointSource : ParticleSystem
    {
        bool isOpen = true;

        public bool IsOpen
        {
            get { return isOpen; }
            set { isOpen = value; }
        }

        Vector2 curDir;

        public Vector2 CurDir
        {
            get { return curDir; }
            set { curDir = value; }
        }

        public PointSource( Vector2 pos, Vector2 direct, float vel, float partiDura, float CreateRate, bool isOpen,
            Texture2D tex, Vector2 texOrign, Nullable<Rectangle> sourceRect, float layerDepth )
        {
            this.BasePos = pos;
            this.CurDir = direct;
            this.
        }

        public PointSource( Vector2 pos, Vector2 direct, float vel, float partiDura, float CreateRate, bool isOpen,
            Texture2D tex, Vector2 texOrign, Nullable<Rectangle> sourceRect, float layerDepth )
            : base( 0, partiDura, pos, tex, texOrign, sourceRect, layerDepth,
            delegate( float curTime, ref float timer )
            {
                if (CreateRate != 0)
                {
                    if (timer > 1 / CreateRate)
                    {
                        float sumInFloat = timer * CreateRate;
                        int createSum = (int)sumInFloat;
                        timer -= (float)createSum / CreateRate;
                        return createSum;
                    }
                }
                return 0;
            },
            delegate( float curPartiTime, float deltaTime, Vector2 lastPos, Vector2 dir, int No )
            {
                if (curPartiTime == 0)
                    return Vector2.Zero;
                return deltaTime * dir + lastPos;
            },
            this.DirFunction,
            delegate( float curPartiTime, float deltaTime, float lastRadius, int No )
            {
                return lastRadius;
            },
            delegate( float curPartiTime, float deltaTime, Color lastColor, int No )
            {
                return Color.White;
            } )
        {
            this.isOpen = isOpen;
        }

        protected override void CreateNewParticle( float seconds )
        {
            if (this.isOpen)
                base.CreateNewParticle( seconds );
        }

        protected virtual Vector2 DirFunction( float curPartiTime, float deltaTime, Vector2 lastDir, int No )
        {
            if (curPartiTime == 0)
                return curDir;
            else
                return lastDir;
        }
    }
}
