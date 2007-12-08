using System;
using System.Collections.Generic;
using System.Text;
using Platform.PhisicalCollision;
using Platform.Shelter;
using Platform.Senses.Vision;
using Microsoft.Xna.Framework;
using GameBase.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Platform.GameObjects.Item
{
    public class ObstacleCommon : IGameObj, ICollideObj, IShelterObj, IEyeableObj, IColChecker
    {
        //static public IEyeableInfo GetEyeableInfoHandlerCommon ( IRaderOwner raderOwner, IEyeableObj obstacle )
        //{
        //    return new ObstacleCommomEyeableInfo( (ObstacleCommon)obstacle );
        //}

        //public class ObstacleCommomEyeableInfo : IEyeableInfo
        //{
        //    EyeableInfo eyeableInfo;

        //    Vector2 pos;
        //    GameObjInfo objInfo;

        //    public ObstacleCommomEyeableInfo ( ObstacleCommon obstacle )
        //    {
        //        this.pos = obstacle.pos;
        //        this.objInfo = obstacle.objInfo;
        //    }

        //    public Vector2 Pos
        //    {
        //        get { return pos; }
        //    }

        //    #region IEyeableInfo 成员

        //    public GameObjInfo ObjInfo
        //    {
        //        get { return objInfo; }
        //    }

        //    #endregion
        //}

        public event OnCollidedEventHandler OnCollided;
        public event OnCollidedEventHandler OnOverLap;

        GameObjInfo objInfo;
        Vector2 pos;
        float azi;
        SpriteColMethod colMethod;
        Sprite sprite;
        Vector2[] keyPoints;


        public ObstacleCommon ( GameObjInfo objInfo, Vector2 pos, float azi,
            string texAssetPath, Vector2 origin, float scale, Color color, float layerDepth, Vector2[] keyPoints )
        {
            this.objInfo = objInfo;
            this.pos = pos;
            this.azi = azi;
            this.sprite = new Sprite( true, texAssetPath, true );
            this.sprite.SetParameters( origin, pos, scale, azi, color, layerDepth, SpriteBlendMode.AlphaBlend );
            this.keyPoints = keyPoints;
            this.colMethod = new SpriteColMethod( new Sprite[] { this.sprite } );
        }

        #region IGameObj 成员

        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        public Microsoft.Xna.Framework.Vector2 Pos
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
            }
        }

        public float Azi
        {
            get { return azi; }
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
            sprite.Draw();
        }

        #endregion

        #region ICollideObj 成员


        public IColChecker ColChecker
        {
            get { return this; }
        }

        #endregion

        #region IHasBorderObj 成员

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

        public Microsoft.Xna.Framework.Vector2[] KeyPoints
        {
            get { return keyPoints; }
        }

        public Microsoft.Xna.Framework.Matrix TransMatrix
        {
            get { return sprite.Transform; }
        }

        GetEyeableInfoHandler getEyeableInfoHandler;

        public GetEyeableInfoHandler GetEyeableInfoHandler
        {
            get
            {
                return getEyeableInfoHandler ;
            }
            set
            {
                getEyeableInfoHandler = value;
            }
        }

        #endregion

        #region IColChecker 成员

        public IColMethod CollideMethod
        {
            get { return colMethod; }
        }

        public void HandleCollision ( GameBase.Graphics.CollisionResult result, GameObjInfo objB )
        {
            if (OnCollided != null)
                OnCollided( this, result, objB );
        }

        public void ClearNextStatus ()
        {

        }

        public void HandleOverlap ( GameBase.Graphics.CollisionResult result, GameObjInfo objB )
        {
            if (OnOverLap != null)
                OnOverLap( this, result, objB );
        }

        #endregion

    }
}
