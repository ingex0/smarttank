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
     * ʵ�ַ�ʽ��Ե�һ�����Թ������ⷽ������ˡ�
     * 
     * */

    public interface IShelterObj : IHasBorderObj
    {
        GameObjInfo ObjInfo { get;}
    }
}
