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



    class Hall : IGameRule
    {
        [StructLayoutAttribute(LayoutKind.Sequential, Size = 32, CharSet = CharSet.Ansi, Pack = 1)]
        struct RankInfo
        {
            public const int size = 32;

            public int rank;
            public int score;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public char[] name;
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

        Listbox roomList;
        Listbox rankList;

        Byte[] rankBuffer = new byte[400];

        TextButton btnOK;

        int selectIndex = -1;

        public Hall()
        {
            BaseGame.ShowMouse = true;

            roomList = new Listbox("roomlist", new Vector2(20, 50), new Point(200, 400), Color.WhiteSmoke, Color.Green);

            rankList = new Listbox("roomlist", new Vector2(280, 50), new Point(400, 500), Color.WhiteSmoke, Color.Green);

            roomList.AddItem("Room 1");

            bgTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.BgContent, "login"));

            bgRect = new Rectangle(0, 0, 800, 600);




            btnOK = new TextButton("OkBtn", new Vector2(700, 500), "Begin", 0, Color.Blue);
            btnOK.OnClick += new EventHandler(btnOK_OnPress);

            rankList.OnChangeSelection += new EventHandler(rankList_OnChangeSelection);
            roomList.OnChangeSelection += new EventHandler(roomList_OnChangeSelection);

            SocketMgr.OnReceivePkg += new SocketMgr.ReceivePkgEventHandler(OnReceivePack);



            stPkgHead head = new stPkgHead();
            //head.iSytle = //包头类型还没初始化


            MemoryStream Stream = new MemoryStream();
            Stream.Write(new byte[1], 0, 1);
            head.dataSize = 1;
            head.iSytle = 50;
            SocketMgr.SendCommonPackge(head, Stream);
            Stream.Close();

            // 连接到服务器
            //SocketMgr.ConnectToServer();
        }
        
        void OnReceivePack(stPkgHead head, MemoryStream data)
        {
            string tmp;
            
            if (head.iSytle == 50)
            {
                RankInfo ri;
                tmp = "";
                for (int i = 0; i < head.dataSize; i += 32)
                {
                    

                    data.Read(rankBuffer, i, 32);

                    ri = (RankInfo)SocketMgr.BytesToStuct(rankBuffer, typeof(RankInfo));
                    tmp = ri.rank + "        " + ri.score;
                    rankList.AddItem(tmp);

                }
            }
        }
        
        void roomList_OnChangeSelection(object sender, EventArgs e)
        {
            selectIndex = roomList.selectedIndex;
        }

        void rankList_OnChangeSelection(object sender, EventArgs e)
        {
            selectIndex = roomList.selectedIndex;
        }

        void btnOK_OnPress(object sender, EventArgs e)
        {


            //if (selectIndex >= 0 && selectIndex <= rulesList.Items.Count)
            {
                // GameManager.ComponentReset();
                // GameManager.AddGameScreen(RuleLoader.CreateRuleInstance(selectIndex));
            }

        }

        #region IGameScreen 成员

        public bool Update(float second)
        {

            btnOK.Update();
            roomList.Update();
            rankList.Update();

            if (InputHandler.IsKeyDown(Keys.L))
                GameManager.AddGameScreen(new StarwarLogic());

            if (InputHandler.IsKeyDown(Keys.Escape))
                return true;

            return false;
        }

        public void Render()
        {
            BaseGame.Device.Clear(Color.LightSkyBlue);
            spriteBatch = (SpriteBatch)BaseGame.SpriteMgr.alphaSprite;
            spriteBatch.Draw(bgTexture, Vector2.Zero, bgRect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth.BackGround);
            roomList.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            rankList.Draw(BaseGame.SpriteMgr.alphaSprite, 1);

            btnOK.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
        }

        public void OnClose()
        {

        }

        #endregion
    }





}
