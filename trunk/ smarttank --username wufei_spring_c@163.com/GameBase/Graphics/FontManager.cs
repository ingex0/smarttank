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
        /// ���߼������л���һ�����֡�
        /// ע�⣬������������а������ģ�����ѡ����������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ�����߼������е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="rota">˳ʱ����ת����</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        /// <param name="fontType">����</param>
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
        /// ���߼������л���һ�����֡�
        /// ע�⣬������������а������ģ�����ѡ����������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ�����߼������е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        /// <param name="fontType">����</param>
        static public void Draw ( string text, Vector2 pos, float scale, Color color, float layerDepth, FontType fontType )
        {
            Draw( text, pos, scale, 0, color, layerDepth, fontType );
        }

        /// <summary>
        /// ���߼������л���һ��Comic���������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ�����߼������е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        static private void DrawComic ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth )
        {
            try
            {
                textSpriteBatch.DrawString( comicFont, text, Coordin.ScreenPos( pos ), color, rota, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
            }
            catch (Exception)
            {
                Log.Write( "�ò�֧�����ĵ�������ư��������ַ����ַ����� " + text );
            }
        }

        /// <summary>
        /// ����Ļ�����л���һ��Comic���������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ������Ļ�����е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        static private void DrawComicInScrnCoord ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth )
        {
            try
            {
                textSpriteBatch.DrawString( comicFont, text, pos, color, rota, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
            }
            catch (Exception)
            {
                Log.Write( "�ò�֧�����ĵ�������ư��������ַ����ַ����� " + text );
            }
        }

        /// <summary>
        /// ���߼������л���һ��Lucida���������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ�����߼������е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        static private void DrawLucida ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth )
        {
            try
            {
                textSpriteBatch.DrawString( lucidaFont, text, Coordin.ScreenPos( pos ), color, rota, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
            }
            catch (Exception)
            {
                Log.Write( "�ò�֧�����ĵ�������ư��������ַ����ַ����� " + text );
            }
        }

        /// <summary>
        /// ����Ļ�����л���һ��Lucida���������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ������Ļ�����е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        static private void DrawLucidaInScrnCoord ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth )
        {
            try
            {
                textSpriteBatch.DrawString( lucidaFont, text, pos, color, rota, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
            }
            catch (Exception)
            {
                Log.Write( "�ò�֧�����ĵ�������ư��������ַ����ַ����� " + text );
            }
        }

        /// <summary>
        /// ����Ļ�����л���һ������
        /// ע�⣬������������а������ģ�����ѡ����������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ������Ļ�����е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="rota">��ת��</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        /// <param name="fontType">����</param>
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
        /// ����Ļ�����л���һ������
        /// ע�⣬������������а������ģ�����ѡ����������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">������ʼ������Ļ�����е�λ��</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">��ȣ�0Ϊ���㣬1Ϊ�����</param>
        /// <param name="fontType">����</param>
        static public void DrawInScrnCoord ( string text, Vector2 pos, float scale, Color color, float layerDepth, FontType fontType )
        {
            DrawInScrnCoord( text, pos, scale, 0f, color, layerDepth, fontType );
        }

        #endregion

        #region LengthOfString

        /// <summary>
        /// ��ȡһ����������Ļ�����ϵĳ���
        /// ע�⣬������������а������ģ�����ѡ����������
        /// </summary>
        /// <param name="text">���ֵ�����</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="fontType">����</param>
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
                Log.Write( "�ò�֧�����ĵ������ȡ���������ַ����ַ����ĳ��ȣ� " + text );
                return -1;
            }
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
        Comic = 0x00,
        /// <summary>
        /// Lucida����
        /// </summary>
        Lucida = 0x01,
        /// <summary>
        /// �������棬�������塣
        /// </summary>
        HanDinJianShu = 0x10,
    }



}

