using System;
using System.Collections.Generic;
using System.Text;
using GameBase.Graphics;
using GameBase.DataStructure;
using Microsoft.Xna.Framework;
using Platform.GameObjects;
using Platform.PhisicalCollision;

namespace Platform.Shelter
{
    /* 
     * 实现方式相对单一，所以姑且与检测方法耦合了。
     * 
     * */

    public interface IShelterObj : IHasBorderObj
    {
        GameObjInfo ObjInfo { get;}
    }
}
