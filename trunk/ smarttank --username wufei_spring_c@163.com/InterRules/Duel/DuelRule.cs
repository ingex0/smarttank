using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameObjects.Tank.Tanks;
using Microsoft.Xna.Framework;
using Platform.Scene;
using Platform.GameDraw;
using GameBase.Graphics.BackGround.VergeTile;
using GameBase;
using GameBase.Input;
using GameBase.Graphics;
using GameBase.DataStructure;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Helpers;
using System.IO;
using Platform.GameRules;
using Platform;
using Platform.GameObjects;
using Platform.GameObjects.Tank.TankSkins;
using Platform.GameObjects.Shell;
using Platform.GameObjects.Tank.TankAIs;
using InterRules.ShootTheBall;
using System.Windows.Forms;
using Platform.DependInject;
using Platform.GameScreens;
using GameBase.UI;
using GameBase.Effects;
using Platform.Sounds;
using Platform.UpdateManage;
using Platform.GameDraw.SceneEffects;

namespace InterRules.Duel
{

    [RuleAttribute( "Duel", "两个坦克在空旷的场景中搏斗（平台的第二个规则）", "SmartTank编写组", 2007, 11, 2 )]
    public class DuelRule : IGameRule
    {
        #region Variables

        Combo AIListForTank1;
        Combo AIListForTank2;

        TextButton btn;

        int selectIndexTank1;
        int selectIndexTank2;

        AILoader aiLoader;

        #endregion

        #region IGameRule 成员

        public string RuleName
        {
            get { return "Duel"; }
        }

        public string RuleIntroduction
        {
            get { return "两个坦克在空旷的场景中搏斗（平台的第二个规则）"; }
        }

        #endregion

        public DuelRule ()
        {
            BaseGame.ShowMouse = true;

            AIListForTank1 = new Combo( "AIListForTank1", new Vector2( 100, 100 ), 250 );
            AIListForTank2 = new Combo( "AIListForTank1", new Vector2( 100, 300 ), 250 );

            AIListForTank1.OnChangeSelection += new EventHandler( AIListForTank1_OnChangeSelection );
            AIListForTank2.OnChangeSelection += new EventHandler( AIListForTank2_OnChangeSelection );

            aiLoader = new AILoader();
            aiLoader.AddInterAI( typeof( DuelerNoFirst ) );
            aiLoader.AddInterAI( typeof( ManualControl ) );
            aiLoader.AddInterAI( typeof( DuelAIModel ) );
            aiLoader.AddInterAI( typeof( AutoShootAI ) );
            aiLoader.InitialCompatibleAIs( typeof( DuelAIOrderServer ), typeof( AICommonServer ) );
            foreach (string name in aiLoader.GetAIList())
            {
                AIListForTank1.AddItem( name );
                AIListForTank2.AddItem( name );
            }
            btn = new TextButton( "OkBtn", new Vector2( 700, 500 ), "Begin", 0, Color.Yellow );
            btn.OnClick += new EventHandler( btn_OnPress );

            LoadResouce();
        }

        private void LoadResouce ()
        {
            ShellExplode.LoadResources();
            Explode.LoadResources();
        }

        void btn_OnPress ( object sender, EventArgs e )
        {
            GameManager.AddGameScreen( new DuelGameScreen( aiLoader.GetAIInstance( selectIndexTank1 ), aiLoader.GetAIInstance( selectIndexTank2 ) ) );
        }

        void AIListForTank1_OnChangeSelection ( object sender, EventArgs e )
        {
            selectIndexTank1 = AIListForTank1.currentIndex;
        }

        void AIListForTank2_OnChangeSelection ( object sender, EventArgs e )
        {
            selectIndexTank2 = AIListForTank2.currentIndex;
        }

        #region IGameScreen 成员

        public void Render ()
        {
            BaseGame.Device.Clear( Color.LawnGreen );

            AIListForTank1.Draw( Sprite.alphaSprite, 1 );
            AIListForTank2.Draw( Sprite.alphaSprite, 1 );
            btn.Draw( Sprite.alphaSprite, 1 );
        }

