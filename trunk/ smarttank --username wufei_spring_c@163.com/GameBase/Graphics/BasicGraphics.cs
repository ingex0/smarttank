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
    public class BasicGraphics
    {

        #region Statics
        static Texture2D pointTexture;
        static Texture2D lineTexture;
        static Texture2D retangleTexture;
        #endregion

        #region Initial
        static public void Initial ()
        {
            pointTexture = BaseGame.Content.Load<Texture2D>( Path.Combine( Directories.BasicGraphicsContent, "point" ) );
            lineTexture = BaseGame.Content.Load<Texture2D>( Path.Combine( Directories.BasicGraphicsContent, "line" ) );
            retangleTexture = BaseGame.Content.Load<Texture2D>( Path.Combine( Directories.BasicGraphicsContent, "retangle" ) );
        }
        #endregion

        #region Draw Point
        static public void DrawPoint ( Vector2 pos, float scale, Color color, float layerDepth )
        {
            DrawPoint( pos, scale, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }


        static public void DrawPoint ( Vector2 pos, float scale, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            if (blendMode == SpriteBlendMode.Additive)
                Sprite.additiveSprite.Draw( pointTexture, Coordin.ScreenPos( pos ), null, color, 0, new Vector2( 16, 16 ), scale, SpriteEffects.None, layerDepth );
            else
                Sprite.alphaSprite.Draw( pointTexture, Coordin.ScreenPos( pos ), null, color, 0, new Vector2( 16, 16 ), scale, SpriteEffects.None, layerDepth );
        }

        #endregion

        #region Draw Line
        static public void DrawLine ( Vector2 startPoint, Vector2 endPoint, float width, Color color, float layerDepth )
        {
            DrawLine( startPoint, endPoint, width, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }


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

        static public void DrawLineInScrn ( Vector2 startPoint, Vector2 endPoint, float width, Color color, float layerDepth )
        {
            DrawLineInScrn( startPoint, endPoint, width, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }

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
        static public void DrawRectangle ( Rectangle rect, float borderWidth, Color color, float layerDepth )
        {
            DrawRectangle( rect, borderWidth, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }

        static public void DrawRectangle ( Rectangle rect, float borderWidth, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            DrawLine( new Vector2( rect.Left, rect.Top ), new Vector2( rect.Right, rect.Top ), borderWidth, color, layerDepth, blendMode );
            DrawLine( new Vector2( rect.Left, rect.Top ), new Vector2( rect.Left, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
            DrawLine( new Vector2( rect.Right, rect.Top ), new Vector2( rect.Right, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
            DrawLine( new Vector2( rect.Left, rect.Bottom ), new Vector2( rect.Right, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
        }

        static public void DrawRectangle ( Rectanglef rect, float borderWidth, Color color, float layerDepth )
        {
            DrawRectangle( rect, borderWidth, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }

        static public void DrawRectangle ( Rectanglef rect, float borderWidth, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            DrawLine( new Vector2( rect.Left, rect.Top ), new Vector2( rect.Right, rect.Top ), borderWidth, color, layerDepth, blendMode );
            DrawLine( new Vector2( rect.Left, rect.Top ), new Vector2( rect.Left, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
            DrawLine( new Vector2( rect.Right, rect.Top ), new Vector2( rect.Right, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
            DrawLine( new Vector2( rect.Left, rect.Bottom ), new Vector2( rect.Right, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
        }

        static public void DrawRectangleInScrn ( Rectangle rect, float borderWidth, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            DrawLineInScrn( new Vector2( rect.Left, rect.Top ), new Vector2( rect.Right, rect.Top ), borderWidth, color, layerDepth, blendMode );
            DrawLineInScrn( new Vector2( rect.Left, rect.Top ), new Vector2( rect.Left, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
            DrawLineInScrn( new Vector2( rect.Right, rect.Top ), new Vector2( rect.Right, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
            DrawLineInScrn( new Vector2( rect.Left, rect.Bottom ), new Vector2( rect.Right, rect.Bottom ), borderWidth, color, layerDepth, blendMode );
        }

        static public void DrawRectangleInScrn ( Rectangle rect, float borderWidth, Color color, float layerDepth )
        {
            DrawRectangleInScrn( rect, borderWidth, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }
        #endregion

        #region Fill Rectangle
        static public void FillRectangle ( Rectangle rect, Color color, float layerDepth )
        {
            FillRectangle( rect, color, layerDepth, SpriteBlendMode.AlphaBlend );
        }


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
