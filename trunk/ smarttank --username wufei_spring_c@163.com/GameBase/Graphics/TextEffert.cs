using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Helpers;

namespace GameBase.Graphics
{
    public static class TextEffect
    {
        static LinkedList<ITextEffect> sEffects = new LinkedList<ITextEffect>();

        static public void Draw ()
        {
            LinkedListNode<ITextEffect> node;
            for (node = sEffects.First; node != null; )
            {
                if (node.Value.Ended)
                {
                    node = node.Next;
                    if (node != null)
                        sEffects.Remove( node.Previous );
                    else
                        sEffects.RemoveLast();
                }
                else
                {
                    node.Value.Draw();
                    node = node.Next;
                }
            }
        }

        static public void AddRiseFade ( string text, Vector2 pos, float Scale, Color color, float layerDepth, FontType fontType, float existedFrame, float step )
        {
            sEffects.AddLast( new RiseFadeEffect( text, Coordin.ScreenPos( pos ), Scale, color, layerDepth, fontType, existedFrame, step ) );
        }

        static public void AddRiseFadeInScrnCoordin ( string text, Vector2 pos, float Scale, Color color, float layerDepth, FontType fontType, float existedFrame, float step )
        {
            sEffects.AddLast( new RiseFadeEffect( text, pos, Scale, color, layerDepth, fontType, existedFrame, step ) );
        }


        public static void Clear ()
        {
            sEffects.Clear();
        }

    }


    public interface ITextEffect
    {
        void Draw ();
        bool Ended { get;}
    }

    class RiseFadeEffect : ITextEffect
    {

        #region Variables


        string mText;

        Vector2 mPos;

        float mScale;

        Color mColor;

        float mLayerDepth;

        FontType mFontType;

        float mSumFrame;
        float mFrameRePlatforms;

        float mSetp;

        bool mEnded = false;
        public bool Ended
        {
            get { return mEnded; }
        }
        #endregion

        public RiseFadeEffect ( string text, Vector2 pos, float scale, Color color, float layerDepth, FontType fontType, float existFrame, float step )
        {
            if (scale <= 0 || existFrame <= 1)
                throw new Exception( "scale and existFrame cann't be below Zero!" );

            mText = text;
            //mPos = Coordin.ScreenPos( pos );
            mPos = pos;
            mScale = scale;
            mColor = color;
            mLayerDepth = layerDepth;
            mFontType = fontType;
            mSumFrame = existFrame;
            mFrameRePlatforms = existFrame;
            mSetp = step;
        }

        #region Draw

        public void Draw ()
        {
            if (mFrameRePlatforms <= 0)
            {
                mEnded = true;
                return;
            }
            else
            {
                mFrameRePlatforms--;
                mPos.Y -= mSetp;
                mColor = ColorHelper.ApplyAlphaToColor( mColor, CalAlpha() );
                FontManager.Draw( mText, Coordin.LogicPos( mPos ), mScale, mColor, mLayerDepth, mFontType );
            }
        }

        #endregion

        float CalAlpha ()
        {
            if (mFrameRePlatforms > 0.5f * mSumFrame)
                return 1f;
            else
                return (mFrameRePlatforms - (int)(0.5f * mSumFrame)) / (0.5f * mSumFrame) + 1;
        }









    }
}
