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
using System.Runtime.InteropServices;

namespace InterRules.Starwar
{
    [RuleAttribute("Starwar", "支持多人联机的空战规则", "编写组成员：...", 2009, 4, 8)]



    class StarwarRule : IGameRule
    {
        [StructLayoutAttribute(LayoutKind.Sequential,Size=42, CharSet = CharSet.Ansi, Pack = 1)]
        struct LoginData
        {
            public const int size = 42;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
            public char[] Name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
            public char[] Password;
        }


        
        public string RuleIntroduction
        {
            get { return "20090328~20090417:编写组成员：..."; }
        }

        public string RuleName
        {
            get { return "Starwar"; }
        }

       // Timer 

        Texture2D bgTexture;

        Rectangle bgRect;

        SpriteBatch spriteBatch;

        Textbox namebox, passbox;

        TextButton btnLogin, btnClear;


        public StarwarRule()
        {
            BaseGame.ShowMouse = true;


            bgTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.BgContent, "login"));

            bgRect = new Rectangle(0, 0, 800, 600);

            namebox = new Textbox("namebox", new Vector2(300, 400), 150, "", false);

            passbox = new Textbox("passbox", new Vector2(300, 430), 150, "", false);

            passbox.bStar = true;


            btnLogin = new TextButton("OkLogin", new Vector2(300, 480), "Login", 0, Color.Gold);
            btnClear = new TextButton("ClearBtn", new Vector2(385, 480), "Clear", 0, Color.Gold);

            SocketMgr.OnReceivePkg += new SocketMgr.ReceivePkgEventHandler(OnReceivePack);
            //SocketMgr.OnReceivePkg -= OnReceivePack;

            btnLogin.OnClick += new EventHandler(btnLogin_OnPress);
            btnClear.OnClick += new EventHandler(btnClear_OnPress);

            // 连接到服务器
            //SocketMgr.ConnectToServer(); 
        }

        void OnReceivePack(stPkgHead head, MemoryStream data)
        {
            if (head.iSytle == 11)
            {
                SocketMgr.OnReceivePkg -= OnReceivePack;
                GameManager.AddGameScreen(new Hall());
            }
        }

        void btnClear_OnPress(object sender, EventArgs e)
        {
            namebox = new Textbox("namebox", new Vector2(300, 400), 150, "", false);
            passbox = new Textbox("passbox", new Vector2(300, 430), 150, "", false);
            passbox.bStar = true;
        }

        void btnLogin_OnPress(object sender, EventArgs e)
        {



            LoginData data;

            data.Name = new char[21];
            char[] temp = namebox.text.ToCharArray();
            for (int i = 0; i < temp.Length; i++)
            {
                data.Name[i] = temp[i];
            }


            data.Password = new char[21];
            temp = passbox.text.ToCharArray();
            for (int i = 0; i < temp.Length; i++)
            {
                data.Password[i] = temp[i];
            }

            stPkgHead head = new stPkgHead();
            //head.iSytle = //包头类型还没初始化


            MemoryStream Stream = new MemoryStream();
            Stream.Write(SocketMgr.StructToBytes(data), 0, LoginData.size);
            head.dataSize = (int)Stream.Length;
            head.iSytle = 10;
            SocketMgr.Initial();
            SocketMgr.ConnectToServer();
            SocketMgr.SendCommonPackge(head, Stream);
            Stream.Close();

            SocketMgr.StartReceiveThread();
            //GameManager.AddGameScreen(new Hall());
        }

        #region IGameScreen 成员

        public bool Update(float second)
        {

            namebox.Update();
            passbox.Update();
            btnLogin.Update();
            btnClear.Update();


            if (InputHandler.IsKeyDown(Keys.F1))
                GameManager.AddGameScreen(new StarwarLogic(0));
            else if (InputHandler.IsKeyDown(Keys.F2))
                GameManager.AddGameScreen(new StarwarLogic(1));
            else if (InputHandler.IsKeyDown(Keys.PageDown))
            {
                SocketMgr.OnReceivePkg -= OnReceivePack;
                GameManager.AddGameScreen(new Hall());
            }
            
            if (InputHandler.IsKeyDown(Keys.Escape))
                return true;

            return false;
        }

        public void Render()
        {   
            BaseGame.Device.Clear(Color.LightSkyBlue);
            spriteBatch = (SpriteBatch)BaseGame.SpriteMgr.alphaSprite;
            spriteBatch.Draw(bgTexture, Vector2.Zero, bgRect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth.BackGround); 
            namebox.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            passbox.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            btnLogin.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            btnClear.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
        }

        public void OnClose()
        {

        }

        #endregion
    }


    


}
