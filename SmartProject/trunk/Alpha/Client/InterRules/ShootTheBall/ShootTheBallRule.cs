using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Scene;
using TankEngine2D.Graphics;
using SmartTank.AI;
using SmartTank.GameObjs.Tank;
using SmartTank.GameObjs.Shell;
using SmartTank.GameObjs.Item;
using System.IO;
using TankEngine2D.Helpers;
using SmartTank.GameObjs;
using SmartTank.Draw;
using TankEngine2D.Input;
using TankEngine2D.DataStructure;
using SmartTank.Draw.BackGround.VergeTile;
using SmartTank.Shelter;
using SmartTank.Screens;
using SmartTank.Senses.Vision;
using SmartTank.Rule;
using SmartTank.Effects.SceneEffects;
using Microsoft.Xna.Framework;
using SmartTank.GameObjs.Tank.SinTur;
using SmartTank;
using SmartTank.Helpers;
using Microsoft.Xna.Framework.Graphics;
using SmartTank.Effects.TextEffects;

namespace InterRules.ShootTheBall
{
    /*
     * 一个Demo，便于测试各种效果。
     * 
     * 已添加的效果：
     *      
     *      炮塔后座。
     * 
     *      物体间相互作用。
     * 
     *      地形表面。
     * 
     * 有待完成的效果：
     * 
     *      炮口火焰。
     * 
     *      坦克换肤。
     * 
     *      履带印痕。
     * 
     *      界面制作。
     * 
     * */
    [RuleAttribute( "ShootTheBall", "坦克炮轰一个运动的球。（平台的第一个规则，帮助我们进行了大量的测试!）", "SmartTank编写组", 2007, 10, 30 )]
    class ShootTheBallRule : IGameRule
    {
        readonly float shellSpeed = 150;

        public string RuleName
        {
            get { return "ShootTheBall"; }
        }

        public string RuleIntroduction
        {
            get { return "坦克炮轰一个运动的球。（平台的第一个规则，帮助我们进行了大量的测试!）"; }
        }

        #region Variables

        readonly Rectangle scrnViewRect = new Rectangle( 30, 30, 750, 550 );

        readonly Rectanglef mapSize = new Rectanglef( 0, 0, 200, 150 );

        //SceneKeeperCommon scene;
        SceneMgr sceneMgr;

        TankSinTur tank;
        //TankSinTur tankPlayer;

        Camera camera;

        //SimpleTileGround simpleGround;

        VergeTileGround vergeGround;

        int Score = 0;
        int shootSum = 0;
        int hitSum = 0;

        bool firstHit = true;
        bool showFirstHit = false;

        bool firstScore = true;
        bool showFirstScore = false;

        bool firstHitTank = true;
        bool showFirstHitTank = false;

        bool showNiceShoot = true;
        int NiceShootSum = 0;

        #endregion

        #region Construction

        public ShootTheBallRule()
        {
            //BaseGame.ShowMouse = true;

            BaseGame.CoordinMgr.SetScreenViewRect( scrnViewRect );

            camera = new Camera( 4, new Vector2( 100, 75 ), 0 );
            camera.maxScale = 4;
            camera.Enable();

            InintialBackGround();

            //scene = new SceneKeeperCommon();
            sceneMgr = new SceneMgr();
            SceneInitial();
            GameManager.LoadScene( sceneMgr );

            LoadResource();
        }

        private void LoadResource()
        {
            BaseGame.FontMgr.BuildTexture( "哦，这个家伙变小了！", GameFonts.HDZB );
            BaseGame.FontMgr.BuildTexture( "当这个物体消失时，你会获得100分！", GameFonts.HDZB );
            BaseGame.FontMgr.BuildTexture( "不过这个物体也会开始运动。", GameFonts.HDZB );
        }

