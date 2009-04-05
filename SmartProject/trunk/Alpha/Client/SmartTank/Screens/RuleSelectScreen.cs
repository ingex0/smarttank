using System;
using System.Collections.Generic;
using System.Text;
using TankEngine2D.Helpers;
using SmartTank.Draw.UI.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankEngine2D.Graphics;
using TankEngine2D.Input;
using SmartTank;
using SmartTank.Helpers.DependInject;
using SmartTank.Helpers;

namespace SmartTank.Screens
{

    class RuleSelectScreen : IGameScreen
    {
        Listbox rulesList;

        TextButton btn;

        int selectIndex = -1;

        public RuleSelectScreen()
        {
            BaseGame.ShowMouse = true;
            RuleLoader.Initial();
            string[] ruleLists = RuleLoader.GetRulesList();
            rulesList = new Listbox( "rulelist", new Vector2( 200, 150 ), new Point( 400, 300 ), Color.WhiteSmoke, Color.Green );
            foreach (string rulename in ruleLists)
            {
                rulesList.AddItem( rulename );
            }
            rulesList.OnChangeSelection += new EventHandler( rulesList_OnChangeSelection );
            btn = new TextButton( "OkBtn", new Vector2( 700, 500 ), "Begin", 0, Color.Blue );
            btn.OnClick += new EventHandler( btn_OnPress );
        }

        void btn_OnPress( object sender, EventArgs e )
        {
            if (selectIndex >= 0 && selectIndex <= rulesList.Items.Count)
            {
                GameManager.ComponentReset();
                GameManager.AddGameScreen( RuleLoader.CreateRuleInstance( selectIndex ) );
            }
        }

        void rulesList_OnChangeSelection( object sender, EventArgs e )
        {
            selectIndex = rulesList.selectedIndex;
        }

        #region IGameScreen ³ÉÔ±

        public bool Update( float second )
        {
            rulesList.Update();
            btn.Update();

            if (InputHandler.JustPressKey( Microsoft.Xna.Framework.Input.Keys.Escape ))
                return true;

            return false;
        }

        public void Render()
        {
            BaseGame.Device.Clear( Color.LightSkyBlue );
            rulesList.Draw( BaseGame.SpriteMgr.alphaSprite, 1 );
            btn.Draw( BaseGame.SpriteMgr.alphaSprite, 1 );
        }

        public void OnClose()
        {
            
        }

        #endregion
    }
}
