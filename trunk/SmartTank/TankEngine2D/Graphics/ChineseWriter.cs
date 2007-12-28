using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Text;
using TankEngine2D.Helpers;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Serialization;

namespace TankEngine2D.Graphics
{
    class ChineseWriter
    {
        struct TextKey
        {
            public string text;
            public string fontName;

            public TextKey( string text, string fontName )
            {
                this.text = text;
                this.fontName = fontName;
            }
        }


        RenderEngine engine;

        PrivateFontCollection privateFontCollection;
        System.Drawing.FontFamily[] fontFamilys;
        System.Drawing.Graphics mesureGraphics;

        Dictionary<string, System.Drawing.Font> fonts;

        Dictionary<TextKey, Texture2D> cache = new Dictionary<TextKey, Texture2D>();

        public ChineseWriter( RenderEngine engine )
        {
            this.engine = engine;
            this.fonts = new Dictionary<string, System.Drawing.Font>();
        }

        public bool HasFont( string fontName )
        {
            return fonts.ContainsKey( fontName );
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        public void Intitial( FontMgr.FontLoadInfo fontLoadInfo )
        {
            try
            {
                privateFontCollection = new PrivateFontCollection();

                foreach (FontMgr.FontInfo info in fontLoadInfo.UnitCodeFontInfos)
                {
                    privateFontCollection.AddFontFile( info.path );
                }

                fontFamilys = privateFontCollection.Families;

                if (fontFamilys.Length != fontLoadInfo.UnitCodeFontInfos.Count)
                    throw new Exception( "����ĸ�������������ڲ�ͬ���" );

                for (int i = 0; i < fontFamilys.Length; i++)
                {
                    fonts.Add( fontLoadInfo.UnitCodeFontInfos[i].name, new System.Drawing.Font( fontFamilys[i], fontLoadInfo.DefualtEmSize ) );
                }

                System.Drawing.Bitmap tempBitMap = new System.Drawing.Bitmap( 1, 1 );
                mesureGraphics = System.Drawing.Graphics.FromImage( tempBitMap );
            }
            catch (Exception)
            {
                throw new Exception( "��ȡ�����ļ�����" );
            }

        }

        /// <summary>
        /// ���ư��������ֵ�����
        /// </summary>
        /// <param name="text">Ҫ���Ƶ�����</param>
        /// <param name="scrnPos">���Ƶ���Ļλ��</param>
        /// <param name="rota">��ת��</param>
        /// <param name="scale">���ű�</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">�������</param>
        /// <param name="fontName">��������</param>
        public void WriteText( string text, Vector2 scrnPos, float rota, float scale, Color color, float layerDepth, string fontName )
        {
            Texture2D texture;

            TextKey key = new TextKey( text, fontName );

            if (cache.ContainsKey( key ))
            {
                texture = cache[key];
            }
            else
            {
                texture = BuildTexture( text, fontName );
            }

            engine.SpriteMgr.alphaSprite.Draw( texture, scrnPos, null, color, rota, new Vector2( 0, 0 ), scale, SpriteEffects.None, layerDepth );

        }

        /// <summary>
        /// ������ͼ����ӵ������С�
        /// �����ڵ�һ�λ���֮ǰ���øú������������Ա��⽨����ͼ�Ĺ��������Ϸ��ͣ��
        /// </summary>
        /// <param name="text">Ҫ�������ַ���</param>
        /// <param name="fontName">����</param>
        /// <returns></returns>
        public Texture2D BuildTexture( string text, string fontName )
        {
            TextKey key = new TextKey( text, fontName );

            if (cache.ContainsKey( key ))
                return null;

            if (!fonts.ContainsKey( fontName ))
            {
                Log.Write( "error fontName used in BuildTexture" );
                return null;
            }

            System.Drawing.Font font = fonts[fontName];

            System.Drawing.SizeF size = mesureGraphics.MeasureString( text, font );
            int texWidth = (int)size.Width;
            int texHeight = (int)size.Height;

            System.Drawing.Bitmap textMap = new System.Drawing.Bitmap( texWidth, texHeight );
            System.Drawing.Graphics curGraphics = System.Drawing.Graphics.FromImage( textMap );
            curGraphics.DrawString( text, font, System.Drawing.Brushes.White, new System.Drawing.PointF() );

            Texture2D texture = new Texture2D( engine.Device, texWidth, texHeight, 1, ResourceUsage.None, SurfaceFormat.Color );
            Microsoft.Xna.Framework.Graphics.Color[] data = new Microsoft.Xna.Framework.Graphics.Color[texWidth * texHeight];
            for (int y = 0; y < texHeight; y++)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    data[y * texWidth + x] = ConvertHelper.SysColorToXNAColor( textMap.GetPixel( x, y ) );
                }
            }

            texture.SetData<Microsoft.Xna.Framework.Graphics.Color>( data );

            cache.Add( key, texture );

            curGraphics.Dispose();

            return texture;
        }

        /// <summary>
        /// �����ַ����ĳ���
        /// </summary>
        /// <param name="text">Ҫ�������ַ���</param>
        /// <param name="scale">�ַ�����������</param>
        /// <param name="fontName">����</param>
        /// <returns></returns>
        public float MeasureString( string text, float scale, string fontName )
        {
            System.Drawing.Font font = fonts[fontName];
            return mesureGraphics.MeasureString( text, font ).Width;
        }

        /// <summary>
        /// �������
        /// </summary>
        public void ClearCache()
        {
            foreach (KeyValuePair<TextKey, Texture2D> pair in cache)
            {
                pair.Value.Dispose();
            }

            cache.Clear();
        }
    }
}
