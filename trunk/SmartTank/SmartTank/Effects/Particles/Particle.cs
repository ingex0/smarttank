using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TankEngine2D.Graphics;
using TankEngine2D.Helpers;
using SmartTank;

namespace SmartTank.Effects.Particles
{
    public delegate Vector2 PosFunc( float curTime, Vector2 lastPos, Vector2 dir );

    public delegate Vector2 DirFunc( float curTime, Vector2 lastDir );

    public delegate float RadiusFunc( float curTime, float lastRadius );

    public delegate Color ColorSetTimeFunc( float curTime, Color lastColor );

    public class Particle
    {
        #region Variables

        protected Vector2 basePos;

        protected Vector2 curPos;
        protected float curRadius;
        protected Vector2 curDir;
        protected Color curColor;
        protected float layerDepth;

        float curTime = -1;
        float duration;

        protected PosFunc posFunc;
        protected RadiusFunc radiusFunc;
        protected DirFunc dirFunc;
        protected ColorSetTimeFunc colorFunc;

        Texture2D tex;
        Vector2 texOrign;
        Nullable<Rectangle> sourceRect;

        bool isEnd = false;

        #endregion

        public Particle( float duration, Vector2 basePos, float layerDepth,
            PosFunc posFunc, DirFunc dirFunc, RadiusFunc radiusFunc,
            Texture2D orignTex, Vector2 texOrign, Nullable<Rectangle> sourceRect,
            ColorSetTimeFunc colorFunc )
        {
            this.duration = duration;
            this.basePos = basePos;
            this.layerDepth = layerDepth;

            this.posFunc = posFunc;
            this.dirFunc = dirFunc;
            this.radiusFunc = radiusFunc;
            this.colorFunc = colorFunc;

            this.tex = orignTex;
            this.texOrign = texOrign;
            this.sourceRect = sourceRect;
        }

        public bool Update()
        {
            curTime++;
            if (curTime > duration)
            {
                isEnd = true;
                return true;
            }

            curDir = dirFunc( curTime, curDir );
            curPos = posFunc( curTime, curPos, curDir );
            curRadius = radiusFunc( curTime, curRadius );
            curColor = colorFunc( curTime, curColor );

            return false;
        }

        public void Draw()
        {
            if (!isEnd)
                BaseGame.SpriteMgr.alphaSprite.Draw( tex, BaseGame.CoordinMgr.ScreenPos( basePos + curPos ), sourceRect, curColor, MathTools.AziFromRefPos( curDir ), texOrign,
                            curRadius / (float)tex.Width, SpriteEffects.None, layerDepth );
        }


        #region IDrawableObj ≥…‘±


        public Vector2 Pos
        {
            get { return basePos + curPos; }
        }

        #endregion
    }
}
