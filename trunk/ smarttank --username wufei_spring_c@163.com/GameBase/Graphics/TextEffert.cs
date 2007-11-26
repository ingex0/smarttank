using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Helpers;

namespace GameBase.Graphics
{
    /// <summary>
    /// ����Ч���Ĺ�����
    /// </summary>
    public static class TextEffect
    {
        static LinkedList<ITextEffect> sEffects = new LinkedList<ITextEffect>();

        /// <summary>
        /// ���Ƶ�ǰ����Ч���岢��������
        /// </summary>
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

        /// <summary>
        /// ָ���߼��������һ��RiseFade������Ч
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">��ʾ���߼�λ��</param>
        /// <param name="Scale">��С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">�������</param>
        /// <param name="fontType">����</param>
        /// <param name="existedFrame">��������Ļ�е�ʱ��ѭ������</param>
        /// <param name="step">ÿ��ʱ��ѭ���У������ĸ߶ȣ�������Ϊ��λ</param>
        static public void AddRiseFade ( string text, Vector2 pos, float Scale, Color color, float layerDepth, FontType fontType, float existedFrame, float step )
        {
            sEffects.AddLast( new RiseFadeEffect( text, Coordin.ScreenPos( pos ), Scale, color, layerDepth, fontType, existedFrame, step ) );
        }

        /// <summary>
        /// ����Ļ���������һ��RiseFade������Ч
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">��ʾ����Ļλ��</param>
        /// <param name="Scale">��С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">�������</param>
        /// <param name="fontType">����</param>
        /// <param name="existedFrame">��������Ļ�е�ʱ��ѭ������</param>
        /// <param name="step">ÿ��ʱ��ѭ���У������ĸ߶ȣ�������Ϊ��λ</param>
        static public void AddRiseFadeInScrnCoordin ( string text, Vector2 pos, float Scale, Color color, float layerDepth, FontType fontType, float existedFrame, float step )
        {
            sEffects.AddLast( new RiseFadeEffect( text, pos, Scale, color, layerDepth, fontType, existedFrame, step ) );
        }

        /// <summary>
        /// ������е�������Ч
        /// </summary>
        public static void Clear ()
        {
            sEffects.Clear();
        }

    }

    /// <summary>
    /// ����������Ч��һ�㷽��
    /// </summary>
    public interface ITextEffect
    {
        /// <summary>
        /// ���Ʋ������Լ�
        /// </summary>
        void Draw ();

        /// <summary>
        /// �Ƿ��Ѿ�����
        /// </summary>
        bool Ended { get;}
    }

    /// <summary>
    /// ʵ��һ���������������ϱ��͸����������Ч
    /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public bool Ended
        {
            get { return mEnded; }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">��ʾ����Ļλ��</param>
        /// <param name="Scale">��С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">�������</param>
        /// <param name="fontType">����</param>
        /// <param name="existedFrame">��������Ļ�е�ʱ��ѭ������</param>
        /// <param name="step">ÿ��ʱ��ѭ���У������ĸ߶ȣ�������Ϊ��λ</param>
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

        /// <summary>
        /// ���Ʋ������Լ�
        /// </summary>
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
