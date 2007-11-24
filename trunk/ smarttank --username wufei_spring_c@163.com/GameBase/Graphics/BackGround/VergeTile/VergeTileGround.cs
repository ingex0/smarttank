using System;
using System.Collections.Generic;
using System.Text;
using Platform.BackGround;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameBase.DataStructure;
using GameBase.Helpers;

namespace GameBase.Graphics.BackGround.VergeTile
{

    /*
     * 使用WarCraft3中的地面资源图片拼接地面。
     * 
     * 主要技术是材质分层以及材质之间边缘的判断。
     * 
     * */
    public class VergeTileGround : IBackGround
    {
        #region private struct

        struct gridData
        {
            public int texIndex;
            public Rectangle sourceRect;

            public gridData ( int texIndex, Rectangle sourceRect )
            {
                this.texIndex = texIndex;
                this.sourceRect = sourceRect;
            }
        }

        #endregion

        #region Variables

        SpriteBatch[] batches;

        Rectangle screenViewRect;

        Vector2 mapSize;

        int gridWidthSum;

        int gridHeightSum;

        float gridScale;

        int[] vertexTexIndexs;

        int[] gridTexIndexs;

        Texture2D[] TexList;

        gridData[][] gridTexData;

        float sourceWidth;

        #endregion

        #region Construction

        public VergeTileGround ( VergeTileData data, Rectangle screenViewRect, Vector2 mapSize )
        {
            gridWidthSum = data.gridWidth;
            gridHeightSum = data.gridHeight;

            this.screenViewRect = screenViewRect;
            this.mapSize = mapSize;

            float gridScaleInWidth = mapSize.X / (float)gridWidthSum;
            float gridScaleInHeight = mapSize.Y / (float)gridHeightSum;

            gridScale = Math.Max( gridScaleInWidth, gridScaleInHeight );

            vertexTexIndexs = new int[data.vertexTexIndexs.Length];
            data.vertexTexIndexs.CopyTo( vertexTexIndexs, 0 );

            gridTexIndexs = new int[data.gridTexIndexs.Length];
            data.gridTexIndexs.CopyTo( gridTexIndexs, 0 );

            InitialTexList( data );

            InitialGridTexData();

            CreateSpriteBatches();

            sourceWidth = gridTexData[0][0].sourceRect.Width;

            BaseGame.Device.DeviceReset += new EventHandler( Device_DeviceReset );
        }

        private void CreateSpriteBatches ()
        {
            batches = new SpriteBatch[TexList.Length];
            for (int i = 0; i < batches.Length; i++)
            {
                batches[i] = new SpriteBatch( BaseGame.Device );
            }
        }

        void Device_DeviceReset ( object sender, EventArgs e )
        {
            CreateSpriteBatches();
        }

        private void InitialGridTexData ()
        {
            gridTexData = new gridData[gridWidthSum * gridHeightSum][];
            for (int y = 0; y < gridHeightSum; y++)
            {
                for (int x = 0; x < gridWidthSum; x++)
                {
                    int[] weights = new int[TexList.Length];
                    weights.Initialize();

                    weights[vertexTexIndexs[x + 1 + (gridWidthSum + 1) * (y + 1)]] += 1;
                    weights[vertexTexIndexs[x + (gridWidthSum + 1) * (y + 1)]] += 2;
                    weights[vertexTexIndexs[x + 1 + (gridWidthSum + 1) * y]] += 4;
                    weights[vertexTexIndexs[x + (gridWidthSum + 1) * y]] += 8;

                    List<gridData> tempGridData = new List<gridData>();
                    tempGridData.Add( new gridData( 0, CalcuSourceRect( 0, TexList[0].Width, TexList[0].Height, 17 ) ) );
                    for (int i = 1; i < weights.Length; i++)
                    {
                        if (weights[i] != 0)
                            tempGridData.Add( new gridData( i, CalcuSourceRect( weights[i], TexList[i].Width, TexList[i].Height, gridTexIndexs[x + gridWidthSum * y] ) ) );
                    }
                    gridTexData[x + gridWidthSum * y] = tempGridData.ToArray();
                }
            }
        }

