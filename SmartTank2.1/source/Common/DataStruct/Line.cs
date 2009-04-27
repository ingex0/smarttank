using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Common.DataStructure
{
    /// <summary>
    /// 表示一条直线
    /// </summary>
    public struct Line
    {
        /// <summary>
        /// 一个经过该直线的顶点
        /// </summary>
        public Vector2 pos;

        /// <summary>
        /// 表示该直线的方向
        /// </summary>
        public Vector2 direction;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos">一个经过该直线的顶点</param>
        /// <param name="direction">表示该直线的方向</param>
        public Line ( Vector2 pos, Vector2 direction )
        {
            this.pos = pos;
            this.direction = direction;
        }

        /// <summary>
        /// 计算两点的中垂线
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Line MidVerLine( Vector2 point1, Vector2 point2 )
        {
            return new Line( 0.5f * (point1 + point2), Vector2.Normalize( new Vector2( -point2.Y + point1.Y, point2.X - point1.X ) ) );
        }

        /// <summary>
        /// 计算通过定点并与指定直线垂直的直线
        /// </summary>
        /// <param name="line"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Line VerticeLine( Line line, Vector2 point )
        {
            return new Line( point, Vector2.Normalize( new Vector2( -line.direction.Y, line.direction.X ) ) );
        }

        /// <summary>
        /// 计算两直线的交点，当两直线存在交点时返回true
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="result"></param>
        /// <returns>当两直线存在交点时返回true</returns>
        public static bool InterPoint( Line line1, Line line2, out Vector2 result )
        {
            if (Vector2.Normalize( line1.direction ) == Vector2.Normalize( line2.direction ) ||
                Vector2.Normalize( line1.direction ) == -Vector2.Normalize( line2.direction ))
            {
                result = Vector2.Zero;
                return false;
            }
            else
            {
                float k = ((line2.pos.X - line1.pos.X) * line2.direction.Y - (line2.pos.Y - line1.pos.Y) * line2.direction.X) /
                    (line1.direction.X * line2.direction.Y - line1.direction.Y * line2.direction.X);
                result = line1.pos + line1.direction * k;
                return true;
            }
        }

        /// <summary>
        /// 判断两个Line对象是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals ( object obj )
        {
            return this.pos == ((Line)obj).pos && this.direction == ((Line)obj).direction;
        }
        /// <summary>
        /// 获得对象的Hash码。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode ()
        {
            return pos.GetHashCode();
        }

        /// <summary>
        /// 将Line的信息转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString ()
        {
            return pos.ToString() + " " + direction.ToString();
        }
    }
}
