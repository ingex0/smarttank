using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameBase;
using System.IO;
using GameBase.Helpers;

namespace Platform.Shelter
{
    public partial class ShelterManager
    {

        class RaderDrawer
        {
            #region Variables

            readonly int partSum = 30;

            VertexDeclaration del;

            VertexBuffer sectorBuffer;

            Effect renderRaderEffect;

            EffectParameter depthMapPara;
            EffectParameter raderAngPara;
            //EffectParameter targetSizePara;
            EffectParameter raderRPara;
            EffectParameter raderColorPara;
            EffectParameter raderRotaMatrixPara;

            #endregion

            #region Construction

            public RaderDrawer ()
            {
                del = new VertexDeclaration( BaseGame.Device, new VertexElement[]{
                new VertexElement(0,0, VertexElementFormat.Vector3,
                VertexElementMethod.Default, VertexElementUsage.Position,0)} );

                sectorBuffer = new VertexBuffer( BaseGame.Device, typeof( Vector3 ),
                    partSum + 2, ResourceUsage.WriteOnly, ResourceManagementMode.Automatic );

                InitialVertexBuffer();

                LoadEffect();
            }

            private void LoadEffect ()
            {
                renderRaderEffect = BaseGame.Content.Load<Effect>( Path.Combine( Directories.ContentDirectory, "EffectFile\\RaderMap" ) );

                depthMapPara = renderRaderEffect.Parameters["DepthMap"];
                raderAngPara = renderRaderEffect.Parameters["RaderAng"];
                //targetSizePara = renderRaderEffect.Parameters["TargetSize"];
                raderRPara = renderRaderEffect.Parameters["RaderR"];
                raderColorPara = renderRaderEffect.Parameters["RaderColor"];
                raderRotaMatrixPara = renderRaderEffect.Parameters["RaderRotaMatrix"];

            }

            private void InitialVertexBuffer ()
            {
                Vector3[] vertexData = new Vector3[partSum + 2];
                vertexData[0] = new Vector3( 0, 0, 0.5f );

                float length = 2f / (float)partSum;
                for (int i = 0; i <= partSum; i++)
                {
                    vertexData[i + 1] = new Vector3( -1 + length * i, 1, 0.5f );
                }

                sectorBuffer.SetData<Vector3>( vertexData );
            }
            #endregion

            #region DrawRader

            /*
             * 需要进行修改，以生成一个用于判断物体是否在雷达区域中的一个略图。
             * 
             * */
            public Texture2D[] DrawRader ( Rader rader, RaderDepthMap depthMap )
            {
                try
                {
                    BaseGame.Device.RenderState.DepthBufferEnable = false;
                    BaseGame.Device.RenderState.DepthBufferWriteEnable = false;
                    BaseGame.Device.RenderState.AlphaBlendEnable = false;

                    RenderTarget2D target = new RenderTarget2D( BaseGame.Device,
                        Coordin.ScrnLength( rader.R ) * 2, Coordin.ScrnLength( rader.R ) * 2,
                        1, BaseGame.Device.PresentationParameters.BackBufferFormat );

                    RenderTarget2D targetSmall = new RenderTarget2D( BaseGame.Device,
                        (int)(Coordin.ScrnLengthf( rader.R ) * 2 * Rader.smallMapScale),
                        (int)(Coordin.ScrnLengthf( rader.R ) * 2 * Rader.smallMapScale),
                        1, BaseGame.Device.PresentationParameters.BackBufferFormat );


                    #region Old Code
                    //if (Coordin.ScrnWidth( rader.R ) * 2 < targetSize1)
                    //{
                    //    target = target;
                    //    targetSizePara.SetValue( targetSize1 );
                    //}
                    //else if (Coordin.ScrnWidth( rader.R ) * 2 < targetSize2)
                    //{
                    //    target = new RenderTarget2D( BaseGame.Device,
                    //    targetSize2, targetSize2,
                    //    1, BaseGame.Device.PresentationParameters.BackBufferFormat );
                    //    targetSizePara.SetValue( targetSize2 );
                    //}
                    //else if (Coordin.ScrnWidth( rader.R ) * 2 < targetSize3)
                    //{
                    //    target = target3;
                    //    targetSizePara.SetValue( targetSize3 );
                    //}
                    //else if (Coordin.ScrnWidth( rader.R ) * 2 < targetSize4)
                    //{
                    //    target = target4;
                    //    targetSizePara.SetValue( targetSize4 );
                    //}
                    //else
                    //    throw new Exception( "the rader is too big!" ); 
                    #endregion

                    depthMapPara.SetValue( depthMap.GetMapTexture() );
                    raderAngPara.SetValue( rader.Ang );
                    //raderRPara.SetValue( Coordin.ScrnWidth( rader.R ) );
                    raderColorPara.SetValue( ColorHelper.ToFloat4( rader.RaderColor ) );
                    raderRotaMatrixPara.SetValue( rader.RotaMatrix  );
                    renderRaderEffect.CommitChanges();


                    BaseGame.Device.SetRenderTarget( 0, target );
                    //BaseGame.Device.SetRenderTarget( 1, targetSmall );
                    BaseGame.Device.Clear( Color.TransparentBlack );
                    //BaseGame.Device.Clear( Color.Black );

                    renderRaderEffect.CurrentTechnique = renderRaderEffect.Techniques[0];
                    renderRaderEffect.Begin();
                    renderRaderEffect.CurrentTechnique.Passes[0].Begin();

                    BaseGame.Device.VertexDeclaration = del;
                    BaseGame.Device.Vertices[0].SetSource( sectorBuffer, 0, del.GetVertexStrideSize( 0 ) );
                    BaseGame.Device.DrawPrimitives( PrimitiveType.TriangleFan, 0, partSum );

                    renderRaderEffect.CurrentTechnique.Passes[0].End();
                    renderRaderEffect.End();
                    BaseGame.Device.ResolveRenderTarget( 0 );


                    BaseGame.Device.SetRenderTarget( 0, targetSmall );

                    BaseGame.Device.Clear( Color.TransparentBlack );
                    //BaseGame.Device.Clear( Color.Black );

                    renderRaderEffect.CurrentTechnique = renderRaderEffect.Techniques[0];
                    renderRaderEffect.Begin();
                    renderRaderEffect.CurrentTechnique.Passes[0].Begin();

                    BaseGame.Device.VertexDeclaration = del;
                    BaseGame.Device.Vertices[0].SetSource( sectorBuffer, 0, del.GetVertexStrideSize( 0 ) );
                    BaseGame.Device.DrawPrimitives( PrimitiveType.TriangleFan, 0, partSum );

                    renderRaderEffect.CurrentTechnique.Passes[0].End();
                    renderRaderEffect.End();
                    BaseGame.Device.ResolveRenderTarget( 0 );
                    BaseGame.Device.SetRenderTarget( 0, null );



                    BaseGame.Device.RenderState.DepthBufferEnable = true;
                    BaseGame.Device.RenderState.DepthBufferWriteEnable = true;

                    Texture2D tex = target.GetTexture();
                    Texture2D texSmall = targetSmall.GetTexture();
                    return new Texture2D[] { tex, texSmall };
                }
                catch (Exception)
                {
                    return new Texture2D[2];
                }
            }

            #endregion
        }
    }
}
