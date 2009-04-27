using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace Common.Helpers
{
    public static class Directories
    {
        #region Game base directory
        public static readonly string GameBaseDirectory =
            StorageContainer.TitleLocation;

        #endregion

        #region Directories

        public static string ContentDirectory
        {
            get { return Path.Combine( GameBaseDirectory, "Content" ); }
        }

        public static string BasicGraphicsContent
        {
            get { return Path.Combine( ContentDirectory, "BasicGraphics" ); }
        }

        public static string FontContent
        {
            get { return Path.Combine( ContentDirectory, "Font" ); }
        }

        public static string UIContent
        {
            get { return Path.Combine( ContentDirectory, "UI" ); }
        }

        public static string TankTexture
        {
            get { return Path.Combine( ContentDirectory, "Tanks" ); }
        }

        public static string SoundDirectory
        {
            get { return Path.Combine( GameBaseDirectory, "Content\\Sounds" ); }
        }

        public static string AIDirectory
        {
            get { return Path.Combine( GameBaseDirectory, "AI" ); }
        }

        public static string MapDirectory
        {
            get { return Path.Combine( GameBaseDirectory, "Map" ); }
        }

        public static string GameObjsDirectory
        {
            get { return Path.Combine( GameBaseDirectory, "GameObjs" ); }
        }

        #endregion

        #region FilePath

        public static string AIListFilePath
        {
            get { return Path.Combine( AIDirectory, "AIList.txt" ); }
        }

        #endregion
    }
}
