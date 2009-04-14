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
using System.Runtime.InteropServices;

namespace InterRules.Starwar
{
    class StarwarLogic : RuleSupNet, IGameScreen
    {
        [StructLayoutAttribute(LayoutKind.Sequential, Size = 29, CharSet = CharSet.Ansi, Pack = 1)]
        struct RankInfo
        {
            int Rank;
            int Score;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
            char[] Name;
        };



        #region Variables

        const float AddObjSpace = 250;

        WarShip[] ships;
        Gold gold;
        List<Rock> rocks = new List<Rock>();
        int rockCount = 0;
        List<string> OutDateShellNames = new List<string>();

        Rectanglef mapRect = new Rectanglef(0, 0, 1600, 1200);
        Camera camera;

        Sprite backGround;

        int shellcount = 0;

        Rectanglef rockDelRect = new Rectanglef(-400, -400, 2400, 2000);
        float rockTimer = 0;

        int controlIndex = -1;

        float gameTotolTime = -1;

        public Vector2 MapCenterPos
        {
            get { return new Vector2(mapRect.X + mapRect.Width / 2, mapRect.Y + mapRect.Height / 2); }
        }

        string[] playerNames;

        #endregion

        #region HandlerObjEvent

        void Gold_OnLiveTimeOut(Gold sender)
        {
            SetGoldNewPos(sender);
        }

        void Warship_OnDead(WarShip sender)
        {
            //sceneMgr.DelGameObj(sender.MgPath);
            if (PurviewMgr.IsMainHost)
            {
                while (true)
                {
                    Vector2 newPos = RandomHelper.GetRandomVector2(0, 1);
                    newPos.X *= mapRect.Width;
                    newPos.X += mapRect.X;
                    newPos.Y *= mapRect.Height;
                    newPos.Y += mapRect.Y;
                    if (CanAddObjAtPos(newPos))
                    {
                        (sender as WarShip).Born(newPos);
                        SyncCasheWriter.SubmitUserDefineInfo("WarshipBorn", "", sender, newPos);
                        break;
                    }
                }
            }
        }

        void WarShip_OnShoot(WarShip firer, Vector2 endPoint, float azi)
        {
            if (PurviewMgr.IsMainHost)
            {
                WarShipShell shell = new WarShipShell("shell" + shellcount, firer, endPoint, azi);
                shell.onCollided += new OnCollidedEventHandler(Shell_onCollided);
                shell.OnOutDate += new WarShipShell.ShellOutDateEventHandler(Shell_OnOutDate);
                sceneMgr.AddGameObj("shell", shell);


                SyncCasheWriter.SubmitCreateObjMg("shell", typeof(WarShipShell), "shell" + shellcount, firer, endPoint, azi);
                shellcount++;
            }
        }

        void Shell_OnOutDate(WarShipShell sender, IGameObj shooter)
        {
            OutDateShellNames.Add(sender.Name);
        }

