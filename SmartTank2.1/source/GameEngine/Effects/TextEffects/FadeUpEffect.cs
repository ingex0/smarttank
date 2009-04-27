using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Common.Helpers;

namespace GameEngine.Effects.TextEffects
{
    /// <summary>
    /// 实现一个不断上升并不断变得透明的文字特效
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
        /// <param name="text">文字内容</param>
        /// <param name="pos">显示的屏幕位置</param>
        /// <param name="Scale">大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">绘制深度</param>
        /// <param name="fontType">字体</param>
        /// <param name="existedFrame">保持在屏幕中的时间循环次数</param>
        /// <param name="step">每次时间循环中，上升的高度，以像素为单位</param>
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
        /// 绘制并更新自己
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
