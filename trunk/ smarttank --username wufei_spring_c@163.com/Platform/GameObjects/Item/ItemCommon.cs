using System;
using System.Collections.Generic;
using System.Text;
using Platform.PhisicalCollision;
using Platform.Shelter;
using GameBase.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platform.Senses.Vision;

namespace Platform.GameObjects.Item
{
    public class ItemCommon : IGameObj, IPhisicalObj, ICollideObj, IShelterObj, IEyeableObj
    {
        #region Variables

        public event OnCollidedEventHandler OnCollided;
        public event OnCollidedEventHandler OnOverlap;


        GameObjInfo objInfo;

        NonInertiasColUpdater phiUpdater;

        Sprite sprite;

        Vector2[] keyPoints;

        public float Scale
        {
            get { return sprite.Scale; }
            set { sprite.Scale = value; }
        }

        public Vector2 Vel
        {
            get { return phiUpdater.Vel; }
            set { phiUpdater.Vel = value; }
        }

        public Vector2 Pos
        {
            get { return phiUpdater.Pos; }
            set { phiUpdater.Pos = value; }
        }

        public float Azi
        {
            get { return phiUpdater.Azi; }
            set { phiUpdater.Azi = value; }
        }


        #endregion

        public ItemCommon ( string name, string script,
            string texPath, Vector2 texOrigin, float scale, Vector2[] keyPoints,
            Vector2 pos, float azi, Vector2 vel, float rotaVel )
        {
            objInfo = new GameObjInfo( name, script );
            sprite = new Sprite( true, texPath, true );
            sprite.SetParameters( texOrigin, pos, scale, azi, Color.White, LayerDepth.GroundObj, SpriteBlendMode.AlphaBlend );
            sprite.UpdateTransformBounding();
            this.keyPoints = keyPoints;
            phiUpdater = new NonInertiasColUpdater( objInfo, pos, vel, azi, rotaVel, new Sprite[] { sprite } );

            phiUpdater.OnCollied += new OnCollidedEventHandler( phiUpdater_OnCollied );
            phiUpdater.OnOverlap += new OnCollidedEventHandler( phiUpdater_OnOverlap );
        }

        void phiUpdater_OnOverlap ( IGameObj sender, CollisionResult result, GameObjInfo objB )
        {
            if (OnOverlap != null)
                OnOverlap( this, result, objB );
        }

        void phiUpdater_OnCollied ( IGameObj sender, CollisionResult result, GameObjInfo objB )
        {
            if (OnCollided != null)
                OnCollided( this, result, objB );
        }


        #region IGameObj 成员

        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            sprite.UpdateTransformBounding();
        }

        #endregion

        #region IDrawableObj 成员

        public void Draw ()
        {
            sprite.Pos = phiUpdater.Pos;
            sprite.Rata = phiUpdater.Azi;
            sprite.Draw();
        }

        #endregion

        #region IPhisicalObj 成员

        public IPhisicalUpdater PhisicalUpdater
        {
            get { return phiUpdater; }
        }

        #endregion

        #region ICollideObj 成员


        public IColChecker ColChecker
        {
            get { return phiUpdater; }
        }

        #endregion

        #region IShelterObj 成员

        public GameBase.DataStructure.CircleList<GameBase.Graphics.Border> BorderData
        {
            get { return sprite.BorderData; }
        }

        public Microsoft.Xna.Framework.Matrix WorldTrans
        {
            get { return sprite.Transform; }
        }

        public GameBase.DataStructure.Rectanglef BoundingBox
        {
            get { return sprite.BoundRect; }
        }

        #endregion

        #region IEyeableObj 成员

        public Vector2[] KeyPoints
        {
            get { return keyPoints; }
        }

        public Matrix TransMatrix
        {
            get { return sprite.Transform; }
        }

        #endregion



    }
}
