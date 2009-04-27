using System;
using System.Collections.Generic;
using System.Text;
using GameEngine.Graphics;

namespace GameEngine.PhiCol
{
    /// <summary>
    /// 表示可进行冲突检测的物体
    /// </summary>
    public interface ICollideObj
    {
        /// <summary>
        /// 获得冲突检测者
        /// </summary>
        IColChecker ColChecker { get;}
    }

    /// <summary>
    /// 表示冲突检测者。
    /// 他能够通过IColMethod检测冲突，并处理冲突。
    /// 如果碰撞物体可能是一个运动物体，则必须根据下一个状态进行冲突检测。
    /// </summary>
    public interface IColChecker
    {
        /// <summary>
        /// 获得检测冲突的具体方法
        /// </summary>
        IColMethod CollideMethod { get;}

        /// <summary>
        /// 发生不可重叠式碰撞时调用这个函数。
        /// </summary>
        /// <param name="result">冲突结果</param>
        /// <param name="objB">冲突对象</param>
        void HandleCollision ( CollisionResult result, ICollideObj objA, ICollideObj objB );

        /// <summary>
        /// 当物理更新将造成不合理的重叠时必须将下一状态的撤销到与当前状态一样。
        /// </summary>
        void ClearNextStatus ();

        /// <summary>
        /// 发生可重叠式碰撞时会调用该处理函数。
        /// </summary>
        /// <param name="result">冲突结果</param>
        /// <param name="objB">冲突对象</param>
        void HandleOverlap( CollisionResult result, ICollideObj objA, ICollideObj objB );

    }

    /// <summary>
    /// 表示检测冲突的具体方法。
    /// 当前仅支持两种类型的冲突检测：
    ///     精灵与精灵的像素检测方法；
    ///     精灵与边界的检测方法。
    /// </summary>
    public interface IColMethod
    {
        /// <summary>
        /// 检测与另一对象是否冲突
        /// </summary>
        /// <param name="colB"></param>
        /// <returns></returns>
        CollisionResult CheckCollision ( IColMethod colB );

        /// <summary>
        /// 检测与精灵对象是否冲突
        /// </summary>
        /// <param name="spriteChecker"></param>
        /// <returns></returns>
        CollisionResult CheckCollisionWithSprites ( SpriteColMethod spriteChecker );

        /// <summary>
        /// 检测与边界对象是否冲突
        /// </summary>
        /// <param name="Border"></param>
        /// <returns></returns>
        CollisionResult CheckCollisionWithBorder ( BorderColMethod Border );
    }

    public delegate void OnCollidedEventHandler (CollisionResult result,ICollideObj objA, ICollideObj objB);
}
