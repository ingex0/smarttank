using System;
using System.Collections.Generic;
using System.Text;
using GameBase;
using Microsoft.Xna.Framework;
using Platform.GameObjects;
using GameBase.Graphics;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Input;
using GameBase.Effects;
using GameBase.DataStructure;

/*
 * 测试结果：能够实现碰撞组的注册。
 * 
 * 但动态地改变注册物体的问题还有待解决。
 * 
 * 还有另一个bug，在振动时有可能发生更新错误。原因应该是振动使sprite的屏幕坐标发生改变，而改变的精度问题带来了碰撞检测的前后不一致。
 * 暂时的处理方法在理论上仍然可能有漏洞，但当前的运行情况正常。
 * 
 * 
 * New: 动态改变注册碰撞组的问题已经解决。只是需要创建一个场景物体管理器来管理物体的注册。
 * 
 * */


namespace Platform.PhisicalCollision.test
{
    class easyItem : ICollideObj, IPhisicalObj, Platform.GameDraw.IDrawableObj
    {
        NonInertiasColUpdater colPhiUpdater;
        GameObjInfo objInfo;
        Sprite sprite;

        #region ICollider 成员

        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        public IColChecker ColChecker
        {
            get { return colPhiUpdater; }
        }

        #endregion

        #region IPhisicalObj 成员

        public IPhisicalUpdater PhisicalUpdater
        {
            get { return colPhiUpdater; }
        }

        #endregion

        public easyItem ( string name, Vector2 pos, Vector2 vel, string texPath, Vector2 origin, Color color )
        {
            objInfo = new GameObjInfo( name, string.Empty );
            sprite = new Sprite( true, texPath, true );
            sprite.SetParameters( origin, pos, 0.4f, 0f, color, 0f, SpriteBlendMode.AlphaBlend );
            colPhiUpdater = new NonInertiasColUpdater( objInfo, pos, vel, 0f, 0f, new Sprite[] { sprite } );
            colPhiUpdater.OnCollied += new OnCollidedEventHandler( CollisionHandler );
            colPhiUpdater.OnOverlap += new OnCollidedEventHandler( colPhiUpdater_OnOverlap );
        }

        Color remColor;
        void colPhiUpdater_OnOverlap ( IGameObj sender, CollisionResult result, GameObjInfo objB )
        {
            if (Color.Black != sprite.Color)
            {
                remColor = sprite.Color;
                sprite.Color = Color.Black;
            }
            else
            {
                sprite.Color = remColor;
            }

        }

        void CollisionHandler ( IGameObj sender, CollisionResult result, GameObjInfo objB )
        {
            if (objB.Name == "Border")
            {
                colPhiUpdater.Vel = -2 * Vector2.Dot( colPhiUpdater.Vel, result.NormalVector ) * result.NormalVector + colPhiUpdater.Vel;
                GameBase.Effects.Quake.BeginQuake( 10, 3 );
            }
            else
                colPhiUpdater.Vel = 100 * result.NormalVector;
        }


        #region IDrawable 成员

        public void Draw ()
        {
            sprite.Pos = colPhiUpdater.Pos;
            sprite.Rata = colPhiUpdater.Azi;
            sprite.Draw();
        }

        #endregion

        #region IDrawableObj 成员


        public Vector2 Pos
        {
            get { return sprite.Pos; }
        }

        #endregion

        #region IHasBorderObj 成员

        public CircleList<GameBase.Graphics.Border> BorderData
        {
            get { return sprite.BorderData; }
        }

        public Matrix WorldTrans
        {
            get { return sprite.Transform; }
        }

        public Rectanglef BoundingBox
        {
            get { return sprite.BoundRect; }
        }

        #endregion
    }


    class testPhiColManager : BaseGame
    {
        MultiLinkedList<easyItem> redItems;
        MultiLinkedList<easyItem> yellowItems;
        MultiLinkedList<easyItem> greenItems;
        Border border;

        PhiColManager phiColManager;

