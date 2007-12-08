using System;
using System.Collections.Generic;
using System.Text;
using Platform.PhisicalCollision;
using Platform.Shelter;
using Platform.Update;
using Platform.GameDraw;
using Microsoft.Xna.Framework;

namespace Platform.GameObjects
{
    /*
     * 定义了场景物体的一般接口。
     * 
     * */


    public interface IGameObj : IUpdater, IDrawableObj
    {
        GameObjInfo ObjInfo { get;}
        new Vector2 Pos { get;set;}
        float Azi { get;}
    }


}
