using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmartTank.GameObjs.Tank
{
    public class TurretLinker
    {
        Vector2 refAxesToOri;
        float scale;
        Vector2 tankOriInLogic;
        Vector2 turretAxesInLogic;

        /// <summary>
        /// 新建一个TurretLinker对象
        /// </summary>
        /// <param name="turretAxesInPix"></param>
        /// <param name="tankOriInPix"></param>
        /// <param name="scale">逻辑尺寸/原图尺寸</param>
        public TurretLinker ( Vector2 turretAxesInPix, Vector2 tankOriInPix, float scale )
        {
            this.scale = scale;
            this.tankOriInLogic = tankOriInPix * scale;
            this.turretAxesInLogic = turretAxesInPix * scale;
            refAxesToOri = turretAxesInLogic - tankOriInLogic;
        }

        public Vector2 GetTexturePos ( Vector2 basePos, float baseRota )
        {
            return basePos + Vector2.Transform( refAxesToOri, Matrix.CreateRotationZ( baseRota ) );
        }

        public void SetTurretAxes ( Vector2 turretAxesInPix )
        {
            Vector2 turretAxesInLogic = turretAxesInPix * scale;
            refAxesToOri = turretAxesInLogic - tankOriInLogic;
        }
    }
}
