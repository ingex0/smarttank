using System;
using System.Collections.Generic;
using System.Text;
using Platform.PhisicalCollision;
using GameBase.Graphics;
using Microsoft.Xna.Framework;
using GameBase.Helpers;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Platform.Scene;

namespace Platform.GameObjects.Shell
{
    public class ShellNormal : IGameObj, ICollideObj, IPhisicalObj
    {
        readonly string texPath = "GameObjs\\ShellNormal";

        public event OnCollidedEventHandler onCollided;
        public event OnCollidedEventHandler onOverlap;


        GameObjInfo objInfo = new GameObjInfo( "ShellNormal", string.Empty );

        IGameObj firer;

        Sprite sprite;

        NonInertiasColUpdater phiUpdater;

        public float Azi
        {
            get { return sprite.Rata; }
        }

        public IGameObj Firer
        {
            get { return firer; }
        }

        public ShellNormal ( IGameObj firer, Vector2 startPos, float startAzi, float speed )
        {
            this.firer = firer;
            sprite = new Sprite( true, Path.Combine( Directories.ContentDirectory, texPath ), true );
            sprite.SetParameters( new Vector2( 5, 0 ), startPos, 0.08f, startAzi, Color.White, LayerDepth.Shell, SpriteBlendMode.AlphaBlend );
            phiUpdater = new NonInertiasColUpdater( ObjInfo, startPos, MathTools.NormalVectorFromAzi( startAzi ) * speed, startAzi, 0f, new Sprite[] { sprite } );
            phiUpdater.OnOverlap += new OnCollidedEventHandler( phiUpdater_OnOverlap );
            phiUpdater.OnCollied += new OnCollidedEventHandler( phiUpdater_OnCollied );
        }

        void phiUpdater_OnCollied ( IGameObj Sender, CollisionResult result, GameObjInfo objB )
        {
            if (onCollided != null)
                onCollided( this, result, objB );
        }

        void phiUpdater_OnOverlap ( IGameObj Sender, CollisionResult result, GameObjInfo objB )
        {
            if (objB.Name == "Border")
            {
                ((SceneKeeperCommon)(GameManager.CurSceneKeeper)).RemoveGameObj( this, true, false, false, false, SceneKeeperCommon.GameObjLayer.lowFlying );
            }

            if (onOverlap != null)
                onOverlap( this, result, objB );
        }


        #region IGameObj 成员

        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }
        public Vector2 Pos
        {
            get { return phiUpdater.Pos; }
            set { phiUpdater.Pos = value; }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {

        }

        #endregion

        #region IDrawable 成员

        public void Draw ()
        {
            sprite.Pos = phiUpdater.Pos;
            sprite.Draw();
        }

        #endregion

        #region ICollideObj 成员


        public IColChecker ColChecker
        {
            get { return phiUpdater; }
        }

        #endregion

        #region IPhisicalObj 成员

        public IPhisicalUpdater PhisicalUpdater
        {
            get { return phiUpdater; }
        }

        #endregion


        #region IHasBorderObj 成员

        public GameBase.DataStructure.CircleList<GameBase.Graphics.Border> BorderData
        {
            get { return sprite.BorderData; }
        }

        public Matrix WorldTrans
        {
            get { return sprite.Transform; }
        }

        public GameBase.DataStructure.Rectanglef BoundingBox
        {
            get { return sprite.BoundRect; }
        }

        #endregion
    }
}
