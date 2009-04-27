using System;
using System.Collections.Generic;
using System.Text;
using GameEngine.Graphics;
using Common.DataStructure;
using Microsoft.Xna.Framework;

namespace GameEngine.Shelter
{
    /* 
     * 实现方式相对单一，所以姑且与检测方法耦合了。
     * 
     * */

    public interface IShelterObj : IHasBorderObj
    {
    }
}
