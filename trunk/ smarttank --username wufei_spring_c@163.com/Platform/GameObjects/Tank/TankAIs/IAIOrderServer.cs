using System;
using System.Collections.Generic;
using System.Text;
using Platform.Senses.Vision;
using Microsoft.Xna.Framework;
using Platform.PhisicalCollision;
using GameBase.Graphics;
using Platform.Senses.Memory;
using GameBase.DataStructure;

namespace Platform.GameObjects.Tank.TankAIs
{
    public delegate void OnCollidedEventHandlerAI ( CollisionResult result, GameObjInfo objB );

    public interface IAIOrderServer
    {
        /// <summary>
        /// 获得可见物体的信息
        /// </summary>
        /// <returns></returns>
        List<IEyeableInfo> GetEyeableInfo ();

        /// <summary>
        /// 获得坦克的当前位置
        /// </summary>
        Vector2 Pos { get;}

        /// <summary>
        /// 获得坦克当前的方位角
        /// </summary>
        float Azi { get;}

        /// <summary>
        /// 获得坦克当前朝向的单位向量
        /// </summary>
        Vector2 Direction { get;}

        /// <summary>
        /// 获得坦克前进的极限速度
        /// </summary>
        float MaxForwardSpeed { get;}

        /// <summary>
        /// 获得坦克后退的极限速度
        /// </summary>
        float MaxBackwardSpeed { get;}

        /// <summary>
        /// 获得坦克的极限旋转速度
        /// </summary>
        float MaxRotaSpeed { get;}

        /// <summary>
        /// 获得坦克当前的前进速度，如果值为负，表示坦克正在后退
        /// </summary>
        float ForwardSpeed { get; set;}

        /// <summary>
        /// 获得坦克当前的旋转角速度。弧度单位，顺时针旋转为正。
        /// </summary>
        float TurnRightSpeed { get; set;}

        /// <summary>
        /// 获得坦克雷达的半径
        /// </summary>
        float RaderRadius { get;}

        /// <summary>
        /// 获得坦克张角的一半
        /// </summary>
        float RaderAng { get;}

        /// <summary>
        /// 获得坦克雷达当前相对于坦克的方向
        /// </summary>
        float RaderAzi { get;}

        /// <summary>
        /// 获得坦克雷达当前的方位角
        /// </summary>
        float RaderAimAzi { get;}

        /// <summary>
        /// 获得坦克雷达的旋转极限速度
        /// </summary>
        float MaxRotaRaderSpeed { get;}

        /// <summary>
        /// 获得当前坦克雷达的旋转速度，顺时针方向为正
        /// </summary>
        float TurnRaderWiseSpeed { get; set;}

        /// <summary>
        /// 获得该坦克发出炮弹的速度
        /// </summary>
        float ShellSpeed { get;}

        /// <summary>
        /// 获得坦克的宽度
        /// </summary>
        float TankWidth { get;}

        /// <summary>
        /// 获得坦克的长度
        /// </summary>
        float TankLength { get;}

        /// <summary>
        /// 获得当前坦克可见的有边界的物体
        /// </summary>
        EyeableBorderObjInfo[] EyeableBorderObjInfos { get;}

        /// <summary>
        /// 计算导航图
        /// </summary>
        /// <param name="selectFun">定义在生成导航图的时候考虑哪些有边界物体的函数</param>
        /// <param name="mapBorder">地图的边界</param>
        /// <param name="spaceForTank">离碰撞物的边界多远的距离上标记警戒线</param>
        /// <returns></returns>
        NavigateMap CalNavigateMap ( NaviMapConsiderObj selectFun, Rectanglef mapBorder, float spaceForTank );

        /// <summary>
        /// 当坦克发生碰撞时
        /// </summary>
        event OnCollidedEventHandlerAI OnCollide;

        /// <summary>
        /// 当坦克发生重叠时
        /// </summary>
        event OnCollidedEventHandlerAI OnOverLap;
    }
}
