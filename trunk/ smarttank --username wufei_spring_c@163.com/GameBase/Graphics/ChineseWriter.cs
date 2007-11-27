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
    class ChineseWriter
    {
        const float fontDrawEmSize = 32;

        static PrivateFontCollection privateFontCollection;
        static System.Drawing.FontFamily[] fontFamilys;
        static System.Drawing.Graphics lastGraphics;

        static System.Drawing.Font fontHanDinJianShu;

        static Dictionary<string, Texture2D> cache = new Dictionary<string, Texture2D>();

        public static void Intitial ()
        {
            privateFontCollection = new PrivateFontCollection();
            privateFontCollection.AddFontFile( Path.Combine( Directories.FontContent, "HDZB_39.TTF" ) );

            fontFamilys = privateFontCollection.Families;

            fontHanDinJianShu = new System.Drawing.Font( fontFamilys[0], fontDrawEmSize );

        }

        public static void WriteText ( string text, Vector2 scrnPos, float rota, float scale, Color color, float layerDepth )
        {
            Texture2D texture;

            if (cache.ContainsKey( text ))
            {
                texture = cache[text];
            }
            else
            {
                texture = BuildTexture( text, fontHanDinJianShu );
            }

            FontManager.textSpriteBatch.Draw( texture, scrnPos, null, color, rota, new Vector2( 0, 0 ), scale, SpriteEffects.None, layerDepth );

        }

        private static Texture2D BuildTexture ( string text, System.Drawing.Font font )
        {
            if (lastGraphics == null)
            {
                System.Drawing.Bitmap tempBitMap = new System.Drawing.Bitmap( 1, 1 );
                lastGraphics = System.Drawing.Graphics.FromImage( tempBitMap );
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

            cache.Add( text, texture );

            curGraphics.Dispose();

            return texture;
        }
    }
}
