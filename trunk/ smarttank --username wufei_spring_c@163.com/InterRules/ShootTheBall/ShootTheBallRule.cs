using System;
using System.Collections.Generic;
using System.Text;
using Platform.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Graphics;
using Platform.GameObjects.Tank.Tanks;
using Platform.GameObjects.Tank.TankAIs;
using Platform.GameObjects.Tank;
using Platform.GameObjects.Shell;
using Platform.GameObjects.Item;
using System.IO;
using GameBase.Helpers;
using Platform.GameObjects;
using Platform.GameDraw;
using GameBase.Input;
using GameBase;
using GameBase.DataStructure;
using GameBase.Graphics.BackGround.VergeTile;
using Platform.Shelter;
using Platform.GameObjects.Tank.TankSkins;
using Platform.GameScreens;
using Platform.Senses.Vision;
using Platform.GameObjects.Tank.TankAIs.testAIs;
using Platform.GameObjects.Tank.TankControls;
using Platform.GameRules;
using Platform;
using Platform.GameDraw.SceneEffects;

namespace InterRules.ShootTheBall
{
    /*
     * 只是一个Demo，便于测试各种效果。
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

        SceneKeeperCommon scene;

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

        public ShootTheBallRule ()
        {
            //BaseGame.ShowMouse = true;

            Coordin.SetScreenViewRect( scrnViewRect );

            camera = new Camera( 4, new Vector2( 100, 75 ), 0 );
            camera.maxScale = 4;
            camera.Enable();

            InintialBackGround();

            scene = new SceneKeeperCommon();
            SceneInitial();
            GameManager.LoadScene( scene );

            LoadResource();
        }

        private void LoadResource ()
        {
            ChineseWriter.BuildTexture( "哦，这个家伙变小了！", FontType.HanDinJianShu );
            ChineseWriter.BuildTexture( "当这个物体消失时，你会获得100分！", FontType.HanDinJianShu );
            ChineseWriter.BuildTexture( "不过这个物体也会开始运动。", FontType.HanDinJianShu );
        }

        private void InintialBackGround ()
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

        private void SceneInitial ()
        {
            tank = new TankSinTur( new GameObjInfo( "Tank", "Player" ), TankSinTur.M60TexPath, TankSinTur.M60Data,
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
            Ball ball = new Ball( 0.031f, new Vector2( 150, 50 ), 0, Vector2.Zero, 0 );
            ball.OnCollided += new Platform.PhisicalCollision.OnCollidedEventHandler( item_OnCollided );

            smoke = new SmokeGenerater( 0, 50, Vector2.Zero, 0.3f, 0f, true, ball );

            scene.AddGameObj( tank, true, false, true, SceneKeeperCommon.GameObjLayer.HighBulge );
            scene.AddGameObj( ball, true, false, false, SceneKeeperCommon.GameObjLayer.HighBulge, new GetEyeableInfoHandler( GetItemInfo ) );
            scene.SetBorder( mapSize );

            //camera.Focus( tank );
        }

        #endregion

        #region IGameScreen 成员

        public bool Update ( float seconds )
        {
            //GameManager.UpdateManager.Update( seconds );
            //GameManager.PhiColManager.Update( seconds );
            //GameManager.ShelterManager.Update();
            //GameManager.VisionManager.Update();
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
        public void Render ()
        {
            //simpleGround.Draw( camera.LogicViewRect );
            vergeGround.Draw();

            GameManager.DrawManager.Draw();

            BasicGraphics.DrawRectangle( mapSize, 3, Color.Red, 0f );
            BasicGraphics.DrawRectangleInScrn( scrnViewRect, 3, Color.Green, 0f );

            BasicGraphics.DrawPoint( Coordin.LogicPos( ConvertHelper.PointToVector2( InputHandler.CurMousePos ) ), 3f, Color.Red, LayerDepth.Mouse );

            if (firstDraw)
            {
                //TextEffect.AddRiseFade( "Use 'AWSD' to Move the Tank, 'JK' to Rotate the Turret,", new Vector2( 40, 150 ), 1.2f, Color.Red, LayerDepth.Text, FontType.Comic, 500, 0.2f );
                //TextEffect.AddRiseFade( "and press Space To Shoot!", new Vector2( 40, 160 ), 1.2f, Color.Red, LayerDepth.Text, FontType.Comic, 500, 0.2f );

                firstDraw = false;
            }

            if (showFirstHit)
            {
                TextEffect.AddRiseFade( "哦，这个家伙变小了！", new Vector2( 50, 70 ), 1.4f, Color.Red, LayerDepth.Text, FontType.HanDinJianShu, 300, 0.2f );
                showFirstHit = false;
            }

            if (showFirstScore)
            {
                TextEffect.AddRiseFade( "当这个物体消失时，你会获得100分！", new Vector2( 20, 70 ), 1.4f, Color.Red, LayerDepth.Text, FontType.HanDinJianShu, 300, 0.2f );
                TextEffect.AddRiseFade( "不过这个物体也会开始运动。", new Vector2( 20, 80 ), 1.4f, Color.Red, LayerDepth.Text, FontType.HanDinJianShu, 300, 0.2f );

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
                    TextEffect.AddRiseFade( "GOD LIKE!", tank.Pos, 1.1f, Color.Black, LayerDepth.Text, FontType.Comic, 200, 1f );
                else if (NiceShootSum >= 17)
                    TextEffect.AddRiseFade( "Master Shoot!", tank.Pos, 1.1f, Color.Pink, LayerDepth.Text, FontType.Comic, 200, 1f );
                else if (NiceShootSum >= 4)
                    TextEffect.AddRiseFade( "Nice Shoot!", tank.Pos, 1.1f, Color.GreenYellow, LayerDepth.Text, FontType.Comic, 200, 1f );

                showNiceShoot = false;
            }

            if (tank.Rader.PointInRader( InputHandler.CurMousePosInLogic ))
            {
                FontManager.DrawInScrnCoord( "mouse In Rader!", ConvertHelper.PointToVector2( InputHandler.CurMousePos ), 0.6f, Color.Yellow, LayerDepth.Text, FontType.Lucida );
            }

            if (tank.Rader.ShelterObjs.Length != 0)
            {
                foreach (IShelterObj obj in tank.Rader.ShelterObjs)
                {
                    FontManager.Draw( "I am Sheltered by " + obj.ObjInfo.Name + " Pos: " + ((IGameObj)obj).Pos.ToString(), tank.Pos, 0.6f, Color.Yellow, LayerDepth.Text, FontType.Lucida );
                }
            }


            FontManager.DrawInScrnCoord( "You Score : " + Score.ToString(), new Vector2( 500, 40 ), 0.6f, Color.Yellow, LayerDepth.Text, FontType.Comic );
            FontManager.DrawInScrnCoord( "Shoot Sum : " + shootSum.ToString(), new Vector2( 500, 60 ), 0.6f, Color.Yellow, LayerDepth.Text, FontType.Comic );
            FontManager.DrawInScrnCoord( "hit Sum   : " + hitSum.ToString(), new Vector2( 500, 80 ), 0.6f, Color.Yellow, LayerDepth.Text, FontType.Comic );
            float hitRate = shootSum != 0 ? (float)hitSum * 100 / (float)shootSum : 0;
            FontManager.DrawInScrnCoord( "hit rate  : " + hitRate.ToString() + "%", new Vector2( 500, 100 ), 0.6f, Color.Yellow, LayerDepth.Text, FontType.Comic );


        }

        #endregion

        #region Rules

        SmokeGenerater smoke;

        bool speedy = false;



        //float lastCollideWithBorderTime = -1;
        void item_OnCollided ( IGameObj Sender, CollisionResult result, GameObjInfo objB )
        {

            if (objB.Name == "Border")
            {
                ((ItemCommon)Sender).Vel = -2 * Vector2.Dot( ((ItemCommon)Sender).Vel, result.NormalVector ) * result.NormalVector + ((ItemCommon)Sender).Vel;

                //float curTime = GameManager.CurTime;
                //if (lastCollideWithBorderTime != -1 && curTime - lastCollideWithBorderTime < 0.05f)
                //    ((ItemCommon)Sender).Scale -= 0.1f * 0.25f;

                //lastCollideWithBorderTime = curTime;
            }
            else if (objB.Name == "Tank")
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
            else if (objB.Name == "ShellNormal")
            {
                smoke.Concen += 0.3f;

                if (((ItemCommon)Sender).Scale < 0.5f * 0.031f)
                {
                    scene.RemoveGameObj( Sender, true, false, false, false, SceneKeeperCommon.GameObjLayer.HighBulge );

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
                    TextEffect.AddRiseFade( "You Got A Speedy Turret!", tank.Pos, 2f, Color.Purple, LayerDepth.Text, FontType.Lucida, 300, 0.2f );
                    tank.FireCDTime = 2f;
                    speedy = true;
                }
            }

        }

        private void AddNewItem ( IGameObj Sender )
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
            scene.AddGameObj( Sender, true, false, false, SceneKeeperCommon.GameObjLayer.HighBulge );
        }

        void Tank_onShoot ( Tank sender, Vector2 turretEnd, float azi )
        {
            ShellNormal newShell = new ShellNormal( sender, turretEnd, azi, shellSpeed );
            newShell.onCollided += new Platform.PhisicalCollision.OnCollidedEventHandler( Shell_onCollided );
            newShell.onOverlap += new Platform.PhisicalCollision.OnCollidedEventHandler( Shell_onOverlap );
            scene.AddGameObj( newShell, true, false, false, SceneKeeperCommon.GameObjLayer.lowFlying );
            shootSum++;
            //camera.Focus( newShell );
        }

        void Shell_onOverlap ( IGameObj Sender, CollisionResult result, GameObjInfo objB )
        {
            if (objB.Name == "Border")
                NiceShootSum = Math.Max( 0, NiceShootSum - 10 );
            //camera.Focus( tank );
        }

        void Shell_onCollided ( IGameObj Sender, CollisionResult result, GameObjInfo objB )
        {
            scene.RemoveGameObj( Sender, true, false, false, false, SceneKeeperCommon.GameObjLayer.lowFlying );


            //camera.Focus( tank );
            //if (objB.Name == "Border")
            //{
            //    NiceShootSum = Math.Max( 0, NiceShootSum - 1 );
            //}
        }

        IEyeableInfo GetItemInfo ( IRaderOwner raderOwner, IEyeableObj item )
        {
            return (IEyeableInfo)(new ItemEyeableInfo( (ItemCommon)item ));
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

        public ItemEyeableInfo ( ItemCommon item )
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
        public Ball ( float scale, Vector2 pos, float azi, Vector2 vel, float rotaVel )
            : base( "item", "Scorpion", Path.Combine( Directories.ItemDirectory, "Internal\\Ball" ),
            GameObjData.Load( File.OpenRead( Path.Combine( Directories.ItemDirectory, "Internal\\Ball\\Ball.xml" ) ) ),
            scale, pos, azi, vel, rotaVel )
        {
        }
    }

    //class TankEyeableInfo : IEyeableInfo
    //{

    //    TankSinTur tank;

    //    public Vector2 Pos
    //    {
    //        get { return tank.Pos; }
    //    }

    //    public Vector2 Vel
    //    {
    //        get { return ((TankContrSinTur)tank.PhisicalUpdater).Vel; }
    //    }

    //    public TankEyeableInfo ( TankSinTur tank )
    //    {
    //        this.tank = tank;
    //    }

    //    #region IEyeableInfo 成员

    //    public GameObjInfo ObjInfo
    //    {
    //        get { return tank.ObjInfo; }
    //    }

    //    #endregion
    //}
}
