using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameDraw.SceneEffects.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameBase;
using GameBase.Helpers;
using System.IO;
using Platform.GameObjects;

namespace Platform.GameDraw.SceneEffects
{
    public class SmokeGenerater : IManagedEffect
    {
        readonly string texAsset = Path.Combine( Directories.ContentDirectory, "SceneEffects\\CloudSingle" );
        readonly Vector2 texOrigin = new Vector2( 32, 32 );
        readonly float startRadius = 10;


        bool isEnd = false;

        ParticleSystem particleSystem;

        float duaration;
        float curSystemTime = -1;

        IGameObj foucsObj;

        float concen;

        public float Concen
        {
            get { return concen; }
            set { concen = value; }
        }

        public SmokeGenerater ( float duaration, float partiDuara, Vector2 pos, Vector2 dir, float speed, float concen, bool managered )
        {
            this.duaration = duaration;
            this.concen = concen;

            Texture2D tex = BaseGame.Content.Load<Texture2D>( texAsset );
            particleSystem = new ParticleSystem( duaration, partiDuara, pos, tex, texOrigin, null, LayerDepth.EffectLow + 0.01f,
                delegate( float curTime )
                {
                    if (this.concen == 0)
                        return 0;
                    else if (this.concen > 1)
                        return (int)(this.concen);
                    else if (curTime % (int)(1 / this.concen) == 0)
                        return 1;
                    else
                        return 0;
                },
                delegate( float curTime, Vector2 lastPos, Vector2 curDir, int No )
                {
                    if (curTime == 0)
                        return Vector2.Zero + RandomHelper.GetRandomVector2( -1, 1 );

                    else
                        return lastPos + curDir * speed;
                },
                delegate( float curTime, Vector2 lastDir, int No )
                {
                    if (curTime == 0)
                        return dir + RandomHelper.GetRandomVector2( -0.3f, 0.3f );

                    else
                        return lastDir;
                },
                delegate( float curTime, float lastRadius, int No )
                {
                    return startRadius * (1 + 0.1f * curTime);
                },
                delegate( float curTime, Color lastColor, int No )
                {
                    return new Color( 160, 160, 160, Math.Max( (byte)0, (byte)(255 * (partiDuara - curTime) / partiDuara) ) );
                } );

            if (managered)
                EffectsManager.AddManagedEffect( this );
        }

        public SmokeGenerater ( float duaration, float partiDuara, Vector2 dir, float speed, float concen, bool managered,
            IGameObj foucsObj )
            : this( duaration, partiDuara, Vector2.Zero, dir, speed, concen, managered )
        {
            this.foucsObj = foucsObj;
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
            curSystemTime++;
            if (duaration != 0 && curSystemTime > duaration)
            {
                isEnd = true;
                return;
            }

            if (foucsObj != null)
                particleSystem.BasePos = foucsObj.Pos;

            particleSystem.Update();

        }

        #endregion

        #region IDrawableObj 成员

        public void Draw ()
        {
            particleSystem.Draw();
        }

        #endregion

        #region IDrawableObj 成员


        public Vector2 Pos
        {
            get { return particleSystem.Pos; }
        }

        #endregion
    }
}
