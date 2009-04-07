using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Rule;
using SmartTank.Screens;
using SmartTank.net;
using SmartTank.GameObjs;
using SmartTank.PhiCol;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankEngine2D.Graphics;
using SmartTank;
using SmartTank.Draw;

namespace InterRules.Starwar
{
    class StarwarRule : IGameRule
    {
        #region IGameRule 成员

        public string RuleIntroduction
        {
            get { return "20090328~20090417:编写组成员：..."; }
        }

        public string RuleName
        {
            get { return "Starwar"; }
        }

        #endregion

        #region IGameScreen 成员

        public void OnClose()
        {
        }

        public void Render()
        {

        }

        public bool Update(float second)
        {
            return false;
        }

        #endregion
    }


    class StarwarLogic : RuleSupNet, IGameScreen
    {



        public StarwarLogic()
        {

        }


        #region IGameScreen 成员

        void IGameScreen.OnClose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void IGameScreen.Render()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool IGameScreen.Update(float second)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }


    class WarShip : IGameObj, ICollideObj, IPhisicalObj
    {
        public WarShip(Vector2 pos, float azi)
        {
            InitializeTex(pos, azi);
        }

        private void InitializeTex(Vector2 pos, float azi)
        {
            texture = new Sprite(BaseGame.RenderEngine);
            texture.LoadTextureFromFile("", true);
            texture.SetParameters(new Vector2(0, 0), pos, 1.0f, azi, Color.White, LayerDepth.TankBase, SpriteBlendMode.AlphaBlend);
        }

        Sprite texture;    


        #region IPhisicalObj 成员

        protected NonInertiasColUpdater phisicalUpdater;

        public IPhisicalUpdater PhisicalUpdater
        {
            get { return phisicalUpdater; }
        }

        #endregion

        #region ICollideObj 成员

        public IColChecker ColChecker
        {
            get { return phisicalUpdater; }
        }

        #endregion

        #region IGameObj 成员

        protected float azi;
        public float Azi
        {
            get { return azi; }
        }

        protected string mgPath;
        public string MgPath
        {
            get
            {
                return mgPath;
            }
            set
            {
                mgPath = value;
            }
        }

        protected string name;
        public string Name
        {
            get { return name; }
        }

        protected GameObjInfo objInfo;
        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        protected Vector2 pos;
        public Microsoft.Xna.Framework.Vector2 Pos
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
            }
        }

        #endregion

        #region IUpdater 成员

        protected bool openControl;
        public void Update(float seconds)
        {
            if (openControl)
                HandlerControl(seconds);
        }

        private void HandlerControl(float seconds)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDrawableObj 成员

        public void Draw()
        {
            texture.Draw();
        }

        #endregion
    }
}
