using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TankEngine2D.DataStructure
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
        /// <summary>
        /// �ж�����Line�����Ƿ����
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals ( object obj )
        {
            return this.pos == ((Line)obj).pos && this.direction == ((Line)obj).direction;
        }
        /// <summary>
        /// ��ö����Hash�롣
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode ()
        {
            return pos.GetHashCode();
        }

        /// <summary>
        /// ��Line����Ϣת��Ϊ�ַ���
        /// </summary>
        /// <returns></returns>
        public override string ToString ()
        {
            return pos.ToString() + " " + direction.ToString();
        }
    }
}
