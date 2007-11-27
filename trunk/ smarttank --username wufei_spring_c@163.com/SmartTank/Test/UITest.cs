using System;
using System.Collections.Generic;
using System.Text;
using GameBase;
using GameBase.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Graphics;
using System.Timers;

namespace SmartTank.Test
{
    class UITest : BaseGame
    {
        Button btn;
        Combo combo;
        TextButton textBtn;
        Listbox listBox;
        Label label;
        Checkbox checkBox;
        Textbox textBox;
        NumericUpDown numeric;
        Progressbar progressBar;
        RadioButton radioBth;

        Scrollbar scrollBar;

        Timer timer;

        protected override void Initialize ()
        {
            Coordin.SetScreenViewRect( new Rectangle( 0, 0, ClientRec.Width, ClientRec.Height ) );
            //Coordin.SetLogicViewRect( new Rectangle( 0, 0, 100, 100 ) );
            Coordin.SetCamera( 2, new Vector2( 200, 150 ), 0 );

            IsMouseVisible = true;
            base.Initialize();
            btn = new Button( "Btn1", "button", new Vector2( 100, 150 ), Color.Gold );
            combo = new Combo( "Combo1", new Vector2( 250, 150 ), 200 );
            textBtn = new TextButton( "textBtn1", new Vector2( 500, 150 ), "Start Game", 100, Color.Red );
            listBox = new Listbox( "listBox1", new Vector2( 100, 250 ), new Point( 200, 150 ), Color.DarkGreen, Color.Gray );
            label = new Label( "label1", new Vector2( 660, 150 ), "Hello Label!", Color.WhiteSmoke, 0.8f );
            checkBox = new Checkbox( "checkBox1", new Vector2( 350, 250 ), "Hello CheckBox", false );
            textBox = new Textbox( "textBox", new Vector2( 350, 300 ), 150, "Hello TextBox", false );
            numeric = new NumericUpDown( "numeric", new Vector2( 350, 350 ), 25, 0, 10, 2, 1 );
            progressBar = new Progressbar( "progressBar", new Vector2( 550, 250 ), Color.Green, 150, 0, true );
            radioBth = new RadioButton( "radioBth", new Vector2( 550, 300 ), "Hello RadioButton", false );

            scrollBar = new Scrollbar( "scroll1", new Vector2( 550, 350 ), Scrollbar.Axis.Horizontal, 150, null, 100, 1 );
            scrollBar.OnValueChange += new EventHandler( scrollBar_OnValueChange );


            timer = new Timer( 500 );
            timer.Start();
            timer.Elapsed += new ElapsedEventHandler( timer_Elapsed );

            combo.AddItem( "GunGun" );
            combo.AddItem( "WoBuHao" );
            combo.AddItem( "Gohan" );
            combo.AddItem( "LinLin" );
            combo.AddItem( "GunGun1" );
            combo.AddItem( "WoBuHao1" );
            combo.AddItem( "Gohan1" );
            combo.AddItem( "LinLin1" );
            combo.AddItem( "GunGun2" );
            combo.AddItem( "WoBuHao2" );
            combo.AddItem( "Gohan2" );
            combo.AddItem( "LinLin2" );
            combo.AddItem( "GunGun3" );
            combo.AddItem( "WoBuHao3" );
            combo.AddItem( "Gohan3" );
            combo.AddItem( "LinLin3" );
            combo.AddItem( "GunGun4" );
            combo.AddItem( "WoBuHao4" );
            combo.AddItem( "Gohan4" );
            combo.AddItem( "LinLin4" );



            listBox.AddItem( "GunGun" );
            listBox.AddItem( "WoBuHao" );
            listBox.AddItem( "Gohan" );
            listBox.AddItem( "LinLin" );
            listBox.AddItem( "GunGun1" );
            listBox.AddItem( "WoBuHao1" );
            listBox.AddItem( "Gohan1" );
            listBox.AddItem( "LinLin1" );
            listBox.AddItem( "GunGun2" );
            listBox.AddItem( "WoBuHao2" );
            listBox.AddItem( "Gohan2" );
            listBox.AddItem( "LinLin2" );
            listBox.AddItem( "GunGun3" );
            listBox.AddItem( "WoBuHao3" );
            listBox.AddItem( "Gohan3" );
            listBox.AddItem( "LinLin3" );

        }

        int scrollValue;
        void scrollBar_OnValueChange ( object sender, EventArgs e )
        {
            scrollValue = scrollBar.value;
        }

        void timer_Elapsed ( object sender, ElapsedEventArgs e )
        {
            progressBar.value += 5;
        }

        protected override void Update ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            btn.Update();
            combo.Update();
            textBtn.Update();
            listBox.Update();
            checkBox.Update();
            textBox.Update();
            numeric.Update();
            progressBar.Update();
            radioBth.Update();
            scrollBar.Update();
            base.Update( gameTime );
        }

        protected override void GameDraw ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            btn.Draw( Sprite.alphaSprite, 1f );
            combo.Draw( Sprite.alphaSprite, 1f );
            textBtn.Draw( Sprite.alphaSprite, 1f );
            listBox.Draw( Sprite.alphaSprite, 1f );
            label.Draw( Sprite.alphaSprite, 1f );
            checkBox.Draw( Sprite.alphaSprite, 1f );
            textBox.Draw( Sprite.alphaSprite, 1f );
            numeric.Draw( Sprite.alphaSprite, 1f );
            progressBar.Draw( Sprite.alphaSprite, 1f );
            radioBth.Draw( Sprite.alphaSprite, 1f );
            scrollBar.Draw( Sprite.alphaSprite, 1f );
            FontManager.DrawInScrnCoord( "The Color and Textures of the Controls can be changed as you like!", new Vector2( 60, 60 ), 1f, Color.Yellow, 0f, FontType.Lucida );
            FontManager.DrawInScrnCoord( "The ScollBar's Value is " + scrollValue, new Vector2( 550, 380 ), 0.7f, Color.Green, 0f, FontType.Comic );
        }
    }
}
