using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameRules;
using GameBase.UI;
using Platform.DependInject;
using GameBase;
using Microsoft.Xna.Framework;
using Platform.GameObjects.Tank.TankAIs;
using Microsoft.Xna.Framework.Graphics;
using Platform;
using GameBase.Graphics;
using GameBase.Input;
using Platform.GameScreens;
using Platform.GameObjects.Tank.Tanks;
using Platform.GameDraw;
using GameBase.Graphics.BackGround.VergeTile;
using Platform.GameObjects.Tank.TankSkins;
using Platform.Scene;
using System.IO;
using GameBase.DataStructure;
using GameBase.Helpers;
using Platform.GameObjects.Item;
using Platform.GameObjects;
using Platform.Senses.Vision;
using Platform.GameDraw.UIElement;

namespace InterRules.FindPath
{
    [RuleAttribute("FindPath", "测试AI的寻路能力", "SmartTank编写组", 2007, 11, 20)]
    public class FindPathRule : IGameRule
    {
        #region Variables

        Combo aiList;

        TextButton btn;

        int selectIndex;

        AILoader aiLoader;

        #endregion

        #region IGameRule 成员

        public string RuleIntroduction
        {
            get { return "测试AI的寻路能力"; }
        }

        public string RuleName
        {
            get { return "FindPath"; }
        }

        #endregion

        public FindPathRule()
        {
            BaseGame.ShowMouse = true;

            aiList = new Combo("AIList", new Vector2(200, 200), 300);

            aiList.OnChangeSelection += new EventHandler(AIList_OnChangeSelection);

            aiLoader = new AILoader();
            aiLoader.AddInterAI(typeof(PathFinderSecond));
            aiLoader.AddInterAI(typeof(ManualControl));
            aiLoader.AddInterAI(typeof(PathFinderFirst));
            aiLoader.InitialCompatibleAIs(typeof(IAIOrderServerSinTur), typeof(AICommonServer));
            foreach (string name in aiLoader.GetAIList())
            {
                aiList.AddItem(name);
            }
            btn = new TextButton("OkBtn", new Vector2(700, 500), "Begin", 0, Color.Blue);
            btn.OnClick += new EventHandler(btn_OnClick);
        }

        void btn_OnClick(object sender, EventArgs e)
        {
            GameManager.AddGameScreen(new FindPathGameScreen(aiLoader.GetAIInstance(selectIndex)));
        }

        void AIList_OnChangeSelection(object sender, EventArgs e)
        {
            selectIndex = aiList.currentIndex;
        }

        #region IGameScreen 成员

        public void Render()
        {
            BaseGame.Device.Clear(Color.BurlyWood);
            aiList.Draw(Sprite.alphaSprite, 1f);
            btn.Draw(Sprite.alphaSprite, 1f);
        }

        public bool Update(float second)
        {
            aiList.Update();
            btn.Update();

            if (InputHandler.JustPressKey(Microsoft.Xna.Framework.Input.Keys.Escape))
                return true;
            return false;
        }

        #endregion
    }

    class FindPathGameScreen : IGameScreen
    {
        static readonly Rectanglef mapSize = new Rectanglef(0, 0, 1000, 1000);
        static readonly Rectangle scrnRect = new Rectangle(0, 0, BaseGame.ClientRect.Width, BaseGame.ClientRect.Height);

        static readonly Vector2 cameraStartPos = new Vector2(50, 50);

        const float tankRaderDepth = 100;
        const float tankRaderAng = MathHelper.PiOver4;
        const float tankMaxForwardSpd = 50;
        const float tankMaxBackwardSpd = 30;
        const float tankMaxRotaSpd = 0.3f * MathHelper.Pi;
        const float tankMaxRotaTurretSpd = 0.6f * MathHelper.Pi;
        const float tankMaxRotaRaderSpd = MathHelper.Pi;
        static readonly Vector2 tankStartPos = new Vector2(20, 20);
        const float tankStartAzi = 0;

        Camera camera;
        Compass compass;
        VergeTileGround backGround;

        SceneKeeperCommon sceneKeeper;


        AICommonServer commonServer;

        TankSinTur tank;

        ObstacleCommon wall1;
        ObstacleCommon wall2;
        ObstacleCommon wall3;

        ItemCommon item;

        public FindPathGameScreen(IAI tankAI)
        {
            Coordin.SetScreenViewRect(scrnRect);
            camera = new Camera(2, cameraStartPos, 0f);
            compass = new Compass(new Vector2(740, 540));
            camera.Enable();

            InitialBackGround();

            InitialScene();

            InitialAI(tankAI);

            camera.Focus(tank, true);
        }

        private void InitialAI(IAI tankAI)
        {
            commonServer = new AICommonServer(mapSize);
            tank.SetTankAI(tankAI);
            tankAI.OrderServer = tank;
            tankAI.CommonServer = commonServer;

            GameManager.ObjMemoryManager.AddSingle(tank);
        }

