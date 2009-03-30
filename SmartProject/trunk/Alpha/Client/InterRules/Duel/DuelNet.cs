using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using SmartTank.Rule;
using SmartTank.Screens;
using SmartTank.AI;
using SmartTank.GameObjs.Tank.SinTur;
using SmartTank.Effects.SceneEffects;
using SmartTank.GameObjs;
using SmartTank.Draw.UI.Controls;
using SmartTank.Helpers.DependInject;
using TankEngine2D.DataStructure;
using SmartTank.Scene;
using SmartTank.Draw;
using SmartTank.Draw.BackGround.VergeTile;
using SmartTank.GameObjs.Tank;
using TankEngine2D.Graphics;
using SmartTank;
using InterRules.ShootTheBall;
using TankEngine2D.Input;
using SmartTank.GameObjs.Shell;
using SmartTank.Helpers;
using TankEngine2D.Helpers;
using SmartTank.Senses.Vision;
using SmartTank.Update;
using SmartTank.Effects.TextEffects;
using SmartTank.Sounds;
using System.Windows.Forms;
using SmartTank.Effects;
using SmartTank.net;

namespace InterRules.Duel
{

    [RuleAttribute("DuelNet", "两个坦克在空旷的场景中搏斗（网络版）", "SmartTank编写组", 2009, 3, 30)]
    public class DuelNetRule : IGameRule
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
            get { return "DuelNet"; }
        }

        public string RuleIntroduction
        {
            get { return "两个坦克在空旷的场景中搏斗（网络版）"; }
        }

        #endregion

        public DuelNetRule()
        {
            BaseGame.ShowMouse = true;

            AIListForTank1 = new Combo("AIListForTank1", new Vector2(100, 100), 250);
            AIListForTank2 = new Combo("AIListForTank1", new Vector2(100, 300), 250);

            AIListForTank1.OnChangeSelection += new EventHandler(AIListForTank1_OnChangeSelection);
            AIListForTank2.OnChangeSelection += new EventHandler(AIListForTank2_OnChangeSelection);

            aiLoader = new AILoader();
            aiLoader.AddInterAI(typeof(DuelerNoFirst));
            aiLoader.AddInterAI(typeof(ManualControl));
            aiLoader.AddInterAI(typeof(DuelAIModel));
            aiLoader.AddInterAI(typeof(AutoShootAI));
            aiLoader.InitialCompatibleAIs(typeof(DuelAIOrderServer), typeof(AICommonServer));
            foreach (string name in aiLoader.GetAIList())
            {
                AIListForTank1.AddItem(name);
                AIListForTank2.AddItem(name);
            }
            btn = new TextButton("OkBtn", new Vector2(700, 500), "Begin", 0, Color.Yellow);
            btn.OnClick += new EventHandler(btn_OnPress);

            LoadResouce();
        }

        private void LoadResouce()
        {
            ShellExplode.LoadResources();
            Explode.LoadResources();
        }

        void btn_OnPress(object sender, EventArgs e)
        {
            GameManager.AddGameScreen(new DuelNetGameScreen(aiLoader.GetAIInstance(selectIndexTank1), aiLoader.GetAIInstance(selectIndexTank2)));
        }

        void AIListForTank1_OnChangeSelection(object sender, EventArgs e)
        {
            selectIndexTank1 = AIListForTank1.currentIndex;
        }

        void AIListForTank2_OnChangeSelection(object sender, EventArgs e)
        {
            selectIndexTank2 = AIListForTank2.currentIndex;
        }

        #region IGameScreen 成员

        public void Render()
        {
            BaseGame.Device.Clear(Color.LawnGreen);

            AIListForTank1.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            AIListForTank2.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            btn.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
        }

        public bool Update(float second)
        {


            AIListForTank1.Update();
            AIListForTank2.Update();
            btn.Update();

            if (InputHandler.JustPressKey(Microsoft.Xna.Framework.Input.Keys.Escape))
                return true;

            return false;
        }

        #endregion

    }

    class DuelNetGameScreen : IGameScreen
    {
        #region Constants
        readonly Rectangle scrnViewRect = new Rectangle(30, 30, 740, 540);
        readonly Rectanglef mapRect = new Rectanglef(0, 0, 300, 220);
        readonly float tankRaderLength = 90;
        readonly float tankMaxForwardSpd = 60;
        readonly float tankMaxBackwardSpd = 50;
        readonly float shellSpeed = 100;
        #endregion

        #region Variables

        SceneMgr sceneMgr;
        Camera camera;

        VergeTileGround vergeGround;

        DuelTank tank1;
        DuelTank tank2;

        AICommonServer commonServer;

        bool gameStart = false;
        bool gameOver = false;

        #endregion

        #region Construction

        public DuelNetGameScreen(IAI TankAI1, IAI TankAI2)
        {
            BaseGame.CoordinMgr.SetScreenViewRect(scrnViewRect);

            camera = new Camera(2.6f, new Vector2(150, 112), 0);
            camera.maxScale = 4.5f;
            camera.minScale = 2f;
            camera.Enable();

            InitialBackGround();

            sceneMgr = new SceneMgr();
            SceneInitial();
            GameManager.LoadScene(sceneMgr);

            RuleInitial();

            AIInitial(TankAI1, TankAI2);

            InitialDrawMgr(TankAI1, TankAI2);

            InitialStartTimer();

            InfoRePath.IsMainHost = true;
        }

        private void AIInitial(IAI tankAI1, IAI tankAI2)
        {
            commonServer = new AICommonServer(mapRect);

            tankAI1.CommonServer = commonServer;
            tankAI1.OrderServer = tank1;
            tank1.SetTankAI(tankAI1);

            tankAI2.CommonServer = commonServer;
            tankAI2.OrderServer = tank2;
            tank2.SetTankAI(tankAI2);

            //GameManager.ObjMemoryMgr.AddSingle( tank1 );
            //GameManager.ObjMemoryMgr.AddSingle( tank2 );
        }

        private void InitialDrawMgr(IAI tankAI1, IAI tankAI2)
        {
            if (tankAI1 is ManualControl)
            {
                DrawMgr.SetCondition(
                    delegate(IDrawableObj obj)
                    {
                        if (tank1.IsDead)
                            return true;

                        if (tank1.Rader.PointInRader(obj.Pos) || obj == tank1 ||
                            ((obj is ShellNormal) && ((ShellNormal)obj).Firer == tank1))
                            return true;
                        else
                            return false;
                    });
            }
            if (tankAI2 is ManualControl)
            {
                DrawMgr.SetCondition(
                    delegate(IDrawableObj obj)
                    {
                        if (tank2.IsDead)
                            return true;

                        if (tank2.Rader.PointInRader(obj.Pos) || obj == tank2 ||
                            ((obj is ShellNormal) && ((ShellNormal)obj).Firer == tank2))
                            return true;
                        else
                            return false;
                    });
            }
        }

        private void InitialBackGround()
        {
            VergeTileData data = new VergeTileData();
            data.gridWidth = 30;
            data.gridHeight = 22;
            data.SetRondomVertexIndex(30, 22, 4);
            data.SetRondomGridIndex(30, 22);
            data.texPaths = new string[]
            {
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_Dirt.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_DirtRough.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_DirtGrass.tga"),
                Path.Combine(Directories.ContentDirectory,"BackGround\\Lords_GrassDark.tga"),
            };
            vergeGround = new VergeTileGround(data, scrnViewRect, mapRect);
        }

        private void SceneInitial()
        {
            tank1 = new DuelTank("tank1", TankSinTur.M60TexPath, TankSinTur.M60Data, new Vector2(mapRect.X + 150 + RandomHelper.GetRandomFloat(-40, 40),
                mapRect.Y + 60 + RandomHelper.GetRandomFloat(-10, 10)),
                MathHelper.Pi + RandomHelper.GetRandomFloat(-MathHelper.PiOver4, MathHelper.PiOver4),
                "Tank1", tankRaderLength, tankMaxForwardSpd, tankMaxBackwardSpd, 10);
            tank2 = new DuelTank("tank2", TankSinTur.M1A2TexPath, TankSinTur.M1A2Data, new Vector2(mapRect.X + 150 + RandomHelper.GetRandomFloat(-40, 40),
                mapRect.Y + 160 + RandomHelper.GetRandomFloat(-10, 10)),
                RandomHelper.GetRandomFloat(-MathHelper.PiOver4, MathHelper.PiOver4),
                "Tank2", tankRaderLength, tankMaxForwardSpd, tankMaxBackwardSpd, 10);

            tank1.ShellSpeed = shellSpeed;
            tank2.ShellSpeed = shellSpeed;

            camera.Focus(tank1, true);

            sceneMgr.AddGroup("", new TypeGroup<DuelTank>("tank"));
            sceneMgr.AddGroup("", new TypeGroup<SmartTank.PhiCol.Border>("border"));
            sceneMgr.AddGroup("", new TypeGroup<ShellNormal>("shell"));
            sceneMgr.PhiGroups.Add("tank");
            sceneMgr.PhiGroups.Add("shell");
            sceneMgr.AddColMulGroups("tank", "border", "shell");
            sceneMgr.ShelterGroups.Add(new SceneMgr.MulPair("tank", new List<string>()));
            sceneMgr.VisionGroups.Add(new SceneMgr.MulPair("tank", new List<string>(new string[] { "tank" })));

            sceneMgr.AddGameObj("tank", tank1);
            sceneMgr.AddGameObj("tank", tank2);
            sceneMgr.AddGameObj("border", new SmartTank.PhiCol.Border(mapRect));
        }

        private void InitialStartTimer()
        {
            new GameTimer(1,
                delegate()
                {
                    TextEffectMgr.AddRiseFadeInScrnCoordin("3", new Vector2(400, 300), 3, Color.Red, LayerDepth.Text, GameFonts.Lucida, 200, 1.4f);
                });

            new GameTimer(2,
                delegate()
                {
                    TextEffectMgr.AddRiseFadeInScrnCoordin("2", new Vector2(400, 300), 3, Color.Red, LayerDepth.Text, GameFonts.Lucida, 200, 1.4f);
                });

            new GameTimer(3,
                delegate()
                {
                    TextEffectMgr.AddRiseFadeInScrnCoordin("1", new Vector2(400, 300), 3, Color.Red, LayerDepth.Text, GameFonts.Lucida, 200, 1.4f);
                });

            new GameTimer(4,
                delegate()
                {
                    TextEffectMgr.AddRiseFadeInScrnCoordin("Start!", new Vector2(400, 300), 3, Color.Red, LayerDepth.Text, GameFonts.Lucida, 200, 1.4f);

                });
            new GameTimer(5,
                delegate()
                {
                    gameStart = true;
                });
        }

        private void RuleInitial()
        {
            tank1.onShoot += new Tank.ShootEventHandler(tank_onShoot);
            tank2.onShoot += new Tank.ShootEventHandler(tank_onShoot);

            tank1.onCollide += new OnCollidedEventHandler(tank_onCollide);
            tank2.onCollide += new OnCollidedEventHandler(tank_onCollide);
        }


        #endregion

        #region Rules

        int shellSum = 0;
        void tank_onShoot(Tank sender, Vector2 turretEnd, float azi)
        {
            ShellNormal shell = new ShellNormal("shell" + shellSum.ToString(), sender, turretEnd, azi, shellSpeed);
            shell.onCollided += new OnCollidedEventHandler(shell_onCollided);
            shellSum++;
            sceneMgr.AddGameObj("shell", shell);
            Sound.PlayCue("CANNON1");
        }

        void tank_onCollide(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            if (objB.ObjClass == "ShellNormal")
            {
                if (Sender == tank1)
                {
                    tank1.Live--;
                    // 在此添加效果
                    tank1.smoke.Concen += 0.2f;

                    if (tank1.Live <= 0 && !tank1.IsDead)
                    {
                        new Explode(tank1.Pos, 0);

                        tank1.Dead();

                        new GameTimer(3,
                            delegate()
                            {
                                MessageBox.Show("Tank2胜利！");
                                gameOver = true;
                            });

                    }
                }
                else if (Sender == tank2)
                {
                    tank2.Live--;

                    tank2.smoke.Concen += 0.2f;
                    // 在此添加效果
                    if (tank2.Live <= 0 && !tank2.IsDead)
                    {
                        new Explode(tank2.Pos, 0);

                        tank2.Dead();

                        new GameTimer(3,
                            delegate()
                            {
                                MessageBox.Show("Tank1胜利！");
                                gameOver = true;
                            });
                    }
                }
            }
        }

        void shell_onCollided(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            sceneMgr.DelGameObj("shell", Sender.Name);
            new ShellExplodeBeta(Sender.Pos, ((ShellNormal)Sender).Azi);

            Quake.BeginQuake(10, 50);
            Sound.PlayCue("EXPLO1");

        }

        #endregion

        #region IGameScreen 成员

        public bool Update(float seconds)
        {
            if (InputHandler.JustPressKey(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                GameManager.ComponentReset();
                return true;
            }

            if (!gameStart)
                return false;

            GameManager.UpdataComponent(seconds);

            if (InputHandler.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                camera.Move(new Vector2(-5, 0));
            if (InputHandler.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                camera.Move(new Vector2(5, 0));
            if (InputHandler.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
                camera.Move(new Vector2(0, -5));
            if (InputHandler.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
                camera.Move(new Vector2(0, 5));
            if (InputHandler.JustPressKey(Microsoft.Xna.Framework.Input.Keys.V))
                camera.Zoom(-0.2f);
            if (InputHandler.JustPressKey(Microsoft.Xna.Framework.Input.Keys.B))
                camera.Zoom(0.2f);
            if (InputHandler.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.T))
                camera.Rota(0.1f);
            if (InputHandler.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.R))
                camera.Rota(-0.1f);

            camera.Update(seconds);

            if (gameOver)
            {
                GameManager.ComponentReset();
                return true;
            }

            return false;
        }

        public void Render()
        {
            vergeGround.Draw();

            GameManager.DrawManager.Draw();

            BaseGame.BasicGraphics.DrawRectangle(mapRect, 3, Color.Red, 0f);
            BaseGame.BasicGraphics.DrawRectangleInScrn(scrnViewRect, 3, Color.Green, 0f);

            //foreach (Vector2 visiPoint in tank1.KeyPoints)
            //{
            //    BasicGraphics.DrawPoint( Vector2.Transform( visiPoint, tank1.TransMatrix ), 0.4f, Color.Blue, 0f );
            //}

            //foreach (Vector2 visiPoint in tank2.KeyPoints)
            //{
            //    BasicGraphics.DrawPoint( Vector2.Transform( visiPoint, tank2.TransMatrix ), 0.4f, Color.Blue, 0f );
            //}

        }

        #endregion
    }

    
}
