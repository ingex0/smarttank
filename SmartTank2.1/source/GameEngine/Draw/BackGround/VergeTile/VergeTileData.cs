using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Common.Helpers;

namespace GameEngine.Draw.BackGround.VergeTile
{
    public class VergeTileData
    {
        #region statics

        static XmlSerializer serializer;

        public static void Save ( Stream stream, VergeTileData data )
        {
            if (serializer == null)
                serializer = new XmlSerializer( typeof( VergeTileData ) );

            try
            {
                serializer.Serialize( stream, data );
            }
            catch (Exception)
            {
                Log.Write( "Save VergeTileData error!" );
            }
            finally
            {
                stream.Close();
            }
        }

        public static VergeTileData Load ( Stream stream )
        {
            if (serializer == null)
                serializer = new XmlSerializer( typeof( VergeTileData ) );

            VergeTileData result = null;
            try
            {
                result = (VergeTileData)serializer.Deserialize( stream );
            }
            catch (Exception)
            {
                Log.Write( "Load VergeTileData error!" );
            }
            finally
            {
                stream.Close();
            }
            return result;
        }

        #endregion

        #region Variables

        public int gridWidth;

        public int gridHeight;

        /// <summary>
        /// 每个顶点处的纹理图索引
        /// sum = (gridWidth + 1) * (gridHeight + 1);
        /// index = x + (gridWidth + 1) * y;
        /// </summary>
        public int[] vertexTexIndexs;

        /// <summary>
        /// 每个格子中的纹理图索引
        /// </summary>
        public int[] gridTexIndexs;

        /// <summary>
        /// 纹理图地址
        /// </summary>
        public string[] texPaths;

        #endregion

        public VergeTileData ()
        {

        }

        public void SetRondomVertexIndex ( int gridWidth, int gridHeight, int usedTexSum )
        {
            vertexTexIndexs = new int[(gridWidth + 1) * (gridHeight + 1)];
            for (int i = 0; i < vertexTexIndexs.Length; i++)
            {
                vertexTexIndexs[i] = RandomHelper.GetRandomInt( 0, usedTexSum );
            }

        }

        public void SetRondomGridIndex ( int gridWidth, int gridHeight )
        {
            gridTexIndexs = new int[gridWidth * gridHeight];
            for (int i = 0; i < gridTexIndexs.Length; i++)
            {
                gridTexIndexs[i] = RandomHelper.GetRandomInt( 0, 17 );
            }
        }
    }
}
