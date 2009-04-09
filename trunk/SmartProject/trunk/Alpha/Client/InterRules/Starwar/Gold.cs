using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.GameObjs;
using SmartTank.PhiCol;
using TankEngine2D.Graphics;
using Microsoft.Xna.Framework;
using SmartTank.GameObjs.Item;
using SmartTank;
using SmartTank.Helpers;
using System.IO;

namespace InterRules.Starwar
{
    class Gold : ItemStatic
    {
        AnimatedSpriteSeries animate;

        public Gold(string name,Vector2 pos,float azi)
        {
            this.name = name;
            this.objInfo = new GameObjInfo("Gold", "");
            this.pos = pos;
            this.azi = azi;

            LoadResource(pos, azi);
        }

        private void LoadResource(Vector2 pos, float azi)
        {
            animate = new AnimatedSpriteSeries(BaseGame.RenderEngine);
            animate.LoadSeriesFromFiles(BaseGame.RenderEngine,Path.Combine( Directories.ContentDirectory ,"Rules\\SpaceWar\\image"),"field_coin_001",".png",
        }

        public override void Update(float seconds)
        {
        }

        public override void Draw()
        {
            
        }

    }
}
