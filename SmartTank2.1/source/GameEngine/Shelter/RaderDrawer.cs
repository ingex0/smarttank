using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
using Common.Helpers;

namespace GameEngine.Shelter
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
            EffectParameter raderRPara;
            EffectParameter raderColorPara;
            EffectParameter raderRotaMatrixPara;

            DepthStencilBuffer depthBuffer;
            DepthStencilBuffer depthBufferSmall;

            #endregion

            #region Construction

            public RaderDrawer()
            {
                del = new VertexDeclaration( BaseGame.Device, new VertexElement[]{
                new VertexElement(0,0, VertexElementFormat.Vector3,
                VertexElementMethod.Default, VertexElementUsage.Position,0)} );

                sectorBuffer = new VertexBuffer( BaseGame.Device, typeof( Vector3 ),
                    partSum + 2, BufferUsage.WriteOnly );

                InitialVertexBuffer();

                depthBuffer = new DepthStencilBuffer( BaseGame.Device, texSize, texSize, BaseGame.Device.DepthStencilBuffer.Format );
                depthBufferSmall = new DepthStencilBuffer( BaseGame.Device, texSizeSmall, texSizeSmall, BaseGame.Device.DepthStencilBuffer.Format );

                LoadEffect();
            }

            private void LoadEffect()
            {
                renderRaderEffect = BaseGame.ContentMgr.Load<Effect>( Path.Combine( Directories.ContentDirectory, "HLSL\\RaderMap" ) );

                depthMapPara = renderRaderEffect.Parameters["DepthMap"];
                raderAngPara = renderRaderEffect.Parameters["RaderAng"];
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

            
            public void DrawRader( Rader rader )
            {
                try
                {
                    BaseGame.Device.RenderState.DepthBufferEnable = false;
                    BaseGame.Device.RenderState.DepthBufferWriteEnable = false;
                    BaseGame.Device.RenderState.AlphaBlendEnable = false;

                    Texture2D depthMapTex = rader.depthMap.GetMapTexture();
                    depthMapPara.SetValue( depthMapTex );

                    raderAngPara.SetValue( rader.Ang );
                    raderColorPara.SetValue( ColorHelper.ToFloat4( rader.RaderColor ) );
                    raderRotaMatrixPara.SetValue( rader.RotaMatrix );
                    renderRaderEffect.CommitChanges();

                    DepthStencilBuffer old = BaseGame.Device.DepthStencilBuffer;

                    BaseGame.Device.SetRenderTarget( 0, rader.target );
                    BaseGame.Device.DepthStencilBuffer = depthBuffer;

                    BaseGame.Device.Clear( Color.TransparentBlack );

                    renderRaderEffect.CurrentTechnique = renderRaderEffect.Techniques[0];
                    renderRaderEffect.Begin();
                    renderRaderEffect.CurrentTechnique.Passes[0].Begin();

                    BaseGame.Device.VertexDeclaration = del;
                    BaseGame.Device.Vertices[0].SetSource( sectorBuffer, 0, del.GetVertexStrideSize( 0 ) );
                    BaseGame.Device.DrawPrimitives( PrimitiveType.TriangleFan, 0, partSum );

                    renderRaderEffect.CurrentTechnique.Passes[0].End();
                    renderRaderEffect.End();

                    BaseGame.Device.SetRenderTarget( 0, rader.targetSmall );
                    BaseGame.Device.DepthStencilBuffer = depthBufferSmall;

                    BaseGame.Device.Clear( Color.TransparentBlack );

                    renderRaderEffect.CurrentTechnique = renderRaderEffect.Techniques[0];
                    renderRaderEffect.Begin();
                    renderRaderEffect.CurrentTechnique.Passes[0].Begin();

                    BaseGame.Device.VertexDeclaration = del;
                    BaseGame.Device.Vertices[0].SetSource( sectorBuffer, 0, del.GetVertexStrideSize( 0 ) );
                    BaseGame.Device.DrawPrimitives( PrimitiveType.TriangleFan, 0, partSum );

                    renderRaderEffect.CurrentTechnique.Passes[0].End();
                    renderRaderEffect.End();
                    BaseGame.Device.SetRenderTarget( 0, null );

                    BaseGame.Device.DepthStencilBuffer = old;

                    BaseGame.Device.RenderState.DepthBufferEnable = true;
                    BaseGame.Device.RenderState.DepthBufferWriteEnable = true;
                }
                catch (Exception ex)
                {
                    Log.Write( ex.Message );
                }
            }

            #endregion
        }
    }
}