        private void InitialScene()
        {
            sceneKeeper = new SceneKeeperCommon();

            tank = new TankSinTur(new Platform.GameObjects.GameObjInfo("Tank", string.Empty),
                new TankSkinSinTur(TankSkinSinTurData.M1A2), tankRaderDepth, tankRaderAng, Color.Wheat,
                tankMaxForwardSpd, tankMaxBackwardSpd, tankMaxRotaSpd, tankMaxRotaTurretSpd, tankMaxRotaRaderSpd,
                0.3f, tankStartPos, tankStartAzi);

            wall1 = new ObstacleCommon(new Platform.GameObjects.GameObjInfo("wall", string.Empty), new Vector2(100, 50), 0,
                Path.Combine(Directories.ContentDirectory, "GameObjs\\wall"),
                new Vector2(90, 15), 1f, Color.White, LayerDepth.GroundObj, new Vector2[] { new Vector2(90, 15) });

            wall2 = new ObstacleCommon(new Platform.GameObjects.GameObjInfo("wall", string.Empty), new Vector2(250, 150), MathHelper.PiOver2,
                            Path.Combine(Directories.ContentDirectory, "GameObjs\\wall"),
                            new Vector2(90, 15), 1f, Color.White, LayerDepth.GroundObj, new Vector2[] { new Vector2(90, 15) });

            wall3 = new ObstacleCommon(new Platform.GameObjects.GameObjInfo("wall", string.Empty), new Vector2(100, 100), 0,
                Path.Combine(Directories.ContentDirectory, "GameObjs\\wall"),
                new Vector2(90, 15), 1f, Color.White, LayerDepth.GroundObj, new Vector2[] { new Vector2(90, 15) });


            item = new ItemCommon("Item", string.Empty,
                Path.Combine(Directories.ContentDirectory, "GameObjs\\scorpion"), new Vector2(128, 128), 0.3f, new Vector2[] { new Vector2(128, 128) },
                new Vector2(300, 250), 0, new Vector2(5, 5), 0);


            sceneKeeper.AddGameObj(tank, true, false, true, SceneKeeperCommon.GameObjLayer.HighBulge);

            sceneKeeper.AddGameObj(wall1, false, true, false, SceneKeeperCommon.GameObjLayer.HighBulge, EyeableInfo.GetEyeableInfoHandler);
            sceneKeeper.AddGameObj(wall2, false, true, false, SceneKeeperCommon.GameObjLayer.HighBulge, EyeableInfo.GetEyeableInfoHandler);
            sceneKeeper.AddGameObj(wall3, false, true, false, SceneKeeperCommon.GameObjLayer.HighBulge, EyeableInfo.GetEyeableInfoHandler);

            sceneKeeper.AddGameObj(item, true, false, false, SceneKeeperCommon.GameObjLayer.HighBulge, EyeableInfo.GetEyeableInfoHandler);
            sceneKeeper.SetBorder(mapSize);
            GameManager.LoadScene(sceneKeeper);
        }

        private void InitialBackGround()
        {
            VergeTileData data = new VergeTileData();
            data.gridWidth = 50;
            data.gridHeight = 50;
            data.SetRondomVertexIndex(50, 50, 4);
            data.SetRondomGridIndex(50, 50);
            data.texPaths = new string[]
            {
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_Dirt.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_DirtRough.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_DirtGrass.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_GrassDark.tga"),
            };
            backGround = new VergeTileGround(data, scrnRect, mapSize);
        }


        #region IGameScreen 成员

        public void Render()
        {
            BaseGame.Device.Clear(Color.Blue);

            backGround.Draw();

            //BasicGraphics.DrawRectangle( wall.BoundingBox, 3f, Color.Red, 0f );

            //BasicGraphics.DrawRectangle( mapSize, 3f, Color.Red, 0f );

            BasicGraphics.DrawPoint(Vector2.Transform(item.KeyPoints[0], item.TransMatrix), 1f, Color.Black, 0f);
            BasicGraphics.DrawPoint(Vector2.Transform(wall1.KeyPoints[0], wall1.TransMatrix), 1f, Color.Black, 0f);
            BasicGraphics.DrawPoint(Vector2.Transform(wall2.KeyPoints[0], wall2.TransMatrix), 1f, Color.Black, 0f);
            BasicGraphics.DrawPoint(Vector2.Transform(wall3.KeyPoints[0], wall3.TransMatrix), 1f, Color.Black, 0f);


            GameManager.DrawManager.Draw();
            compass.Draw();

        }

        public bool Update(float second)
        {
            camera.Update(second);
            compass.Update();

            GameManager.UpdataComponent(second);

            if (InputHandler.JustPressKey(Microsoft.Xna.Framework.Input.Keys.Escape))
                return true;
            return false;
        }

        #endregion
    }
}
