using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameBase.DataStructure;
using Microsoft.Xna.Framework.Graphics;

namespace GameBase.Helpers
{
    public static class ConvertHelper
    {
        public static Vector2 PointToVector2 ( Point p )
        {
            return new Vector2( p.X, p.Y );
        }

        public static Point Vector2ToPoint ( Vector2 v )
        {
            return new Point( MathTools.Round( v.X ), MathTools.Round( v.Y ) );
        }

        public static Rectanglef RectangleToRectanglef ( Rectangle rect )
        {
            return new Rectanglef( rect.X, rect.Y, rect.Width, rect.Height );
        }

        public static Rectangle RectanglefToRectangle ( Rectanglef rect )
        {
            return new Rectangle( (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height );
        }

        public static Color SysColorToXNAColor ( System.Drawing.Color sysColor )
        {
            return new Color( sysColor.R, sysColor.G, sysColor.B, sysColor.A );
        }
    }
}
