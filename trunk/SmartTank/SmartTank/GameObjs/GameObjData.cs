using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using System.IO;
using TankEngine2D.Helpers;
using System.Collections;
using System.Xml;

namespace SmartTank.GameObjects
{
    public class GameObjDataNode : IEnumerable<GameObjDataNode>
    {
        public string nodeName;
        public List<string> texPaths;
        public List<Vector2> visiKeyPoints;
        public List<Vector2> structKeyPoints;

        // 暂时只添加这两类数据格式
        public List<int> intDatas;
        public List<float> floatDatas;

        public GameObjDataNode parent;
        public List<GameObjDataNode> childNodes;

        public GameObjDataNode()
        {
            texPaths = new List<string>();
            visiKeyPoints = new List<Vector2>();
            structKeyPoints = new List<Vector2>();
            childNodes = new List<GameObjDataNode>();

            intDatas = new List<int>();
            floatDatas = new List<float>();
        }

        public GameObjDataNode( string nodeName )
            : this( nodeName, null )
        {
        }

        public GameObjDataNode( string nodeName, GameObjDataNode parent )
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
        public IEnumerator<GameObjDataNode> GetEnumerator()
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

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public void WriteToXML( XmlWriter writer )
        {
            writer.WriteStartElement( "GameObjDataNode" );

            writer.WriteElementString( "NodeName", nodeName );

            writer.WriteStartElement( "TexPaths" );
            writer.WriteElementString( "count", texPaths.Count.ToString() );
            foreach (string texPath in texPaths)
            {
                writer.WriteElementString( "string", texPath );
            }
            writer.WriteEndElement();

            writer.WriteStartElement( "VisiKeyPoints" );
            writer.WriteElementString( "count", visiKeyPoints.Count.ToString() );
            foreach (Vector2 visiPoint in visiKeyPoints)
            {
                writer.WriteStartElement( "Vector2" );
                writer.WriteElementString( "X", visiPoint.X.ToString() );
                writer.WriteElementString( "Y", visiPoint.Y.ToString() );
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement( "StructKeyPoints" );
            writer.WriteElementString( "count", structKeyPoints.Count.ToString() );
            foreach (Vector2 structPoint in structKeyPoints)
            {
                writer.WriteStartElement( "Vector2" );
                writer.WriteElementString( "X", structPoint.X.ToString() );
                writer.WriteElementString( "Y", structPoint.Y.ToString() );
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement( "intDatas" );
            writer.WriteElementString( "count", intDatas.Count.ToString() );
            foreach (int i in intDatas)
            {
                writer.WriteElementString( "int", i.ToString() );
            }
            writer.WriteEndElement();

            writer.WriteStartElement( "floatDatas" );
            writer.WriteElementString( "count", floatDatas.Count.ToString() );
            foreach (float f in floatDatas)
            {
                writer.WriteElementString( "float", f.ToString() );
            }
            writer.WriteEndElement();

            writer.WriteStartElement( "Childs" );
            writer.WriteElementString( "count", childNodes.Count.ToString() );
            foreach (GameObjDataNode child in childNodes)
            {
                child.WriteToXML( writer );
            }
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        static public GameObjDataNode ReadFromXML( XmlReader reader )
        {
            GameObjDataNode result = new GameObjDataNode();

            reader.ReadStartElement( "GameObjDataNode" );

            result.nodeName = reader.ReadElementString( "NodeName" );

            reader.ReadStartElement( "TexPaths" );
            int countTex = int.Parse( reader.ReadElementString( "count" ) );
            for (int i = 0; i < countTex; i++)
            {
                result.texPaths.Add( reader.ReadElementString( "string" ) );
            }
            reader.ReadEndElement();

            reader.ReadStartElement( "VisiKeyPoints" );
            int countVisi = int.Parse( reader.ReadElementString( "count" ) );
            for (int i = 0; i < countVisi; i++)
            {
                reader.ReadStartElement( "Vector2" );
                float x = float.Parse( reader.ReadElementString( "X" ) );
                float y = float.Parse( reader.ReadElementString( "Y" ) );
                reader.ReadEndElement();

                result.visiKeyPoints.Add( new Vector2( x, y ) );
            }
            reader.ReadEndElement();

            reader.ReadStartElement( "StructKeyPoints" );
            int countStruct = int.Parse( reader.ReadElementString( "count" ) );
            for (int i = 0; i < countStruct; i++)
            {
                reader.ReadStartElement( "Vector2" );
                float x = float.Parse( reader.ReadElementString( "X" ) );
                float y = float.Parse( reader.ReadElementString( "Y" ) );
                reader.ReadEndElement();

                result.structKeyPoints.Add( new Vector2( x, y ) );
            }
            reader.ReadEndElement();

            reader.ReadStartElement( "intDatas" );
            int countInt = int.Parse( reader.ReadElementString( "count" ) );
            for (int i = 0; i < countInt; i++)
            {
                result.intDatas.Add( int.Parse( reader.ReadElementString( "int" ) ) );
            }
            reader.ReadEndElement();

            reader.ReadStartElement( "floatDatas" );
            int countFloat = int.Parse( reader.ReadElementString( "count" ) );
            for (int i = 0; i < countFloat; i++)
            {
                result.floatDatas.Add( float.Parse( reader.ReadElementString( "float" ) ) );
            }
            reader.ReadEndElement();

            reader.ReadStartElement( "Childs" );
            int countChild = int.Parse( reader.ReadElementString( "count" ) );
            for (int i = 0; i < countChild; i++)
            {
                result.childNodes.Add( GameObjDataNode.ReadFromXML( reader ) );
            }
            reader.ReadEndElement();

            reader.ReadEndElement();

            return result;
        }
    }

    public class GameObjData
    {
        #region statics

        static public void Save( Stream stream, GameObjData data )
        {
            try
            {
                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                XmlWriter writer = XmlWriter.Create( stream, setting );

                writer.WriteStartElement( "GameObjData" );
                writer.WriteElementString( "Name", data.name );
                writer.WriteElementString( "Creater", data.creater );
                writer.WriteElementString( "Year", data.year.ToString() );
                writer.WriteElementString( "Month", data.month.ToString() );
                writer.WriteElementString( "Day", data.day.ToString() );

                data.baseNode.WriteToXML( writer );

                writer.WriteEndElement();

                writer.Flush();
            }
            catch (Exception e)
            {
                Log.Write( "Save GameObjData error!" + e.Message );
            }
            finally
            {
                stream.Close();
            }
        }

        static public GameObjData Load( Stream stream )
        {
            GameObjData result = new GameObjData();

            try
            {
                XmlReader reader = XmlReader.Create( stream );

                reader.ReadStartElement( "GameObjData" );
                result.name = reader.ReadElementString( "Name" );
                result.creater = reader.ReadElementString( "Creater" );
                result.year = int.Parse( reader.ReadElementString( "Year" ) );
                result.month = int.Parse( reader.ReadElementString( "Month" ) );
                result.day = int.Parse( reader.ReadElementString( "Day" ) );

                result.baseNode = GameObjDataNode.ReadFromXML( reader );

                reader.ReadEndElement();
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

        public GameObjData()
        {

        }

        public GameObjData( string objName )
        {
            this.name = objName;
            //this.baseNode = new GameObjDataNode( "Base", null );
        }
    }
}
