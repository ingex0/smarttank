using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Update;
using SmartTank.Draw;
using Microsoft.Xna.Framework;
using TankEngine2D.Graphics;

namespace SmartTank.GameObjs
{
    /*
     * �����˳��������һ��ӿڡ�
     * 
     * */


    public interface IGameObj : IUpdater, IDrawableObj
    {
        GameObjInfo ObjInfo { get;}
        new Vector2 Pos { get;set;}
        float Azi { get;}
    }

    public delegate void OnCollidedEventHandler( IGameObj Sender, CollisionResult result, GameObjInfo objB );
}
