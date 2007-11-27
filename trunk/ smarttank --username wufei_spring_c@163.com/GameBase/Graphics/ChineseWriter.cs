using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Text;
using GameBase.Helpers;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameBase.Graphics
{
    public class ChineseWriter
    {
        const float fontDrawEmSize = 20;

        static PrivateFontCollection privateFontCollection;
        static System.Drawing.FontFamily[] fontFamilys;
        static System.Drawing.Graphics lastGraphics;

        static System.Drawing.Font fontHanDinJianShu;

        struct TextKey
        {
            public string text;
            public FontType fontType;

            public TextKey ( string text, FontType fontType )
            {
                this.text = text;
                this.fontType = fontType;
            }
        }

        static Dictionary<TextKey, Texture2D> cache = new Dictionary<TextKey, Texture2D>();

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Intitial ()
        {
            privateFontCollection = new PrivateFontCollection();
            privateFontCollection.AddFontFile( Path.Combine( Directories.FontContent, "HDZB_39.TTF" ) );

            fontFamilys = privateFontCollection.Families;

            fontHanDinJianShu = new System.Drawing.Font( fontFamilys[0], fontDrawEmSize );


            System.Drawing.Bitmap tempBitMap = new System.Drawing.Bitmap( 1, 1 );
            lastGraphics = System.Drawing.Graphics.FromImage( tempBitMap );
        }

        /// <summary>
        /// 绘制包含中文字的文字
        /// </summary>
        /// <param name="text"></param>
        /// <param name="scrnPos"></param>
        /// <param name="rota"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        /// <param name="layerDepth"></param>
        public static void WriteText ( string text, Vector2 scrnPos, float rota, float scale, Color color, float layerDepth, FontType type )
        {
            Texture2D texture;

            TextKey key = new TextKey( text, type );

            if (cache.ContainsKey( key ))
            {
                texture = cache[key];
            }
            else
            {
                texture = BuildTexture( text, type );
            }

            FontManager.textSpriteBatch.Draw( texture, scrnPos, null, color, rota, new Vector2( 0, 0 ), scale, SpriteEffects.None, layerDepth );

        }

        /// <summary>
        /// 建立贴图并添加到缓冲中。
        /// 尽量在第一次绘制之前调用该函数，这样可以避免建立贴图的过程造成游戏的停滞
        /// </summary>
        /// <param name="text">要创建的字符传</param>
        /// <param name="fontType">字体</param>
        /// <returns></returns>
        public static Texture2D BuildTexture ( string text, FontType fontType )
        {
            TextKey key = new TextKey( text, fontType );

            if (cache.ContainsKey( key ))
                return null;

            System.Drawing.Font font = fontHanDinJianShu;

            switch (fontType)
            {
                case FontType.HanDinJianShu:
                    font = fontHanDinJianShu;
                    break;
                default:
                    return null;
            }

            System.Drawing.SizeF size = lastGraphics.MeasureString( text, font );
            int texWidth = (int)size.Width;
            int texHeight = (int)size.Height;

            System.Drawing.Bitmap textMap = new System.Drawing.Bitmap( texWidth, texHeight );
            System.Drawing.Graphics curGraphics = System.Drawing.Graphics.FromImage( textMap );
            curGraphics.DrawString( text, font, System.Drawing.Brushes.White, new System.Drawing.PointF() );

            Texture2D texture = new Texture2D( BaseGame.Device, texWidth, texHeight, 1, ResourceUsage.None, SurfaceFormat.Color );
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

        public static float MeasureString ( string text, float scale, FontType type )
        {
            switch (type)
            {
                case FontType.HanDinJianShu:
                    return lastGraphics.MeasureString( text, fontHanDinJianShu ).Width;
                default:
                    return -1;
            }
        }


        public static void ClearCache ()
        {
            foreach (KeyValuePair<TextKey, Texture2D> pair in cache)
            {
                pair.Value.Dispose();
            }

            cache.Clear();
        }
    }
}
