using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Platform.GameObjects.Tank.TankSkins
{
    public struct TankSkinSinTurData
    {
        public static TankSkinSinTurData M1A2 = new TankSkinSinTurData(
            "M1A2\\base", "M1A2\\turret", 0.1f,
            new Vector2( 43, 100 ), new Vector2( 43, 100 ), new Vector2( 38, 145 ), 145,
            3, 7, 2 );

        public static TankSkinSinTurData Tiger = new TankSkinSinTurData(
            "Tiger\\base", "Tiger\\turret", 0.12f,
            new Vector2( 36, 59 ), new Vector2( 36, 62 ), new Vector2( 21, 101 ), 101, 3, 7, 2 );

        public static TankSkinSinTurData M60 = new TankSkinSinTurData(
            "M60\\base", "M60\\turret", 0.1f,
            new Vector2( 41, 81 ), new Vector2( 41, 68 ), new Vector2( 35, 131 ), 131, 3, 7, 2 );

        #region Variables

        readonly public string baseTexPath;
        readonly public string turretTexPath;

        /// <summary>
        /// Âß¼­³ß´ç/Ô­Í¼³ß´ç
        /// </summary>
        readonly public float texScale;

        readonly public Vector2 baseTexOrigin;
        readonly public Vector2 turretAxesPos;
        readonly public Vector2 turretTexOrigin;
        readonly public float TurretTexels;

        readonly public float recoilTexels;

        readonly public int backFrame;
        readonly public int recoilFrame;
        #endregion

        #region Construction
        public TankSkinSinTurData ( string baseTexPath, string turretTexPath, float texScale,
            Vector2 baseTexOrigin, Vector2 turretAxesPos, Vector2 turretTexOrigin,
            float TurretTexels, int backFrame, int recoilFrame, float recoilTexels )
        {
            this.baseTexPath = baseTexPath;
            this.turretTexPath = turretTexPath;
            this.texScale = texScale;
            this.baseTexOrigin = baseTexOrigin;
            this.turretAxesPos = turretAxesPos;
            this.turretTexOrigin = turretTexOrigin;
            this.TurretTexels = TurretTexels;
            this.recoilTexels = recoilTexels;
            this.backFrame = backFrame;
            this.recoilFrame = recoilFrame;
        }
        #endregion
    }
}
