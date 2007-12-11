using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TankEngine2D.DataStructure;
using Microsoft.Xna.Framework.Graphics;

namespace TankEngine2D.Helpers
{
    /// <summary>
    /// 类型转换辅助类
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        /// 将Point类型转换为Vector2类型
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Vector2 PointToVector2 ( Point p )
        {
            return new Vector2( p.X, p.Y );
        }
        /// <summary>
        /// 将Vector2类型转换为Point类型，进行四舍五入操作
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Point Vector2ToPoint ( Vector2 v )
        {
            return new Point( MathTools.Round( v.X ), MathTools.Round( v.Y ) );
        }
        /// <summary>
        /// 将Rectangle类型转换为Rectanglef类型
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rectanglef RectangleToRectanglef ( Rectangle rect )
        {
            return new Rectanglef( rect.X, rect.Y, rect.Width, rect.Height );
        }
        /// <summary>
        /// 将Rectanglef类型转换为Rectangle类型，进行取整操作
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rectangle RectanglefToRectangle ( Rectanglef rect )
        {
            return new Rectangle( (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height );
        }
        /// <summary>
        /// 将C#标准库中的Color类型转换为XNA的Color类型
        /// </summary>
        /// <param name="sysColor"></param>
        /// <returns></returns>
        public static Color SysColorToXNAColor ( System.Drawing.Color sysColor )
        {
            return new Color( sysColor.R, sysColor.G, sysColor.B, sysColor.A );
        }
    }
}