        public bool Update ( float second )
        {


            AIListForTank1.Update();
            AIListForTank2.Update();
            btn.Update();

            if (InputHandler.JustPressKey( Microsoft.Xna.Framework.Input.Keys.Escape ))
                return true;

            return false;
        }

        #endregion

    }

    class DuelGameScreen : IGameScreen
    {
        #region Constants
        readonly Rectangle scrnViewRect = new Rectangle( 30, 30, 740, 540 );
        readonly Vector2 mapSize = new Vector2( 300, 220 );
        readonly float tankRaderLength = 90;
        readonly float tankMaxForwardSpd = 60;
        readonly float tankMaxBackwardSpd = 50;
        readonly float shellSpeed = 100;
        #endregion

        #region Variables

        SceneKeeperCommon scene;
        Camera camera;

        VergeTileGround vergeGround;

        DuelTank tank1;
        DuelTank tank2;

        AICommonServer commonServer;

        bool gameStart = false;
        bool gameOver = false;

        #endregion

        #region Construction

        public DuelGameScreen ( IAI TankAI1, IAI TankAI2 )
        {
            Coordin.SetScreenViewRect( scrnViewRect );

            camera = new Camera( 2.6f, new Vector2( 150, 112 ), 0 );
            camera.maxScale = 4.5f;
            camera.minScale = 2f;
            camera.Enable();

            InitialBackGround();

            scene = new SceneKeeperCommon();
            SceneInitial();
            GameManager.LoadScene( scene );

            RuleInitial();

            AIInitial( TankAI1, TankAI2 );

            InitialDrawManager( TankAI1, TankAI2 );

            InitialStartTimer();
        }

        private void AIInitial ( IAI tankAI1, IAI tankAI2 )
        {
            commonServer = new AICommonServer( mapSize );

            tankAI1.CommonServer = commonServer;
            tankAI1.OrderServer = tank1;
            tank1.SetTankAI( tankAI1 );

            tankAI2.CommonServer = commonServer;
            tankAI2.OrderServer = tank2;
            tank2.SetTankAI( tankAI2 );

            //GameManager.ObjMemoryManager.AddSingle( tank1 );
            //GameManager.ObjMemoryManager.AddSingle( tank2 );
        }

        private void InitialDrawManager ( IAI tankAI1, IAI tankAI2 )
        {
            if (tankAI1 is ManualControl)
            {
                DrawManager.SetCondition(
                    delegate( IDrawableObj obj )
                    {
                        if (tank1.IsDead)
                            return true;

                        if (tank1.Rader.PointInRader( obj.Pos ) || obj == tank1 ||
                            ((obj is ShellNormal) && ((ShellNormal)obj).Firer == tank1))
                            return true;
                        else
                            return false;
                    } );
            }
            if (tankAI2 is ManualControl)
            {
                DrawManager.SetCondition(
                    delegate( IDrawableObj obj )
                    {
                        if (tank2.IsDead)
                            return true;

                        if (tank2.Rader.PointInRader( obj.Pos ) || obj == tank2 ||
                            ((obj is ShellNormal) && ((ShellNormal)obj).Firer == tank2))
                            return true;
                        else
                            return false;
                    } );
            }
        }

        private void InitialBackGround ()
        {
            VergeTileData data = new VergeTileData();
            data.gridWidth = 30;
            data.gridHeight = 22;
            data.SetRondomVertexIndex( 30, 22, 4 );
            data.SetRondomGridIndex( 30, 22 );
            data.texPaths = new string[]
            {
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_Dirt.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_DirtRough.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_DirtGrass.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_GrassDark.tga"),
            };
            vergeGround = new VergeTileGround( data, scrnViewRect, mapSize );
        }

