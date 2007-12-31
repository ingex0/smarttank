using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Graphics;
using TankEngine2D.DataStructure;
using Microsoft.Xna.Framework;
using SmartTank.GameObjs;

namespace SmartTank.Shelter
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
