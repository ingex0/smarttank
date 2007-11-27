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
        /// 在逻辑坐标中绘制一段文字。
        /// 注意，如果文字内容中包含中文，必须选用中文字体
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在逻辑坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="rota">顺时针旋转弧度</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        /// <param name="fontType">字体</param>
        static public void Draw ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth, FontType fontType )
        {
            switch (fontType)
            {
                case FontType.Comic:
                    DrawComic( text, pos, scale, rota, color, layerDepth );
                    break;
                case FontType.Lucida:
                    DrawLucida( text, pos, scale, rota, color, layerDepth );
                    break;
                case FontType.HanDinJianShu:
                    ChineseWriter.WriteText( text, Coordin.ScreenPos( pos ), scale, rota, color, layerDepth, fontType );
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 在逻辑坐标中绘制一段文字。
        /// 注意，如果文字内容中包含中文，必须选用中文字体
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在逻辑坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        /// <param name="fontType">字体</param>
        static public void Draw ( string text, Vector2 pos, float scale, Color color, float layerDepth, FontType fontType )
        {
            Draw( text, pos, scale, 0, color, layerDepth, fontType );
        }

        /// <summary>
        /// 在逻辑坐标中绘制一段Comic字体的文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在逻辑坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        static private void DrawComic ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth )
        {
            try
            {
                textSpriteBatch.DrawString( comicFont, text, Coordin.ScreenPos( pos ), color, rota, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
            }
            catch (Exception)
            {
                Log.Write( "用不支持中文的字体绘制包含中文字符的字符串： " + text );
            }
        }

        /// <summary>
        /// 在屏幕坐标中绘制一段Comic字体的文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在屏幕坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        static private void DrawComicInScrnCoord ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth )
        {
            try
            {
                textSpriteBatch.DrawString( comicFont, text, pos, color, rota, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
            }
            catch (Exception)
            {
                Log.Write( "用不支持中文的字体绘制包含中文字符的字符串： " + text );
            }
        }

        /// <summary>
        /// 在逻辑坐标中绘制一段Lucida字体的文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在逻辑坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        static private void DrawLucida ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth )
        {
            try
            {
                textSpriteBatch.DrawString( lucidaFont, text, Coordin.ScreenPos( pos ), color, rota, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
            }
            catch (Exception)
            {
                Log.Write( "用不支持中文的字体绘制包含中文字符的字符串： " + text );
            }
        }

        /// <summary>
        /// 在屏幕坐标中绘制一段Lucida字体的文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在屏幕坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        static private void DrawLucidaInScrnCoord ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth )
        {
            try
            {
                textSpriteBatch.DrawString( lucidaFont, text, pos, color, rota, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
            }
            catch (Exception)
            {
                Log.Write( "用不支持中文的字体绘制包含中文字符的字符串： " + text );
            }
        }

        /// <summary>
        /// 在屏幕坐标中绘制一段文字
        /// 注意，如果文字内容中包含中文，必须选用中文字体
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在屏幕坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="rota">旋转角</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        /// <param name="fontType">字体</param>
        static public void DrawInScrnCoord ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth, FontType fontType )
        {
            if (fontType == FontType.Comic)
                DrawComicInScrnCoord( text, pos, scale, rota, color, layerDepth );
            else if (fontType == FontType.Lucida)
                DrawLucidaInScrnCoord( text, pos, scale, rota, color, layerDepth );
            else if (((int)fontType & 0x10) == 0x10)
                ChineseWriter.WriteText( text, pos, rota, scale, color, layerDepth, fontType );

        }

        /// <summary>
        /// 在屏幕坐标中绘制一段文字
        /// 注意，如果文字内容中包含中文，必须选用中文字体
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="pos">文字起始处在屏幕坐标中的位置</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0为最表层，1为最深层</param>
        /// <param name="fontType">字体</param>
        static public void DrawInScrnCoord ( string text, Vector2 pos, float scale, Color color, float layerDepth, FontType fontType )
        {
            DrawInScrnCoord( text, pos, scale, 0f, color, layerDepth, fontType );
        }

        #endregion

        #region LengthOfString

        /// <summary>
        /// 获取一段文字在屏幕坐标上的长度
        /// 注意，如果文字内容中包含中文，必须选用中文字体
        /// </summary>
        /// <param name="text">文字的内容</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="fontType">字体</param>
        /// <returns></returns>
        static public float LengthOfString ( string text, float scale, FontType fontType )
        {
            try
            {
                if (fontType == FontType.Comic)
                {
                    return comicFont.MeasureString( text ).X * scale;
                }
                else if (fontType == FontType.Lucida)
                {
                    return lucidaFont.MeasureString( text ).X * scale;
                }
                else if (((int)fontType & 0x10) == 0x10)
                {
                    return ChineseWriter.MeasureString( text, scale, fontType );
                }
                else return -1;
            }
            catch (Exception)
            {
                Log.Write( "用不支持中文的字体获取包含中文字符的字符串的长度： " + text );
                return -1;
            }
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
        Comic = 0x00,
        /// <summary>
        /// Lucida字体
        /// </summary>
        Lucida = 0x01,
        /// <summary>
        /// 汉鼎简舒，中文字体。
        /// </summary>
        HanDinJianShu = 0x10,
    }



}

