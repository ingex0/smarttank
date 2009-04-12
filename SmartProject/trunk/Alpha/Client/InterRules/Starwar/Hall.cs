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
    class Hall : IGameScreen
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

        Texture2D bgTexture;

        Rectangle bgRect;

        SpriteBatch spriteBatch;

        Listbox roomList;
        Listbox rankList;

        Byte[] rankBuffer = new byte[400];

        TextButton btnCreate, btnEnter, btnRank;

        int selectIndexRank = -1;
        int selectIndexRoom = -1;

        public Hall()
        {
            BaseGame.ShowMouse = true;

            roomList = new Listbox("roomlist", new Vector2(30, 100), new Point(200, 350), Color.WhiteSmoke, Color.Green);

            rankList = new Listbox("ranklist", new Vector2(300, 100), new Point(450, 350), Color.WhiteSmoke, Color.Green);

            roomList.AddItem("Room 1");

            bgTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.BgContent, "login"));

            bgRect = new Rectangle(0, 0, 800, 600);



            btnCreate = new TextButton("CreateBtn", new Vector2(130, 460), "Create", 0, Color.Gold);
            btnEnter = new TextButton("EnterBtn", new Vector2(50, 460), "Enter", 0, Color.Gold);
            btnRank = new TextButton("RankBtn", new Vector2(650, 460), "Rank List", 0, Color.Gold);

            btnCreate.OnClick += new EventHandler(btnCreate_OnPress);
            btnEnter.OnClick += new EventHandler(btnEnter_OnPress);
            btnRank.OnClick += new EventHandler(btnRank_OnPress);

            rankList.OnChangeSelection += new EventHandler(rankList_OnChangeSelection);
            roomList.OnChangeSelection += new EventHandler(roomList_OnChangeSelection);

            // asoka 底层收包接口改了一下，直接返回byte[]。麻烦seek改一下解析包的逻辑了
            //SocketMgr.OnReceivePkg += new SocketMgr.ReceivePkgEventHandler(OnReceivePack);



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
            if (head.iSytle == 50)
            {
                RankInfo ri;
                string str;

                for (int i = 0; i < head.dataSize; i += 32)
                {
   
                    str = "";
                    data.Read(rankBuffer, 0, 32);

                    ri = (RankInfo)SocketMgr.BytesToStuct(rankBuffer, typeof(RankInfo));

                    for (int j = 0; ri.name[j] != '\0'; ++j)
                    {
                        str += ri.name[j];
                    }


                    rankList.AddItem(ri.rank + "        " + str + "              " + ri.score);

                }
            }
        }
        
        void roomList_OnChangeSelection(object sender, EventArgs e)
        {
            selectIndexRoom = roomList.selectedIndex;
        }

        void rankList_OnChangeSelection(object sender, EventArgs e)
        {
            selectIndexRank = rankList.selectedIndex;
        }

        void btnCreate_OnPress(object sender, EventArgs e)
        {
            
        }

        void btnEnter_OnPress(object sender, EventArgs e)
        {

        }

        void btnRank_OnPress(object sender, EventArgs e)
        {

        }

        #region IGameScreen 成员

        public bool Update(float second)
        {

            btnCreate.Update();
            btnEnter.Update();
            btnRank.Update();
            
            roomList.Update();
            rankList.Update();

            if (InputHandler.IsKeyDown(Keys.F1))
                GameManager.AddGameScreen(new StarwarLogic(0));
            else if (InputHandler.IsKeyDown(Keys.F2))
                GameManager.AddGameScreen(new StarwarLogic(1));
            else if (InputHandler.IsKeyDown(Keys.PageDown))
            {
                SocketMgr.OnReceivePkg -= OnReceivePack;
                GameManager.AddGameScreen(new Rank());
            }
            else if (InputHandler.IsKeyDown(Keys.PageUp))
            {
                SocketMgr.OnReceivePkg -= OnReceivePack;
                GameManager.AddGameScreen(new Room());
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
            roomList.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            rankList.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            btnEnter.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            btnCreate.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            btnRank.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            
        }

        public void OnClose()
        {

        }

        #endregion
    }





}