        private void InintialBackGround()
        {
            VergeTileData data2 = new VergeTileData();
            data2.gridWidth = 20;
            data2.gridHeight = 15;
            //data2.vertexTexIndexs = new int[]
            //{
            //    0,0,0,1,1,1,1,2,2,2,3,
            //    0,0,1,2,1,1,1,2,2,3,3,
            //    0,1,3,3,3,3,1,2,2,3,3,
            //    0,1,3,3,3,1,2,2,3,3,3,
            //    0,1,1,2,3,1,2,2,3,3,3,
            //    1,1,1,2,2,2,2,2,2,3,2,
            //    1,1,1,1,2,2,2,3,3,2,2,
            //    1,1,2,2,2,1,2,2,3,3,2,
            //    1,1,1,2,2,2,2,3,3,2,2,
            //    1,1,2,1,2,3,2,3,3,2,2,
            //    1,1,2,2,2,2,2,3,2,2,2,

            //};
            data2.SetRondomVertexIndex( 20, 15, 4 );
            data2.SetRondomGridIndex( 20, 15 );
            data2.texPaths = new string[]
            {
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_Dirt.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_DirtRough.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_DirtGrass.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_GrassDark.tga"),

                //"Cave_Dirt.tga",
                //"Cave_LavaCracks.tga",
                //"Cave_Brick.tga",
                //"Cave_RedStones.tga",
                //"Cave_SquareTiles.tga",
                //"Cave_Lava.tga",
                //"Cave_GreyStones.tga",

                //"Lordw_Dirt.tga",
                //"Lordw_DirtRough.tga",
                //"Lordw_Grass.tga",
                //"Lordw_SnowGrass.tga",
                //"Lordw_Rock.tga",
                //"Lordw_Snow.tga",
                
                //"North_dirt.tga",
                //"North_dirtdark.tga",
                //"North_Grass.tga",
                //"North_ice.tga",
                //"North_rock.tga",
                //"North_Snow.tga",
                //"North_SnowRock.tga",

                //"Village_Dirt.tga",
                //"Village_DirtRough.tga",
                //"Village_GrassShort.tga",
                //"Village_Crops.tga",

            };

            vergeGround = new VergeTileGround( data2, scrnViewRect, mapSize );
        }

        private void SceneInitial()
        {
            tank = new TankSinTur( "tank", new GameObjInfo( "Tank", "Player" ), TankSinTur.M60TexPath, TankSinTur.M60Data,
                170, MathHelper.PiOver4, Color.Yellow,
                0f, 80, 60, 0.6f * MathHelper.Pi, 0.5f * MathHelper.Pi, MathHelper.Pi, 2f,
                new Vector2( 100, 50 ), 0, 0 );

            AutoShootAI autoShoot = new AutoShootAI();
            autoShoot.OrderServer = tank;
            ManualControl manualControl = new ManualControl();
            manualControl.OrderServer = tank;
            //tank.SetTankAI( manualControl );
            tank.SetTankAI( autoShoot );
            tank.onShoot += new Tank.ShootEventHandler( Tank_onShoot );
            tank.ShellSpeed = shellSpeed;

            //ItemCommon item = new ItemCommon( "item", "Scorpion", Path.Combine( Directories.ContentDirectory, "GameObjs\\scorpion" ), new Vector2( 128, 128 ), 0.031f, new Vector2[] { new Vector2( 128, 128 ) },
            //    new Vector2( 150, 50 ), 0f, Vector2.Zero, 0 );
            Ball ball = new Ball( "ball", 0.031f, new Vector2( 150, 50 ), 0, Vector2.Zero, 0 );
            ball.OnCollided += new OnCollidedEventHandler( item_OnCollided );

            smoke = new SmokeGenerater( 0, 50, Vector2.Zero, 0.3f, 0f, true, ball );

            sceneMgr.AddGroup( string.Empty, new TypeGroup<TankSinTur>( "tank" ) );
            sceneMgr.AddGroup( string.Empty, new TypeGroup<Ball>( "ball" ) );
            sceneMgr.AddGroup( string.Empty, new TypeGroup<SmartTank.PhiCol.Border>( "border" ) );
            sceneMgr.AddGroup( string.Empty, new TypeGroup<ShellNormal>( "shell" ) );

            sceneMgr.PhiGroups.Add( "tank" );
            sceneMgr.PhiGroups.Add( "ball" );
            sceneMgr.PhiGroups.Add( "shell" );

            sceneMgr.ColPairGroups.Add( new SceneMgr.Pair( "tank", "ball" ) );
            sceneMgr.ColPairGroups.Add( new SceneMgr.Pair( "tank", "border" ) );
            sceneMgr.ColPairGroups.Add( new SceneMgr.Pair( "ball", "border" ) );
            sceneMgr.ColPairGroups.Add( new SceneMgr.Pair( "shell", "border" ) );
            sceneMgr.ColPairGroups.Add( new SceneMgr.Pair( "shell", "ball" ) );

            sceneMgr.ShelterGroups.Add( new SceneMgr.MulPair( "tank", new List<string>() ) );
            sceneMgr.VisionGroups.Add( new SceneMgr.MulPair( "tank", new List<string>( new string[] { "ball" } ) ) );

            sceneMgr.AddGameObj( "tank", tank );
            sceneMgr.AddGameObj( "ball", ball );
            sceneMgr.AddGameObj( "border", new SmartTank.PhiCol.Border( mapSize ) );


            //camera.Focus( tank );
        }

