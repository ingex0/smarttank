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
            sEffects.AddLast( new RiseFadeEffect( text, true, pos, Scale, color, layerDepth, fontType, existedFrame, step ) );
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
            sEffects.AddLast( new RiseFadeEffect( text, false, pos, Scale, color, layerDepth, fontType, existedFrame, step ) );
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


        string text;

        Vector2 pos;

        float scale;

        Color color;

        float layerDepth;

        FontType fontType;

        float sumFrame;
        float frameRePlatforms;

        float step;

        bool ended = false;

        bool inLogic = false;

        float curDest = 0;

        /// <summary>
        /// 
        /// </summary>
        public bool Ended
        {
            get { return ended; }
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
        public RiseFadeEffect ( string text, bool inLogic, Vector2 pos, float scale, Color color, float layerDepth, FontType fontType, float existFrame, float step )
        {
            if (scale <= 0 || existFrame <= 1)
                throw new Exception( "scale and existFrame cann't be below Zero!" );

            this.text = text;
            this.inLogic = inLogic;
            this.pos = pos;
            this.scale = scale;
            this.color = color;
            this.layerDepth = layerDepth;
            this.fontType = fontType;
            this.sumFrame = existFrame;
            this.frameRePlatforms = existFrame;
            this.step = step;
        }

        #region Draw

        /// <summary>
        /// ���Ʋ������Լ�
        /// </summary>
        public void Draw ()
        {
            if (frameRePlatforms <= 0)
            {
                ended = true;
                return;
            }
            else
            {
                frameRePlatforms--;
                curDest += step;
                color = ColorHelper.ApplyAlphaToColor( color, CalAlpha() );
                if (inLogic)
                    FontManager.DrawInScrnCoord( text, Coordin.ScreenPos( pos ) - new Vector2( 0, curDest ), scale, color, layerDepth, fontType );
                else
                    FontManager.DrawInScrnCoord( text, pos - new Vector2( 0, curDest ), scale, color, layerDepth, fontType );
            }
        }

        #endregion

        float CalAlpha ()
        {
            if (frameRePlatforms > 0.5f * sumFrame)
                return 1f;
            else
                return (frameRePlatforms - (int)(0.5f * sumFrame)) / (0.5f * sumFrame) + 1;
        }









    }
}
