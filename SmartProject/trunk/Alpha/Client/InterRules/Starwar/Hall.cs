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

        [StructLayoutAttribute(LayoutKind.Sequential, Size = 56, CharSet = CharSet.Ansi, Pack = 1)]
        struct UserInfo
        {
            public const int size = 56;

            public int state;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
            public char[] name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 23)]
            public char[] pass;
            public int score;
            public int rank;
        }
        Texture2D bgTexture;

        Rectangle bgRect;

        SpriteBatch spriteBatch;

        Listbox roomList;
        Listbox rankList;
        int playerCount;

        TextButton btnCreate, btnEnter, btnRank, btnRefresh, btnStart, btnQuit;

        stPkgHead headSend;
        MemoryStream Stream;

        string[] userNames;

        int selectIndexRank = -1;
        int selectIndexRoom = -1;
        bool bHasError;

        bool bInRoom, bIsHost, bWaitEnter;

        int myRoomId;
        string myName;

        public Hall(string tmpName)
        {
            myName = tmpName;
            
            BaseGame.ShowMouse = true;

            roomList = new Listbox("roomlist", new Vector2(50, 120), new Point(200, 350), Color.White, Color.Green);

            rankList = new Listbox("ranklist", new Vector2(300, 120), new Point(450, 350), Color.White, Color.Green);


            bgTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.BgContent, "login"));

            bgRect = new Rectangle(0, 0, 800, 600);


            btnRefresh = new TextButton("RefreshBtn", new Vector2(150, 480), "Refresh", 0, Color.Gold);
            btnCreate = new TextButton("CreateBtn", new Vector2(310, 480), "Create a new room", 0, Color.Gold);
            btnQuit = new TextButton("QuitBtn", new Vector2(310, 480), "Quit", 0, Color.Gold);
            btnEnter = new TextButton("EnterBtn", new Vector2(70, 480), "Enter", 0, Color.Gold);
            btnRank = new TextButton("RankBtn", new Vector2(650, 480), "Rank List", 0, Color.Gold);
            btnStart = new TextButton("StartBtn", new Vector2(550, 410), "Start", 0, Color.Gold);

            btnRefresh.OnClick += new EventHandler(btnRefresh_OnPress);
            btnCreate.OnClick += new EventHandler(btnCreate_OnPress);
            btnQuit.OnClick += new EventHandler(btnQuit_OnPress);
            btnEnter.OnClick += new EventHandler(btnEnter_OnPress);
            btnRank.OnClick += new EventHandler(btnRank_OnPress);
            btnStart.OnClick += new EventHandler(btnStart_OnPress);

            rankList.OnChangeSelection += new EventHandler(rankList_OnChangeSelection);
            roomList.OnChangeSelection += new EventHandler(roomList_OnChangeSelection);

            SocketMgr.OnReceivePkg += new SocketMgr.ReceivePkgEventHandler(OnReceivePack);



            headSend = new stPkgHead();
            Stream = new MemoryStream();
            headSend.dataSize = 0;
            headSend.iSytle = 33;
            SocketMgr.SendCommonPackge(headSend, Stream);
            Stream.Close();
            bInRoom = false;
            bWaitEnter = false;
            bIsHost = false;
            bHasError = false;
        }
        
        void OnReceivePack(stPkgHead head, byte[] data)
        {

            if (head.iSytle == 40)
            {
                head.iSytle = 40;
            }
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
            else if (head.iSytle == 35)
            {
                //创建房间成功
                bWaitEnter = false;

                headSend = new stPkgHead();
                Stream = new MemoryStream();
                headSend.dataSize = 0;
                headSend.iSytle = 33;
                SocketMgr.SendCommonPackge(headSend, Stream);
                Stream.Close();
                bInRoom = true;

                headSend = new stPkgHead();
                Stream = new MemoryStream();
                headSend.dataSize = 0;
                headSend.iSytle = 34;
                SocketMgr.SendCommonPackge(headSend, Stream);
                Stream.Close();
                
 
            }
            else if (head.iSytle == 36)
            {
                //创建房间失败
                bWaitEnter = false;

                bInRoom = false;
                bIsHost = false;
            }
            else if (head.iSytle == 37)
            {
                //加入房间成功
                bWaitEnter = false;

                headSend = new stPkgHead();
                Stream = new MemoryStream();
                headSend.dataSize = 0;
                headSend.iSytle = 33;
                SocketMgr.SendCommonPackge(headSend, Stream);
                Stream.Close();
                bInRoom = true;

                headSend = new stPkgHead();
                Stream = new MemoryStream();
                headSend.dataSize = 0;
                headSend.iSytle = 34;
                SocketMgr.SendCommonPackge(headSend, Stream);
                Stream.Close();
 
            }
            else if (head.iSytle == 38)
            {
                //加入房间失败
                bWaitEnter = false;

                bInRoom = false;
                bIsHost = false;

            }
            else if (head.iSytle == 34)
            {
                //列举用户信息


                string str;
                UserInfo player;
                byte[] tmpData;

                

                tmpData = new byte[head.dataSize];
                bIsHost = false;
                string[] tmpNames = new string[6];
                playerCount = 0;
                for (int i = 0; i < head.dataSize; i += 56)
                {

                    str = "";
                    //data.Read(roomBuffer, 0, 32);

                    for (int k = 0; k < 56; ++k)
                    {
                        tmpData[k] = data[i + k];
                    }

                    player = (UserInfo)SocketMgr.BytesToStuct(tmpData, typeof(UserInfo));

                    for (int j = 0; player.name[j] != '\0'; ++j)
                    {
                        str += player.name[j];
                    }
                    if (str == myName && player.state == 1)
                        bIsHost = true;
                    tmpNames[playerCount] = str;//, Font font)

                    playerCount++;

                    //roomList.AddItem("room 1" + " ( " + room.players + " / 6 )", room.id);

                }

                userNames = new string[playerCount];
                for (int i = 0; i < playerCount; i++)
                {
                    userNames[i] = tmpNames[i];
                }
            }
            else if (head.iSytle == 70)
            {
                //开始游戏
                bWaitEnter = false;
                if (bIsHost)
                    GameManager.AddGameScreen(new StarwarLogic(0, userNames));
                else
                {
                    int tmp = 0;
                    for (int i = 0; i < playerCount; i++)
                    {
                        tmp++;
                        if (userNames[i] == myName)
                            GameManager.AddGameScreen(new StarwarLogic(tmp, userNames));
                    }
                }
            }
            else if (head.iSytle == 71)
            {
                bWaitEnter = false;
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
            if (bInRoom || bWaitEnter)
                return;

            headSend = new stPkgHead();
            //head.iSytle = //包头类型还没初始化


            Stream = new MemoryStream();
            headSend.dataSize = 0;
            headSend.iSytle = 30;
            SocketMgr.SendCommonPackge(headSend, Stream);
            Stream.Close();
            bWaitEnter = true;
        }

        void btnStart_OnPress(object sender, EventArgs e)
        {
            headSend = new stPkgHead();
            Stream = new MemoryStream();
            headSend.dataSize = 0;
            headSend.iSytle = 70;
            SocketMgr.SendCommonPackge(headSend, Stream);
            Stream.Close();
            bWaitEnter = true;
        }

        void btnQuit_OnPress(object sender, EventArgs e)
        {
            headSend = new stPkgHead();
            Stream = new MemoryStream();
            headSend.dataSize = 0;
            headSend.iSytle = 39;
            SocketMgr.SendCommonPackge(headSend, Stream);
            Stream.Close();
            
            bIsHost = false;
            bInRoom = false;

            headSend = new stPkgHead();
            Stream = new MemoryStream();
            headSend.dataSize = 0;
            headSend.iSytle = 33;
            roomList.Clear();
            SocketMgr.SendCommonPackge(headSend, Stream);
            Stream.Close();
        }

        void btnEnter_OnPress(object sender, EventArgs e)
        {
            if (bWaitEnter || bInRoom)
                return;


            if (roomList.selectedIndex == -1)
                return;

            headSend = new stPkgHead();
            //byte[] roomid;

            //roomid = SocketMgr.StructToBytes(roomList.MyIDs[roomList.selectedIndex]);

            Stream = new MemoryStream();
            //Stream.Write(roomid, 0, 4);
            //head.dataSize = 4;
            headSend.dataSize = 0;
            headSend.iSytle = 31;
            SocketMgr.SendCommonPackge(headSend, Stream);
            Stream.Close();
            bWaitEnter = true;
        }

        void btnRefresh_OnPress(object sender, EventArgs e)
        {
            headSend = new stPkgHead();
            Stream = new MemoryStream();
            headSend.dataSize = 0;
            headSend.iSytle = 33;
            SocketMgr.SendCommonPackge(headSend, Stream);
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
            if (bInRoom)
                btnQuit.Update();
            else
                btnCreate.Update();
            btnEnter.Update();
            btnRank.Update();
            if (bInRoom && bIsHost)
                btnStart.Update();
            
            
            roomList.Update();
            rankList.Update();

            if (InputHandler.IsKeyDown(Keys.PageDown))
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
            //roomList.Clear();
            roomList.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            rankList.Draw(BaseGame.SpriteMgr.alphaSprite, 1);

            btnEnter.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            if (bInRoom)
                btnQuit.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            else
                btnCreate.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            btnRank.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            btnRefresh.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            if (bInRoom)
            {
                for (int i = 0; i < playerCount; i++)
                {
                    BaseGame.FontMgr.DrawInScrnCoord(userNames[i], new Vector2(310, 100 + i * 30), Control.fontScale, Color.Black, 0f, Control.fontName);
                }
            }

            if (bInRoom && bIsHost)
            {
                btnStart.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            }
        }

        public void OnClose()
        {
            SocketMgr.OnReceivePkg -= OnReceivePack;
        }

        #endregion
    }





}
