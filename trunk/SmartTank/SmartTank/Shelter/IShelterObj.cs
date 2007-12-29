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
     * ʵ�ַ�ʽ��Ե�һ�����Թ������ⷽ������ˡ�
     * 
     * */

    public interface IShelterObj : IHasBorderObj
    {
        GameObjInfo ObjInfo { get;}
    }
}
