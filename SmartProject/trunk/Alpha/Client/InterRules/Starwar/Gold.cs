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
using Microsoft.Xna.Framework.Graphics;
using SmartTank.Draw;

namespace InterRules.Starwar
{
    class Gold : ItemStatic
    {
        public delegate void GoldLiveTimeOutEventHandler(Gold sender);
        public event GoldLiveTimeOutEventHandler OnLiveTimeOut;

        AnimatedSpriteSeries animate;
        float liveTimer = 0;

        public Gold(string name, Vector2 pos, float azi)
        {
            this.name = name;
            this.objInfo = new GameObjInfo("Gold", "");
            this.pos = pos;
            this.azi = azi;

            LoadResource(pos, azi);
            SetCollidSprite();
        }

        private void SetCollidSprite()
        {
            this.sprite = new Sprite(BaseGame.RenderEngine, Path.Combine(Directories.ContentDirectory, "Rules\\SpaceWar\\image\\field_coin_001.png"), true);
            sprite.SetParameters(new Vector2(32, 32), pos, 1, 0, Color.White, LayerDepth.GroundObj, SpriteBlendMode.AlphaBlend);
            this.colMethod = new SpriteColMethod(new Sprite[] { sprite });
        }

        private void LoadResource(Vector2 pos, float azi)
        {
            animate = new AnimatedSpriteSeries(BaseGame.RenderEngine);
            animate.LoadSeriesFromFiles(BaseGame.RenderEngine, Path.Combine(Directories.ContentDirectory, "Rules\\SpaceWar\\image"), "field_coin_00", ".png", 1, 5, false);
            animate.SetSpritesParameters(new Vector2(32, 32), pos, 1, azi, Color.White, LayerDepth.GroundObj, SpriteBlendMode.AlphaBlend);
            GameManager.AnimatedMgr.Add(animate);
            animate.Interval = 10;
            animate.Start();

        }

        public void Dead()
        {
            animate.Stop();
        }

        public override void Update(float seconds)
        {
            liveTimer += seconds;
            if (liveTimer > SpaceWarConfig.GoldLiveTime)
            {
                if (OnLiveTimeOut != null)
                    OnLiveTimeOut(this);
                liveTimer = 0;
            }
        }

        public override void Draw()
        {

        }

        internal void Born(Vector2 pos)
        {
            this.pos = pos;
            animate.SetSpritesParameters(new Vector2(32, 32), pos, 1, azi, Color.White, LayerDepth.GroundObj, SpriteBlendMode.AlphaBlend);
            sprite.Pos = pos;
        }
    }
}
