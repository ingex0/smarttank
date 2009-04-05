using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using InterRules.ShootTheBall;
using SmartTank.Rule;
using SmartTank.Screens;
using SmartTank.Draw.UI.Controls;
using SmartTank.Helpers.DependInject;
using TankEngine2D.DataStructure;
using SmartTank.Draw;
using SmartTank.Draw.BackGround.VergeTile;
using SmartTank.Scene;
using SmartTank.AI;
using SmartTank.GameObjs.Tank.SinTur;
using SmartTank.GameObjs.Item;
using SmartTank.Draw.UI;
using SmartTank;
using TankEngine2D.Input;
using SmartTank.Update;
using SmartTank.Effects.TextEffects;
using SmartTank.GameObjs;
using SmartTank.Helpers;
using SmartTank.Senses.Vision;
using TankEngine2D.Graphics;

namespace InterRules.FindPath
{
    [RuleAttribute( "FindPath", "测试AI的寻路能力", "SmartTank编写组", 2007, 11, 20 )]
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

            aiList = new Combo( "AIList", new Vector2( 200, 200 ), 300 );

            aiList.OnChangeSelection += new EventHandler( AIList_OnChangeSelection );

            aiLoader = new AILoader();
            aiLoader.AddInterAI( typeof( PathFinderThird ) );
            aiLoader.AddInterAI( typeof( PathFinderSecond ) );
            aiLoader.AddInterAI( typeof( ManualControl ) );
            aiLoader.AddInterAI( typeof( PathFinderFirst ) );
            aiLoader.InitialCompatibleAIs( typeof( IAIOrderServerSinTur ), typeof( AICommonServer ) );
            foreach (string name in aiLoader.GetAIList())
            {
                aiList.AddItem( name );
            }
            btn = new TextButton( "OkBtn", new Vector2( 700, 500 ), "Begin", 0, Color.Blue );
            btn.OnClick += new EventHandler( btn_OnClick );
        }

        void btn_OnClick( object sender, EventArgs e )
        {
            GameManager.AddGameScreen( new FindPathGameScreen( aiLoader.GetAIInstance( selectIndex ) ) );
        }

        void AIList_OnChangeSelection( object sender, EventArgs e )
        {
            selectIndex = aiList.currentIndex;
        }

        #region IGameScreen 成员

        public void Render()
        {
            BaseGame.Device.Clear( Color.BurlyWood );
            aiList.Draw( BaseGame.SpriteMgr.alphaSprite, 1f );
            btn.Draw( BaseGame.SpriteMgr.alphaSprite, 1f );
        }

        public bool Update( float second )
        {
            aiList.Update();
            btn.Update();

            if (InputHandler.JustPressKey( Microsoft.Xna.Framework.Input.Keys.Escape ))
                return true;
            return false;
        }

        #endregion

        #region IGameScreen 成员

        public void OnClose()
        {
            
        }

        #endregion
    }

    class FindPathGameScreen : IGameScreen
    {
        static readonly Rectanglef mapSize = new Rectanglef( 0, 0, 1000, 1000 );
        static readonly Rectangle scrnRect = new Rectangle( 0, 0, BaseGame.ClientRect.Width, BaseGame.ClientRect.Height );

        static readonly Vector2 cameraStartPos = new Vector2( 50, 50 );

        const float tankRaderDepth = 100;
        const float tankRaderAng = MathHelper.PiOver4;
        const float tankMaxForwardSpd = 50;
        const float tankMaxBackwardSpd = 30;
        const float tankMaxRotaSpd = 0.3f * MathHelper.Pi;
        const float tankMaxRotaTurretSpd = 0.6f * MathHelper.Pi;
        const float tankMaxRotaRaderSpd = MathHelper.Pi;
        static readonly Vector2 tankStartPos = new Vector2( 20, 20 );
        const float tankStartAzi = 0;

        Camera camera;
        Compass compass;
        VergeTileGround backGround;

        SceneMgr sceneMgr;


        AICommonServer commonServer;

        TankSinTur tank;

        ObstacleCommon wall1;
        ObstacleCommon wall2;
        ObstacleCommon wall3;

        Ball ball;

        public FindPathGameScreen( IAI tankAI )
        {
            BaseGame.CoordinMgr.SetScreenViewRect( scrnRect );
            camera = new Camera( 2, cameraStartPos, 0f );
            compass = new Compass( new Vector2( 740, 540 ) );
            camera.Enable();

            InitialBackGround();

            InitialScene();

            InitialAI( tankAI );

            camera.Focus( tank, true );

            GameTimer timer = new GameTimer( 5,
                delegate()
                {
                    TextEffectMgr.AddRiseFadeInScrnCoordin( "test FadeUp in Scrn!", new Vector2( 100, 100 ), 1f, Color.Black, LayerDepth.Text, GameFonts.Lucida, 300, 0.5f );
                    TextEffectMgr.AddRiseFade( "test FadeUp in Login!", new Vector2( 100, 100 ), 1f, Color.White, LayerDepth.Text, GameFonts.Lucida, 300, 0.5f );
                } );
        }

        private void InitialAI( IAI tankAI )
        {
            commonServer = new AICommonServer( mapSize );
            tank.SetTankAI( tankAI );
            tankAI.OrderServer = tank;
            tankAI.CommonServer = commonServer;

            GameManager.ObjMemoryMgr.AddSingle( tank );
        }

        private void InitialScene()
        {
            sceneMgr = new SceneMgr();

            tank = new TankSinTur( "tank", new GameObjInfo( "Tank", string.Empty ),
                TankSinTur.M60TexPath, TankSinTur.M60Data,
                tankRaderDepth, tankRaderAng, Color.Wheat,
                tankMaxForwardSpd, tankMaxBackwardSpd, tankMaxRotaSpd, tankMaxRotaTurretSpd, tankMaxRotaRaderSpd,
                0.3f, tankStartPos, tankStartAzi );

            wall1 = new ObstacleCommon( "wall1", new GameObjInfo( "wall", string.Empty ), new Vector2( 100, 50 ), 0,
                Path.Combine( Directories.ContentDirectory, "GameObjs\\wall" ),
                new Vector2( 90, 15 ), 1f, Color.White, LayerDepth.GroundObj, new Vector2[] { new Vector2( 90, 15 ) } );
            wall1.GetEyeableInfoHandler = EyeableInfo.GetEyeableInfoHandler;

            wall2 = new ObstacleCommon( "wall2", new GameObjInfo( "wall", string.Empty ), new Vector2( 250, 150 ), MathHelper.PiOver2,
                            Path.Combine( Directories.ContentDirectory, "GameObjs\\wall" ),
                            new Vector2( 90, 15 ), 1f, Color.White, LayerDepth.GroundObj, new Vector2[] { new Vector2( 90, 15 ) } );
            wall2.GetEyeableInfoHandler = EyeableInfo.GetEyeableInfoHandler;

            wall3 = new ObstacleCommon( "wall3", new GameObjInfo( "wall", string.Empty ), new Vector2( 100, 100 ), 0,
                Path.Combine( Directories.ContentDirectory, "GameObjs\\wall" ),
                new Vector2( 90, 15 ), 1f, Color.White, LayerDepth.GroundObj, new Vector2[] { new Vector2( 90, 15 ) } );
            wall3.GetEyeableInfoHandler = EyeableInfo.GetEyeableInfoHandler;


            ball = new Ball( "ball", 0.3f, new Vector2( 300, 250 ), 0, new Vector2( 5, 5 ), 0 );

            sceneMgr.AddGroup( "", new TypeGroup<TankSinTur>( "tank" ) );
            sceneMgr.AddGroup( "", new Group( "obstacle" ) );
            sceneMgr.AddGroup( "obstacle", new TypeGroup<ObstacleCommon>( "wall" ) );
            sceneMgr.AddGroup( "obstacle", new TypeGroup<Ball>( "ball" ) );
            sceneMgr.AddGroup( "obstacle", new TypeGroup<SmartTank.PhiCol.Border>( "border" ) );
            sceneMgr.AddColMulGroups( "tank", "obstacle" );
            sceneMgr.PhiGroups.AddRange( new string[] { "tank", "obstacle\\ball" } );
            sceneMgr.AddShelterMulGroups( "tank", "obstacle\\wall" );
            sceneMgr.VisionGroups.Add( new SceneMgr.MulPair( "tank", new List<string>( new string[] { "obstacle\\ball", "obstacle\\wall" } ) ) );

            sceneMgr.AddGameObj( "tank", tank );
            sceneMgr.AddGameObj( "obstacle\\wall", wall1, wall2, wall3 );
            sceneMgr.AddGameObj( "obstacle\\ball", ball );
            sceneMgr.AddGameObj( "obstacle\\border", new SmartTank.PhiCol.Border( mapSize ) );
            
            GameManager.LoadScene( sceneMgr );
        }

        private void InitialBackGround()
        {
            VergeTileData data = new VergeTileData();
            data.gridWidth = 50;
            data.gridHeight = 50;
            data.SetRondomVertexIndex( 50, 50, 4 );
            data.SetRondomGridIndex( 50, 50 );
            data.texPaths = new string[]
            {
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_Dirt.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_DirtRough.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_DirtGrass.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_GrassDark.tga"),
            };
            backGround = new VergeTileGround( data, scrnRect, mapSize );
        }


        #region IGameScreen 成员

        public void Render()
        {
            BaseGame.Device.Clear( Color.Blue );

            backGround.Draw();

            //BasicGraphics.DrawRectangle( wall.BoundingBox, 3f, Color.Red, 0f );

            //BasicGraphics.DrawRectangle( mapSize, 3f, Color.Red, 0f );
            foreach (Vector2 keyPoint in ball.KeyPoints)
            {
                BaseGame.BasicGraphics.DrawPoint( Vector2.Transform( keyPoint, ball.TransMatrix ), 1f, Color.Black, 0f );
            }
            BaseGame.BasicGraphics.DrawPoint( Vector2.Transform( wall1.KeyPoints[0], wall1.TransMatrix ), 1f, Color.Black, 0f );
            BaseGame.BasicGraphics.DrawPoint( Vector2.Transform( wall2.KeyPoints[0], wall2.TransMatrix ), 1f, Color.Black, 0f );
            BaseGame.BasicGraphics.DrawPoint( Vector2.Transform( wall3.KeyPoints[0], wall3.TransMatrix ), 1f, Color.Black, 0f );


            GameManager.DrawManager.Draw();
            compass.Draw();

        }

        public bool Update( float second )
        {
            camera.Update( second );
            compass.Update();

            GameManager.UpdataComponent( second );

            if (InputHandler.JustPressKey( Microsoft.Xna.Framework.Input.Keys.Escape ))
                return true;
            return false;
        }

        #endregion

        #region IGameScreen 成员

        public void OnClose()
        {
        }

        #endregion
    }
}
