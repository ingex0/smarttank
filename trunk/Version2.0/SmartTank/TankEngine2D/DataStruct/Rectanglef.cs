using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TankEngine2D.DataStructure
{
    /// <summary>
    /// 以浮点格式表示一个矩形
    /// </summary>
    [Serializable]
    public struct Rectanglef
    {
        /// <summary>
        /// 矩形的左上角坐标
        /// </summary>
        public float X, Y;

        /// <summary>
        /// 矩形的宽度和高度
        /// </summary>
        public float Width, Height;

        /// <summary>
        /// 获得矩形的上边沿的Y坐标
        /// </summary>
        public float Top
        {
            get { return Y; }
        }
        /// <summary>
        /// 获得矩形左边沿的X坐标
        /// </summary>
        public float Left
        {
            get { return X; }
        }
        /// <summary>
        /// 获得矩形下边沿的Y坐标
        /// </summary>
        public float Bottom
        {
            get { return Y + Height; }
        }
        /// <summary>
        /// 获得矩形右边沿的X坐标
        /// </summary>
        public float Right
        {
            get { return X + Width; }
        }
        /// <summary>
        /// 获得矩形的左上角顶点
        /// </summary>
        public Vector2 UpLeft
        {
            get { return new Vector2( X, Y ); }
        }
        /// <summary>
        /// 获得矩形的右上角顶点
        /// </summary>
        public Vector2 UpRight
        {
            get { return new Vector2( X + Width, Y ); }
        }
        /// <summary>
        /// 获得矩形的左下角顶点
        /// </summary>
        public Vector2 DownLeft
        {
            get { return new Vector2( X, Y + Height ); }
        }
        /// <summary>
        /// 获得矩形的右下角顶点
        /// </summary>
        public Vector2 DownRight
        {
            get { return new Vector2( X + Width, Y + Height ); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">矩形左上角顶点的X坐标</param>
        /// <param name="y">矩形左上角顶点的Y坐标</param>
        /// <param name="width">矩形的宽度</param>
        /// <param name="height">矩形的高度</param>
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
        /// <param name="min">矩形的左上角顶点</param>
        /// <param name="max">矩形的右下角顶点</param>
        public Rectanglef ( Vector2 min, Vector2 max )
        {
            this.X = min.X;
            this.Y = min.Y;
            this.Width = max.X - min.X;
            this.Height = max.Y - min.Y;
        }

        /// <summary>
        /// 判断两个矩形是否有重叠部分
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
        /// 判断一个点是否在矩形之中
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
        /// 判断两个Rectanglef实例是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals ( object obj )
        {
            Rectanglef b = (Rectanglef)obj;
            return this.X == b.X && this.Y == b.Y && this.Width == b.Width && this.Height == b.Height;
        }
        /// <summary>
        /// 获得Hash码
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode ()
        {
            return X.GetHashCode();
        }
        /// <summary>
        /// 将信息转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString ()
        {
            return X.ToString() + " " + Y.ToString() + " " + Width.ToString() + " " + Height.ToString();
        }
    }
}
