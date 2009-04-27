using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Common.DataStructure;
using GameEngine.Graphics;

namespace GameEngine.Shelter
{
    public interface IHasBorderObj
    {
        CircleList<BorderPoint> BorderData { get;}
        
        /// <summary>
        /// 暂时定义为贴图坐标到屏幕坐标的转换，当改写Coordin类以后修该为贴图坐标到逻辑坐标的转换。
        /// </summary>
        Matrix WorldTrans { get;}

        Rectanglef BoundingBox { get;}
    }
}
