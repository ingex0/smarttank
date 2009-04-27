using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Common.Helpers;

namespace GameEngine.Effects.TextEffects
{
    /// <summary>
    /// ʵ��һ���������������ϱ��͸����������Ч
    /// </summary>
    class FadeUpEffect : ITextEffect
    {

        #region Variables


        string text;

        Vector2 pos;

        float scale;

        Color color;

        float layerDepth;

        string fontType;

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
        public FadeUpEffect( string text, bool inLogic, Vector2 pos, float scale, Color color, float layerDepth, string fontType, float existFrame, float step )
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
        public void Draw()
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
                    BaseGame.FontMgr.DrawInScrnCoord( text, BaseGame.CoordinMgr.ScreenPos( pos ) - new Vector2( 0, curDest ), scale, color, layerDepth, fontType );
                else
                    BaseGame.FontMgr.DrawInScrnCoord( text, pos - new Vector2( 0, curDest ), scale, color, layerDepth, fontType );
            }
        }

        #endregion

        float CalAlpha()
        {
            if (frameRePlatforms > 0.5f * sumFrame)
                return 1f;
            else
                return (frameRePlatforms - (int)(0.5f * sumFrame)) / (0.5f * sumFrame) + 1;
        }
    }
}
