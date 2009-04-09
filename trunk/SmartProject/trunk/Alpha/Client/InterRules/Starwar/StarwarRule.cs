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
using SmartTank.Draw.UI.Controls;
using SmartTank.net;
using System.Runtime.InteropServices;

namespace InterRules.Starwar
{
    [RuleAttribute("Starwar", "支持多人联机的空战规则", "编写组成员：...", 2009, 4, 8)]



    class StarwarRule : IGameRule
    {
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct LoginData
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
            public string Name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
            public string Password;
        }
        
        public string RuleIntroduction
        {
            get { return "20090328~20090417:编写组成员：..."; }
        }

        public string RuleName
        {
            get { return "Starwar"; }
        }

        Texture2D bgTexture;

        Rectangle bgRect;

        SpriteBatch spriteBatch;

        Textbox namebox, passbox;

        TextButton btnOK;

        int selectIndex = -1;

        public StarwarRule()
        {
            BaseGame.ShowMouse = true;


            bgTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.BgContent, "login"));

            bgRect = new Rectangle(0, 0, 800, 600);

            

            namebox = new Textbox("namebox", new Vector2(100, 200), 100, "", false);

            passbox = new Textbox("namebox", new Vector2(100, 300), 100, "", false);


            btnOK = new TextButton("OkBtn", new Vector2(700, 500), "Begin", 0, Color.Blue);
            btnOK.OnClick += new EventHandler(btnOK_OnPress);
        }

        void btnOK_OnPress(object sender, EventArgs e)
        {



            LoginData data;

            data.Name = namebox.text;
            data.Password = passbox.text;




        }

        #region IGameScreen 成员

        public bool Update(float second)
        {
            
            namebox.Update();
            passbox.Update();
            btnOK.Update();

            //if (InputHandler.IsKeyDown(Keys.L))
            //    GameManager.AddGameScreen(new StarwarLogic());

            if (InputHandler.IsKeyDown(Keys.Escape))
                return true;

            return false;
        }

        public void Render()
        {
            //Color dynamicColor = new Color(new Vector4(color.ToVector3().X, color.ToVector3().Y, color.ToVector3().Z, alpha));
            
            BaseGame.Device.Clear(Color.LightSkyBlue);

            spriteBatch = (SpriteBatch)BaseGame.SpriteMgr.alphaSprite;

            spriteBatch.Draw(bgTexture, Vector2.Zero, bgRect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth.BackGround); 

            //spriteBatch.Draw(bgTexture, Vector2.Zero, Color.White);
            //spriteBatch.Draw( parts[1], middlePartRect, dynamicColor );
            //spriteBatch.Draw( parts[2], position + new Vector2( parts[0].Width + middlePartRect.Width, 0f ), dynamicColor );
            
            //spriteBatch.Draw(bgTexture, Vector2.Zero, 
            //spriteBatch.Draw(
            
            //spriteBatch.Draw(bgTexture, Vector2.Zero, bgRect, Color.White, Vector2.Zero, new Vector2(0, 0), SpriteEffects.None, LayerDepth.BackGround);
            //spriteBatch.Draw(bgTexture, bgRect, bgRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, LayerDepth.BackGround);
            namebox.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            passbox.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            btnOK.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
        }

        public void OnClose()
        {

        }

        #endregion
    }


    class StarwarLogic : RuleSupNet, IGameScreen
    {
        const float AddObjSpace = 250;

        WarShip[] ships = new WarShip[6];
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

        public Vector2 MapCenterPos
        {
            get { return new Vector2(mapRect.X + mapRect.Width / 2, mapRect.Y + mapRect.Height / 2); }
        }



        void Gold_OnLiveTimeOut(Gold sender)
        {
            SetGoldNewPos(sender);
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
                ship.Vel = result.NormalVector * SpaceWarConfig.ShellSpeed;
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
        }

        private Vector2 CalMirrorVel(Vector2 curVel, Vector2 mirrorVertical)
        {
            float mirVecLength = Vector2.Dot(curVel, mirrorVertical);
            Vector2 horizVel = curVel - mirVecLength * mirrorVertical;
            Vector2 newVel = horizVel + Math.Abs(mirVecLength) * mirrorVertical;
            return newVel;
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
            for (int i = 0; i < 1; i++)
            {
                ships[i] = new WarShip("ship" + 0, new Vector2(500, 500), 0, true);
                ships[i].OnCollied += new OnCollidedEventHandler(WarShip_OnCollied);
                ships[i].OnOverLap += new OnCollidedEventHandler(Warship_OnOverLap);
                ships[i].OnShoot += new WarShip.WarShipShootEventHandler(WarShip_OnShoot);
                ships[i].OnDead += new WarShip.WarShipDeadEventHandler(Warship_OnDead);
            }
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
            sceneMgr.PhiGroups.Add("rock");
            sceneMgr.AddColMulGroups("warship", "shell", "border");
            sceneMgr.AddColMulGroups("warship", "shell", "rock");
            sceneMgr.ColSinGroups.Add("warship");
            sceneMgr.ColSinGroups.Add("rock");

            sceneMgr.AddLapMulGroups("warship", "gold");

            sceneMgr.AddGameObj("warship", ships[0]);
            sceneMgr.AddGameObj("border", new SmartTank.PhiCol.Border(mapRect));
            sceneMgr.AddGameObj("gold", gold);

            GameManager.LoadScene(sceneMgr);

            camera.Focus(ships[0], false);
        }



        private void InitialCamera()
        {
            BaseGame.CoordinMgr.SetScreenViewRect(new Rectangle(0, 0, 800, 600));

            camera = new Camera(1f, new Vector2(400, 300), 0);
            camera.maxScale = 4.5f;
            camera.minScale = 0.5f;
            camera.Enable();
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

        #region IGameScreen 成员

        void IGameScreen.OnClose()
        {

        }

        void IGameScreen.Render()
        {
            backGround.Draw();
            GameManager.DrawManager.Draw();
            BaseGame.BasicGraphics.DrawRectangle(mapRect, 3, Color.Red, 0f);


            GameManager.RenderEngine.FontMgr.DrawInScrnCoord("Ship1 Live: " + ships[0].HP + "  Score: " + ships[0].Score, new Vector2(10, 500), 1.0f, Color.White, LayerDepth.Text, GameFonts.Comic);
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

            Rock newRock = new Rock("Rock" + rockCount, newPos, Vel, aziVel, scale, (RockTexNo)kind);

            newRock.OnCollided += new OnCollidedEventHandler(Rock_OnCollided);
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
