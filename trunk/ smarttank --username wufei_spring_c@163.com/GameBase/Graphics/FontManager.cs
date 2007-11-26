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
    /// �ṩ�ַ��Ļ��ƹ��ܡ�
    /// ��ʱ�޷���ʾ���ġ�
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
        /// ���߼������л���һ������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ�����߼������е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        /// <param name="fontType">����</param>
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
        /// ���߼������л���һ��Comic���������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ�����߼������е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        static public void DrawComic ( string text, Vector2 pos, float scale, Color color, float layerDepth )
        {
            textSpriteBatch.DrawString( comicFont, text, Coordin.ScreenPos( pos ), color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
        }

        /// <summary>
        /// ����Ļ�����л���һ��Comic���������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ������Ļ�����е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        static public void DrawComicInScrnCoord ( string text, Vector2 pos, float scale, Color color, float layerDepth )
        {
            textSpriteBatch.DrawString( comicFont, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
        }

        /// <summary>
        /// ���߼������л���һ��Lucida���������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ�����߼������е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        static public void DrawLucida ( string text, Vector2 pos, float scale, Color color, float layerDepth )
        {
            textSpriteBatch.DrawString( lucidaFont, text, Coordin.ScreenPos( pos ), color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
        }

        /// <summary>
        /// ����Ļ�����л���һ��Lucida���������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ������Ļ�����е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        static public void DrawLucidaInScrnCoord ( string text, Vector2 pos, float scale, Color color, float layerDepth )
        {
            textSpriteBatch.DrawString( lucidaFont, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
        }

        /// <summary>
        /// ����Ļ�����л���һ������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ������Ļ�����е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        /// <param name="fontType">����</param>
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
        /// ��ȡһ����������Ļ�����ϵĳ���
        /// </summary>
        /// <param name="text">���ֵ�����</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="fontType">����</param>
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
    /// ��ʾ֧�ֵ�����
    /// </summary>
    public enum FontType
    {
        /// <summary>
        /// Comic����
        /// </summary>
        Comic,
        /// <summary>
        /// Lucida����
        /// </summary>
        Lucida
    }



}

