using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TankEngine2D.Graphics
{
    /// <summary>
    /// 精灵绘制的管理者
    /// </summary>
    public class SpriteMgr
    {
        /// <summary>
        /// AlphaSprite
        /// </summary>
        public SpriteBatch alphaSprite;
        /// <summary>
        /// AdditiveSprite
        /// </summary>
        public SpriteBatch additiveSprite;

        GraphicsDevice device;

        internal SpriteMgr ( RenderEngine engine )
        {
            this.device = engine.Device;
            alphaSprite = new SpriteBatch( device );
            additiveSprite = new SpriteBatch( device );
        }

        internal GraphicsDevice Device
        {
            get { return device; }
        }

        internal void HandleDeviceReset ()
        {
            Intial( device );
        }

        private void Intial ( GraphicsDevice device )
        {
            alphaSprite = new SpriteBatch( device );
            additiveSprite = new SpriteBatch( device );
        }

        /// <summary>
        /// alphaSprite.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None );
        /// additiveSprite.Begin( SpriteBlendMode.Additive, SpriteSortMode.BackToFront, SaveStateMode.None );
        /// </summary>
        internal void SpriteBatchBegin ()
        {
            alphaSprite.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None );
            additiveSprite.Begin( SpriteBlendMode.Additive, SpriteSortMode.BackToFront, SaveStateMode.None );
        }

        /// <summary>
        /// Sprite.alphaSprite.End();
        /// Sprite.additiveSprite.End();
        /// </summary>
        internal void SpriteBatchEnd ()
        {
            alphaSprite.End();
            additiveSprite.End();
        }
    }
}
