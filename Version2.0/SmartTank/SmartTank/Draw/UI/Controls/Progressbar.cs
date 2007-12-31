//=================================================
// xWinForms
// Copyright ?2007 by Eric Grossinger
// http://psycad007.spaces.live.com/
//=================================================
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmartTank.Draw.UI.Controls
{
    public class Progressbar : Control
    {
        #region Variables
        Texture2D border;
        Texture2D progressBar;
        public int width = 200;

        int numberOfBlocks = 20;
        int blockWidth = 10;

        private Rectangle sourceRect = Rectangle.Empty;
        private Rectangle destRect = Rectangle.Empty;

        public Vector4 color;

        public Style style;
        public enum Style
        {
            Continuous,
            Blocks
        }
        #endregion

        #region Construction
        public Progressbar ( string name, Vector2 position, Color color, int width, int value, bool bContinuous )
        {
            Type = ControlType.ProgressBar;
            this.name = name;
            //this.origin = position;
            this.position = position;
            this.width = width;
            this.value = value;
            this.color = color.ToVector4();

            if (bContinuous)
                style = Style.Continuous;
            else
                style = Style.Blocks;

            numberOfBlocks = width / blockWidth + 1;
            max = 100;

            CreateTextures();
        }

        private void CreateTextures ()
        {
            border = new Texture2D( BaseGame.Device, width, 18, 1, Control.resourceUsage, SurfaceFormat.Color );
            progressBar = new Texture2D( BaseGame.Device, width - 8, border.Height - 8, 1, Control.resourceUsage, SurfaceFormat.Color );

            sourceRect = new Rectangle( 0, 0, progressBar.Width, progressBar.Height );
            destRect = new Rectangle( 0, 0, progressBar.Width, progressBar.Height );

            Color[] borderPixel = new Color[border.Width * border.Height];
            Color[] progressBarPixel = new Color[progressBar.Width * progressBar.Height];

            for (int y = 0; y < border.Height; y++)
            {
                for (int x = 0; x < border.Width; x++)
                {
                    if (x < 3 || y < 3 || x > border.Width - 3 || y > border.Height - 3)
                    {
                        if (x == 0 || y == 0 || x == border.Width - 1 || y == border.Height - 1)
                            borderPixel[x + y * border.Width] = Color.Black;
                        if (x == 1 || y == 1 || x == border.Width - 2 || y == border.Height - 2)
                            borderPixel[x + y * border.Width] = Color.LightGray;
                        if (x == 2 || y == 2 || x == border.Width - 3 || y == border.Height - 3)
                            borderPixel[x + y * border.Width] = Color.Gray;
                    }
                    else
                        borderPixel[x + y * border.Width] = new Color( new Vector4( 0.8f, 0.8f, 0.8f, 1f ) );
                }
            }

            for (int y = 0; y < progressBar.Height; y++)
            {
                for (int x = 0; x < progressBar.Width; x++)
                {
                    bool bInvisiblePixel = false;

                    if (style == Style.Blocks)
                    {
                        int xPos = x % (int)(progressBar.Width / (float)numberOfBlocks);
                        if (xPos == 0 || xPos == 1)
                            bInvisiblePixel = true;
                    }

                    if (!bInvisiblePixel)
                    {
                        float gradient = 0.7f - y * 0.065f;
                        Color pixelColor = new Color( new Vector4( color.X * gradient, color.Y * gradient, color.Z * gradient, 1f ) );
                        progressBarPixel[x + y * progressBar.Width] = pixelColor;
                    }
                }
            }

            border.SetData<Color>( borderPixel );
            progressBar.SetData<Color>( progressBarPixel );
        }
        #endregion

        #region Update
        public override void Update ()// Vector2 formPosition, Vector2 formSize )
        {
            //base.Update( formPosition, formSize );

            if (value < 0)
                value = 0;
            if (value > 100)
                value = 100;

            int rectWidth = (int)(progressBar.Width * ((float)value / (float)max));

            //Console.WriteLine(rectWidth);

            if (style == Style.Continuous)
            {
                destRect.Width = rectWidth;
                sourceRect.Width = destRect.Width;
            }
            else
            {
                int totalBlocks = (int)(numberOfBlocks * ((float)value / (float)max));

                int blockWidth = 0;
                if (totalBlocks > 0)
                    blockWidth = (int)(progressBar.Width / (numberOfBlocks));

                destRect.Width = blockWidth * totalBlocks;
                sourceRect.Width = destRect.Width;
            }

            destRect.X = (int)position.X + 4;
            destRect.Y = (int)position.Y + 4;

            //CheckVisibility( formPosition, formSize );
        }

        //private void CheckVisibility ( Vector2 formPosition, Vector2 formSize )
        //{
        //    if (position.X + border.Width > formPosition.X + formSize.X - 15f)
        //        bVisible = false;
        //    else if (position.Y + border.Height > formPosition.Y + formSize.Y - 25f)
        //        bVisible = false;
        //    else
        //        bVisible = true;
        //} 
        #endregion

        #region Draw
        public override void Draw ( SpriteBatch spriteBatch, float alpha )
        {
            Color dynamicColor = new Color( new Vector4( 1f, 1f, 1f, alpha ) );
            spriteBatch.Draw( border, position, dynamicColor );
            spriteBatch.Draw( progressBar, destRect, sourceRect, dynamicColor );
        } 
        #endregion
    }
}
