using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TankEngine2D.DataStructure
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
