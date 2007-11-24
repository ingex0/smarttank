using System;
using System.Collections.Generic;
using System.Text;
using GameBase.DataStructure;
using Microsoft.Xna.Framework;

namespace GameBase.Graphics.BackGround.VergeTile.test
{

    public class TestVergeTileGround : BaseGame
    {
        VergeTileGround backGround;

        VergeTileData data;

        protected override void Initialize ()
        {
            base.Initialize();
            Coordin.SetScreenViewRect( new Rectangle( 0, 0, clientRec.Width, clientRec.Height ) );
            //Coordin.SetLogicViewRect( new Rectangle( 0, 0, 60, 60 ) );
            Coordin.SetCamera( 10, new Vector2( 40, 30 ), 0 );


            data = new VergeTileData();
            data.gridWidth = 10;
            data.gridHeight = 10;
            data.vertexTexIndexs = new int[]
            {
                0,0,0,1,1,1,1,2,2,2,3,
                0,0,1,2,1,1,1,2,2,3,3,
                0,1,3,3,3,3,1,2,2,3,3,
                0,1,3,3,3,1,2,2,3,3,3,
                0,1,1,2,3,1,2,2,3,3,3,
                1,1,1,2,2,2,2,2,2,3,2,
                1,1,1,1,2,2,2,3,3,2,2,
                1,1,2,1,2,1,2,2,3,3,2,
                1,1,1,1,2,2,2,3,3,2,2,
                1,1,2,1,2,3,2,3,3,2,2,
                1,1,2,2,2,2,2,3,2,2,2,

            };
            //data.SetRondomVertexIndex( 10, 10, 4 );
            data.SetRondomGridIndex( 10, 10 );
            data.texPaths = new string[]
            {
                "Lords_Dirt.tga",
                "Lords_DirtRough.tga",
                "Lords_DirtGrass.tga",
                "Lords_GrassDark.tga",
                
            };

            backGround = new VergeTileGround( data, new Rectangle( 0, 0, 800, 600 ), new Vector2( 80, 60 ) );
        }

        protected override void GameDraw ( Microsoft.Xna.Framework.GameTime gameTime )
        {
            backGround.Draw();
        }

    }
}
