using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TankEngine2D.DataStructure
{
    /// <summary>
    /// �Ը����ʽ��ʾһ������
    /// </summary>
    [Serializable]
    public struct Rectanglef
    {
        /// <summary>
        /// ���ε����Ͻ�����
        /// </summary>
        public float X, Y;

        /// <summary>
        /// ���εĿ�Ⱥ͸߶�
        /// </summary>
        public float Width, Height;

        /// <summary>
        /// ��þ��ε��ϱ��ص�Y����
        /// </summary>
        public float Top
        {
            get { return Y; }
        }
        /// <summary>
        /// ��þ�������ص�X����
        /// </summary>
        public float Left
        {
            get { return X; }
        }
        /// <summary>
        /// ��þ����±��ص�Y����
        /// </summary>
        public float Bottom
        {
            get { return Y + Height; }
        }
        /// <summary>
        /// ��þ����ұ��ص�X����
        /// </summary>
        public float Right
        {
            get { return X + Width; }
        }
        /// <summary>
        /// ��þ��ε����ϽǶ���
        /// </summary>
        public Vector2 UpLeft
        {
            get { return new Vector2( X, Y ); }
        }
        /// <summary>
        /// ��þ��ε����ϽǶ���
        /// </summary>
        public Vector2 UpRight
        {
            get { return new Vector2( X + Width, Y ); }
        }
        /// <summary>
        /// ��þ��ε����½Ƕ���
        /// </summary>
        public Vector2 DownLeft
        {
            get { return new Vector2( X, Y + Height ); }
        }
        /// <summary>
        /// ��þ��ε����½Ƕ���
        /// </summary>
        public Vector2 DownRight
        {
            get { return new Vector2( X + Width, Y + Height ); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">�������ϽǶ����X����</param>
        /// <param name="y">�������ϽǶ����Y����</param>
        /// <param name="width">���εĿ��</param>
        /// <param name="height">���εĸ߶�</param>
        public Rectanglef ( float x, float y, float width, float height )
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min">���ε����ϽǶ���</param>
        /// <param name="max">���ε����½Ƕ���</param>
        public Rectanglef ( Vector2 min, Vector2 max )
        {
            this.X = min.X;
            this.Y = min.Y;
            this.Width = max.X - min.X;
            this.Height = max.Y - min.Y;
        }

        /// <summary>
        /// �ж����������Ƿ����ص�����
        /// </summary>
        /// <param name="rectB"></param>
        /// <returns></returns>
        public bool Intersects ( Rectanglef rectB )
        {
            if (rectB.X < this.X + this.Width && rectB.X + rectB.Width > this.X &&
                rectB.Y + rectB.Height > this.Y && rectB.Y < this.Y + this.Height ||
                rectB.X + rectB.Width > this.X && rectB.X < this.X + this.Width &&
                rectB.Y + rectB.Height > this.Y && rectB.Y < this.Y + this.Height)
                return true;
            else
                return false;
        }

        /// <summary>
        /// �ж�һ�����Ƿ��ھ���֮��
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains ( Vector2 point )
        {
            if (X <= point.X && point.X <= X + Width && Y <= point.Y && point.Y <= Y + Height)
                return true;
            else
                return false;
        }
        /// <summary>
        /// �ж�����Rectanglefʵ���Ƿ����
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals ( object obj )
        {
            Rectanglef b = (Rectanglef)obj;
            return this.X == b.X && this.Y == b.Y && this.Width == b.Width && this.Height == b.Height;
        }
        /// <summary>
        /// ���Hash��
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode ()
        {
            return X.GetHashCode();
        }
        /// <summary>
        /// ����Ϣת��Ϊ�ַ���
        /// </summary>
        /// <returns></returns>
        public override string ToString ()
        {
            return X.ToString() + " " + Y.ToString() + " " + Width.ToString() + " " + Height.ToString();
        }
    }
}
