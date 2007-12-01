using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PictureBoxGird
{
    public partial class PictureBoxGird : PictureBox
    {
        #region Variables

        Bitmap bitmap;

        float scale;
        PointF texFocusPos;

        public event PaintEventHandler LastPaint;

        #endregion

        #region Properties

        public int TexWidth
        {
            get { return bitmap.Width; }
        }

        public int TexHeight
        {
            get { return bitmap.Height; }
        }

        public PointF ScrnCenter
        {
            get { return new PointF( 0.5f * Width, 0.5f * Height ); }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public PointF TexFocusPos
        {
            get { return texFocusPos; }
            set { texFocusPos = value; }
        }

        #endregion

        public PictureBoxGird ()
        {
            InitializeComponent();
        }

        #region Load/Clear Piciture

        public void LoadPicture ( Bitmap bitmap )
        {
            if (bitmap == null)
                return;

            this.bitmap = bitmap;

            ResetScrn();
        }

        private void ResetScrn ()
        {
            float scaleW = ((float)Width) / (float)(bitmap.Width + 2);
            float scaleH = ((float)Height) / (float)(bitmap.Height + 2);

            scale = Math.Min( scaleW, scaleH );
            texFocusPos = new PointF( 0.5f * TexWidth, 0.5f * TexHeight );
        }

        public void ClearPicture ()
        {
            this.bitmap = null;
        }

        public Bitmap CurBitMap
        {
            get { return bitmap; }
        }

        #endregion

        #region Coordin

        public RectangleF RectAtPos ( int x, int y )
        {
            PointF scrnCenter = ScrnCenter;
            PointF upLeft = new PointF( (x - texFocusPos.X) * scale + scrnCenter.X, (y - texFocusPos.Y) * scale + scrnCenter.Y );
            return new RectangleF( upLeft, new SizeF( scale, scale ) );
        }

        public PointF ScrnPos ( int x, int y )
        {
            PointF scrnCenter = ScrnCenter;
            return new PointF( (x - texFocusPos.X) * scale + scrnCenter.X, (y - texFocusPos.Y) * scale + scrnCenter.Y );
        }

        public PointF ScrnPos ( float x, float y )
        {
            PointF scrnCenter = ScrnCenter;
            return new PointF( (x - texFocusPos.X) * scale + scrnCenter.X, (y - texFocusPos.Y) * scale + scrnCenter.Y );
        }

        public PointF TexPos ( float scrnX, float scrnY )
        {
            PointF scrnCenter = ScrnCenter;
            return new PointF( (scrnX - scrnCenter.X) / scale + texFocusPos.X, (scrnY - scrnCenter.Y) / scale + texFocusPos.Y );
        }

        #endregion

        #region Paint

        bool alphaMode = false;

        public bool AlphaMode
        {
            get { return alphaMode; }
            set { alphaMode = value; }
        }

        public void ToggleAlphaMode ()
        {
            alphaMode = !alphaMode;
            this.Invalidate();
        }

        protected override void OnPaint ( PaintEventArgs pe )
        {
            base.OnPaint( pe );
            if (bitmap != null)
            {
                DrawBitMap( pe.Graphics );
                DrawGrid( pe.Graphics );
            }
            if (LastPaint != null)
                LastPaint( this, pe );
        }

        private void DrawBitMap ( Graphics graphics )
        {

            PointF minTexPos = TexPos( 0, 0 );
            PointF maxTexPos = TexPos( Width, Height );

            float width = maxTexPos.X - minTexPos.X;

            if (width > 60)
            {
                graphics.DrawImage( bitmap, new RectangleF( ScrnPos( 0, 0 ), new SizeF( TexWidth * scale, TexHeight * scale ) ) );
            }
            else
            {
                if (alphaMode)
                {
                    for (int y = Math.Max( 0, (int)minTexPos.Y ); y <= Math.Min( bitmap.Height - 1, maxTexPos.Y ); y++)
                    {
                        for (int x = Math.Max( 0, (int)minTexPos.X ); x <= Math.Min( bitmap.Width - 1, maxTexPos.X ); x++)
                        {
                            Color color = bitmap.GetPixel( x, y );
                            graphics.FillRectangle( new SolidBrush( Color.FromArgb( color.A, 255 - color.A, 255 - color.A, 255 - color.A ) ), RectAtPos( x, y ) );
                        }
                    }
                }
                else
                {
                    for (int y = Math.Max( 0, (int)minTexPos.Y ); y <= Math.Min( bitmap.Height - 1, maxTexPos.Y ); y++)
                    {
                        for (int x = Math.Max( 0, (int)minTexPos.X ); x <= Math.Min( bitmap.Width - 1, maxTexPos.X ); x++)
                        {
                            Color color = bitmap.GetPixel( x, y );
                            graphics.FillRectangle( new SolidBrush( color ), RectAtPos( x, y ) );
                        }
                    }
                }
            }

        }

        private void DrawGrid ( Graphics graphics )
        {
            int interval = (int)(Height / (scale * 100));
            interval = Math.Max( 1, interval );

            float width = scale * (bitmap.Width + 2);
            float height = scale * (bitmap.Height + 2);

            for (int i = -1; i <= bitmap.Width + 1; i += interval)
            {
                graphics.DrawLine( Pens.Brown, ScrnPos( i, -1 ), ScrnPos( i, TexHeight + 1 ) );
            }
            for (int i = -1; i <= bitmap.Height + 1; i += interval)
            {
                graphics.DrawLine( Pens.Brown, ScrnPos( -1, i ), ScrnPos( TexWidth + 1, i ) );
            }
        }

        #endregion

        #region MouseControl

        PointF mouseDownPos;
        bool mouseDown = false;
        PointF preFocusPos;
        float preScale;

        enum CoodinChange
        {
            None,
            Move,
            Zoom,
        }
        CoodinChange coordinChange;

        float moveFactor = 1f;

        public bool Controlling
        {
            get { return coordinChange != CoodinChange.None; }
        }

        [Description( "指定平移的鼠标灵敏度" ), Category( "控制" )]
        public float MoveFactor
        {
            get { return moveFactor; }
            set { moveFactor = value; }
        }

        float zoomFactor = 1f;

        [Description( "指定缩放的鼠标灵敏度" ), Category( "控制" )]
        public float ZoomFactor
        {
            get { return zoomFactor; }
            set { zoomFactor = value; }
        }

        float zoomWheelFactor = 1f;

        [Description( "指定缩放的鼠标滑轮灵敏度" ), Category( "控制" )]
        public float ZoomWheelFactor
        {
            get { return zoomWheelFactor; }
            set { zoomWheelFactor = value; }
        }

        protected override void OnMouseEnter ( EventArgs e )
        {
            base.OnMouseEnter( e );
            this.Focus();
        }

        protected override void OnMouseDown ( MouseEventArgs e )
        {
            base.OnMouseDown( e );
            mouseDownPos = new PointF( e.X, e.Y );
            preFocusPos = TexFocusPos;
            preScale = Scale;
            mouseDown = true;
        }

        protected override void OnMouseMove ( MouseEventArgs e )
        {
            base.OnMouseMove( e );

            if (mouseDown)
            {
                PointF curMousePos = new PointF( e.X, e.Y );
                if (coordinChange == CoodinChange.Move)
                {
                    texFocusPos = new PointF( preFocusPos.X + (curMousePos.X - mouseDownPos.X) * moveFactor * 0.2f, preFocusPos.Y + (curMousePos.Y - mouseDownPos.Y) * moveFactor * 0.2f );
                }
                else if (coordinChange == CoodinChange.Zoom)
                {
                    float delta = (curMousePos.Y - mouseDownPos.Y) * 0.01f * zoomFactor;
                    delta = Math.Max( -0.9f, delta );
                    scale = preScale * (1 + delta);
                }
                else
                    return;

                this.Invalidate();
            }
        }

        protected override void OnMouseUp ( MouseEventArgs e )
        {
            base.OnMouseUp( e );

            mouseDown = false;
        }

        protected override void OnKeyDown ( KeyEventArgs e )
        {
            base.OnKeyDown( e );

            if (e.Control || e.KeyCode == Keys.Z)
            {
                if (e.Control)
                    coordinChange = CoodinChange.Move;
                else if (e.KeyCode == Keys.Z)
                    coordinChange = CoodinChange.Zoom;
            }
            else if (e.Shift)
            {
                ResetScrn();
                this.Invalidate();
            }
            else if (e.KeyData == Keys.A)
            {
                ToggleAlphaMode();
            }
        }

        protected override void OnKeyUp ( KeyEventArgs e )
        {
            base.OnKeyUp( e );

            coordinChange = CoodinChange.None;
        }

        protected override void OnLostFocus ( EventArgs e )
        {
            base.OnLostFocus( e );
            coordinChange = CoodinChange.None;
        }

        protected override void OnMouseWheel ( MouseEventArgs e )
        {
            base.OnMouseWheel( e );
            float delta = Math.Max( -0.9f, Math.Min( 9f, (float)(e.Delta) * 0.002f * zoomWheelFactor ) );
            scale = scale * (1 + delta);
            this.Invalidate();
        }

        #endregion
    }
}