        private Rectangle CalcuSourceRect ( int weight, int texWidth, int texHeight, int gridIndex )
        {
            int tileWidth = texHeight / 4;
            int x = 0;
            int y = 0;



            bool squreMap = (texHeight == texWidth);


            if (weight != 0 && weight != 15)
            {
                x = weight % 4;
                y = weight / 4;
            }
            else if (squreMap)
            {
                if (gridIndex < 9)
                {
                    x = 0;
                    y = 0;
                }
                else
                {
                    x = 3;
                    y = 3;
                }
            }
            else
            {
                if (gridIndex < 16)
                {
                    x = gridIndex % 4 + 4;
                    y = gridIndex / 4;
                }
                else if (gridIndex == 16)
                {
                    x = 0;
                    y = 0;
                }
                else if (gridIndex == 17)
                {
                    x = 3;
                    y = 3;
                }
            }
            return new Rectangle( tileWidth * x, tileWidth * y, tileWidth, tileWidth );
        }

        private void InitialTexList ( VergeTileData data )
        {
            TexList = new Texture2D[data.texPaths.Length];
            for (int i = 0; i < data.texPaths.Length; i++)
            {
                TexList[i] = Texture2D.FromFile( BaseGame.Device, data.texPaths[i] );
            }
        }

        #endregion

        #region IBackGround 成员

        public void Draw ()
        {

            try
            {
                Rectanglef logicViewRect;

                Vector2 upLeft = Coordin.LogicPos( new Vector2( screenViewRect.X, screenViewRect.Y ) );
                Vector2 upRight = Coordin.LogicPos( new Vector2( screenViewRect.X + screenViewRect.Width, screenViewRect.Y ) );
                Vector2 downLeft = Coordin.LogicPos( new Vector2( screenViewRect.X, screenViewRect.Y + screenViewRect.Height ) );
                Vector2 downRight = Coordin.LogicPos( new Vector2( screenViewRect.X + screenViewRect.Width, screenViewRect.Y + screenViewRect.Height ) );

                logicViewRect = MathTools.BoundBox( upLeft, upRight, downLeft, downRight );

                int minX, minY, maxX, maxY;

                minX = Math.Max( 0, (int)(logicViewRect.X / gridScale) - 1 );
                minY = Math.Max( 0, (int)(logicViewRect.Y / gridScale) - 1 );

                maxX = (int)((logicViewRect.X + logicViewRect.Width) / gridScale) + 1;
                maxY = (int)((logicViewRect.Y + logicViewRect.Height) / gridScale) + 1;

                foreach (SpriteBatch batch in batches)
                {
                    batch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None );
                }

                float gridWidthInScrn = Coordin.ScrnLengthf( gridScale );
                float gridHeightInScrn = Coordin.ScrnLengthf( gridScale );

                Vector2 startPosInScrn = Coordin.ScreenPos( new Vector2( minX * gridScale, minY * gridScale ) );

                Vector2 UnitX = Vector2.Transform( new Vector2( gridWidthInScrn, 0 ), Coordin.RotaMatrixFromLogicToScrn );
                Vector2 UnitY = Vector2.Transform( new Vector2( 0, gridHeightInScrn ), Coordin.RotaMatrixFromLogicToScrn );

                float drawScale = gridWidthInScrn / sourceWidth;


                for (int y = minY; y <= maxY && y < gridHeightSum && y >= 0; y++)
                {
                    for (int x = minX; x <= maxX && x < gridWidthSum && x >= 0; x++)
                    {
                        Vector2 drawPos = startPosInScrn + (x - minX) * UnitX + (y - minY) * UnitY;

                        foreach (gridData data in gridTexData[x + gridWidthSum * y])
                        {
                            batches[data.texIndex].Draw( TexList[data.texIndex], drawPos, data.sourceRect, Color.White, -Coordin.Rota, new Vector2( 0, 0 ), drawScale, SpriteEffects.None, 1f );
                        }
                    }
                }

                foreach (SpriteBatch batch in batches)
                {
                    batch.End();
                }
            }
            catch (Exception)
            {
                CreateSpriteBatches();
            }
        }

        #endregion


    }
}
