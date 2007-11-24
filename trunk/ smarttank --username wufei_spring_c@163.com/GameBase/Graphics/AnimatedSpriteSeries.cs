using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameBase.Graphics
{
    public class AnimatedSpriteSeries : AnimatedSprite, IDisposable
    {
        static public void LoadResource ( string assetHead, int firstNo, int sumFrame )
        {
            try
            {
                for (int i = 0; i < sumFrame; i++)
                {
                    BaseGame.Content.Load<Texture2D>( assetHead + (firstNo + i) );
                }
            }
            catch (Exception)
            {
                throw new Exception( "导入动画资源错误，请检查图片资源是否完整" );
            }
        }

        #region Variables

        Sprite[] mSprites;

        bool alreadyLoad = false;

        public Sprite CurSprite
        {
            get { return mSprites[mCurFrameIndex]; }
        }

        public Sprite[] Sprites
        {
            get { return mSprites; }
        }

        #endregion

        #region Load Textures

        public void LoadSeriesFromFiles ( string path, string fileHeadName, string extension, int firstNo, int sumFrame, bool supportInterDect )
        {
            if (alreadyLoad)
                throw new Exception( "重复导入动画资源。" );

            alreadyLoad = true;

            mSprites = new Sprite[sumFrame];
            try
            {
                for (int i = 0; i < sumFrame; i++)
                {
                    mSprites[i] = new Sprite();
                    mSprites[i].LoadTextureFromFile( Path.Combine( path, fileHeadName + (i + firstNo) + extension ), supportInterDect );
                }
            }
            catch (Exception)
            {
                mSprites = null;
                alreadyLoad = false;

                throw new Exception( "导入动画资源错误，请检查图片资源是否完整" );
            }

            mSumFrame = sumFrame;
            mCurFrameIndex = 0;
            AnimatedManager.Add( this );
        }

        public void LoadSeriesFormContent ( string assetHead, int firstNo, int sumFrame, bool supportInterDect )
        {
            if (alreadyLoad)
                throw new Exception( "重复导入动画资源。" );

            alreadyLoad = true;

            mSprites = new Sprite[sumFrame];

            try
            {
                for (int i = 0; i < sumFrame; i++)
                {
                    mSprites[i] = new Sprite();
                    mSprites[i].LoadTextureFromContent( assetHead + (firstNo + i), supportInterDect );
                }
            }
            catch (Exception)
            {
                mSprites = null;
                alreadyLoad = false;

                throw new Exception( "导入动画资源错误，请检查图片资源是否完整" );
            }

            mSumFrame = sumFrame;
            mCurFrameIndex = 0;
            AnimatedManager.Add( this );
        }

        #endregion

        #region Dispose

        public void Dispose ()
        {
            foreach (Sprite sprite in mSprites)
            {
                sprite.Dispose();
            }
        }

        #endregion

        #region Set Sprites Parameters

        public void SetSpritesParameters ( Vector2 origin, Vector2 pos, float width, float height, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            foreach (Sprite sprite in mSprites)
            {
                sprite.SetParameters( origin, pos, width, height, rata, color, layerDepth, blendMode );
            }
        }

        public void SetSpritesParameters ( Vector2 origin, Vector2 pos, float scale, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            foreach (Sprite sprite in mSprites)
            {
                sprite.SetParameters( origin, pos, scale, rata, color, layerDepth, blendMode );
            }
        }

        #endregion

        #region Draw Current Frame

        protected override void Draw ()
        {
            CurSprite.Draw();
        }

        #endregion


    }
}
