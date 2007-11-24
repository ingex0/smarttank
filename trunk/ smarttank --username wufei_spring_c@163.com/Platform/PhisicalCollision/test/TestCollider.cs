using System;
using System.Collections.Generic;
using System.Text;
using Platform.PhisicalCollision;
using GameBase.Graphics;
using Platform.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platform.PhisicalCollision.test
{
    class TestCollider : IPhisicalObj
    {
        GameObjInfo objInfo;

        NonInertiasColUpdater collideUpdater;

        Sprite[] sprites;

        public TestCollider ( string script, Vector2 pos, Vector2 vel, float azi, float angVel, Color color )
        {
            objInfo = new GameObjInfo( "TestCollider", script );

            sprites = new Sprite[]
            {
                new Sprite(true,"Test\\item6",true)
            };

            sprites[0].SetParameters( new Vector2( 30, 30 ), pos, 30, 30, 0f, color, 0f, SpriteBlendMode.AlphaBlend );

            collideUpdater = new NonInertiasColUpdater( objInfo, pos, vel, azi, angVel, sprites );
        }

        #region IPhisicalObj 成员

        public IPhisicalUpdater PhisicalUpdater
        {
            get { return collideUpdater; }
        }

        #endregion

        #region IDrawable 成员

        public void Draw ()
        {
            foreach (Sprite sprite in sprites)
            {
                sprite.Pos = collideUpdater.Pos;
                sprite.Rata = collideUpdater.Azi;
                sprite.Draw();
            }
        }

        #endregion
    }
}