        void Shell_onCollided(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {

            if (objB.ObjClass == "WarShip")
            {
                if (PurviewMgr.IsMainHost)
                {
                    WarShipShell shell = Sender as WarShipShell;
                    WarShip firer = shell.Firer as WarShip;
                    if (objB.Script != firer.ObjInfo.Script)
                    {
                        (shell.Firer as WarShip).Score += SpaceWarConfig.ScoreByHit;
                        SyncShipScoreHp(shell.Firer as WarShip, true);
                    }
                }

                sceneMgr.DelGameObj("shell", Sender.Name);
                new ShellExplodeBeta(Sender.Pos, ((ShellNormal)Sender).Azi);

                Quake.BeginQuake(10, 50);
                Sound.PlayCue("EXPLO1");
            }
            else
            {
                WarShipShell shell = (WarShipShell)Sender;
                shell.MirrorPath(result);

                //
                //BroadcastObjPhiStatus(shell, true);
            }

        }

        void WarShip_OnCollied(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            if (objB.ObjClass == "WarShipShell")
            {
                WarShip ship = (WarShip)Sender;
                ship.BeginStill();
                if (PurviewMgr.IsMainHost)
                    ship.HitByShell();
                ship.Vel = result.NormalVector * SpaceWarConfig.ShellSpeed;

                SyncShipScoreHp(ship, false);
            }
            else if (objB.ObjClass == "WarShip")
            {
                WarShip ship = (WarShip)Sender;
                Vector2 newVel = CalMirrorVel(ship.Vel, result.NormalVector);
                ship.Vel = newVel;
                ship.BeginStill();
            }
            else if (objB.ObjClass == "Rock")
            {
                WarShip ship = (WarShip)Sender;
                Vector2 newVel = CalMirrorVel(ship.Vel, result.NormalVector);
                ship.Vel = newVel;
                ship.BeginStill();
            }
        }

        void Warship_OnOverLap(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            if (objB.ObjClass == "Gold")
            {
                WarShip ship = Sender as WarShip;
                ship.Score += SpaceWarConfig.GoldScore;

                SyncShipScoreHp(ship, true);
            }
        }

        private void SyncShipScoreHp(WarShip ship, bool subScore)
        {
            if (PurviewMgr.IsMainHost)
            {
                if (subScore)
                    SyncCasheWriter.SubmitUserDefineInfo("Score", "", ship, ship.Score);
                else
                    SyncCasheWriter.SubmitUserDefineInfo("Hp", "", ship, ship.HP);
            }
        }

        void Gold_OnOverLap(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            SetGoldNewPos(Sender as Gold);
        }

        void Rock_OnCollided(IGameObj Sender, CollisionResult result, GameObjInfo objB)
        {
            if (objB.ObjClass == "WarShip")// || objB.ObjClass == "Rock")
            {
                Vector2 newVel = CalMirrorVel((Sender as Rock).Vel, result.NormalVector);
                (Sender as Rock).Vel = newVel;
            }
            else if (objB.ObjClass == "Rock")
            {
                Vector2 newVel = CalMirrorVel((Sender as Rock).Vel, result.NormalVector);
                (Sender as Rock).Vel = newVel;
            }

            //BroadcastObjPhiStatus(Sender, true);
        }

        void SyncCasheReader_onUserDefineInfo(string infoName, string infoID, object[] args)
        {
            if (infoName == "WarshipBorn")
            {
                WarShip ship = args[0] as WarShip;
                if (!PurviewMgr.IsMainHost)
                    ship.Dead();
                ship.Born((Vector2)args[1]);
            }
            else if (infoName == "ObjPhiCollide")
            {
                if (args[0] != null)
                    ((NonInertiasPhiUpdater)((args[0] as IPhisicalObj).PhisicalUpdater))
                        .SetServerStatue((Vector2)args[1], (Vector2)args[2], (float)args[3], (float)args[4], phiSyncTime, true);
            }
            else if (infoName == "ObjPhi")
            {
                if (args[0] != null
                    && args[0] != ships[controlIndex])
                    ((NonInertiasPhiUpdater)((args[0] as IPhisicalObj).PhisicalUpdater))
                        .SetServerStatue((Vector2)args[1], (Vector2)args[2], (float)args[3], (float)args[4], phiSyncTime, false);
            }
            else if (infoName == "Score")
            {
                if (args[0] != null)
                {
                    WarShip ship = args[0] as WarShip;
                    ship.Score = (int)args[1];
                }
            }
            else if (infoName == "Hp")
            {
                if (args[0] != null)
                {
                    WarShip ship = args[0] as WarShip;
                    ship.HP = (int)args[1];
                }
            }
            else if (infoName == "GoldBorn")
            {
                if (args[0] != null)
                {
                    (args[0] as Gold).Born((Vector2)args[1]);
                }
            }
        }

        void SyncCasheReader_onCreateObj(IGameObj obj)
        {
            if (obj is WarShipShell)
            {
                WarShipShell shell = obj as WarShipShell;
                shell.onCollided += new OnCollidedEventHandler(Shell_onCollided);
                shell.OnOutDate += new WarShipShell.ShellOutDateEventHandler(Shell_OnOutDate);
                shellcount++;
            }
            else if (obj is Rock)
            {
                Rock newRock = obj as Rock;
                newRock.OnCollided += new OnCollidedEventHandler(Rock_OnCollided);
                rocks.Add(newRock);
                rockCount++;
            }
        }



        private void SyncWarShip()
        {
            if (PurviewMgr.IsMainHost)
            {
                for (int i = 0; i < ships.Length; i++)
                {
                    BroadcastObjPhiStatus(ships[i], false);
                }
            }
            else
            {
                BroadcastObjPhiStatus(ships[controlIndex], false);
            }


        }

        private void BroadcastObjPhiStatus(IGameObj obj, bool collide)
        {
            if (obj is IPhisicalObj)
            {
                if (collide)
                {
                    NonInertiasPhiUpdater phiUpdater = (NonInertiasPhiUpdater)((IPhisicalObj)obj).PhisicalUpdater;
                    SyncCasheWriter.SubmitUserDefineInfo("ObjPhiCollide",
                        obj.ObjInfo.ObjClass, obj, phiUpdater.Pos, phiUpdater.Vel, phiUpdater.Azi, phiUpdater.AngVel);
                }
                else
                {
                    NonInertiasPhiUpdater phiUpdater = (NonInertiasPhiUpdater)((IPhisicalObj)obj).PhisicalUpdater;
                    SyncCasheWriter.SubmitUserDefineInfo("ObjPhi",
                        obj.ObjInfo.ObjClass, obj, phiUpdater.Pos, phiUpdater.Vel, phiUpdater.Azi, phiUpdater.AngVel);
                }
            }
        }

        private Vector2 CalMirrorVel(Vector2 curVel, Vector2 mirrorVertical)
        {
            float mirVecLength = Vector2.Dot(curVel, mirrorVertical);
            Vector2 horizVel = curVel - mirVecLength * mirrorVertical;
            Vector2 newVel = horizVel + Math.Abs(mirVecLength) * mirrorVertical;
            return newVel;
        }

        private void SetGoldNewPos(Gold Sender)
        {
            if (PurviewMgr.IsMainHost)
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
                        SyncCasheWriter.SubmitUserDefineInfo("GoldBorn", "", Sender, Sender.Pos);
                        break;
                    }
                }
            }
        }

        private bool CanAddObjAtPos(Vector2 pos)
        {
            foreach (WarShip ship in ships)
            {
                if (ship == null)
                    continue;

                if (Math.Abs(ship.Pos.X - pos.X) < AddObjSpace
                    && Math.Abs(ship.Pos.Y - pos.Y) < AddObjSpace)
                    return false;
            }

            foreach (Rock rock in rocks)
            {
                if (Math.Abs(rock.Pos.X - pos.X) < AddObjSpace
                    && Math.Abs(rock.Pos.Y - pos.Y) < AddObjSpace)
                    return false;
            }

            return true;
        }


        #endregion

        #region Initailize

        public StarwarLogic(int controlIndex, string[] playerNames)
        {
            this.controlIndex = controlIndex;

            this.playerNames = playerNames;

            LoadConfig();

            StartTimer();

            InitialCamera();
            InitializeScene();
            InitialBackGround();
            InitailizePurview(controlIndex);

            SyncCasheReader.onCreateObj += new SyncCasheReader.CreateObjInfoHandler(SyncCasheReader_onCreateObj);
            SyncCasheReader.onUserDefineInfo += new SyncCasheReader.UserDefineInfoHandler(SyncCasheReader_onUserDefineInfo);
        }

        private void StartTimer()
        {
            gameTotolTime = SpaceWarConfig.GameTotolTime;
        }

        private void InitailizePurview(int controlIndex)
        {
            if (controlIndex == 0)
            {
                PurviewMgr.IsMainHost = true;
                for (int i = 0; i < ships.Length; i++)
                {
                    if (i != controlIndex)
                        PurviewMgr.RegistSlaveMgObj(ships[i].MgPath);
                }
            }
            else
            {
                PurviewMgr.IsMainHost = false;
                PurviewMgr.RegistSlaveMgObj(ships[controlIndex].MgPath);
            }


        }

        const float phiSyncTime = 0.1f;

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
            sceneMgr = new SceneMgr();

            sceneMgr.AddGroup("", new TypeGroup<WarShip>("warship"));
            sceneMgr.AddGroup("", new TypeGroup<WarShipShell>("shell"));
            sceneMgr.AddGroup("", new TypeGroup<SmartTank.PhiCol.Border>("border"));
            sceneMgr.AddGroup("", new TypeGroup<Gold>("gold"));
            sceneMgr.AddGroup("", new TypeGroup<Rock>("rock"));

            sceneMgr.PhiGroups.Add("warship");
            sceneMgr.PhiGroups.Add("shell");
            sceneMgr.PhiGroups.Add("rock");
            sceneMgr.AddColMulGroups("warship", "shell", "border");
            sceneMgr.AddColMulGroups("warship", "shell", "rock");
            sceneMgr.ColSinGroups.Add("warship");
            sceneMgr.ColSinGroups.Add("rock");

            sceneMgr.AddLapMulGroups("warship", "gold");

            ships = new WarShip[playerNames.Length];
            for (int i = 0; i < playerNames.Length; i++)
            {
                bool openControl = false;
                if (i == controlIndex)
                    openControl = true;

                ships[i] = new WarShip("ship" + i, new Vector2(200 + i * 100, 500), 0, openControl);
                ships[i].OnCollied += new OnCollidedEventHandler(WarShip_OnCollied);
                ships[i].OnOverLap += new OnCollidedEventHandler(Warship_OnOverLap);
                ships[i].OnShoot += new WarShip.WarShipShootEventHandler(WarShip_OnShoot);
                ships[i].OnDead += new WarShip.WarShipDeadEventHandler(Warship_OnDead);
                ships[i].PlayerName = playerNames[i];
                sceneMgr.AddGameObj("warship", ships[i]);
            }

            gold = new Gold("gold", new Vector2(800, 600), 0);
            gold.OnOverLap += new OnCollidedEventHandler(Gold_OnOverLap);
            gold.OnLiveTimeOut += new Gold.GoldLiveTimeOutEventHandler(Gold_OnLiveTimeOut);

            sceneMgr.AddGameObj("border", new SmartTank.PhiCol.Border(mapRect));
            sceneMgr.AddGameObj("gold", gold);

            GameManager.LoadScene(sceneMgr);

            camera.Focus(ships[controlIndex], false);
        }

        private void InitialCamera()
        {
            BaseGame.CoordinMgr.SetScreenViewRect(new Rectangle(0, 0, 800, 600));

            camera = new Camera(1f, new Vector2(400, 300), 0);
            camera.maxScale = 4.5f;
            camera.minScale = 0.5f;
            camera.Enable();
        }

        #endregion

        #region IGameScreen ³ÉÔ±

        void IGameScreen.OnClose()
        {
            base.OnClose();
        }

        void IGameScreen.Render()
        {
            BaseGame.Device.Clear(Color.Black);
            backGround.Draw();
            GameManager.DrawManager.Draw();
            BaseGame.BasicGraphics.DrawRectangle(mapRect, 3, Color.White, LayerDepth.TankBase);


            DrawScore();
        }

        private void DrawScore()
        {
            for (int i = 0; i < ships.Length; i++)
            {
                //GameManager.RenderEngine.FontMgr.DrawInScrnCoord(" Live: " + ships[i].HP, new Vector2(50 + 120 * i, 500), 0.4f, Color.White, LayerDepth.Text, GameFonts.Comic);
                GameManager.RenderEngine.FontMgr.DrawInScrnCoord(ships[i].PlayerName + " Score: " + ships[i].Score, new Vector2(50 + 120 * i, 550), 0.4f, Color.White, LayerDepth.Text, GameFonts.Comic);
            }
            GameManager.RenderEngine.BasicGrahpics.FillRectangleInScrn(new Rectangle(50, 50, ships[controlIndex].HP / 3, 20), Color.Red, LayerDepth.UI, SpriteBlendMode.AlphaBlend);
        }

        //Vector2 serPos = new Vector2(400, 400);
        //Vector2 serVel = new Vector2(100, 0);

        bool IGameScreen.Update(float second)
        {
            base.Update(second);

            UpdateCamera(second);

            DeleteOutDateShells();

            CreateDelRock(second);

            UpdateTimer(second);

            //SyncWarShip();
            //if (InputHandler.IsKeyDown(Keys.U))
            //{
            //    ((NonInertiasPhiUpdater)ships[0].PhisicalUpdater).SetServerStatue(serPos, serVel, 0, 0, 10);

            //}

            if (InputHandler.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                GameManager.ComponentReset();
                return true;
            }
            else
                return false;
        }

        private bool UpdateTimer(float second)
        {
            gameTotolTime -= second;
            if (gameTotolTime < 0)
            {
                GameOver();
            }
            return false;
        }

        private void GameOver()
        {
            stPkgHead head = new stPkgHead();
            head.iSytle = 80;

            //for(int i = 0; i < )
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
                if (rock.Pos.X < rockDelRect.X
                    || rock.Pos.X > rockDelRect.X + rockDelRect.Width
                    || rock.Pos.Y < rockDelRect.Y
                    || rock.Pos.Y > rockDelRect.Y + rockDelRect.Height)
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
            if (PurviewMgr.IsMainHost)
            {
                Vector2 newPos = new Vector2();
                while (true)
                {
                    newPos = RandomHelper.GetRandomVector2(-100, 100);

                    int posType = RandomHelper.GetRandomInt(0, 2);

                    if (posType == 0)
                    {
                        if (newPos.X < 0)
                            newPos.X -= mapRect.X + AddObjSpace;
                        else
                            newPos.X += mapRect.Width + mapRect.X + AddObjSpace;

                        if (newPos.Y < 0)
                            newPos.Y -= mapRect.Y + AddObjSpace;
                        else
                            newPos.Y += mapRect.Y + mapRect.Height + AddObjSpace;
                    }
                    else if (posType == 1)
                    {
                        newPos.X *= mapRect.Width / 200;
                        newPos.X += mapRect.Width / 2 + mapRect.X;

                        if (newPos.Y < 0)
                            newPos.Y -= mapRect.Y + AddObjSpace;
                        else
                            newPos.Y += mapRect.Y + mapRect.Height + AddObjSpace;
                    }
                    else if (posType == 2)
                    {
                        newPos.Y *= mapRect.Height / 200;
                        newPos.Y += mapRect.Y + mapRect.Height / 2;

                        if (newPos.X < 0)
                            newPos.X -= mapRect.X + AddObjSpace;
                        else
                            newPos.X += mapRect.Width + mapRect.X + AddObjSpace;

                    }
                    if (CanAddObjAtPos(newPos))
                        break;

                }
                float speed = RandomHelper.GetRandomFloat(SpaceWarConfig.RockMinSpeed, SpaceWarConfig.RockMaxSpeed);
                Vector2 way = Vector2.Normalize(MapCenterPos - newPos);
                Vector2 delta = RandomHelper.GetRandomVector2(-0.7f, 0.7f);
                Vector2 Vel = Vector2.Normalize(way + delta) * speed;

                float aziVel = RandomHelper.GetRandomFloat(0, SpaceWarConfig.RockMaxAziSpeed);
                float scale = RandomHelper.GetRandomFloat(0.4f, 1.4f);
                int kind = RandomHelper.GetRandomInt(0, ((int)RockTexNo.Max) - 1);

                Rock newRock = new Rock("Rock" + rockCount, newPos, Vel, aziVel, scale, kind);

                newRock.OnCollided += new OnCollidedEventHandler(Rock_OnCollided);
                sceneMgr.AddGameObj("rock", newRock);

                rocks.Add(newRock);
                SyncCasheWriter.SubmitCreateObjMg("rock", typeof(Rock), "Rock" + rockCount, newPos, Vel, aziVel, scale, kind);

                rockCount++;

            }
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
