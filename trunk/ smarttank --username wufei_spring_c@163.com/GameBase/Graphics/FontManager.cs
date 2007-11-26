using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using GameBase.Helpers;

namespace GameBase.Graphics
{
    /// <summary>
    /// 提供字符的绘制功能。
    /// 暂时无法显示中文。
    /// </summary>
    public static class FontManager
    {
        #region Varibles
        internal static SpriteBatch textSpriteBatch;

        static SpriteFont comicFont;

        static SpriteFont lucidaFont;

        static internal void Initial ()
        {
            textSpriteBatch = new SpriteBatch( BaseGame.Device );
            comicFont = BaseGame.Content.Load<SpriteFont>( Path.Combine( Directories.FontContent, "SpriteFontComit" ) );
            lucidaFont = BaseGame.Content.Load<SpriteFont>( Path.Combine( Directories.FontContent, "SpriteFontLucida" ) );
        }

        #endregion

        #region SpriteBatch Methods
        /// <summary>
        /// textSpriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None ); 
        /// </summary>
        internal static void SpriteBatchBegin ()
        {
            textSpriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None );
        }

        /// <summary>
        /// textSpriteBatch.End();
        /// </summary>
        internal static void SpriteBatchEnd ()
        {
            textSpriteBatch.End();
        }

        internal static void HandleDeviceReset ()
        {
            Initial();
        }

        #endregion

        #region Draw Functions

        /// <summary>
        /// 在逻辑坐标中绘制一段文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在逻辑坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        /// <param name="fontType">字体</param>
        static public void Draw ( string text, Vector2 pos, float scale, Color color, float layerDepth, FontType fontType )
        {
            switch (fontType)
            {
                case FontType.Comic:
                    DrawComic( text, pos, scale, color, layerDepth );
                    break;
                case FontType.Lucida:
                    DrawLucida( text, pos, scale, color, layerDepth );
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 在逻辑坐标中绘制一段Comic字体的文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在逻辑坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        static public void DrawComic ( string text, Vector2 pos, float scale, Color color, float layerDepth )
        {
            textSpriteBatch.DrawString( comicFont, text, Coordin.ScreenPos( pos ), color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
        }

        /// <summary>
        /// 在屏幕坐标中绘制一段Comic字体的文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在屏幕坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        static public void DrawComicInScrnCoord ( string text, Vector2 pos, float scale, Color color, float layerDepth )
        {
            textSpriteBatch.DrawString( comicFont, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
        }

        /// <summary>
        /// 在逻辑坐标中绘制一段Lucida字体的文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在逻辑坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        static public void DrawLucida ( string text, Vector2 pos, float scale, Color color, float layerDepth )
        {
            textSpriteBatch.DrawString( lucidaFont, text, Coordin.ScreenPos( pos ), color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
        }

        /// <summary>
        /// 在屏幕坐标中绘制一段Lucida字体的文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在屏幕坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        static public void DrawLucidaInScrnCoord ( string text, Vector2 pos, float scale, Color color, float layerDepth )
        {
            textSpriteBatch.DrawString( lucidaFont, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
        }

        /// <summary>
        /// 在屏幕坐标中绘制一段文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在屏幕坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        /// <param name="fontType">字体</param>
        static public void DrawInScrnCoord ( string text, Vector2 pos, float scale, Color color, float layerDepth, FontType fontType )
        {
            switch (fontType)
            {
                case FontType.Comic:
                    DrawComicInScrnCoord( text, pos, scale, color, layerDepth );
                    break;
                case FontType.Lucida:
                    DrawLucidaInScrnCoord( text, pos, scale, color, layerDepth );
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region LengthOfString

        /// <summary>
        /// 获取一段文字在屏幕坐标上的长度
        /// </summary>
        /// <param name="text">文字的内容</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="fontType">字体</param>
        /// <returns></returns>
        static public float LengthOfString ( string text, float scale, FontType fontType )
        {
            if (fontType == FontType.Comic)
            {
                return comicFont.MeasureString( text ).X * scale;
            }
            else if (fontType == FontType.Lucida)
            {
                return lucidaFont.MeasureString( text ).X * scale;
            }
            else return 0;
        } 
        #endregion
    }

    /// <summary>
    /// 表示支持的字体
    /// </summary>
    public enum FontType
    {
        /// <summary>
        /// Comic字体
        /// </summary>
        Comic,
        /// <summary>
        /// Lucida字体
        /// </summary>
        Lucida
    }



}

