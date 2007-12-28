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
    /// 提供字符的绘制功能。
    /// </summary>
    public class FontMgr
    {
        /// <summary>
        /// 表示字体的名称与导入路径
        /// </summary>
        public struct FontInfo
        {
            /// <summary>
            /// 字体的名称
            /// </summary>
            public string name;
            /// <summary>
            /// 导入路径
            /// </summary>
            public string path;
            /// <summary>
            /// 表示字体的名称与导入路径
            /// </summary>
            /// <param name="name">字体的名称</param>
            /// <param name="path">导入路径</param>
            public FontInfo ( string name, string path )
            {
                this.name = name;
                this.path = path;
            }
        }
        /// <summary>
        /// 表示字体的导入信息
        /// </summary>
        public class FontLoadInfo
        {
            /// <summary>
            /// 从文件中读取字体的导入信息
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
                        model.ASCIIFontInfos.Add( new FontInfo( "fontName2", "fontAssetPath : 请填入XNA专用字体信息文件路径" ) );
                        model.UnitCodeFontInfos.Add( new FontInfo( "fontName1", "TrueTypeFontPath : 请填入TrueType字体文件路径" ) );
                        Save( filePath, model );
                        Log.Write( "没找到字体导入配置文件，自动生成配置文件模板" );
                    }

                    throw;
                }
            }
            /// <summary>
            /// 将字体的导入信息储存到文件中
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
            /// 字体默认大小（Scale = 1)时的磅值
            /// </summary>
            public float DefualtEmSize;
            /// <summary>
            /// TrueType字体文件的导入信息
            /// </summary>
            public List<FontInfo> UnitCodeFontInfos;
            /// <summary>
            /// XNA素材管道专用字体文件的导入信息
            /// </summary>
            public List<FontInfo> ASCIIFontInfos;

            /// <summary>
            /// 表示字体的导入信息
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
        /// 提供字符的绘制功能
        /// </summary>
        /// <param name="engine">渲染组件</param>
        /// <param name="contentMgr">素材管理者</param>
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
        /// 建立贴图并添加到缓冲中。
        /// 请在第一次绘制之前调用该函数，这样可以避免建立贴图的过程造成游戏的停滞
        /// </summary>
        /// <param name="text">要创建的字符串</param>
        /// <param name="fontName">字体</param>
        public void BuildTexture( string text, string fontName )
        {
            chineseWriter.BuildTexture( text, fontName );
        }

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
        /// <param name="fontName">字体</param>
        public void Draw ( string text, Vector2 pos, float scale, float rota, Color color, float layerDepth, string fontName )
        {
            DrawInScrnCoord( text, engine.CoordinMgr.ScreenPos( pos ), scale, rota - engine.CoordinMgr.Rota, color, layerDepth, fontName );
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
        /// <param name="fontName">字体</param>
        public void Draw ( string text, Vector2 pos, float scale, Color color, float layerDepth, string fontName )
        {
            Draw( text, pos, scale, 0, color, layerDepth, fontName );
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
        /// <param name="fontName">字体</param>
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
                    Log.Write( "尝试使用非UnitCode字体绘制UnitCode字符:  " + text );
                }
            }
            else if (chineseWriter.HasFont( fontName ))
                chineseWriter.WriteText( text, pos, rota, scale, color, layerDepth, fontName );
            else
                Log.Write( "未找到指定的字体，请检查字体导入配置文件:  " + fontName );
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
        /// <param name="fontName">字体</param>
        public void DrawInScrnCoord ( string text, Vector2 pos, float scale, Color color, float layerDepth, string fontName )
        {
            DrawInScrnCoord( text, pos, scale, 0f, color, layerDepth, fontName );
        }

        #endregion

        #region LengthOfString

        /// <summary>
        /// 获取一段文字在屏幕坐标上的长度
        /// 注意，如果文字内容中包含中文，必须选用中文字体
        /// </summary>
        /// <param name="text">文字的内容</param>
        /// <param name="scale">文字的大小</param>
        /// <param name="fontName">字体</param>
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
                    Log.Write( "用不支持中文的字体获取包含中文字符的字符串的长度： " + text );
                    return -1;
                }
            }
            else if (chineseWriter.HasFont( fontName ))
                return chineseWriter.MeasureString( text, scale, fontName );
            else
            {
                Log.Write( "未找到指定的字体，请检查字体导入配置文件:  " + fontName );
                return -1;
            }

        }
        #endregion
    }

}

