using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using TankEngine2D.Helpers;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;

namespace TankEngine2D.Graphics
{
    /// <summary>
    /// �ṩ�ַ��Ļ��ƹ��ܡ�
    /// </summary>
    public class FontMgr
    {
        /// <summary>
        /// ��ʾ����������뵼��·��
        /// </summary>
        public struct FontInfo
        {
            /// <summary>
            /// ���������
            /// </summary>
            public string name;
            /// <summary>
            /// ����·��
            /// </summary>
            public string path;
            /// <summary>
            /// ��ʾ����������뵼��·��
            /// </summary>
            /// <param name="name">���������</param>
            /// <param name="path">����·��</param>
            public FontInfo ( string name, string path )
            {
                this.name = name;
                this.path = path;
            }
        }
        /// <summary>
        /// ��ʾ����ĵ�����Ϣ
        /// </summary>
        public class FontLoadInfo
        {
            /// <summary>
            /// ���ļ��ж�ȡ����ĵ�����Ϣ
            /// </summary>
            /// <param name="filePath"></param>
            /// <returns></returns>
            static public FontLoadInfo Load ( string filePath )
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer( typeof( FontLoadInfo ) );
                    FileStream stream = File.Open( filePath, FileMode.Open );
                    return (FontLoadInfo)(serializer.Deserialize( stream ));
                }
                catch (Exception e)
                {
                    if (e is FileNotFoundException)
                    {
                        FontLoadInfo model = new FontLoadInfo();
                        model.ASCIIFontInfos.Add( new FontInfo( "fontName2", "fontAssetPath : ������XNAר��������Ϣ�ļ�·��" ) );
                        model.UnitCodeFontInfos.Add( new FontInfo( "fontName1", "TrueTypeFontPath : ������TrueType�����ļ�·��" ) );
                        Save( filePath, model );
                        Log.Write( "û�ҵ����嵼�������ļ����Զ����������ļ�ģ��" );
                    }

                    throw;
                }
            }
            /// <summary>
            /// ������ĵ�����Ϣ���浽�ļ���
            /// </summary>
            /// <param name="filePath"></param>
            /// <param name="info"></param>
            static public void Save ( string filePath, FontLoadInfo info )
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer( typeof( FontLoadInfo ) );
                    serializer.Serialize( File.Open( filePath, FileMode.Create ), info );
                }
                catch (Exception)
                {
                    throw;
                }
            }

            /// <summary>
            /// ����Ĭ�ϴ�С��Scale = 1)ʱ�İ�ֵ
            /// </summary>
            public float DefualtEmSize;
            /// <summary>
            /// TrueType�����ļ��ĵ�����Ϣ
            /// </summary>
            public List<FontInfo> UnitCodeFontInfos;
            /// <summary>
            /// XNA�زĹܵ�ר�������ļ��ĵ�����Ϣ
            /// </summary>
            public List<FontInfo> ASCIIFontInfos;

            /// <summary>
            /// ��ʾ����ĵ�����Ϣ
            /// </summary>
            public FontLoadInfo ()
            {
                this.UnitCodeFontInfos = new List<FontInfo>();
                this.ASCIIFontInfos = new List<FontInfo>();
                DefualtEmSize = fontDrawEmSize;
            }
        }

        const string configFilePath = "font.cfg";
        const float fontDrawEmSize = 20;


        #region Varibles

        RenderEngine engine;
        ChineseWriter chineseWriter;

        Dictionary<string, SpriteFont> fonts;

        #endregion

        /// <summary>
        /// �ṩ�ַ��Ļ��ƹ���
        /// </summary>
        /// <param name="engine">��Ⱦ���</param>
        /// <param name="contentMgr">�زĹ�����</param>
        public FontMgr ( RenderEngine engine, ContentManager contentMgr )
        {
            this.engine = engine;
            chineseWriter = new ChineseWriter( engine );
            fonts = new Dictionary<string, SpriteFont>();

            LoadFonts( contentMgr );
        }

        private void LoadFonts ( ContentManager contentMgr )
        {
            FontLoadInfo fontLoadInfo = FontLoadInfo.Load( configFilePath );
            chineseWriter.Intitial( fontLoadInfo );

            if (contentMgr == null)
                return;

            try
            {
                foreach (FontInfo info in fontLoadInfo.ASCIIFontInfos)
                {
                    fonts.Add( info.name, contentMgr.Load<SpriteFont>( info.path ) );
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// ������ͼ����ӵ������С�
        /// ���ڵ�һ�λ���֮ǰ���øú������������Ա��⽨����ͼ�Ĺ��������Ϸ��ͣ��
        /// </summary>
        /// <param name="text">Ҫ�������ַ���</param>
        /// <param name="fontName">����</param>
        public void BuildTexture( string text, string fontName )
        {
            chineseWriter.BuildTexture( text, fontName );
        }

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
        /// <param name="fontName">����</param>
        public void Draw ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth, string fontName )
        {
            DrawInScrnCoord( text, engine.CoordinMgr.ScreenPos( pos ), scale, rota - engine.CoordinMgr.Rota, color, layerDepth, fontName );
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
        /// <param name="fontName">����</param>
        public void Draw ( string text, Vector2 pos, float scale, Color color, float layerDepth, string fontName )
        {
            Draw( text, pos, scale, 0, color, layerDepth, fontName );
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
        /// <param name="fontName">����</param>
        public void DrawInScrnCoord ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth, string fontName )
        {
            if (fonts.ContainsKey( fontName ))
            {
                try
                {
                    engine.SpriteMgr.alphaSprite.DrawString( fonts[fontName], text, pos, color, rota, Vector2.Zero, scale, SpriteEffects.None, layerDepth );
                }
                catch (Exception)
                {
                    Log.Write( "����ʹ�÷�UnitCode�������UnitCode�ַ�:  " + text );
                }
            }
            else if (chineseWriter.HasFont( fontName ))
                chineseWriter.WriteText( text, pos, rota, scale, color, layerDepth, fontName );
            else
                Log.Write( "δ�ҵ�ָ�������壬�������嵼�������ļ�:  " + fontName );
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
        /// <param name="fontName">����</param>
        public void DrawInScrnCoord ( string text, Vector2 pos, float scale, Color color, float layerDepth, string fontName )
        {
            DrawInScrnCoord( text, pos, scale, 0f, color, layerDepth, fontName );
        }

        #endregion

        #region LengthOfString

        /// <summary>
        /// ��ȡһ����������Ļ�����ϵĳ���
        /// ע�⣬������������а������ģ�����ѡ����������
        /// </summary>
        /// <param name="text">���ֵ�����</param>
        /// <param name="scale">���ֵĴ�С</param>
        /// <param name="fontName">����</param>
        /// <returns></returns>
        public float LengthOfString ( string text, float scale, string fontName )
        {

            if (fonts.ContainsKey( fontName ))
            {
                try
                {
                    return fonts[fontName].MeasureString( text ).X * scale;
                }
                catch (Exception)
                {
                    Log.Write( "�ò�֧�����ĵ������ȡ���������ַ����ַ����ĳ��ȣ� " + text );
                    return -1;
                }
            }
            else if (chineseWriter.HasFont( fontName ))
                return chineseWriter.MeasureString( text, scale, fontName );
            else
            {
                Log.Write( "δ�ҵ�ָ�������壬�������嵼�������ļ�:  " + fontName );
                return -1;
            }

        }
        #endregion
    }

}