        private void SceneInitial ()
        {
            tank1 = new DuelTank( TankSkinSinTurData.M60, new Vector2( 150 + RandomHelper.GetRandomFloat( -40, 40 ),
                60 + RandomHelper.GetRandomFloat( -10, 10 ) ),
                MathHelper.Pi + RandomHelper.GetRandomFloat( -MathHelper.PiOver4, MathHelper.PiOver4 ),
                "Tank1", tankRaderLength, tankMaxForwardSpd, tankMaxBackwardSpd, 10 );
            tank2 = new DuelTank( TankSkinSinTurData.M1A2, new Vector2( 150 + RandomHelper.GetRandomFloat( -40, 40 ),
                160 + RandomHelper.GetRandomFloat( -10, 10 ) ),
                RandomHelper.GetRandomFloat( -MathHelper.PiOver4, MathHelper.PiOver4 ),
                "Tank2", tankRaderLength, tankMaxForwardSpd, tankMaxBackwardSpd, 10 );

            tank1.ShellSpeed = shellSpeed;
            tank2.ShellSpeed = shellSpeed;

            camera.Focus( tank1, true );


            scene.AddGameObj( tank1, true, false, true, SceneKeeperCommon.GameObjLayer.HighBulge, new Platform.Senses.Vision.GetEyeableInfoHandler( TankSinTur.GetCommonEyeInfoFun ) );
            scene.AddGameObj( tank2, true, false, true, SceneKeeperCommon.GameObjLayer.HighBulge, new Platform.Senses.Vision.GetEyeableInfoHandler( TankSinTur.GetCommonEyeInfoFun ) );
            scene.SetBorder( 0, mapSize.X, 0, mapSize.Y );
        }

        private void InitialStartTimer ()
        {
            new GameTimer( 1,
                delegate()
                {
                    TextEffect.AddRiseFadeInScrnCoordin( "3", new Vector2( 400, 300 ), 3, Color.Red, LayerDepth.Text, FontType.Lucida, 200, 1.4f );
                } );

            new GameTimer( 2,
                delegate()
                {
                    TextEffect.AddRiseFadeInScrnCoordin( "2", new Vector2( 400, 300 ), 3, Color.Red, LayerDepth.Text, FontType.Lucida, 200, 1.4f );
                } );

            new GameTimer( 3,
                delegate()
                {
                    TextEffect.AddRiseFadeInScrnCoordin( "1", new Vector2( 400, 300 ), 3, Color.Red, LayerDepth.Text, FontType.Lucida, 200, 1.4f );
                } );

            new GameTimer( 4,
                delegate()
                {
                    TextEffect.AddRiseFadeInScrnCoordin( "Start!", new Vector2( 400, 300 ), 3, Color.Red, LayerDepth.Text, FontType.Lucida, 200, 1.4f );

                } );
            new GameTimer( 5,
                delegate()
                {
                    gameStart = true;
                } );
        }

        private void RuleInitial ()
        {
            tank1.onShoot += new Platform.GameObjects.Tank.Tank.ShootEventHandler( tank_onShoot );
            tank2.onShoot += new Platform.GameObjects.Tank.Tank.ShootEventHandler( tank_onShoot );

            tank1.onCollide += new Platform.PhisicalCollision.OnCollidedEventHandler( tank_onCollide );
            tank2.onCollide += new Platform.PhisicalCollision.OnCollidedEventHandler( tank_onCollide );
        }


        #endregion

        #region Rules

        void tank_onShoot ( Platform.GameObjects.Tank.Tank sender, Vector2 turretEnd, float azi )
        {
            ShellNormal shell = new ShellNormal( sender, turretEnd, azi, shellSpeed );
            shell.onCollided += new Platform.PhisicalCollision.OnCollidedEventHandler( shell_onCollided );
            scene.AddGameObj( shell, true, false, false, SceneKeeperCommon.GameObjLayer.lowFlying );
            Sound.PlayCue( "CANNON1" );
        }

        void tank_onCollide ( IGameObj Sender, CollisionResult result, GameObjInfo objB )
        {
            if (objB.Name == "ShellNormal")
            {
                if (Sender == tank1)
                {
                    tank1.Live--;
                    // 在此添加效果
                    tank1.smoke.Concen += 0.2f;

                    if (tank1.Live <= 0 && !tank1.IsDead)
                    {
                        new Explode( tank1.Pos, 0 );

                        tank1.Dead();

                        new GameTimer( 3,
                            delegate()
                            {
                                MessageBox.Show( "Tank2胜利！" );
                                gameOver = true;
                            } );

                    }
                }
                else if (Sender == tank2)
                {
                    tank2.Live--;

                    tank2.smoke.Concen += 0.2f;
                    // 在此添加效果
                    if (tank2.Live <= 0 && !tank2.IsDead)
                    {
                        new Explode( tank2.Pos, 0 );

                        tank2.Dead();

                        new GameTimer( 3,
                            delegate()
                            {
                                MessageBox.Show( "Tank1胜利！" );
                                gameOver = true;
                            } );
                    }
                }
            }
        }

