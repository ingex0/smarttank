using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameBase.DataStructure
{
    /// <summary>
    /// ��ʾһ��ֱ��
    /// </summary>
    public struct Line
    {
        /// <summary>
        /// һ��������ֱ�ߵĶ���
        /// </summary>
        public Vector2 pos;

        /// <summary>
        /// ��ʾ��ֱ�ߵķ���
        /// </summary>
        public Vector2 direction;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos">һ��������ֱ�ߵĶ���</param>
        /// <param name="direction">��ʾ��ֱ�ߵķ���</param>
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
