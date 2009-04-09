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
using TankEngine2D.Helpers;
using TankEngine2D.Input;
using Microsoft.Xna.Framework.Input;
using SmartTank.Scene;
using System.IO;
using SmartTank.Helpers;
using SmartTank.GameObjs.Shell;
using TankEngine2D.DataStructure;
using SmartTank.Effects.SceneEffects;
using SmartTank.Effects;
using SmartTank.Sounds;

namespace InterRules.Starwar
{
    [RuleAttribute("Starwar", "支持多人联机的空战规则", "编写组成员：...", 2009, 4, 8)]
    public class StarwarRule : IGameRule
    {
        #region IGameRule 成员

        public string RuleIntroduction
        {
            get { return "20090328~20090417:编写组成员：..."; }
        }

        public string RuleName
        {
            get { return "Starwar"; }
        }

        #endregion

        #region IGameScreen 成员

        public void OnClose()
        {
        }

        public void Render()
        {

        }

        public bool Update(float second)
        {
            if (InputHandler.IsKeyDown(Keys.L))
                GameManager.AddGameScreen(new StarwarLogic());

            if (InputHandler.IsKeyDown(Keys.Escape))
                return true;

            return false;
        }

        #endregion
    }


    class StarwarLogic : RuleSupNet, IGameScreen
    {
        WarShip[] ships = new WarShip[6];
        Gold gold;
        List<Rock> rocks = new List<Rock>();
        int rockCount = 0;

        Rectanglef mapRect = new Rectanglef(0, 0, 1600, 1200);
        Camera camera;

        Sprite backGround;

        int shellcount = 0;

        Rectanglef RockCreateRect = new Rectanglef(-100, -100, 1800, 1400);
        float rockTimer = 0;

        public Vector2 MapCenterPos
        {
            get { return new Vector2(mapRect.X + mapRect.Width / 2, mapRect.Y + mapRect.Height / 2); }
        }

        public StarwarLogic()
        {
            LoadConfig();

            InitialCamera();
            InitializeScene();
            InitialBackGround();
        }

        private void InitialBackGround()
        {
            backGround = new Sprite(GameManager.RenderEngine, Path.Combine(Directories.ContentDirectory, "Rules\\SpaceWar\\image\\field_space_001.png"), false);
            backGround.SetParameters(new Vector2(0, 0), new Vector2(0, 0), 1.0f, 0f, Color.White, LayerDepth.BackGround, SpriteBlendMode.AlphaBlend);
        }

        private void LoadConfig()
        {
            SpaceWarConfig.LoadConfig();
        }

        private void InitializeScene()
        {

            ships[0] = new WarShip("ship" + 0, new Vector2(500, 500), 0, true);
            ships[0].OnCollied += new OnCollidedEventHandler(WarShip_OnCollied);
            ships[0].OnShoot += new WarShip.WarShipShootEventHandler(WarShip_OnShoot);
            ships[0].OnDead += new WarShip.WarShipDeadEventHandler(Warship_OnDead);

            gold = new Gold("gold", new Vector2(800, 600), 0);
            gold.OnOverLap += new OnCollidedEventHandler(Gold_OnOverLap);
            gold.OnLiveTimeOut += new Gold.GoldLiveTimeOutEventHandler(Gold_OnLiveTimeOut);

            sceneMgr = new SceneMgr();


            sceneMgr.AddGroup("", new TypeGroup<WarShip>("warship"));
            sceneMgr.AddGroup("", new TypeGroup<WarShipShell>("shell"));
            sceneMgr.AddGroup("", new TypeGroup<SmartTank.PhiCol.Border>("border"));
            sceneMgr.AddGroup("", new TypeGroup<Gold>("gold"));
            sceneMgr.AddGroup("", new TypeGroup<Rock>("rock"));

            sceneMgr.PhiGroups.Add("warship");
            sceneMgr.PhiGroups.Add("shell");
            sceneMgr.AddColMulGroups("warship", "shell", "border");
            sceneMgr.AddColMulGroups("warship", "shell", "rock");
            sceneMgr.AddColMulGroups("warship");
            sceneMgr.AddColMulGroups("rock");

            sceneMgr.AddLapMulGroups("warship", "gold");

            sceneMgr.AddGameObj("warship", ships[0]);
            sceneMgr.AddGameObj("border", new SmartTank.PhiCol.Border(mapRect));
            sceneMgr.AddGameObj("gold", gold);

            GameManager.LoadScene(sceneMgr);

            camera.Focus(ships[0], false);
        }

        void Gold_OnLiveTimeOut(Gold sender)
        {
            SetGoldNewPos(sender);
        }

        void Gold_OnOverLap(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            SetGoldNewPos(Sender as Gold);
        }

        private void SetGoldNewPos(Gold Sender)
        {
            while (true)
            {
                Vector2 pos = RandomHelper.GetRandomVector2(0, 1);
                pos.X *= mapRect.Width - 200;
                pos.Y *= mapRect.Height - 200;
                pos += new Vector2(100, 100);

                if (CanAddObjAtPos(pos))
                {
                    Sender.Born(pos);
                    break;
                }
            }
        }

        private bool CanAddObjAtPos(Vector2 pos)
        {
            return true;
        }

        void Warship_OnDead(WarShip sender)
        {
            sceneMgr.DelGameObj(sender.MgPath);
        }

        void WarShip_OnShoot(WarShip firer, Vector2 endPoint, float azi)
        {
            WarShipShell shell = new WarShipShell("shell" + shellcount, firer, endPoint, azi);
            shell.onCollided += new OnCollidedEventHandler(Shell_onCollided);
            shell.OnOutDate += new WarShipShell.ShellOutDateEventHandler(Shell_OnOutDate);
            sceneMgr.AddGameObj("shell", shell);
            shellcount++;

        }

