using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TankEngine2D.DataStructure;
using Microsoft.Xna.Framework.Graphics;

namespace TankEngine2D.Helpers
{
    /// <summary>
    /// ����ת��������
    /// </summary>
    public static class ConvertHelper
    {
        /// <summary>
        /// ��Point����ת��ΪVector2����
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Vector2 PointToVector2 ( Point p )
        {
            return new Vector2( p.X, p.Y );
        }
        /// <summary>
        /// ��Vector2����ת��ΪPoint���ͣ����������������
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Point Vector2ToPoint ( Vector2 v )
        {
            return new Point( MathTools.Round( v.X ), MathTools.Round( v.Y ) );
        }
        /// <summary>
        /// ��Rectangle����ת��ΪRectanglef����
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rectanglef RectangleToRectanglef ( Rectangle rect )
        {
            return new Rectanglef( rect.X, rect.Y, rect.Width, rect.Height );
        }
        /// <summary>
        /// ��Rectanglef����ת��ΪRectangle���ͣ�����ȡ������
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Rectangle RectanglefToRectangle ( Rectanglef rect )
        {
            return new Rectangle( (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height );
        }
        /// <summary>
        /// ��C#��׼���е�Color����ת��ΪXNA��Color����
        /// </summary>
        /// <param name="sysColor"></param>
        /// <returns></returns>
        public static Color SysColorToXNAColor ( System.Drawing.Color sysColor )
        {
            return new Color( sysColor.R, sysColor.G, sysColor.B, sysColor.A );
        }
    }
}