        #endregion

        #region IGameScreen 成员

        public bool Update( float seconds )
        {
            GameManager.UpdataComponent( seconds );

            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Left ))
                camera.Move( new Vector2( -5, 0 ) );
            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Right ))
                camera.Move( new Vector2( 5, 0 ) );
            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Up ))
                camera.Move( new Vector2( 0, -5 ) );
            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Down ))
                camera.Move( new Vector2( 0, 5 ) );
            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.V ))
                camera.Zoom( -0.2f );
            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.B ))
                camera.Zoom( 0.2f );

            camera.Update( seconds );

            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Escape ))
            {
                GameManager.ComponentReset();
                return true;
            }
            else
                return false;
        }

        bool firstDraw = true;
        public void Render()
        {
            //simpleGround.Draw( camera.LogicViewRect );
            vergeGround.Draw();

            GameManager.DrawManager.Draw();

            BaseGame.BasicGraphics.DrawRectangle( mapSize, 3, Color.Red, 0f );
            BaseGame.BasicGraphics.DrawRectangleInScrn( scrnViewRect, 3, Color.Green, 0f );

            BaseGame.BasicGraphics.DrawPoint( BaseGame.CoordinMgr.LogicPos( ConvertHelper.PointToVector2( InputHandler.CurMousePos ) ), 3f, Color.Red, LayerDepth.Mouse );

            if (firstDraw)
            {
                //TextEffectMgr.AddRiseFade( "Use 'AWSD' to Move the Tank, 'JK' to Rotate the Turret,", new Vector2( 40, 150 ), 1.2f, Color.Red, LayerDepth.Text, FontType.Comic, 500, 0.2f );
                //TextEffectMgr.AddRiseFade( "and press Space To Shoot!", new Vector2( 40, 160 ), 1.2f, Color.Red, LayerDepth.Text, FontType.Comic, 500, 0.2f );

                firstDraw = false;
            }

            if (showFirstHit)
            {
                TextEffectMgr.AddRiseFade( "哦，这个家伙变小了！", new Vector2( 50, 70 ), 1.4f, Color.Red, LayerDepth.Text, GameFonts.HDZB, 300, 0.2f );
                showFirstHit = false;
            }

            if (showFirstScore)
            {
                TextEffectMgr.AddRiseFade( "当这个物体消失时，你会获得100分！", new Vector2( 20, 70 ), 1.4f, Color.Red, LayerDepth.Text, GameFonts.HDZB, 300, 0.2f );
                TextEffectMgr.AddRiseFade( "不过这个物体也会开始运动。", new Vector2( 20, 80 ), 1.4f, Color.Red, LayerDepth.Text, GameFonts.HDZB, 300, 0.2f );

                showFirstScore = false;
            }

            if (showFirstHitTank)
            {
                //TextEffect.AddRiseFade( "Oh, the Object hit you, it grows bigger!", new Vector2( 30, 150 ), 1.1f, Color.Red, LayerDepth.Text, FontType.Comic, 300, 0.2f );
                showFirstHitTank = false;
            }

            if (showNiceShoot)
            {
                if (NiceShootSum >= 30)
                    TextEffectMgr.AddRiseFade( "GOD LIKE!", tank.Pos, 1.1f, Color.Black, LayerDepth.Text, GameFonts.Comic, 200, 1f );
                else if (NiceShootSum >= 17)
                    TextEffectMgr.AddRiseFade( "Master Shoot!", tank.Pos, 1.1f, Color.Pink, LayerDepth.Text, GameFonts.Comic, 200, 1f );
                else if (NiceShootSum >= 4)
                    TextEffectMgr.AddRiseFade( "Nice Shoot!", tank.Pos, 1.1f, Color.GreenYellow, LayerDepth.Text, GameFonts.Comic, 200, 1f );

                showNiceShoot = false;
            }

            if (tank.Rader.PointInRader( InputHandler.GetCurMousePosInLogic( BaseGame.RenderEngine ) ))
            {
                BaseGame.FontMgr.DrawInScrnCoord( "mouse In Rader!", ConvertHelper.PointToVector2( InputHandler.CurMousePos ), 0.6f, Color.Yellow, LayerDepth.Text, GameFonts.Lucida );
            }

            if (tank.Rader.ShelterObjs.Length != 0)
            {
                foreach (IShelterObj obj in tank.Rader.ShelterObjs)
                {
                    BaseGame.FontMgr.Draw( "I am Sheltered by " + (obj as IGameObj).Name + " Pos: " + ((IGameObj)obj).Pos.ToString(), tank.Pos, 0.6f, Color.Yellow, LayerDepth.Text, GameFonts.Lucida );
                }
            }


            BaseGame.FontMgr.DrawInScrnCoord( "You Score : " + Score.ToString(), new Vector2( 500, 40 ), 0.6f, Color.Yellow, LayerDepth.Text, GameFonts.Comic );
            BaseGame.FontMgr.DrawInScrnCoord( "Shoot Sum : " + shootSum.ToString(), new Vector2( 500, 60 ), 0.6f, Color.Yellow, LayerDepth.Text, GameFonts.Comic );
            BaseGame.FontMgr.DrawInScrnCoord( "hit Sum   : " + hitSum.ToString(), new Vector2( 500, 80 ), 0.6f, Color.Yellow, LayerDepth.Text, GameFonts.Comic );
            float hitRate = shootSum != 0 ? (float)hitSum * 100 / (float)shootSum : 0;
            BaseGame.FontMgr.DrawInScrnCoord( "hit rate  : " + hitRate.ToString() + "%", new Vector2( 500, 100 ), 0.6f, Color.Yellow, LayerDepth.Text, GameFonts.Comic );


        }

        #endregion

        #region Rules

        SmokeGenerater smoke;

        bool speedy = false;



        //float lastCollideWithBorderTime = -1;
        void item_OnCollided( IGameObj Sender, CollisionResult result, GameObjInfo objB )
        {

            if (objB.ObjClass == "Border")
            {
                ((ItemCommon)Sender).Vel = -2 * Vector2.Dot( ((ItemCommon)Sender).Vel, result.NormalVector ) * result.NormalVector + ((ItemCommon)Sender).Vel;

                //float curTime = GameManager.CurTime;
                //if (lastCollideWithBorderTime != -1 && curTime - lastCollideWithBorderTime < 0.05f)
                //    ((ItemCommon)Sender).Scale -= 0.1f * 0.25f;

                //lastCollideWithBorderTime = curTime;
            }
            else if (objB.ObjClass == "Tank")
            {
                //((ItemCommon)Sender).Scale += 0.1f * 0.25f;
                //((ItemCommon)Sender).Pos += result.NormalVector * 10f;
                ((ItemCommon)Sender).Vel = ((ItemCommon)Sender).Vel.Length() * result.NormalVector;


                if (firstHitTank)
                {
                    showFirstHitTank = true;
                    firstHitTank = false;
                }
            }
            else if (objB.ObjClass == "ShellNormal")
            {
                smoke.Concen += 0.3f;

                if (((ItemCommon)Sender).Scale < 0.5f * 0.031f)
                {
                    //scene.RemoveGameObj( Sender, true, false, false, false, SceneKeeperCommon.GameObjLayer.HighBulge );
                    sceneMgr.DelGameObj( "shell", Sender.Name );
                    Score += 100;

                    hitSum++;

                    AddNewItem( Sender );

                    if (firstScore)
                    {
                        showFirstScore = true;
                        firstScore = false;
                    }

                    smoke.Concen = 0;
                }
                else
                {
                    ((ItemCommon)Sender).Scale -= 0.15f * 0.031f;
                    ((ItemCommon)Sender).Vel = -result.NormalVector * ((ItemCommon)Sender).Vel.Length();
                    hitSum++;
                }

                if (firstHit)
                {
                    showFirstHit = true;
                    firstHit = false;
                }

                NiceShootSum += 2;
                showNiceShoot = true;

                if (NiceShootSum >= 30 && !speedy)
                {
                    TextEffectMgr.AddRiseFade( "You Got A Speedy Turret!", tank.Pos, 2f, Color.Purple, LayerDepth.Text, GameFonts.Lucida, 300, 0.2f );
                    tank.FireCDTime = 2f;
                    speedy = true;
                }
            }

        }

        private void AddNewItem( IGameObj Sender )
        {
            Vector2 pos;
            do
            {
                pos = RandomHelper.GetRandomVector2( 10, 140 );
            }
            while (Vector2.Distance( pos, tank.Pos ) < 30);

            Vector2 vel = RandomHelper.GetRandomVector2( Score / 10, Score / 5 );
            ((ItemCommon)Sender).Pos = pos;
            ((ItemCommon)Sender).Vel = vel;
            ((ItemCommon)Sender).Azi = RandomHelper.GetRandomFloat( 0, 10 );
            ((ItemCommon)Sender).Scale = 0.031f;
            //scene.AddGameObj( Sender, true, false, false, SceneKeeperCommon.GameObjLayer.HighBulge );
            sceneMgr.AddGameObj( "ball", Sender );
        }

        int shellCount = 0;

        void Tank_onShoot( Tank sender, Vector2 turretEnd, float azi )
        {
            ShellNormal newShell = new ShellNormal( "shell" + shellCount.ToString(), sender, turretEnd, azi, shellSpeed );
            newShell.onCollided += new OnCollidedEventHandler( Shell_onCollided );
            newShell.onOverlap += new OnCollidedEventHandler( Shell_onOverlap );
            //scene.AddGameObj( newShell, true, false, false, SceneKeeperCommon.GameObjLayer.lowFlying );
            sceneMgr.AddGameObj( "shell", newShell );
            shootSum++;
            //camera.Focus( newShell );
        }

        void Shell_onOverlap( IGameObj Sender, CollisionResult result, GameObjInfo objB )
        {
            if (objB.ObjClass == "Border")
                NiceShootSum = Math.Max( 0, NiceShootSum - 10 );
            //camera.Focus( tank );
        }

        void Shell_onCollided( IGameObj Sender, CollisionResult result, GameObjInfo objB )
        {
            //scene.RemoveGameObj( Sender, true, false, false, false, SceneKeeperCommon.GameObjLayer.lowFlying );
            sceneMgr.DelGameObj( "shell", Sender.Name );

            //camera.Focus( tank );
            //if (objB.Name == "Border")
            //{
            //    NiceShootSum = Math.Max( 0, NiceShootSum - 1 );
            //}
        }



        #endregion


    }

    class ItemEyeableInfo : IEyeableInfo
    {
        EyeableInfo eyeableInfo;
        Vector2 vel;

        public Vector2 Pos
        {
            get { return eyeableInfo.Pos; }
        }

        public Vector2 Vel
        {
            get { return vel; }
        }

        public ItemEyeableInfo( ItemCommon item )
        {
            this.eyeableInfo = new EyeableInfo( item );
            this.vel = item.Vel;
        }

        #region IEyeableInfo 成员

        public GameObjInfo ObjInfo
        {
            get { return eyeableInfo.ObjInfo; }
        }

        public Vector2[] CurKeyPoints
        {
            get { return eyeableInfo.CurKeyPoints; }
        }

        public Matrix CurTransMatrix
        {
            get { return eyeableInfo.CurTransMatrix; }
        }

        #endregion
    }

    class Ball : ItemCommon
    {
        public Ball( string name, float scale, Vector2 pos, float azi, Vector2 vel, float rotaVel )
            : base( name, "item" , "Scorpion" , Path.Combine( Directories.GameObjsDirectory, "Internal\\Ball" ) ,
            GameObjData.Load( File.OpenRead( Path.Combine( Directories.GameObjsDirectory, "Internal\\Ball\\Ball.xml" ) ) ) ,
            scale , pos , azi , vel , rotaVel )
        {
            this.GetEyeableInfoHandler = new GetEyeableInfoHandler( GetItemInfo );
        }

        IEyeableInfo GetItemInfo( IRaderOwner raderOwner, IEyeableObj item )
        {
            return (IEyeableInfo)(new ItemEyeableInfo( (ItemCommon)item ));
        }
    }
}
