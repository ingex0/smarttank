using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Effects.Particles
{
    public delegate Vector2 PosGroupFunc( float curPartiTime, Vector2 lastPos, Vector2 dir, int No );

    public delegate Vector2 DirGroupFunc( float curPartiTime, Vector2 lastDir, int No );

    public delegate float RadiusGroupFunc( float curPartiTime, float lastRadius, int No );

    public delegate Color ColorGroupFunc( float curPartiTime, Color lastColor, int No );

    public delegate int CreateSumFunc( float curTime );

    public class ParticleSystem
    {
        #region Variables

        Texture2D tex;

        Vector2 texOrigin;

        Nullable<Rectangle> sourceRect;

        float layerDepth;

        CreateSumFunc createSumFunc;

        PosGroupFunc posGroupFunc;

        DirGroupFunc dirGroupFunc;

        RadiusGroupFunc radiusGroupFunc;

        ColorGroupFunc colorGroupFunc;

        List<Particle> particles;

        float curTime = -1;

        float duration;
        float partiDura;

        Vector2 basePos;

        #endregion

        public Vector2 BasePos
        {
            get { return basePos; }
            set { basePos = value; }
        }

        public ParticleSystem( float duration, float partiDuration, Vector2 basePos,
            Texture2D tex, Vector2 texOrigin, Nullable<Rectangle> sourceRect, float layerDepth,
            CreateSumFunc createSumFunc, PosGroupFunc posGroupFunc, DirGroupFunc dirGroupFunc,
            RadiusGroupFunc radiusGroupFunc, ColorGroupFunc colorGroupFunc )
        {
            this.tex = tex;
            this.texOrigin = texOrigin;
            this.sourceRect = sourceRect;
            this.layerDepth = layerDepth;
            this.createSumFunc = createSumFunc;
            this.posGroupFunc = posGroupFunc;
            this.dirGroupFunc = dirGroupFunc;
            this.radiusGroupFunc = radiusGroupFunc;
            this.colorGroupFunc = colorGroupFunc;

            this.duration = duration;
            this.partiDura = partiDuration;
            this.basePos = basePos;

            this.particles = new List<Particle>();

        }

        #region Updates

        public bool Update()
        {
            curTime++;

            if (duration != 0 && curTime > duration)
                return true;

            CreateNewParticle();

            UpdateParticles();

            return false;
        }

        private void UpdateParticles()
        {
            foreach (Particle particle in particles)
            {
                particle.Update();
            }
        }

        private void CreateNewParticle()
        {
            int createSum = createSumFunc( curTime );

            int preCount = particles.Count;

            for (int i = 0; i < createSum; i++)
            {
                particles.Add( new Particle( partiDura, basePos, layerDepth,
                    delegate( float curParticleTime, Vector2 lastPos, Vector2 dir )
                    {
                        return posGroupFunc( curParticleTime, lastPos, dir, preCount + i );
                    },
                    delegate( float curParticleTime, Vector2 lastDir )
                    {
                        return dirGroupFunc( curParticleTime, lastDir, preCount + i );
                    },
                    delegate( float curParticleTime, float lastRadius )
                    {
                        return radiusGroupFunc( curParticleTime, lastRadius, preCount + i );
                    },
                    tex, texOrigin, sourceRect,
                    delegate( float curParticleTime, Color lastColor )
                    {
                        return colorGroupFunc( curParticleTime, lastColor, preCount + i );
                    } ) );
            }
        }

        #endregion

        #region IDrawableObj 成员

        public void Draw()
        {
            foreach (Particle parti in particles)
            {
                parti.Draw();
            }
        }

        #endregion

        #region IDrawableObj 成员


        public Vector2 Pos
        {
            get { return BasePos; }
        }

        #endregion
    }
}
