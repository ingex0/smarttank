using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine.Graphics
{
    /// <summary>
    /// 表示碰撞检测的结果
    /// </summary>
    public class CollisionResult
    {
        /// <summary>
        /// 是否存在碰撞
        /// </summary>
        public bool IsCollided;

        /// <summary>
        /// 碰撞位置，逻辑坐标
        /// </summary>
        public Vector2 InterPos;

        /// <summary>
        /// 碰撞单位法向量，指向自身
        /// </summary>
        public Vector2 NormalVector;

        /// <summary>
        /// 空构造函数
        /// </summary>
        public CollisionResult ()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isCollided">是否发生碰撞</param>
        public CollisionResult ( bool isCollided )
        {
            this.IsCollided = isCollided;
        }

        /// <summary>
        /// 发生了碰撞情况下的构造函数
        /// </summary>
        /// <param name="interPos">碰撞位置，逻辑坐标</param>
        /// <param name="normalVector">碰撞单位法向量，指向自身</param>
        public CollisionResult ( Vector2 interPos, Vector2 normalVector )
        {
            IsCollided = true;
            InterPos = interPos;
            NormalVector = normalVector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isCollided">是否发生碰撞</param>
        /// <param name="interPos">碰撞位置，逻辑坐标</param>
        /// <param name="nornalVector">碰撞单位法向量，指向自身</param>
        public CollisionResult ( bool isCollided, Vector2 interPos, Vector2 nornalVector )
        {
            this.IsCollided = isCollided;
            this.InterPos = interPos;
            this.NormalVector = nornalVector;
        }
    }
}
