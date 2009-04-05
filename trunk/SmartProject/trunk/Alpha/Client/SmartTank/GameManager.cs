using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SmartTank.PhiCol;
using SmartTank.Draw;
using TankEngine2D.DataStructure;
using SmartTank.Shelter;
using SmartTank.Screens;
using SmartTank.Update;
using SmartTank.Senses.Vision;
using SmartTank.Sounds;
using TankEngine2D.Graphics;
using SmartTank.Senses.Memory;
using SmartTank.Effects.SceneEffects;
using SmartTank.Scene;
using SmartTank.Effects.TextEffects;

namespace SmartTank
{
    /*
     * 使用堆栈结构储存游戏屏幕
     * 
     * 一切游戏屏幕（界面，游戏画面）都从IGameScreen继承。
     * 
     * */

    public class GameManager : BaseGame
    {
        public static event EventHandler OnExit;

        #region Variables

        protected static Stack<IGameScreen> gameScreens = new Stack<IGameScreen>();

        protected static PhiColMgr phiColManager;

        protected static ShelterMgr shelterMgr;

        protected static DrawMgr drawManager;

        protected static UpdateMgr updateMgr;

        protected static VisionMgr visionMgr;

        protected static ObjMemoryMgr objMemoryMananger;


        protected static ISceneKeeper curSceneKeeper;


        #endregion

        #region Properties

        public static PhiColMgr PhiColManager
        {
            get { return phiColManager; }
        }

        public static ShelterMgr ShelterMgr
        {
            get { return shelterMgr; }
        }

        public static DrawMgr DrawManager
        {
            get { return drawManager; }
        }

        public static UpdateMgr UpdateMgr
        {
            get { return updateMgr; }
        }

        public static VisionMgr VisionMgr
        {
            get { return visionMgr; }
        }

        public static ObjMemoryMgr ObjMemoryMgr
        {
            get { return objMemoryMananger; }
        }

        public static ISceneKeeper CurSceneKeeper
        {
            get { return curSceneKeeper; }
        }

        #endregion

        #region Initialize



        protected override void Initialize ()
        {
            base.Initialize();

            phiColManager = new PhiColMgr();
            shelterMgr = new ShelterMgr();
            drawManager = new DrawMgr();
            updateMgr = new UpdateMgr();
            visionMgr = new VisionMgr();
            objMemoryMananger = new ObjMemoryMgr();

            this.Exiting += new EventHandler(GameManager_Exiting);

            Sound.Initial();

            // 在此处将主界面压入堆栈。


            // test

            gameScreens.Push( new RuleSelectScreen() );

            //

        }

        void GameManager_Exiting(object sender, EventArgs e)
        {
            if (OnExit != null)
                OnExit(sender, e);

            while (gameScreens.Count != 0)
            {
                gameScreens.Pop().OnClose();
            }
        }

        #endregion

        #region Update

        protected override void Update ( GameTime gameTime )
        {
            base.Update( gameTime );

            if (gameScreens.Count == 0)
            {
                Exit();
                return;
            }


            float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            GameTimer.UpdateTimers( elapsedSeconds );

            if (gameScreens.Peek().Update( elapsedSeconds ))
            {
                gameScreens.Peek().OnClose();
                gameScreens.Pop();
            }
        }

        #endregion

        #region Add GameScreen

        public static void AddGameScreen ( IGameScreen gameScreen )
        {
            if (gameScreen != null)
                gameScreens.Push( gameScreen );
        }

        #endregion

        #region Draw

        protected override void GameDraw ( GameTime gameTime )
        {
            if (gameScreens.Count != 0)
                gameScreens.Peek().Render();
        }

        #endregion

        #region LoadScene

        public static void LoadScene ( ISceneKeeper scene )
        {
            phiColManager.ClearGroups();
            ShelterMgr.ClearGroups();
            drawManager.ClearGroups();
            updateMgr.ClearGroups();
            visionMgr.ClearGroups();

            scene.RegistDrawables( drawManager );
            scene.RegistPhiCol( phiColManager );
            scene.RegistShelter( ShelterMgr );
            scene.RegistUpdaters( updateMgr );
            scene.RegistVision( visionMgr );

            curSceneKeeper = scene;
        }

        #endregion

        public static void UpdataComponent ( float seconds )
        {
            GameManager.UpdateMgr.Update( seconds );
            GameManager.PhiColManager.Update( seconds );
            GameManager.ShelterMgr.Update();
            GameManager.VisionMgr.Update();
            GameManager.objMemoryMananger.Update();
            EffectsMgr.Update( seconds );
        }

        public static void ComponentReset ()
        {
            EffectsMgr.Clear();
            Sound.Clear();
            GameTimer.ClearAllTimer();
            TextEffectMgr.Clear();
            GameManager.objMemoryMananger.ClearGroups();
            DrawMgr.SetCondition( null );
        }
    }
}
