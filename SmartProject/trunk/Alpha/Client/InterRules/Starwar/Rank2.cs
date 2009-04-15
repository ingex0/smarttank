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
    class Rank2 : IGameScreen
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

        struct RankIF
        { 
            public int rank;
            public string name;
            public int score;
        }
        RankIF myInfo;
        Texture2D bgTexture, rkTexture, piTexture, hdTexture;
        List<string> devHeads;

        Rectangle bgRect;

        SpriteBatch spriteBatch;

        Listbox2 roomList;
        Listbox2 rankList;

        Byte[] rankBuffer = new byte[400];

        TextButton btnOK;

        List<RankIF> rankItems;
        Vector2 rankPos;

        bool bOK, bLoaded;

        int selectIndexRank = -1;
        int selectIndexRoom = -1;


        public Rank2()
        {
            devHeads = new List<string>();
            devHeads.Add("asokawu");
            devHeads.Add("ddli");
            devHeads.Add("jehutyhu");
            devHeads.Add("zashchen");
            devHeads.Add("orrischen");
            devHeads.Add("johntan");
            devHeads.Add("seekyao");
            
            BaseGame.ShowMouse = true;


            rankPos = new Vector2(50, 120);

            roomList = new Listbox2("roomlist", new Vector2(550, 120), new Point(200, 150), Color.White, Color.Black);
            rankList = new Listbox2("ranklist", rankPos, new Point(450, 350), Color.White, Color.Green);
            bgTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.BgContent, "bg22"));
            rkTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.UIContent, "ranklist2"));
            piTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.UIContent, "yourinfo2"));
            bgRect = new Rectangle(0, 0, 800, 600);
            btnOK = new TextButton("OKBtn", new Vector2(550, 370), "OK", 0, Color.Gold);
            btnOK.OnClick += new EventHandler(btnOK_OnPress);
            rankList.OnChangeSelection += new EventHandler(rankList_OnChangeSelection);
            roomList.OnChangeSelection += new EventHandler(roomList_OnChangeSelection);
            SocketMgr.OnReceivePkg += new SocketMgr.ReceivePkgEventHandler(OnReceivePack);



            stPkgHead head = new stPkgHead();
            //head.iSytle = //包头类型还没初始化
            byte[] rankcode = new byte[4];
            rankcode[0] = 1;
            rankcode[1] = 0;
            rankcode[2] = 0;
            rankcode[3] = 0;

            MemoryStream Stream = new MemoryStream();
            Stream.Write(rankcode, 0, 4);
            head.dataSize = 4;
            head.iSytle = 50;
            SocketMgr.SendCommonPackge(head, Stream);
            Stream.Close();


            stPkgHead head2 = new stPkgHead();
            MemoryStream Stream2 = new MemoryStream();
            head2.dataSize = 0;
            head2.iSytle = 40;
            SocketMgr.SendCommonPackge(head2, Stream2);
            Stream2.Close();

            head = new stPkgHead();
            //head.iSytle = //包头类型还没初始化
            rankcode = new byte[4];
            rankcode[0] = 1;
            rankcode[1] = 0;
            rankcode[2] = 0;
            rankcode[3] = 0;

            Stream = new MemoryStream();
            Stream.Write(rankcode, 0, 4);
            head.dataSize = 4;
            head.iSytle = 50;
            SocketMgr.SendCommonPackge(head, Stream);
            Stream.Close();

            bOK = false;
            rankItems = new List<RankIF>();
            bLoaded = false;
            // 连接到服务器
            //SocketMgr.ConnectToServer();
        }
        
        void OnReceivePack(stPkgHead head, byte[] data)
        {
            byte[] tmpData;

            tmpData = new Byte[head.dataSize];
            
            if (head.iSytle == 50)
            {
                RankInfo ri;
                RankIF tmpItem;
                string str;
                rankItems.Clear();



                for (int i = 0; i < head.dataSize; i += 32)
                {
                    str = "";
                    for (int k = 0; k < 32; k++)
                    {
                        tmpData[k] = data[i + k];
                    }
                    ri = (RankInfo)SocketMgr.BytesToStuct(tmpData, typeof(RankInfo));
                    for (int j = 0; ri.name[j] != '\0'; ++j)
                    {
                        str += ri.name[j];
                    }
                    tmpItem.rank = ri.rank;
                    tmpItem.score = ri.score;
                    tmpItem.name = str;
                    rankItems.Add(tmpItem);

                }
            }
            else if (head.iSytle == 40)
            {
                
                UserInfo player;
                string str;
                str = "";
                //data.Read(roomBuffer, 0, 32);

                player = (UserInfo)SocketMgr.BytesToStuct(data, typeof(UserInfo));

                for (int j = 0; player.name[j] != '\0'; ++j)
                {
                    str += player.name[j];
                }
                
                myInfo.name = str;
                myInfo.rank = player.rank;
                myInfo.score = player.score;

                if (devHeads.Contains(myInfo.name))
                    hdTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.UIContent, myInfo.name));
                else
                    hdTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.UIContent, "head"));
                bLoaded = true;
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

        void btnOK_OnPress(object sender, EventArgs e)
        {
            bOK = true;
            SocketMgr.OnReceivePkg -= OnReceivePack;
        }

        #region IGameScreen 成员

        public bool Update(float second)
        {

            btnOK.Update();
            
            roomList.Update();

            if (InputHandler.IsKeyDown(Keys.Escape))
                return true;

            return bOK;

        }

        public void Render()
        {
            BaseGame.Device.Clear(Color.LightSkyBlue);
            spriteBatch = (SpriteBatch)BaseGame.SpriteMgr.alphaSprite;
            spriteBatch.Draw(bgTexture, Vector2.Zero, bgRect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth.BackGround);
            spriteBatch.Draw(rkTexture, new Vector2(50, 102), new Rectangle(0, 0, 75, 18), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.Draw(piTexture, new Vector2(550, 102), new Rectangle(0, 0, 137, 18), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0f);
            roomList.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            rankList.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
            BaseGame.FontMgr.DrawInScrnCoord("Rank", rankPos + new Vector2(8f, 4f), Control.fontScale, Color.White, 0f, Control.fontName);
            BaseGame.FontMgr.DrawInScrnCoord("Name", rankPos + new Vector2(100f, 4f), Control.fontScale, Color.White, 0f, Control.fontName);
            BaseGame.FontMgr.DrawInScrnCoord("Score", rankPos + new Vector2(250f, 4f), Control.fontScale, Color.White, 0f, Control.fontName);

            for (int i = 0; i < rankItems.Count; i++)
            {
                BaseGame.FontMgr.DrawInScrnCoord(rankItems[i].rank.ToString(), rankPos + new Vector2(10f, 20f + 15 * i), Control.fontScale, Color.White, 0f, Control.fontName);
                BaseGame.FontMgr.DrawInScrnCoord(rankItems[i].name, rankPos + new Vector2(102f, 20f + 15 * i), Control.fontScale, Color.White, 0f, Control.fontName);
                BaseGame.FontMgr.DrawInScrnCoord(rankItems[i].score.ToString(), rankPos + new Vector2(252f, 20f + 15 * i), Control.fontScale, Color.White, 0f, Control.fontName);
            }
            if (bLoaded)
            {
                spriteBatch.Draw(hdTexture, new Vector2(560, 128), new Rectangle(0, 0, 70, 70), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth.UI - 0.1f);
                BaseGame.FontMgr.DrawInScrnCoord("Name: " + myInfo.name, new Vector2(560, 200), Control.fontScale, Color.White, 0f, Control.fontName);
                BaseGame.FontMgr.DrawInScrnCoord("Score: " + myInfo.score, new Vector2(560, 215), Control.fontScale, Color.White, 0f, Control.fontName);
                BaseGame.FontMgr.DrawInScrnCoord("Rank:  " + myInfo.rank, new Vector2(560, 230), Control.fontScale, Color.White, 0f, Control.fontName);
            }
            btnOK.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
        }

        public void OnClose()
        {
            SocketMgr.OnReceivePkg -= OnReceivePack;
        }

        #endregion
    }





}
