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
     * ʹ�ö�ջ�ṹ������Ϸ��Ļ
     * 
     * һ����Ϸ��Ļ�����棬��Ϸ���棩����IGameScreen�̳С�
     * 
     * */

    public class GameManager : BaseGame
    {
        #region Variables

        protected static Stack<IGameScreen> gameScreens = new Stack<IGameScreen>();

        protected static PhiColMgr phiColManager;

        protected static ShelterMgr shelterMgr;

        protected static DrawManager drawManager;

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

        public static DrawManager DrawManager
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
            drawManager = new DrawManager();
            updateMgr = new UpdateMgr();
            visionMgr = new VisionMgr();
            objMemoryMananger = new ObjMemoryMgr();

            Sound.Initial();

            // �ڴ˴���������ѹ���ջ��


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

            GameTimer.UpdateTimers( elapsedSeconds );

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
            TextEffect.Clear();
            GameManager.objMemoryMananger.ClearGroups();
            DrawManager.SetCondition( null );
        }
    }
}
