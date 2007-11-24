using System;
using System.Collections.Generic;
using System.Text;
using GameBase;
using Microsoft.Xna.Framework;
using Platform.PhisicalCollision;
using Platform.GameDraw;
using GameBase.DataStructure;
using Platform.Shelter;
using Platform.GameScreens;
using Platform.Update;
using Platform.Scene;
using Platform.Senses.Vision;
using Platform.Sounds;
using GameBase.Graphics;
using Platform.UpdateManage;
using Platform.GameDraw.SceneEffects;
using Platform.Senses.Memory;

namespace Platform
{
    /*
     * 使用堆栈结构储存游戏屏幕
     * 
     * 一切游戏屏幕（界面，游戏画面）都从IGameScreen继承。
     * 
     * */

    public class GameManager : BaseGame
    {
        #region Variables

        protected static Stack<IGameScreen> gameScreens = new Stack<IGameScreen>();

        protected static PhiColManager phiColManager;

        protected static ShelterManager shelterManager;

        protected static DrawManager drawManager;

        protected static UpdateManager updateManager;

        protected static VisionManager visionManager;

        protected static ObjMemoryManager objMemoryMananger;


        protected static ISceneKeeper curSceneKeeper;


        #endregion

        #region Properties

        public static PhiColManager PhiColManager
        {
            get { return phiColManager; }
        }

        public static ShelterManager ShelterManager
        {
            get { return shelterManager; }
        }

        public static DrawManager DrawManager
        {
            get { return drawManager; }
        }

        public static UpdateManager UpdateManager
        {
            get { return updateManager; }
        }

        public static VisionManager VisionManager
        {
            get { return visionManager; }
        }

        public static ObjMemoryManager ObjMemoryManager
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

            phiColManager = new PhiColManager();
            shelterManager = new ShelterManager();
            drawManager = new DrawManager();
            updateManager = new UpdateManager();
            visionManager = new VisionManager();
            objMemoryMananger = new ObjMemoryManager();

            Sound.Initial();

            // 在此处将主界面压入堆栈。


            // test

            gameScreens.Push( new RuleSelectScreen() );

            //

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

            if (gameScreens.Peek().Update( elapsedSeconds ))
            {
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
            shelterManager.ClearGroups();
            drawManager.ClearGroups();
            updateManager.ClearGroups();
            visionManager.ClearGroups();

            scene.RegistDrawables( drawManager );
            scene.RegistPhiCol( phiColManager );
            scene.RegistShelter( shelterManager );
            scene.RegistUpdaters( updateManager );
            scene.RegistVision( visionManager );

            curSceneKeeper = scene;
        }

        #endregion

        public static void UpdataComponent ( float seconds )
        {
            GameManager.UpdateManager.Update( seconds );
            GameManager.PhiColManager.Update( seconds );
            GameManager.ShelterManager.Update();
            GameManager.VisionManager.Update();
            GameManager.objMemoryMananger.Update();
            EffectsManager.Update( seconds );
        }

        public static void ComponentReset ()
        {
            EffectsManager.Clear();
            Sound.Clear();
            GameTimer.ClearAllTimer();
            TextEffect.Clear();
            GameManager.objMemoryMananger.ClearGroups();
            DrawManager.SetCondition( null );
        }
    }
}
