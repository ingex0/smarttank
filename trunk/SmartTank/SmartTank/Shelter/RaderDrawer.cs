using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using TankEngine2D.Helpers;
using SmartTank.Helpers;

namespace SmartTank.Shelter
{
    public partial class ShelterMgr
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

            public RaderDrawer()
            {
                del = new VertexDeclaration( BaseGame.Device, new VertexElement[]{
                new VertexElement(0,0, VertexElementFormat.Vector3,
                VertexElementMethod.Default, VertexElementUsage.Position,0)} );

                sectorBuffer = new VertexBuffer( BaseGame.Device, typeof( Vector3 ),
                    partSum + 2, ResourceUsage.WriteOnly );

                InitialVertexBuffer();

                LoadEffect();
            }

            private void LoadEffect()
            {
                renderRaderEffect = BaseGame.ContentMgr.Load<Effect>( Path.Combine( Directories.ContentDirectory, "HLSL\\RaderMap" ) );

                depthMapPara = renderRaderEffect.Parameters["DepthMap"];
                raderAngPara = renderRaderEffect.Parameters["RaderAng"];
                //targetSizePara = renderRaderEffect.Parameters["TargetSize"];
                raderRPara = renderRaderEffect.Parameters["RaderR"];
                raderColorPara = renderRaderEffect.Parameters["RaderColor"];
                raderRotaMatrixPara = renderRaderEffect.Parameters["RaderRotaMatrix"];

            }

            private void InitialVertexBuffer()
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
             * 这段代码到2.0以后就会在运行过程中越来越慢
             * 
             * */
            public Texture2D[] DrawRader( Rader rader, RaderDepthMap depthMap )
            {
                try
                {
                    BaseGame.Device.RenderState.DepthBufferEnable = false;
                    BaseGame.Device.RenderState.DepthBufferWriteEnable = false;
                    BaseGame.Device.RenderState.AlphaBlendEnable = false;

                    int targetSize = BaseGame.CoordinMgr.ScrnLength( rader.R ) * 2;

                    RenderTarget2D target = new RenderTarget2D( BaseGame.Device,
                        targetSize, targetSize,
                        1, BaseGame.Device.PresentationParameters.BackBufferFormat,
                        BaseGame.Device.PresentationParameters.MultiSampleType,
                        BaseGame.Device.PresentationParameters.MultiSampleQuality );

                    //DepthStencilBuffer stencil = new DepthStencilBuffer( target.GraphicsDevice,
                    //    targetSize, targetSize, target.GraphicsDevice.DepthStencilBuffer.Format,
                    //    target.MultiSampleType, target.MultiSampleQuality );


                    int targetSizeSmall = (int)(targetSize * Rader.smallMapScale);

                    RenderTarget2D targetSmall = new RenderTarget2D( BaseGame.Device,
                        targetSizeSmall, targetSizeSmall,
                        1, BaseGame.Device.PresentationParameters.BackBufferFormat,
                        BaseGame.Device.PresentationParameters.MultiSampleType,
                        BaseGame.Device.PresentationParameters.MultiSampleQuality );

                    //DepthStencilBuffer stencilSmall = new DepthStencilBuffer( targetSmall.GraphicsDevice,
                    //    targetSizeSmall, targetSizeSmall, targetSmall.GraphicsDevice.DepthStencilBuffer.Format,
                    //    targetSmall.MultiSampleType, targetSmall.MultiSampleQuality );

                    Texture2D depthMapTex = depthMap.GetMapTexture();
                    depthMapPara.SetValue( depthMapTex );

                    raderAngPara.SetValue( rader.Ang );
                    //raderRPara.SetValue( Coordin.ScrnWidth( rader.R ) );
                    raderColorPara.SetValue( ColorHelper.ToFloat4( rader.RaderColor ) );
                    raderRotaMatrixPara.SetValue( rader.RotaMatrix );
                    renderRaderEffect.CommitChanges();

                    //DepthStencilBuffer old = BaseGame.Device.DepthStencilBuffer;

                    BaseGame.Device.SetRenderTarget( 0, target );
                    //BaseGame.Device.DepthStencilBuffer = stencil;

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
                    //BaseGame.Device.DepthStencilBuffer = stencilSmall;

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

                    //BaseGame.Device.DepthStencilBuffer = old;

                    BaseGame.Device.RenderState.DepthBufferEnable = true;
                    BaseGame.Device.RenderState.DepthBufferWriteEnable = true;

                    Texture2D tex = target.GetTexture();
                    Texture2D texSmall = targetSmall.GetTexture();

                    //stencil.Dispose();
                    //stencilSmall.Dispose();
                    //target.Dispose();
                    //targetSmall.Dispose();
                    //depthMapTex.Dispose();

                    return new Texture2D[] { tex, texSmall };
                }
                catch (Exception ex)
                {
                    Log.Write( ex.Message );
                    return new Texture2D[2];
                }
            }

            #endregion
        }
    }
}
