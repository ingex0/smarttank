using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Helpers;
using System.IO;
using GameBase.DataStructure;

namespace GameBase.Graphics
{
    /// <summary>
    /// 提供点，线，矩形的绘制功能
    /// </summary>
    public class BasicGraphics
    {

        #region Statics
        static Texture2D pointTexture;
        static Texture2D lineTexture;
        static Texture2D retangleTexture;
        #endregion

        #region Initial
        static internal void Initial ()
        {
            pointTexture = BaseGame.Content.Load<Texture2D>( Path.Combine( Directories.BasicGraphicsContent, "point" ) );
            lineTexture = BaseGame.Content.Load<Texture2D>( Path.Combine( Directories.BasicGraphicsContent, "line" ) );
            retangleTexture = BaseGame.Content.Load<Texture2D>( Path.Combine( Directories.BasicGraphicsContent, "retangle" ) );
        }
        #endregion

        #region Draw Point

        /// <summary>
        /// 绘制一个标注点
        /// </summary>
        /// <param name="pos">逻辑坐标</param>
        /// <param name="scale">大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        static public void DrawPoint ( Vector2 pos, float scale, Color color, float layerDepth )
        {
            DrawPoint( pos, scale, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }

        /// <summary>
        /// 绘制一个标注点
        /// </summary>
        /// <param name="pos">逻辑坐标</param>
        /// <param name="scale">大小</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        /// <param name="blendMode">混合模式</param>
        static public void DrawPoint ( Vector2 pos, float scale, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            if (blendMode == SpriteBlendMode.Additive)
                Sprite.additiveSprite.Draw( pointTexture, Coordin.ScreenPos( pos ), null, color, 0, new Vector2( 16, 16 ), scale, SpriteEffects.None, layerDepth );
            else
                Sprite.alphaSprite.Draw( pointTexture, Coordin.ScreenPos( pos ), null, color, 0, new Vector2( 16, 16 ), scale, SpriteEffects.None, layerDepth );
        }

        #endregion

        #region Draw Line

        /// <summary>
        /// 在逻辑坐标中绘制一条直线
        /// </summary>
        /// <param name="startPoint">直线的起始点</param>
        /// <param name="endPoint">直线的终点</param>
        /// <param name="width">直线的宽度，以像素为单位</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        static public void DrawLine ( Vector2 startPoint, Vector2 endPoint, float width, Color color, float layerDepth )
        {
            DrawLine( startPoint, endPoint, width, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }

        /// <summary>
        /// 在逻辑坐标中绘制一条直线
        /// </summary>
        /// <param name="startPoint">直线的起始点</param>
        /// <param name="endPoint">直线的终点</param>
        /// <param name="width">直线的宽度，以像素为单位</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        /// <param name="blendMode">混合模式</param>
        static public void DrawLine ( Vector2 startPoint, Vector2 endPoint, float width, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            float lengthPix = Coordin.ScrnLength( Vector2.Distance( startPoint, endPoint ) );
            Vector2 midPoint = Coordin.ScreenPos( new Vector2( 0.5f * (startPoint.X + endPoint.X), 0.5f * (startPoint.Y + endPoint.Y) ) );
            float rota = (float)Math.Atan( (double)Coordin.ScrnLengthf( startPoint.Y - endPoint.Y ) / (double)Coordin.ScrnLengthf( startPoint.X - endPoint.X ) );
            if (blendMode == SpriteBlendMode.Additive)
                Sprite.additiveSprite.Draw( lineTexture, new Rectangle( (int)(midPoint.X), (int)(midPoint.Y), (int)(lengthPix), (int)width ),
                    null, color, rota - Coordin.Rota, new Vector2( 64, 8 ), SpriteEffects.None, layerDepth );
            else
                Sprite.alphaSprite.Draw( lineTexture, new Rectangle( (int)(midPoint.X), (int)(midPoint.Y), (int)(lengthPix), (int)width ),
                null, color, rota - Coordin.Rota, new Vector2( 64, 8 ), SpriteEffects.None, layerDepth );
        }

        /// <summary>
        /// 在屏幕坐标中绘制一条直线
        /// </summary>
        /// <param name="startPoint">直线的起始点</param>
        /// <param name="endPoint">直线的终点</param>
        /// <param name="width">直线的宽度，以像素为单位</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        static public void DrawLineInScrn ( Vector2 startPoint, Vector2 endPoint, float width, Color color, float layerDepth )
        {
            DrawLineInScrn( startPoint, endPoint, width, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }

        /// <summary>
        /// 在屏幕坐标中绘制一条直线
        /// </summary>
        /// <param name="startPoint">直线的起始点</param>
        /// <param name="endPoint">直线的终点</param>
        /// <param name="width">直线的宽度，以像素为单位</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        /// <param name="blendMode">混合模式</param>
        static public void DrawLineInScrn ( Vector2 startPoint, Vector2 endPoint, float width, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            float lengthPix = Vector2.Distance( startPoint, endPoint );
            Vector2 midPoint = new Vector2( 0.5f * (startPoint.X + endPoint.X), 0.5f * (startPoint.Y + endPoint.Y) );
            float rota = (float)Math.Atan( (double)(startPoint.Y - endPoint.Y) / (double)(startPoint.X - endPoint.X) );
            if (blendMode == SpriteBlendMode.Additive)
                Sprite.additiveSprite.Draw( lineTexture, new Rectangle( (int)(midPoint.X), (int)(midPoint.Y), (int)(lengthPix), (int)width ),
                    null, color, rota, new Vector2( 64, 8 ), SpriteEffects.None, layerDepth );
            else
                Sprite.alphaSprite.Draw( lineTexture, new Rectangle( (int)(midPoint.X), (int)(midPoint.Y), (int)(lengthPix), (int)width ),
                null, color, rota, new Vector2( 64, 8 ), SpriteEffects.None, layerDepth );
        }
        #endregion

        #region Draw Rectangle

        /// <summary>
        /// 在逻辑坐标中绘制矩形
        /// </summary>
        /// <param name="rect">要绘制的矩形</param>
        /// <param name="borderWidth">矩形的边线的宽度，以像素为单位</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        static public void DrawRectangle ( Rectangle rect, float borderWidth, Color color, float layerDepth )
        {
            DrawRectangle( rect, borderWidth, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }

        /// <summary>
        /// 在逻辑坐标中绘制矩形
        /// </summary>
        /// <param name="rect">要绘制的矩形</param>
        /// <param name="borderWidth">矩形的边线的宽度，以像素为单位</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        /// <param name="blendMode">混合模式</param>
        static public void DrawRectangle ( Rectangle rect, float borderWidth, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            DrawLine( new Vector2( rect.Left, rect.Top ), new Vector2( rect.Right, rect.Top ), borderWidth, color, layerDepth, blendMode );
            DrawLine( new Vector2( rect.Left, rect.Top ), new Vector2( rect.Left, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
            DrawLine( new Vector2( rect.Right, rect.Top ), new Vector2( rect.Right, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
            DrawLine( new Vector2( rect.Left, rect.Bottom ), new Vector2( rect.Right, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
        }

        /// <summary>
        /// 在逻辑坐标中绘制矩形
        /// </summary>
        /// <param name="rect">要绘制的矩形</param>
        /// <param name="borderWidth">矩形的边线的宽度，以像素为单位</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        static public void DrawRectangle ( Rectanglef rect, float borderWidth, Color color, float layerDepth )
        {
            DrawRectangle( rect, borderWidth, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }

        /// <summary>
        /// 在逻辑坐标中绘制矩形
        /// </summary>
        /// <param name="rect">要绘制的矩形</param>
        /// <param name="borderWidth">矩形的边线的宽度，以像素为单位</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        /// <param name="blendMode">混合模式</param>
        static public void DrawRectangle ( Rectanglef rect, float borderWidth, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            DrawLine( new Vector2( rect.Left, rect.Top ), new Vector2( rect.Right, rect.Top ), borderWidth, color, layerDepth, blendMode );
            DrawLine( new Vector2( rect.Left, rect.Top ), new Vector2( rect.Left, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
            DrawLine( new Vector2( rect.Right, rect.Top ), new Vector2( rect.Right, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
            DrawLine( new Vector2( rect.Left, rect.Bottom ), new Vector2( rect.Right, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
        }

        /// <summary>
        /// 在屏幕坐标中绘制矩形
        /// </summary>
        /// <param name="rect">要绘制的矩形</param>
        /// <param name="borderWidth">矩形的边线的宽度，以像素为单位</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        /// <param name="blendMode">混合模式</param>
        static public void DrawRectangleInScrn ( Rectangle rect, float borderWidth, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            DrawLineInScrn( new Vector2( rect.Left, rect.Top ), new Vector2( rect.Right, rect.Top ), borderWidth, color, layerDepth, blendMode );
            DrawLineInScrn( new Vector2( rect.Left, rect.Top ), new Vector2( rect.Left, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
            DrawLineInScrn( new Vector2( rect.Right, rect.Top ), new Vector2( rect.Right, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
            DrawLineInScrn( new Vector2( rect.Left, rect.Bottom ), new Vector2( rect.Right, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
        }

        /// <summary>
        /// 在屏幕坐标中绘制矩形
        /// </summary>
        /// <param name="rect">要绘制的矩形</param>
        /// <param name="borderWidth">矩形的边线的宽度，以像素为单位</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        static public void DrawRectangleInScrn ( Rectangle rect, float borderWidth, Color color, float layerDepth )
        {
            DrawRectangleInScrn( rect, borderWidth, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }
        #endregion

        #region Fill Rectangle

        /// <summary>
        /// 在逻辑坐标中绘制一个填充的矩形
        /// </summary>
        /// <param name="rect">要填充的矩形</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        static public void FillRectangle ( Rectangle rect, Color color, float layerDepth )
        {
            FillRectangle( rect, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }

        /// <summary>
        /// 在逻辑坐标中绘制一个填充的矩形
        /// </summary>
        /// <param name="rect">要填充的矩形</param>
        /// <param name="color">颜色</param>
        /// <param name="layerDepth">深度，0表示最表层，1表示最深层</param>
        /// <param name="blenMode">混合模式</param>
        static public void FillRectangle ( Rectangle rect, Color color, float layerDepth, SpriteBlendMode blenMode )
        {
            Vector2 scrnPos = Coordin.ScreenPos( new Vector2( rect.X, rect.Y ) );
            Rectangle destinRect = new Rectangle( (int)scrnPos.X, (int)scrnPos.Y, Coordin.ScrnLength( rect.Width ), Coordin.ScrnLength( rect.Height ) );
            if (blenMode == SpriteBlendMode.Additive)
                Sprite.additiveSprite.Draw( retangleTexture, destinRect, null, color, -Coordin.Rota, Vector2.Zero, SpriteEffects.None, layerDepth );
            else
                Sprite.alphaSprite.Draw( retangleTexture, destinRect, null, color, -Coordin.Rota, Vector2.Zero, SpriteEffects.None, layerDepth );
        }
        #endregion

    }
}
