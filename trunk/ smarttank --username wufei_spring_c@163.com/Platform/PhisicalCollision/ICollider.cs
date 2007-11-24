using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Graphics;
using Platform.GameObjects;

namespace Platform.PhisicalCollision
{
    public interface ICollideObj : IHasBorderObj
    {
        GameObjInfo ObjInfo { get;}
        IColChecker ColChecker { get;}
    }

    public interface IColChecker
    {
        // 新的处理碰撞的办法。不在与具体的碰撞检测方法耦合。
        IColMethod CollideMethod { get;}

        /// <summary>
        /// 发生不可重叠式碰撞时调用这个函数。
        /// </summary>
        /// <param name="result"></param>
        /// <param name="objB"></param>
        void HandleCollision ( CollisionResult result, GameObjInfo objB );

        /// <summary>
        /// 当物理更新将造成不合理的重叠时必须将下一状态的撤销到与当前状态一样。
        /// </summary>
        void ClearNextStatus ();

        /// <summary>
        /// 发生可重叠式碰撞时会调用该处理函数。
        /// </summary>
        /// <param name="result"></param>
        /// <param name="gameObjInfo"></param>
        void HandleOverlap ( CollisionResult result, GameObjInfo objB );

    }

    public interface IColMethod
    {
        CollisionResult CheckCollision ( IColMethod colB );
        CollisionResult CheckCollisionWithSprites ( SpriteColMethod spriteChecker );
        CollisionResult CheckCollisionWithBorder ( BorderMethod Border );
    }

    public delegate void OnCollidedEventHandler ( IGameObj Sender, CollisionResult result, GameObjInfo objB );
}
