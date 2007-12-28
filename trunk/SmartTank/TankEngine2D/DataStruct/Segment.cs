using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TankEngine2D.Helpers;

namespace TankEngine2D.DataStructure
{
    /// <summary>
    /// 表示一条线段
    /// </summary>
    public struct Segment
    {
        /// <summary>
        /// 线段的起始端点
        /// </summary>
        public Vector2 startPoint;
        /// <summary>
        /// 线段的终止端点
        /// </summary>
        public Vector2 endPoint;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint">线段的起始端点</param>
        /// <param name="endPoint">线段的终止端点</param>
        public Segment ( Vector2 startPoint, Vector2 endPoint )
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
        }

        /// <summary>
        /// 判断两Segmet实例是否相等，以重载
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals ( object obj )
        {
            Segment b = (Segment)obj;
            if (this.startPoint == b.startPoint && this.endPoint == b.endPoint ||
                this.startPoint == b.endPoint && this.endPoint == b.startPoint)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 获得Hash值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode ()
        {
            return startPoint.GetHashCode();
        }

        /// <summary>
        /// 将信息转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString ()
        {
            return startPoint.ToString() + " " + endPoint.ToString();
        }

        /// <summary>
        /// 判断两条线段是否相交
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        static public bool IsCross ( Segment a, Segment b )
        {
            return Math.Max( a.startPoint.X, a.endPoint.X ) >= Math.Min( b.startPoint.X, b.endPoint.X ) &&
                   Math.Max( b.startPoint.X, b.endPoint.X ) >= Math.Min( a.startPoint.X, a.endPoint.X ) &&
                   Math.Max( a.startPoint.Y, a.endPoint.Y ) >= Math.Min( b.startPoint.Y, b.endPoint.Y ) &&
                   Math.Max( b.startPoint.Y, b.endPoint.Y ) >= Math.Min( a.startPoint.Y, a.endPoint.Y ) &&
                   MathTools.Vector2Cross( a.endPoint - b.startPoint, a.startPoint - a.endPoint ) *
                   MathTools.Vector2Cross( b.endPoint - a.endPoint, a.startPoint - b.endPoint ) > 0 &&
                   MathTools.Vector2Cross( b.endPoint - a.startPoint, b.startPoint - b.endPoint ) *
                   MathTools.Vector2Cross( a.endPoint - b.endPoint, b.startPoint - a.endPoint ) > 0;
        }
    }
}
