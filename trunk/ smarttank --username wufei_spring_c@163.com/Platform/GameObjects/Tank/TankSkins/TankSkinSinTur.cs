using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Helpers;
using GameBase;

namespace Platform.GameObjects.Tank.TankSkins
{
    public class TankSkinSinTur
    {
        #region Variables

        TankSkinSinTurData data;

        Sprite baseSprite;
        Sprite turretSprite;

        TurretLinker turretLinker;


        bool recoiling = false;
        bool backing = false;
        int leftBackingFrame;
        int leftRecoilFrame;

        /// <summary>
        /// 以原图像素为单位
        /// </summary>
        Vector2 curTurretAxesPosInPix;

        #endregion

        #region Properties

        public Vector2 Pos
        {
            get { return BasePos; }
        }

        public Vector2 BasePos
        {
            get { return baseSprite.Pos; }
        }

        public float BaseAzi
        {
            get { return baseSprite.Rata; }
        }

        public float TurretAimAzi
        {
            get { return turretSprite.Rata; }
        }

        public float TurretAzi
        {
            get { return turretSprite.Rata - baseSprite.Rata; }
        }

        public Vector2 TurretAxesPos
        {
            get
            {
                return turretSprite.Pos;
            }
        }

        public Vector2 TurretEndPos
        {
            get
            {
                return TurretAxesPos + MathTools.NormalVectorFromAzi( TurretAimAzi ) * data.TurretTexels;
            }
        }

        #endregion

        #region Constuctions

        public TankSkinSinTur ( TankSkinSinTurData data )
        {
            this.data = data;
        }

        public void Initial ( Vector2 pos, float baseRota, float turretRota )
        {
            InitialTurretLinker();
            InitialSprites( pos, baseRota, turretRota );
        }

        private void InitialSprites ( Vector2 pos, float baseRota, float turretRota )
        {
            baseSprite = new Sprite();
            baseSprite.LoadTextureFromFile( data.baseTexPath, true );
            baseSprite.SetParameters( data.baseTexOrigin, pos, data.texScale, baseRota, Color.White, LayerDepth.TankBase, SpriteBlendMode.AlphaBlend );

            turretSprite = new Sprite();
            turretSprite.LoadTextureFromFile( data.turretTexPath, true );
            turretSprite.SetParameters( data.turretTexOrigin, turretLinker.GetTexturePos( pos, baseRota ), data.texScale, turretRota + baseRota, Color.White, LayerDepth.TankTurret, SpriteBlendMode.AlphaBlend );
        }

        private void InitialTurretLinker ()
        {
            curTurretAxesPosInPix = data.turretAxesPos;
            turretLinker = new TurretLinker( curTurretAxesPosInPix, data.baseTexOrigin, data.texScale );
        }

        #endregion

        #region IDrawableObj 成员

        public void Draw ()
        {
            baseSprite.Draw();
            turretSprite.Draw();
        }

        #endregion

        #region ITankSkin 成员

        public Sprite[] Sprites
        {
            get { return new Sprite[] { baseSprite, turretSprite }; }
        }

        public void ResetSprites ( Vector2 pos, float baseRota, float turretRota )
        {
            baseSprite.Pos = pos;
            baseSprite.Rata = baseRota;
            turretSprite.Pos = turretLinker.GetTexturePos( pos, baseRota );
            turretSprite.Rata = baseRota + turretRota;
        }

        public float TurretLength
        {
            get { return data.TurretTexels * data.texScale; }
        }

        #endregion

        #region GetTurretEndPos

        public Vector2 GetTurretEndPos ( Vector2 tankPos, float tankAzi, float turretAzi )
        {
            return turretLinker.GetTexturePos( tankPos, tankAzi ) + MathTools.NormalVectorFromAzi( tankAzi + turretAzi ) * data.TurretTexels * data.texScale;
        }

        #endregion

        #region Recoil Animation

        public void BeginRecoil ()
        {
            recoiling = true;
            backing = true;
            leftRecoilFrame = data.recoilFrame;
            leftBackingFrame = data.backFrame;
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            if (recoiling)
            {
                if (backing)
                {
                    //Log.Write( "leftBackingFrame = " + leftBackingFrame.ToString() );
                    //curTurretAxesPosInPix = turretAxesPos - MathTools.NormalVectorFromAzi( TurretAzi ) * (backFrame - leftBackingFrame) * recoilTexels;
                    curTurretAxesPosInPix = data.turretAxesPos - MathTools.NormalVectorFromAzi( TurretAzi ) * data.backFrame * Coordin.ScrnLengthf( data.recoilTexels );
                    leftBackingFrame--;
                    if (leftBackingFrame == 0)
                        backing = false;
                }
                else
                {
                    //Log.Write( "leftRecoilFrame = " + leftRecoilFrame.ToString() );

                    curTurretAxesPosInPix = data.turretAxesPos - MathTools.NormalVectorFromAzi( TurretAzi ) * leftRecoilFrame * Coordin.ScrnLengthf( data.recoilTexels );
                    leftRecoilFrame--;
                    if (leftRecoilFrame == 0)
                        recoiling = false;
                }
                turretLinker.SetTurretAxes( curTurretAxesPosInPix );


            }
        }

        #endregion

        public Vector2[] VisiKeyPoints
        {
            get { return data.visiKeyPoints; }
        }
    }
}
