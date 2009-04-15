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
using System.Threading;

namespace InterRules.Starwar
{
    class Hall2 : IGameScreen
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
        Texture2D bgTexture, rlTexture, riTexture;
        List<Texture2D> heads;
        List<int> ranks;
        List<int> scores;

        Rectangle bgRect;

        SpriteBatch spriteBatch;

        Listbox2 roomList;
        Listbox2 rankList;
        int playerCount;

        TextButton2 btnCreate, btnEnter, btnRank, btnRefresh;
        TextButton btnStart, btnQuit;

        stPkgHead headSend;
        MemoryStream Stream;

        string[] userNames;
        List<string> devHeads;

        int selectIndexRank = -1;
        int selectIndexRoom = -1;
        bool bHasError;

        bool bInRoom, bIsHost, bWaitEnter;

        int myRoomId;
        string myName;

        public Hall2(string tmpName)
        {

            devHeads = new List<string>();
            devHeads.Add("asokawu");
            devHeads.Add("ddli");
            devHeads.Add("jehutyhu");
            devHeads.Add("zashchen");
            devHeads.Add("orrischen");
            devHeads.Add("johntan");
            devHeads.Add("seekyao");
            myName = tmpName;

            heads = new List<Texture2D>();
            ranks = new List<int>();
            scores = new List<int>();
            
            BaseGame.ShowMouse = true;

            roomList = new Listbox2("roomlist", new Vector2(50, 120), new Point(200, 350), Color.White, Color.White);

            rankList = new Listbox2("ranklist", new Vector2(300, 120), new Point(450, 350), Color.White, Color.White);


            bgTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.BgContent, "bg21"));
            rlTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.UIContent, "roomlist2"));
            riTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.UIContent, "roominfo2"));

            bgRect = new Rectangle(0, 0, 800, 600);


            btnRefresh = new TextButton2("RefreshBtn", new Vector2(150, 480), "Refresh", 0, Color.Gold);
            btnCreate = new TextButton2("CreateBtn", new Vector2(310, 480), "Create a new room", 0, Color.Gold);
            btnQuit = new TextButton("QuitBtn", new Vector2(450, 410), "Quit", 0, Color.Gold);
            btnEnter = new TextButton2("EnterBtn", new Vector2(70, 480), "Enter", 0, Color.Gold);
            btnRank = new TextButton2("RankBtn", new Vector2(650, 480), "Rank List", 0, Color.Gold);
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


                    roomList.AddItem(" " + str + " ( " + room.players + " / 2 )", room.id);

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

                Monitor.Enter(heads);
                Monitor.Enter(ranks);
                Monitor.Enter(scores);
                heads.Clear();
                ranks.Clear();
                scores.Clear();

                

                tmpData = new byte[head.dataSize];
                bIsHost = false;
                string[] tmpNames = new string[6];
                int playerNum = 0;
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
                    tmpNames[playerNum] = str;//, Font font)
                    ranks.Add(player.rank);
                    scores.Add(player.score);
                    Texture2D tex;
                    if (devHeads.Contains(str))
                    {
                        tex = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.UIContent, str));
                    }
                    else
                    {
                        tex = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.UIContent, "head"));
                    }
                    if (tex == null)
                    {
                        throw new Exception("");
                    }



                    heads.Add(tex);
                    playerNum++;

                    //roomList.AddItem("room 1" + " ( " + room.players + " / 6 )", room.id);

                }
                playerCount = playerNum;

                userNames = new string[playerNum];
                for (int i = 0; i < playerNum; i++)
                {
                    userNames[i] = tmpNames[i];
                }

                Monitor.Exit(scores);
                Monitor.Exit(ranks);
                Monitor.Exit(heads);

                headSend = new stPkgHead();
                Stream = new MemoryStream();
                headSend.dataSize = 0;
                headSend.iSytle = 33;
                SocketMgr.SendCommonPackge(headSend, Stream);
                Stream.Close();
            }
            else if (head.iSytle == 70)
            {
                //开始游戏
                bWaitEnter = false;
                if (bIsHost)
                    GameManager.AddGameScreen(new StarwarLogic(0, userNames));
                else
                {
                    for (int i = 0; i < playerCount; i++)
                    {
                        if (userNames[i] == myName)
                            GameManager.AddGameScreen(new StarwarLogic(i, userNames));
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
            if (bWaitEnter)
                return;

            if (bInRoom)
            {
                headSend = new stPkgHead();
                Stream = new MemoryStream();
                headSend.dataSize = 0;
                headSend.iSytle = 39;
                SocketMgr.SendCommonPackge(headSend, Stream);
                Stream.Close();

                bIsHost = false;
                bInRoom = false;
            }

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
            GameManager.AddGameScreen(new Rank2());
        }

        #region IGameScreen 成员

        public bool Update(float second)
        {
            btnRefresh.Update();
            if (bInRoom)
                btnQuit.Update();

            btnCreate.Update();
            btnEnter.Update();
            btnRank.Update();
            if (bInRoom && bIsHost)
                btnStart.Update();
            
            
            roomList.Update();
            rankList.Update();

            if (InputHandler.IsKeyDown(Keys.Escape))
                return true;

            return false;
        }

        public void Render()
        {
            BaseGame.Device.Clear(Color.LightSkyBlue);
            spriteBatch = (SpriteBatch)BaseGame.SpriteMgr.alphaSprite;
            spriteBatch.Draw(bgTexture, Vector2.Zero, bgRect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth.BackGround);
            spriteBatch.Draw(rlTexture, new Vector2(50, 102), new Rectangle(0, 0, 79, 18), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(riTexture, new Vector2(300, 102), new Rectangle(0, 0, 145, 18), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);

            //roomList.Clear();
            roomList.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            rankList.Draw(BaseGame.SpriteMgr.alphaSprite, 1);

            btnEnter.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            btnCreate.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            btnRank.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            btnRefresh.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            if (bInRoom)
            {
                Monitor.Enter(heads);

                for (int i = 0; i < playerCount; i++)
                {
                    if (heads.Count >= i + 1)
                        spriteBatch.Draw(heads[i], new Vector2(334, 157 + i * 140), new Rectangle(0, 0, 70, 70), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth.UI - 0.1f);
                    BaseGame.FontMgr.DrawInScrnCoord("Name: " + userNames[i], new Vector2(335, 230 + i * 140), Control.fontScale, Color.Black, 0f, Control.fontName);
                    BaseGame.FontMgr.DrawInScrnCoord("Score: " + scores[i], new Vector2(335, 245 + i * 140), Control.fontScale, Color.Black, 0f, Control.fontName);
                    BaseGame.FontMgr.DrawInScrnCoord("Rank:  " + ranks[i], new Vector2(335, 260 + i * 140), Control.fontScale, Color.Black, 0f, Control.fontName);
                }
                Monitor.Exit(heads);
            }

            if (bInRoom)
            {
                if  (bIsHost)
                    btnStart.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
                btnQuit.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            }
        }

        public void OnClose()
        {
            SocketMgr.OnReceivePkg -= OnReceivePack;
        }

        #endregion
    }





}
