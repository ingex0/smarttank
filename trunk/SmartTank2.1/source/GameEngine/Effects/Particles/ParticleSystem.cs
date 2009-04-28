using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEngine.Effects.Particles
{
    public delegate Vector2 PosGroupFunc( float curPartiTime,float deltaTime, Vector2 lastPos, Vector2 dir, int No );

    public delegate Vector2 DirGroupFunc( float curPartiTime, float deltaTime, Vector2 lastDir, int No );

    public delegate float RadiusGroupFunc( float curPartiTime, float deltaTime, float lastRadius, int No );

    public delegate Color ColorGroupFunc( float curPartiTime, float deltaTime, Color lastColor, int No );

    public delegate int CreateSumFunc( float curTime, ref float timer );


    public class ParticleSystem: IManagedEffect
    {
        #region Variables

        Texture2D tex;

        Vector2 texOrigin;

        Nullable<Rectangle> sourceRect;

        float layerDepth;

        public CreateSumFunc createSumFunc;

        public PosGroupFunc posGroupFunc;

        public DirGroupFunc dirGroupFunc;

        public RadiusGroupFunc radiusGroupFunc;

        public ColorGroupFunc colorGroupFunc;

        List<Particle> particles;

        float curTime = -1;

        float duration;
        float partiDura;

        Vector2 basePos;

        bool isEnd = false;

        protected float createTimer = 0;

        #endregion

        public Vector2 BasePos
        {
            get { return basePos; }
            set { basePos = value; }
        }

        public ParticleSystem()
        {
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

        #region Update

        public void Update(float seconds)
        {
            curTime += seconds;
            if (duration != 0 && curTime > duration)
            {
                isEnd = true;
            }

            CreateNewParticle(seconds);

            UpdateParticles(seconds);
        }

        protected virtual void UpdateParticles( float seconds )
        {
            foreach (Particle particle in particles)
            {
                particle.Update(seconds);
            }
        }

        protected virtual void CreateNewParticle(float seconds)
        {
            createTimer += seconds;
            int createSum = createSumFunc( curTime, ref  createTimer);
            int preCount = particles.Count;

            for (int i = 0; i < createSum; i++)
            {
                particles.Add( new Particle( partiDura, basePos, layerDepth,
                    delegate( float curParticleTime,float deltaTime, Vector2 lastPos, Vector2 dir )
                    {
                        return posGroupFunc( curParticleTime,deltaTime, lastPos, dir, preCount + i );
                    },
                    delegate( float curParticleTime, float deltaTime, Vector2 lastDir )
                    {
                        return dirGroupFunc( curParticleTime, deltaTime, lastDir, preCount + i );
                    },
                    delegate( float curParticleTime, float deltaTime, float lastRadius )
                    {
                        return radiusGroupFunc( curParticleTime, deltaTime, lastRadius, preCount + i );
                    },
                    tex, texOrigin, sourceRect,
                    delegate( float curParticleTime, float deltaTime, Color lastColor )
                    {
                        return colorGroupFunc( curParticleTime, deltaTime, lastColor, preCount + i );
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

        #region IManagedEffect 成员

        public bool IsEnd
        {
            get { return isEnd; }
        }

       

        #endregion
    }
}