        List<string> OutDateShellNames = new List<string>();
        void Shell_OnOutDate(WarShipShell sender, IGameObj shooter)
        {
            OutDateShellNames.Add(sender.Name);
        }

        void Shell_onCollided(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            if (objB.ObjClass == "WarShip")
            {
                sceneMgr.DelGameObj("shell", Sender.Name);
                new ShellExplodeBeta(Sender.Pos, ((ShellNormal)Sender).Azi);

                Quake.BeginQuake(10, 50);
                Sound.PlayCue("EXPLO1");
            }
            else
            {
                WarShipShell shell = (WarShipShell)Sender;
                shell.MirrorPath(result);
            }

        }

        void WarShip_OnCollied(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            if (objB.ObjClass == "WarShipShell")
            {
                WarShip ship = (WarShip)Sender;
                ship.BeginStill();
                ship.HitByShell();
            }
        }

        private void InitialCamera()
        {
            BaseGame.CoordinMgr.SetScreenViewRect(new Rectangle(0, 0, 800, 600));

            camera = new Camera(1f, new Vector2(400, 300), 0);
            camera.maxScale = 4.5f;
            camera.minScale = 0.5f;
            camera.Enable();
        }


        #region IGameScreen 成员

        void IGameScreen.OnClose()
        {

        }

        void IGameScreen.Render()
        {
            backGround.Draw();
            GameManager.DrawManager.Draw();
            BaseGame.BasicGraphics.DrawRectangle(mapRect, 3, Color.Red, 0f);


            GameManager.RenderEngine.FontMgr.DrawInScrnCoord("Ship1 Live: " + ships[0].HP, new Vector2(10, 500), 1.0f, Color.White, LayerDepth.Text, GameFonts.Comic);
        }

        bool IGameScreen.Update(float second)
        {
            base.Update(second);

            UpdateCamera(second);

            DeleteOutDateShells();

            CreateDelRock(second);

            if (InputHandler.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                GameManager.ComponentReset();
                return true;
            }
            else
                return false;
        }

        private void CreateDelRock(float second)
        {
            rockTimer += second;
            if (rockTimer > SpaceWarConfig.RockCreateTime)
            {
                rockTimer = 0;

                CreateRock();
            }

            List<Rock> delRocks = new List<Rock>();

            foreach (Rock rock in rocks)
            {
                if (rock.Pos.X < RockCreateRect.X
                    || rock.Pos.X > RockCreateRect.X + RockCreateRect.Width
                    || rock.Pos.Y < RockCreateRect.Y
                    || rock.Pos.Y > RockCreateRect.Y + RockCreateRect.Height)
                {
                    delRocks.Add(rock);
                }
            }

            foreach (Rock rock in delRocks)
            {
                sceneMgr.DelGameObj("rock", rock.Name);
                rocks.Remove(rock);
            }
        }

        private void CreateRock()
        {
            Vector2 newPos = new Vector2();
            while (true)
            {
                newPos = RandomHelper.GetRandomVector2(-100, 100);
                if (newPos.X < 0)
                    newPos.X -= mapRect.X;
                else 
                    newPos.X += mapRect.Width + mapRect.X;

                if (newPos.Y < 0)
                    newPos.Y -= mapRect.Y;
                else
                    newPos.Y += mapRect.Y + mapRect.Height;
                if (CanAddObjAtPos(newPos))
                    break;

            }
            float speed = RandomHelper.GetRandomFloat(SpaceWarConfig.RockMinSpeed, SpaceWarConfig.RockMaxSpeed);
            Vector2 way = Vector2.Normalize(MapCenterPos - newPos);
            Vector2 delta = RandomHelper.GetRandomVector2(-0.7f, 0.7f);
            Vector2 Vel = Vector2.Normalize(way + delta) * speed;

            float aziVel = RandomHelper.GetRandomFloat(0, SpaceWarConfig.RockMaxAziSpeed);
            float scale = RandomHelper.GetRandomFloat(0.4f,1.4f);
            int kind = RandomHelper.GetRandomInt(0,((int)RockTexNo.Max)-1);

            Rock newRock = new Rock("Rock" + rockCount, newPos, Vel, aziVel, scale, (RockTexNo)kind);
            sceneMgr.AddGameObj("rock", newRock);
            rocks.Add(newRock);

            rockCount++;
        }


        private void DeleteOutDateShells()
        {
            if (OutDateShellNames.Count != 0)
            {
                foreach (string name in OutDateShellNames)
                {
                    sceneMgr.DelGameObj("shell", name);
                }
                OutDateShellNames.Clear();
            }
        }

        private void UpdateCamera(float second)
        {
            if (InputHandler.IsKeyDown(Keys.Left))
                camera.Move(new Vector2(-5, 0));
            if (InputHandler.IsKeyDown(Keys.Right))
                camera.Move(new Vector2(5, 0));
            if (InputHandler.IsKeyDown(Keys.Up))
                camera.Move(new Vector2(0, -5));
            if (InputHandler.IsKeyDown(Keys.Down))
                camera.Move(new Vector2(0, 5));
            if (InputHandler.JustPressKey(Keys.V))
                camera.Zoom(-0.2f);
            if (InputHandler.JustPressKey(Keys.B))
                camera.Zoom(0.2f);
            if (InputHandler.IsKeyDown(Keys.T))
                camera.Rota(0.1f);
            if (InputHandler.IsKeyDown(Keys.R))
                camera.Rota(-0.1f);
            camera.Update(second);
        }

        #endregion
    }



}
