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

        Texture2D bgTexture;

        Rectangle bgRect;

        SpriteBatch spriteBatch;

        Listbox roomList;

        TextButton btnOK;

        int selectIndex = -1;

        public Hall()
        {
            BaseGame.ShowMouse = true;

            roomList = new Listbox("roomlist", new Vector2(50, 50), new Point(200, 400), Color.WhiteSmoke, Color.Green);

            roomList.AddItem("Room 1");

            bgTexture = BaseGame.ContentMgr.Load<Texture2D>(Path.Combine(Directories.BgContent, "login"));

            bgRect = new Rectangle(0, 0, 800, 600);


            

            btnOK = new TextButton("OkBtn", new Vector2(700, 500), "Begin", 0, Color.Blue);
            btnOK.OnClick += new EventHandler(btnOK_OnPress);

            roomList.OnChangeSelection += new EventHandler(roomList_OnChangeSelection);

            // 连接到服务器
            //SocketMgr.ConnectToServer();
        }

        void roomList_OnChangeSelection(object sender, EventArgs e)
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
            
            btnOK.Draw(BaseGame.SpriteMgr.alphaSprite, 1);
        }

        public void OnClose()
        {

        }

        #endregion
    }


    


}
