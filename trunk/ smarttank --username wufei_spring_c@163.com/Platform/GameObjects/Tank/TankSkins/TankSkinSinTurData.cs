using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using GameBase.Helpers;

namespace Platform.GameObjects.Tank.TankSkins
{
    struct TankSkinSinTurData
    {
        #region Old Code

        //public static TankSkinSinTurData M1A2 = new TankSkinSinTurData(
        //    "M1A2\\base", "M1A2\\turret", 0.1f,
        //    new Vector2( 43, 100 ), new Vector2( 43, 100 ), new Vector2( 38, 145 ), 145,
        //    3, 7, 2 );

        //public static TankSkinSinTurData Tiger = new TankSkinSinTurData(
        //    "Tiger\\base", "Tiger\\turret", 0.12f,
        //    new Vector2( 36, 59 ), new Vector2( 36, 62 ), new Vector2( 21, 101 ), 101, 3, 7, 2 );

        //public static TankSkinSinTurData M60 = new TankSkinSinTurData(
        //    "M60\\base", "M60\\turret", 0.1f,
        //    new Vector2( 41, 81 ), new Vector2( 41, 68 ), new Vector2( 35, 131 ), 131, 3, 7, 2 );

        //public static TankSkinSinTurData M1A2 = CreateM1A2();

        //public static TankSkinSinTurData Tiger = CreateTiger();

        //public static TankSkinSinTurData M60 = CreateM60();

        //private static TankSkinSinTurData CreateM1A2 ()
        //{
        //    string path = Path.Combine( Directories.ItemDirectory, "Internal\\M1A2" );
        //    GameObjData data = GameObjData.Load( File.OpenRead( Path.Combine( path, "M1A2.xml" ) ) );

        //    return new TankSkinSinTurData( path, data );
        //}

        //private static TankSkinSinTurData CreateTiger ()
        //{
        //    string path = Path.Combine( Directories.ItemDirectory, "Internal\\Tiger" );
        //    GameObjData data = GameObjData.Load( File.OpenRead( Path.Combine( path, "Tiger.xml" ) ) );

        //    return new TankSkinSinTurData( path, data );
        //}

        //private static TankSkinSinTurData CreateM60 ()
        //{
        //    string path = Path.Combine( Directories.ItemDirectory, "Internal\\M60" );
        //    GameObjData data = GameObjData.Load( File.OpenRead( Path.Combine( path, "M60.xml" ) ) );

        //    return new TankSkinSinTurData( path, data );
        //}

        #endregion

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

        readonly public Vector2[] visiKeyPoints;

        //readonly public string texPath;

        #endregion

        #region Construction
        public TankSkinSinTurData ( string baseTexPath, string turretTexPath, float texScale,
            Vector2 baseTexOrigin, Vector2 turretAxesPos, Vector2 turretTexOrigin,
            float TurretTexels, Vector2[] visiKeyPoints, int backFrame, int recoilFrame, float recoilTexels )
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
            this.visiKeyPoints = visiKeyPoints;
        }

        public TankSkinSinTurData ( string texPath, GameObjData data )
            : this( Path.Combine( texPath, data.baseNode.texPaths[0] ), Path.Combine( texPath, data.baseNode.childNodes[0].texPaths[0] ), data.baseNode.floatDatas[0],
            data.baseNode.structKeyPoints[0], data.baseNode.structKeyPoints[1], data.baseNode.childNodes[0].structKeyPoints[0],
            data.baseNode.childNodes[0].structKeyPoints[0].Y, data.baseNode.visiKeyPoints.ToArray(), 3, 7, data.baseNode.intDatas[0] )
        {

        }

        #endregion
    }
}
