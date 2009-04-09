using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.GameObjs.Item;
using Microsoft.Xna.Framework;
using System.IO;
using SmartTank.Helpers;

namespace InterRules.Starwar
{
    enum RockTexNo
    {
        oo1,
        ooo1,
        oo2,
        ooo2,
        oo3,
        ooo3,
        Max,
    }

    class Rock : ItemCommon
    {
        static string[] texPaths = {
            "field_map_001.png","field_map_0001.png",
            "field_map_002.png","field_map_0002.png",
            "field_map_003.png","field_map_0003.png",
            "field_map_004.png","field_map_0004.png"};
        static Vector2[] texOrigin = {
            new Vector2(150,120),new Vector2(37,32),
            new Vector2(75,60),new Vector2(60,45),
            new Vector2(74,63),new Vector2(60,32),
            new Vector2(60,45),new Vector2(50,47)};


        public Rock(string name, Vector2 startPos, Vector2 vel, float aziVel, float scale, RockTexNo texNo)
            : base(name, "Rock", "",
            Path.Combine(Directories.ContentDirectory, "Rules\\SpaceWar\\image\\" + texPaths[(int)texNo]),
            texOrigin[(int)texNo], scale,new Vector2[0], startPos, 0, vel, aziVel)
        {
        }

        
    }
}