        protected override void Initialize ()
        {
            base.Initialize();
            Coordin.SetScreenViewRect( new Rectangle( 0, 0, ClientRec.Width, ClientRec.Height ) );
            //Coordin.SetLogicViewRect( new Rectanglef( 0, 0, 300, 200 ) );
            Coordin.SetCamera( 2, new Vector2( 200, 150 ), 0 );

            redItems = new MultiLinkedList<easyItem>();
            yellowItems = new MultiLinkedList<easyItem>();
            greenItems = new MultiLinkedList<easyItem>();
            border = new Border( 10, 290, 10, 190 );

            redItems.AddLast( new easyItem( "red1", new Vector2( 50, 50 ), new Vector2( 30, 40 ), "Test\\item6", new Vector2( 30, 30 ), Color.Red ) );
            redItems.AddLast( new easyItem( "red2", new Vector2( 200, 60 ), new Vector2( -10, 50 ), "Test\\item6", new Vector2( 30, 30 ), Color.Red ) );
            redItems.AddLast( new easyItem( "red3", new Vector2( 100, 160 ), new Vector2( 40, -20 ), "Test\\item6", new Vector2( 30, 30 ), Color.Red ) );

            yellowItems.AddLast( new easyItem( "yellow1", new Vector2( 150, 50 ), new Vector2( 30, 40 ), "Test\\item6", new Vector2( 30, 30 ), Color.Yellow ) );
            yellowItems.AddLast( new easyItem( "yellow2", new Vector2( 100, 60 ), new Vector2( -10, 50 ), "Test\\item6", new Vector2( 30, 30 ), Color.Yellow ) );
            yellowItems.AddLast( new easyItem( "yellow3", new Vector2( 50, 160 ), new Vector2( 40, -20 ), "Test\\item6", new Vector2( 30, 30 ), Color.Yellow ) );

            greenItems.AddLast( new easyItem( "green1", new Vector2( 260, 150 ), new Vector2( -60, 40 ), "Test\\item6", new Vector2( 30, 30 ), Color.Green ) );
            greenItems.AddLast( new easyItem( "green2", new Vector2( 80, 170 ), new Vector2( -10, -50 ), "Test\\item6", new Vector2( 30, 30 ), Color.Green ) );
            greenItems.AddLast( new easyItem( "green3", new Vector2( 70, 60 ), new Vector2( 40, 80 ), "Test\\item6", new Vector2( 30, 30 ), Color.Green ) );

            phiColManager = new PhiColManager();

            IEnumerable<ICollideObj> redItemsCopy = redItems.GetConvertList<ICollideObj>();
            IEnumerable<ICollideObj> yellowItemsCopy = yellowItems.GetConvertList<ICollideObj>();
            IEnumerable<ICollideObj> greenItemsCopy = greenItems.GetConvertList<ICollideObj>();


            phiColManager.AddColGroup(
                //redItems.ConvertAll<ICollider>(
                //delegate( easyItem target )
                //{
                //    return (ICollider)target;
                //} ),
                redItemsCopy,
                new ICollideObj[] { (ICollideObj)border } );
            phiColManager.AddColGroup(
                //redItems.ConvertAll<ICollider>(
                //delegate( easyItem target )
                //{
                //    return (ICollider)target;
                //} ) );
                redItemsCopy );

            phiColManager.AddColGroup(
                //yellowItems.ConvertAll<ICollider>(
                //delegate( easyItem target )
                //{
                //    return (ICollider)target;
                //} ), 
                yellowItemsCopy,
                new ICollideObj[] { (ICollideObj)border } );
            phiColManager.AddColGroup(
                //yellowItems.ConvertAll<ICollider>(
                //delegate( easyItem target )
                //{
                //    return (ICollider)target;
                //} ),
                yellowItemsCopy,
                //redItems.ConvertAll<ICollider>(
                //delegate( easyItem target )
                //{
                //    return (ICollider)target;
                //} ) );
                redItemsCopy );

            phiColManager.AddColGroup(
                //greenItems.ConvertAll<ICollider>(
                //delegate( easyItem target )
                //{
                //    return (ICollider)target;
                //} ), new ICollider[] { (ICollider)border } );
                greenItemsCopy,
                new ICollideObj[] { (ICollideObj)border } );

            phiColManager.AddOverlapColGroup( greenItemsCopy, yellowItemsCopy );



            phiColManager.AddPhiGroup(
                //redItems.ConvertAll<IPhisicalObj>(
                //delegate( easyItem target )
                //{
                //    return (IPhisicalObj)target;
                //} ) );
                redItems.GetConvertList<IPhisicalObj>() );
            phiColManager.AddPhiGroup(
                //yellowItems.ConvertAll<IPhisicalObj>(
                //delegate( easyItem target )
                //{
                //    return (IPhisicalObj)target;
                //} ) );
                yellowItems.GetConvertList<IPhisicalObj>() );
            phiColManager.AddPhiGroup(
                //greenItems.ConvertAll<IPhisicalObj>(
                //delegate( easyItem target )
                //{
                //    return (IPhisicalObj)target;
                //} ) );
                greenItems.GetConvertList<IPhisicalObj>() );
        }

        protected override void Update ( GameTime gameTime )
        {
            base.Update( gameTime );
            phiColManager.Update( (float)gameTime.ElapsedGameTime.TotalSeconds );
            if (InputHandler.MouseJustClickLeft)
            {
                Quake.BeginQuake( 70, 50 );
            }
        }

        protected override void GameDraw ( GameTime gameTime )
        {
            base.GameDraw( gameTime );
            foreach (easyItem item in redItems)
            {
                item.Draw();
            }
            foreach (easyItem item in yellowItems)
            {
                item.Draw();
            }
            foreach (easyItem item in greenItems)
            {
                item.Draw();
            }
            border.Draw( Color.Red );
        }
    }
}
