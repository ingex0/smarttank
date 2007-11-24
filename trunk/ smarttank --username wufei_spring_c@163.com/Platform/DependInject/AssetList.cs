using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using GameBase.Helpers;

namespace Platform.DependInject
{
    public struct NameAndTypeName
    {
        public string name;
        public string typeNames;

        public NameAndTypeName ( string name, string typeNames )
        {
            this.name = name;
            this.typeNames = typeNames;
        }
    }

    public struct AssetItem
    {
        public string DLLName;
        public NameAndTypeName[] names;

        public AssetItem ( string DLLName, NameAndTypeName[] names )
        {
            this.DLLName = DLLName;
            this.names = names;
        }
    }

    public struct TypeAssetPath
    {
        public string DLLName;
        public string typeName;
        public string name;

        public TypeAssetPath ( string DLLName, string typeName, string name )
        {
            this.DLLName = DLLName;
            this.typeName = typeName;
            this.name = name;
        }
    }

    public class AssetList
    {
        #region Type Def


        #endregion

        #region statics
        static XmlSerializer serializer;

        static public void Save ( Stream stream, AssetList data )
        {
            if (serializer == null)
                serializer = new XmlSerializer( typeof( AssetList ) );

            try
            {
                serializer.Serialize( stream, data );
            }
            catch (Exception)
            {
                Log.Write( "Save AssetList error!" );
            }
            finally
            {
                stream.Close();
            }
        }

        static public AssetList Load ( Stream stream )
        {
            if (serializer == null)
            {
                serializer = new XmlSerializer( typeof( AssetList ) );
            }
            AssetList result = null;
            try
            {
                result = (AssetList)serializer.Deserialize( stream );
            }
            catch (Exception)
            {
                Log.Write( "Load AssetList error!" );
            }
            finally
            {
                stream.Close();
            }
            return result;
        }
        #endregion

        public List<AssetItem> list = new List<AssetItem>();

        List<TypeAssetPath> pathList;

        public AssetList ()
        {

        }

        public string[] GetTypeList ()
        {
            if (pathList == null)
            {
                InitialPathList();
            }

            List<string> result = new List<string>();
            foreach (TypeAssetPath assetPath in pathList)
            {
                result.Add( assetPath.name );
            }
            return result.ToArray();
        }

        public TypeAssetPath GetTypeAssetPath ( int index )
        {
            if (pathList == null)
                InitialPathList();

            if (index >= pathList.Count || index < 0)
                return new TypeAssetPath();

            return pathList[index];
        }

        public int IndexOf ( string DLLName )
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].DLLName.Equals( DLLName ))
                {
                    return i;
                }
            }
            return -1;
        }

        private void InitialPathList ()
        {
            pathList = new List<TypeAssetPath>();
            foreach (AssetItem item in list)
            {
                foreach (NameAndTypeName names in item.names)
                {
                    pathList.Add( new TypeAssetPath( item.DLLName, names.typeNames, names.name ) );
                }
            }
        }
    }
}
