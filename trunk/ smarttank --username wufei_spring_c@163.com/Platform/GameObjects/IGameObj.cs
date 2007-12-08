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
     * �����˳��������һ��ӿڡ�
     * 
     * */


    public interface IGameObj : IUpdater, IDrawableObj
    {
        GameObjInfo ObjInfo { get;}
        new Vector2 Pos { get;set;}
        float Azi { get;}
    }


}
