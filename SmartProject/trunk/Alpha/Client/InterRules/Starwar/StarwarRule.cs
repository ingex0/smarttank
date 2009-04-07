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
        #region IGameRule ��Ա

        public string RuleIntroduction
        {
            get { return "20090328~20090417:��д���Ա��..."; }
        }

        public string RuleName
        {
            get { return "Starwar"; }
        }

        #endregion

        #region IGameScreen ��Ա

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


        #region IGameScreen ��Ա

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


        #region IPhisicalObj ��Ա

        protected NonInertiasColUpdater phisicalUpdater;

        public IPhisicalUpdater PhisicalUpdater
        {
            get { return phisicalUpdater; }
        }

        #endregion

        #region ICollideObj ��Ա

        public IColChecker ColChecker
        {
            get { return phisicalUpdater; }
        }

        #endregion

        #region IGameObj ��Ա

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

        #region IUpdater ��Ա

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

        #region IDrawableObj ��Ա

        public void Draw()
        {
            texture.Draw();
        }

        #endregion
    }
}
