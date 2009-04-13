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
    class Rank : IGameScreen
    {
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

        Byte[] rankBuffer = new byte[400];

        TextButton btnOK;


        bool bOK;

        int selectIndexRank = -1;
        int selectIndexRoom = -1;

        public Rank()
        {
            BaseGame.ShowMouse = true;

            roomList = new Listbox("roomlist", new Vector2(30, 100), new Point(200, 350), Color.White, Color.Green);
            rankList = new Listbox("ranklist", new Vector2(300, 100), new Point(450, 350), Color.White, Color.Green);
            roomList.AddItem("Room 1");
            bgTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.BgContent, "login"));
            bgRect = new Rectangle(0, 0, 800, 600);
            btnOK = new TextButton("OKBtn", new Vector2(150, 460), "OK", 0, Color.Gold);
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
            // 连接到服务器
            //SocketMgr.ConnectToServer();
        }
        
        void OnReceivePack(stPkgHead head, byte[] data)
        {
            byte[] tmpData;

            tmpData = new Byte[head.dataSize];
            
            if (head.iSytle == 50)
            {
                UserInfo ri;
                string str;

                for (int i = 0; i < head.dataSize; i += 32)
                {
   
                    str = "";

                    for (int k = 0; k < 32; k++)
                    {
                        tmpData[k] = data[i + k];
                    }

                    ri = (UserInfo)SocketMgr.BytesToStuct(tmpData, typeof(UserInfo));

                    for (int j = 0; ri.name[j] != '\0'; ++j)
                    {
                        str += ri.name[j];
                    }


                    rankList.AddItem(ri.rank + "        " + str + "              " + ri.score);

                }
            }
            else if (head.iSytle == 40)
            {

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
            rankList.Update();

            if (InputHandler.IsKeyDown(Keys.F1))
                GameManager.AddGameScreen(new StarwarLogic(0));
            else if (InputHandler.IsKeyDown(Keys.F2))
                GameManager.AddGameScreen(new StarwarLogic(1));

            if (InputHandler.IsKeyDown(Keys.Escape))
                return true;

            return bOK;

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
            stPkgHead head = new stPkgHead();
            MemoryStream Stream = new MemoryStream();
            head.dataSize = 0;
            head.iSytle = 21;
            SocketMgr.SendCommonPackge(head, Stream);
            Stream.Close();
            
            SocketMgr.CloseThread();
            SocketMgr.Close();
        }

        #endregion
    }





}
