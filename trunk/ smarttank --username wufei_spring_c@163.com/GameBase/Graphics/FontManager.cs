using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using GameBase.Helpers;

namespace GameBase.Graphics
{
    public static class FontManager
    {
        #region Varibles
        public static SpriteBatch textSpriteBatch;

        static SpriteFont comicFont;

        static SpriteFont lucidaFont;

        static public void Initial ()
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
        public static void SpriteBatchBegin ()
        {
            textSpriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None );
        }

        /// <summary>
        /// textSpriteBatch.End();
        /// </summary>
        public static void SpriteBatchEnd ()
        {
            textSpriteBatch.End();
        }

        public static void HandleDeviceReset ()
        {
            Initial();
        }

        #endregion

        #region Draw Functions
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

        static public void DrawComic ( string text, Vector2 pos, float scale, Color color, float layerDepth )
        {
            textSpriteBatch.DrawString( comicFont, text, Coordin.ScreenPos( pos ), color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
        }

        static public void DrawComicInScrnCoord ( string text, Vector2 pos, float scale, Color color, float layerDepth )
        {
            textSpriteBatch.DrawString( comicFont, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
        }

        static public void DrawLucida ( string text, Vector2 pos, float scale, Color color, float layerDepth )
        {
            textSpriteBatch.DrawString( lucidaFont, text, Coordin.ScreenPos( pos ), color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
        }

        static public void DrawLucidaInScrnCoord ( string text, Vector2 pos, float scale, Color color, float layerDepth )
        {
            textSpriteBatch.DrawString( lucidaFont, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
        }

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

    public enum FontType
    {
        Comic,
        Lucida
    }



}

