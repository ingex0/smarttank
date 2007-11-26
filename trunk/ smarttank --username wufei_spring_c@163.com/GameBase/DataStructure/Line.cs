using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameBase.DataStructure
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

        public override string ToString ()
        {
            return pos.ToString() + " " + direction.ToString();
        }
    }
}
