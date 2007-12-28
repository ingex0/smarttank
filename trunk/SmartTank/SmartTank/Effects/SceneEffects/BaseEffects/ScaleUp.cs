using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SmartTank.Effects.Particles;
using TankEngine2D.Graphics;

namespace SmartTank.Effects.SceneEffects.BaseEffects
{
    public delegate float SingleFunc ( float curTime );

    class ScaleUp : IManagedEffect
    {
        Texture2D tex;

        Vector2 pos;
        Vector2 origin;

        RadiusFunc radiusFunc;
        SingleFunc rotaFunc;

        float duration;

        bool isEnd = false;

        float curTime = -1;

        float curRadius;
        float curRota = 0;

        float layerDepth;

        Rectangle DestinRect;

        public ScaleUp ( string texPath, Vector2 origin, float layerDepth,
            Vector2 pos, RadiusFunc radiusFunc, SingleFunc rotaFunc,
            float duration )
        {
            this.tex = BaseGame.ContentMgr.Load<Texture2D>( texPath );
            this.origin = origin;
            this.pos = pos;
            this.radiusFunc = radiusFunc;
            this.rotaFunc = rotaFunc;
            this.duration = duration;
            this.layerDepth = layerDepth;
            EffectsManager.AddManagedEffect( this );
        }

        #region IManagedEffect 成员

        public bool IsEnd
        {
            get { return isEnd; }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            curTime++;

            if (curTime > duration)
            {
                isEnd = true;
                return;
            }

            curRadius = radiusFunc( curTime, curRadius );
            curRota = rotaFunc( curTime );

            UpdateDestinRect();
        }

        private void UpdateDestinRect ()
        {
            Vector2 ScrnPos = BaseGame.CoordinMgr.ScreenPos( pos );
            float ScrnRadius = BaseGame.CoordinMgr.ScrnLengthf( curRadius );
            DestinRect = new Rectangle( (int)ScrnPos.X, (int)ScrnPos.Y, (int)(2 * ScrnRadius), (int)(2 * ScrnRadius) );
        }

        #endregion

        #region IDrawableObj 成员

        public void Draw ()
        {
            BaseGame.SpriteMgr.alphaSprite.Draw( tex, DestinRect, null, Color.White, curRota, origin, SpriteEffects.None, layerDepth );
        }

        #endregion

        #region IDrawableObj 成员


        public Vector2 Pos
        {
            get { return pos; }
        }

        #endregion
    }
}
