using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using System.IO;
using GameBase.Helpers;

namespace Platform.GameObjects
{
    public class GameObjDataNode : IEnumerable<GameObjDataNode>
    {
        public string nodeName;
        public List<string> texPaths;
        public List<Vector2> visiKeyPoint;
        public List<Vector2> structKeyPoint;

        public GameObjDataNode parent;
        public List<GameObjDataNode> childNodes;

        public GameObjDataNode ()
        {
            texPaths = new List<string>();
            visiKeyPoint = new List<Vector2>();
            structKeyPoint = new List<Vector2>();
            childNodes = new List<GameObjDataNode>();
        }

        public GameObjDataNode ( string nodeName, GameObjDataNode parent )
            : this()
        {
            this.nodeName = nodeName;
            this.parent = parent;
        }

        #region IEnumerable<GameObjDataNode> 成员

        /// <summary>
        /// 采用前序遍历
        /// </summary>
        /// <returns></returns>
        public IEnumerator<GameObjDataNode> GetEnumerator ()
        {
            yield return this;

            if (childNodes != null)
            {
                foreach (GameObjDataNode childNode in childNodes)
                {
                    foreach (GameObjDataNode node in childNode)
                    {
                        yield return node;
                    }
                }
            }
        }



        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        #endregion
    }

    public class GameObjData
    {
        #region statics

        static XmlSerializer serializer;

        static public void Save ( Stream stream, GameObjData data )
        {
            if (serializer == null)
                serializer = new XmlSerializer( typeof( GameObjData ) );

            try
            {
                serializer.Serialize( stream, data );
            }
            catch (Exception)
            {
                Log.Write( "Save GameObjData error!" );
            }
            finally
            {
                stream.Close();
            }
        }

        static public GameObjData Load ( Stream stream )
        {
            if (serializer == null)
                serializer = new XmlSerializer( typeof( GameObjData ) );

            GameObjData result = null;

            try
            {
                result = (GameObjData)serializer.Deserialize( stream );
            }
            catch (Exception)
            {
                Log.Write( "Load GameObjData error!" );
            }
            finally
            {
                stream.Close();
            }

            return result;
        }

        #endregion

        public string name;
        public string creater;
        public int year;
        public int month;
        public int day;

        public GameObjDataNode baseNode;

        public GameObjDataNode this[int index]
        {
            get
            {
                int i = 0;
                foreach (GameObjDataNode node in baseNode)
                {
                    if (i == index)
                        return node;
                    i++;
                }
                return null;
            }
        }

        public GameObjData ()
        {

        }

        public GameObjData ( string objName )
        {
            this.name = objName;
            this.baseNode = new GameObjDataNode( "Base", null );
        }
    }
}
