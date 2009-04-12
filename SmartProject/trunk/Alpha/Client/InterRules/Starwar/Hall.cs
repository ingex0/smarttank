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
        [StructLayoutAttribute(LayoutKind.Sequential, Size = 36, CharSet = CharSet.Ansi, Pack = 1)]
        struct RoomInfo
        {
            public const int size = 36;

            public int id;
            public int players;
            public int bBegin;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public char[] name;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, Size = 32, CharSet = CharSet.Ansi, Pack = 1)]
        struct UserInfo
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

        TextButton btnCreate, btnEnter, btnRank, btnRefresh, btnStart;

        stPkgHead head;
        MemoryStream Stream;

        List<Label> players;

        int selectIndexRank = -1;
        int selectIndexRoom = -1;

        bool bInRoom, bIsHost;

        public Hall()
        {
            BaseGame.ShowMouse = true;

            roomList = new Listbox("roomlist", new Vector2(30, 100), new Point(200, 350), Color.White, Color.Green);

            rankList = new Listbox("ranklist", new Vector2(300, 100), new Point(450, 350), Color.White, Color.Green);


            bgTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.BgContent, "login"));

            bgRect = new Rectangle(0, 0, 800, 600);


            btnRefresh = new TextButton("RefreshBtn", new Vector2(130, 460), "Refresh", 0, Color.Gold);
            btnCreate = new TextButton("CreateBtn", new Vector2(310, 460), "Create a new room", 0, Color.Gold);
            btnEnter = new TextButton("EnterBtn", new Vector2(50, 460), "Enter", 0, Color.Gold);
            btnRank = new TextButton("RankBtn", new Vector2(650, 460), "Rank List", 0, Color.Gold);
            btnStart = new TextButton("StartBtn", new Vector2(550, 390), "Start", 0, Color.Gold);

            btnRefresh.OnClick += new EventHandler(btnRefresh_OnPress);
            btnCreate.OnClick += new EventHandler(btnCreate_OnPress);
            btnEnter.OnClick += new EventHandler(btnEnter_OnPress);
            btnRank.OnClick += new EventHandler(btnRank_OnPress);
            btnStart.OnClick += new EventHandler(btnStart_OnPress);

            rankList.OnChangeSelection += new EventHandler(rankList_OnChangeSelection);
            roomList.OnChangeSelection += new EventHandler(roomList_OnChangeSelection);

            SocketMgr.OnReceivePkg += new SocketMgr.ReceivePkgEventHandler(OnReceivePack);



            head = new stPkgHead();
            Stream = new MemoryStream();
            head.dataSize = 0;
            head.iSytle = 33;
            SocketMgr.SendCommonPackge(head, Stream);
            Stream.Close();
            players = new List<Label>();
            bInRoom = false;

        }
        
        void OnReceivePack(stPkgHead head, byte[] data)
        {
            
            if (head.iSytle == 33)
            {
                //刷房间列表成功
                string str;
                RoomInfo room;
                byte[] tmpData;
                roomList.Clear();

                tmpData = new byte[head.dataSize];

                for (int i = 0; i < head.dataSize; i += 36)
                {

                    str = "";
                    //data.Read(roomBuffer, 0, 32);

                    for (int k = 0; k < 36; ++k)
                    {
                        tmpData[k] = data[i + k];
                    }

                    room = (RoomInfo)SocketMgr.BytesToStuct(tmpData, typeof(RoomInfo));

                    for (int j = 0; room.name[j] != '\0'; ++j)
                    {
                        str += room.name[j];
                    }


                    roomList.AddItem("room 1" + " ( " + room.players + " / 6 )", room.id);

                }
            }
            else if (head.iSytle == 11)
            {
                //创建房间成功
                players.Clear();
                head = new stPkgHead();
                Stream = new MemoryStream();
                head.dataSize = 0;
                head.iSytle = 33;
                SocketMgr.SendCommonPackge(head, Stream);
                Stream.Close();
                bInRoom = true;
                bIsHost = true;

                head = new stPkgHead();
                Stream = new MemoryStream();
                head.dataSize = 0;
                head.iSytle = 34;
                SocketMgr.SendCommonPackge(head, Stream);
                Stream.Close();
                
 
            }
            else if (head.iSytle == 37)
            {
                //加入房间成功
                players.Clear();
                head = new stPkgHead();
                Stream = new MemoryStream();
                head.dataSize = 0;
                head.iSytle = 33;
                SocketMgr.SendCommonPackge(head, Stream);
                Stream.Close();
                bInRoom = true;

                head = new stPkgHead();
                Stream = new MemoryStream();
                head.dataSize = 0;
                head.iSytle = 34;
                SocketMgr.SendCommonPackge(head, Stream);
                Stream.Close();
 
            }
            else if (head.iSytle == 34)
            {
                //列举用户信息


                string str;
                UserInfo player;
                byte[] tmpData;

                

                tmpData = new byte[head.dataSize];

                for (int i = 0; i < head.dataSize; i += 32)
                {

                    str = "";
                    //data.Read(roomBuffer, 0, 32);

                    for (int k = 0; k < 32; ++k)
                    {
                        tmpData[k] = data[i + k];
                    }

                    player = (UserInfo)SocketMgr.BytesToStuct(tmpData, typeof(UserInfo));

                    for (int j = 0; player.name[j] != '\0'; ++j)
                    {
                        str += player.name[j];
                    }

                    players.Add(new Label(str, new Vector2(300, 100 + i * 20), str, Color.Black, 1));//, Font font)

                    //roomList.AddItem("room 1" + " ( " + room.players + " / 6 )", room.id);

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
            head = new stPkgHead();
            //head.iSytle = //包头类型还没初始化


            Stream = new MemoryStream();
            head.dataSize = 0;
            head.iSytle = 30;
            SocketMgr.SendCommonPackge(head, Stream);
            Stream.Close();
        }

        void btnStart_OnPress(object sender, EventArgs e)
        {

        }

        void btnEnter_OnPress(object sender, EventArgs e)
        {
            if (roomList.selectedIndex == -1)
                return;

            head = new stPkgHead();
            byte[] roomid;

            //roomid = SocketMgr.StructToBytes(roomList.MyIDs[roomList.selectedIndex]);

            MemoryStream Stream = new MemoryStream();
            //Stream.Write(roomid, 0, 4);
            //head.dataSize = 4;
            head.dataSize = 0;
            head.iSytle = 31;
            SocketMgr.SendCommonPackge(head, Stream);
            Stream.Close();
        }

        void btnRefresh_OnPress(object sender, EventArgs e)
        {
            head = new stPkgHead();
            Stream = new MemoryStream();
            head.dataSize = 0;
            head.iSytle = 33;
            SocketMgr.SendCommonPackge(head, Stream);
            Stream.Close();
        }

        void btnRank_OnPress(object sender, EventArgs e)
        {
            //SocketMgr.OnReceivePkg -= OnReceivePack;
            GameManager.AddGameScreen(new Rank());
        }

        #region IGameScreen 成员

        public bool Update(float second)
        {
            btnRefresh.Update();
            btnCreate.Update();
            btnEnter.Update();
            btnRank.Update();
            if (bInRoom && bIsHost)
                btnStart.Update();
            
            
            roomList.Update();
            rankList.Update();

            if (InputHandler.IsKeyDown(Keys.F1))
                GameManager.AddGameScreen(new StarwarLogic(0));
            else if (InputHandler.IsKeyDown(Keys.F2))
                GameManager.AddGameScreen(new StarwarLogic(1));
            else if (InputHandler.IsKeyDown(Keys.PageDown))
            {
                //SocketMgr.OnReceivePkg -= OnReceivePack;
                GameManager.AddGameScreen(new Rank());
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
            btnRefresh.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            if (bInRoom)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].Draw(BaseGame.SpriteMgr.alphaSprite, 1);
                }
            }

            if (bInRoom && bIsHost)
            {
                btnStart.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            }
        }

        public void OnClose()
        {
            SocketMgr.CloseThread();
            SocketMgr.Close();
        }

        #endregion
    }





}