        void shell_onCollided ( IGameObj Sender, CollisionResult result, GameObjInfo objB )
        {
            scene.RemoveGameObj( Sender, true, false, false, false, SceneKeeperCommon.GameObjLayer.lowFlying );
            new ShellExplodeBeta( Sender.Pos, ((ShellNormal)Sender).Azi );

            Quake.BeginQuake( 10, 50 );
            Sound.PlayCue( "EXPLO1" );

        }

        #endregion

        #region IGameScreen 成员

        public bool Update ( float seconds )
        {
            if (InputHandler.JustPressKey( Microsoft.Xna.Framework.Input.Keys.Escape ))
            {
                GameManager.ComponentReset();
                return true;
            }

            GameTimer.UpdateTimers( seconds );

            if (!gameStart)
                return false;

            GameManager.UpdataComponent( seconds );

            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Left ))
                camera.Move( new Vector2( -5, 0 ) );
            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Right ))
                camera.Move( new Vector2( 5, 0 ) );
            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Up ))
                camera.Move( new Vector2( 0, -5 ) );
            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.Down ))
                camera.Move( new Vector2( 0, 5 ) );
            if (InputHandler.JustPressKey( Microsoft.Xna.Framework.Input.Keys.V ))
                camera.Zoom( -0.2f );
            if (InputHandler.JustPressKey( Microsoft.Xna.Framework.Input.Keys.B ))
                camera.Zoom( 0.2f );
            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.T ))
                camera.Rota( 0.1f );
            if (InputHandler.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.R ))
                camera.Rota( -0.1f );

            camera.Update( seconds );

            if (gameOver)
            {
                GameManager.ComponentReset();
                return true;
            }

            return false;
        }

        public void Render ()
        {
            vergeGround.Draw();

            GameManager.DrawManager.Draw();

            BasicGraphics.DrawRectangle( new Rectanglef( 0, 0, mapSize.X, mapSize.Y ), 3, Color.Red, 0f );
            BasicGraphics.DrawRectangleInScrn( scrnViewRect, 3, Color.Green, 0f );
        }

        #endregion
    }

    #region class Define

    public interface DuelAIOrderServer : IAIOrderServerSinTur
    {
        float Live { get;}
    }

    /// <summary>
    /// 写本规则的AI，请继承该接口。
    /// </summary>
    [IAIAttribute( typeof( DuelAIOrderServer ), typeof( IAICommonServer ) )]
    public interface IDuelAI : IAISinTur
    {

    }


    class DuelTank : TankSinTur, DuelAIOrderServer
    {
        float live;

        public SmokeGenerater smoke;

        public DuelTank ( TankSkinSinTurData skinData, Vector2 pos, float Azi, string tankName,
            float tankRaderLength, float tankMaxForwardSpd, float tankMaxBackwardSpd,
            float live )
            : base( new GameObjInfo( "DuelTank", tankName ), new TankSkinSinTur( skinData ),
                tankRaderLength, MathHelper.Pi * 0.15f, Color.Yellow,
                tankMaxForwardSpd, tankMaxBackwardSpd, MathHelper.PiOver4, MathHelper.PiOver4, MathHelper.Pi, 0.8f, pos, Azi )
        {
            this.live = live;
            smoke = new SmokeGenerater( 0, 30, Vector2.Zero, 0.3f, 0, false, this );
        }

        public override void Update ( float seconds )
        {
            base.Update( seconds );
            smoke.Update( seconds );
        }

        public override void Draw ()
        {
            base.Draw();
            smoke.Draw();
        }

        #region DuelAIOrderServer 成员

        public float Live
        {
            get { return live; }
            set { live = value; }
        }

        #endregion
    }

    #endregion
}
