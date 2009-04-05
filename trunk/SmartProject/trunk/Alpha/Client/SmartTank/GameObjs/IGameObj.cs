using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Update;
using SmartTank.Draw;
using Microsoft.Xna.Framework;
using TankEngine2D.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace SmartTank.GameObjs
{
    /*
     * 定义了场景物体的一般接口。
     * 
     * */

    public interface IGameObj : IUpdater, IDrawableObj
    {
        string Name { get;}
        string MgPath { get; set;}
        GameObjInfo ObjInfo { get;}
        new Vector2 Pos { get;set;}
        float Azi { get;}
    }

    public delegate void OnCollidedEventHandler( IGameObj Sender, CollisionResult result, GameObjInfo objB );
}
