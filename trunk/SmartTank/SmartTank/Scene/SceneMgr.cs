using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.GameObjs;
using System.Xml.Serialization;
using System.IO;
using TankEngine2D.Helpers;
using SmartTank.PhiCol;
using SmartTank.Shelter;
using SmartTank.Senses.Vision;
using Microsoft.Xna.Framework;
using SmartTank.Update;
using SmartTank.Draw;
using System.Xml;

namespace SmartTank.Scene
{
    /*
     * 对场景路径的说明：
     * 
     * 若TankGroup是SceneMgr的直接子组之一。
     * 
     * 而有一个名为Tank3的物体位于TankGroup下AlliedGroup中，
     * 则该物体的路径为"TankGroup\\AlliedGroup\\Tank3"
     * 
     * AlliedGroup组的路径则为"TankGroup\\AlliedGroup"。
     * 
     * */

    public class SceneMgr : ISceneKeeper
    {
        #region Serialize

        /// <summary>
        /// 序列化SceneMgr
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="sceneMgr"></param>
        /// <returns></returns>
        public static bool Save ( Stream stream, SceneMgr sceneMgr )
        {
            try
            {
                XmlWriter writer = XmlWriter.Create( stream );

                stream.Close();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="sceneFilePath"></param>
        /// <returns></returns>
        public static SceneMgr Load ( string sceneFilePath )
        {
            try
            {
                //XmlSerializer serializer = new XmlSerializer( typeof( SceneMgr ) );
                //Stream file = File.OpenRead( sceneFilePath );
                //file.Close();
                //return (SceneMgr)serializer.Deserialize( file );
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SceneMgr ()
        {
            phiGroups = new List<string>();
            colSinGroups = new List<string>();
            lapSinGroups = new List<string>();
            colPairGroups = new List<Pair>();
            lapPairGroups = new List<Pair>();
            shelterGroups = new List<MulPair>();
            visionGroups = new List<Pair>();
        }

        #region Groups and Objs

        protected Group groups;

        /// <summary>
        /// 获得一个组
        /// </summary>
        /// <param name="groupPath"></param>
        /// <returns></returns>
        public Group GetGroup ( string groupPath )
        {
            return FindGroup( groupPath );
        }

        /// <summary>
        /// 获得一个类型组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groupPath"></param>
        /// <returns></returns>
        public TypeGroup<T> GetTypeGroup<T> ( string groupPath ) where T : class, newIGameObj
        {
            Group result = FindGroup( groupPath );
            if (result != null && result is TypeGroup<T>)
                return result as TypeGroup<T>;
            else
                return null;
        }

        /// <summary>
        /// 制定场景物体的存放路径而获得场景物体
        /// </summary>
        /// <param name="objPath">物体的场景路径</param>
        /// <returns></returns>
        public newIGameObj GetGameObj ( string objPath )
        {
            return FindObj( objPath );
        }

        /// <summary>
        /// 添加组
        /// </summary>
        /// <param name="fatherPath">添加到的父组的路径，空则表示添加到根组下</param>
        /// <param name="group">要添加的组</param>
        /// <returns></returns>
        public bool AddGroup ( string fatherPath, Group group )
        {
            Group fatherGroup = FindGroup( fatherPath );
            if (fatherGroup != null)
            {
                if (fatherGroup.AddChildGroup( group ))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 添加物体
        /// </summary>
        /// <param name="groupPath">要添加到的组的路径，该组必须为TypeGroup</param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool AddGameObj ( string groupPath, newIGameObj obj )
        {
            Group fatherGroup = FindGroup( groupPath );
            if (fatherGroup != null && fatherGroup is TypeGroup)
            {
                return (fatherGroup as TypeGroup).AddObj( obj );
            }
            else
                return false;
        }

        /// <summary>
        /// 删除物体
        /// </summary>
        /// <param name="objPath">物体的路径</param>
        /// <returns></returns>
        public bool DelGameObj ( string objPath )
        {
            int i;
            for (i = objPath.Length - 1; i >= 0; i--)
            {
                if (objPath[i] == '\\')
                    break;
            }
            if (i > 0)
            {
                string groupPath = objPath.Substring( 0, i );
                string objName = objPath.Substring( i + 1 );
                return DelGameObj( groupPath, objName );
            }
            return false;
        }
        /// <summary>
        /// 删除物体
        /// </summary>
        /// <param name="groupPath">物体所在组的路径</param>
        /// <param name="objName">物体的名称</param>
        /// <returns></returns>
        public bool DelGameObj ( string groupPath, string objName )
        {
            Group fatherGroup = FindGroup( groupPath );
            if (fatherGroup != null && fatherGroup is TypeGroup)
                return (fatherGroup as TypeGroup).DelObj( objName );
            else
                return false;
        }

        #endregion

        #region Register groups to Game Components

        /// <summary>
        /// 表示组二元关系
        /// </summary>
        public struct Pair
        {
            public string groupPath1;
            public string groupPath2;

            public Pair ( string groupPath1, string groupPath2 )
            {
                this.groupPath1 = groupPath1;
                this.groupPath2 = groupPath2;
            }
        }
        /// <summary>
        /// 表示一对多的关系
        /// </summary>
        public struct MulPair
        {
            public string groupPath;
            public List<string> groupPaths;

            public MulPair ( string groupPath, List<string> groupPaths )
            {
                this.groupPath = groupPath;
                this.groupPaths = groupPaths;
            }
        }

        private List<string> phiGroups;
        private List<string> colSinGroups;
        private List<string> lapSinGroups;
        private List<Pair> colPairGroups;
        private List<Pair> lapPairGroups;

        private List<MulPair> shelterGroups;

        private List<Pair> visionGroups;

        /// <summary>
        /// 将注册的物理更新组
        /// </summary>
        public List<string> PhiGroups
        {
            get { return phiGroups; }
        }
        /// <summary>
        /// 将注册的单独碰撞组
        /// </summary>
        public List<string> ColSinGroups
        {
            get { return colSinGroups; }
        }
        /// <summary>
        /// 将注册的单独重叠组
        /// </summary>
        public List<string> LapSinGroups
        {
            get { return LapSinGroups; }
        }
        /// <summary>
        /// 将注册的碰撞组对
        /// </summary>
        public List<Pair> ColPairGroups
        {
            get { return colPairGroups; }
        }
        /// <summary>
        /// 将注册的重叠组对
        /// </summary>
        public List<Pair> LapPairGroups
        {
            get { return lapPairGroups; }
        }
        /// <summary>
        /// 将注册的遮挡关系
        /// </summary>
        public List<MulPair> ShelterGroups
        {
            get { return shelterGroups; }
        }
        /// <summary>
        /// 将注册的可视关系
        /// </summary>
        public List<Pair> VisionGroups
        {
            get { return visionGroups; }
        }

        /// <summary>
        /// 注册物理更新
        /// </summary>
        /// <param name="manager"></param>
        public void RegistPhiCol ( SmartTank.PhiCol.PhiColMgr manager )
        {
            foreach (string groupPath in phiGroups)
            {
                try
                {
                    TypeGroup group = FindGroup( groupPath ) as TypeGroup;
                    manager.AddPhiGroup( group.GetEnumerableCopy<IPhisicalObj>() );
                }
                catch (Exception ex)
                {
                    Log.Write( "RegistPhiGroup error: " + groupPath + ", " + ex.Message );
                }
            }

            foreach (string groupPath in colSinGroups)
            {
                try
                {
                    TypeGroup group = FindGroup( groupPath ) as TypeGroup;
                    manager.AddCollideGroup( group.GetEnumerableCopy<ICollideObj>() );
                }
                catch (Exception ex)
                {
                    Log.Write( "RegistColSinGroup error: " + groupPath + ", " + ex.Message );
                }
            }

            foreach (string groupPath in lapSinGroups)
            {
                try
                {
                    TypeGroup group = FindGroup( groupPath ) as TypeGroup;
                    manager.AddOverlapGroup( group.GetEnumerableCopy<ICollideObj>() );
                }
                catch (Exception ex)
                {
                    Log.Write( "RegistLapSinGroup error: " + groupPath + ", " + ex.Message );
                }
            }

            foreach (Pair pair in colPairGroups)
            {
                try
                {
                    TypeGroup group1 = FindGroup( pair.groupPath1 ) as TypeGroup;
                    TypeGroup group2 = FindGroup( pair.groupPath2 ) as TypeGroup;
                    manager.AddCollideGroup(
                        group1.GetEnumerableCopy<ICollideObj>(),
                        group2.GetEnumerableCopy<ICollideObj>() );
                }
                catch (Exception ex)
                {
                    Log.Write( "RegistColPairGroup error: " + pair.groupPath1 + ", " +
                        pair.groupPath2 + ", " + ex.Message );
                }
            }

            foreach (Pair pair in lapPairGroups)
            {
                try
                {
                    TypeGroup group1 = FindGroup( pair.groupPath1 ) as TypeGroup;
                    TypeGroup group2 = FindGroup( pair.groupPath2 ) as TypeGroup;
                    manager.AddOverlapGroup(
                        group1.GetEnumerableCopy<ICollideObj>(),
                        group2.GetEnumerableCopy<ICollideObj>() );
                }
                catch (Exception ex)
                {
                    Log.Write( "RegistLapPairGroup error: " + pair.groupPath1 + ", " +
                        pair.groupPath2 + ", " + ex.Message );
                }
            }
        }
        /// <summary>
        /// 注册遮挡关系
        /// </summary>
        /// <param name="manager"></param>
        public void RegistShelter ( SmartTank.Shelter.ShelterMgr manager )
        {
            foreach (MulPair pair in shelterGroups)
            {
                try
                {
                    List<IEnumerable<IShelterObj>> shelGroups = new List<IEnumerable<IShelterObj>>();

                    foreach (string shelGroupPath in pair.groupPaths)
                    {
                        TypeGroup shelGroup = FindGroup( shelGroupPath ) as TypeGroup;
                        shelGroups.Add( shelGroup.GetEnumerableCopy<IShelterObj>() );
                    }

                    TypeGroup group = FindGroup( pair.groupPath ) as TypeGroup;

                    manager.AddRaderShelterGroup(
                        group.GetEnumerableCopy<IRaderOwner>(),
                        shelGroups.ToArray() );
                }
                catch (Exception ex)
                {
                    Log.Write( "RegistShelterGroup error: " + pair.groupPath + ex.Message );
                }
            }
        }
        /// <summary>
        /// 注册可绘制组
        /// </summary>
        /// <param name="manager"></param>
        public void RegistDrawables ( SmartTank.Draw.DrawManager manager )
        {
            ForEachTypeGroup( groups, new ForEachTypeGroupHandler(
                delegate( TypeGroup typeGroup )
                {
                    manager.AddGroup( typeGroup.GetEnumerableCopy<IDrawableObj>() );
                } ) );
        }
        /// <summary>
        /// 注册可更新组
        /// </summary>
        /// <param name="manager"></param>
        public void RegistUpdaters ( SmartTank.Update.UpdateMgr manager )
        {
            ForEachTypeGroup( groups, new ForEachTypeGroupHandler(
                delegate( TypeGroup typeGroup )
                {
                    manager.AddGroup( typeGroup.GetEnumerableCopy<IUpdater>() );
                } ) );
        }
        /// <summary>
        /// 注册可视关系
        /// </summary>
        /// <param name="manager"></param>
        public void RegistVision ( SmartTank.Senses.Vision.VisionMgr manager )
        {
            foreach (Pair pair in visionGroups)
            {
                try
                {
                    TypeGroup group1 = FindGroup( pair.groupPath1 ) as TypeGroup;
                    TypeGroup group2 = FindGroup( pair.groupPath2 ) as TypeGroup;
                    manager.AddVisionGroup(
                        group1.GetEnumerableCopy<IRaderOwner>(),
                        group2.GetEnumerableCopy<IEyeableObj>() );
                }
                catch (Exception ex)
                {
                    Log.Write( "RegistVisionGroup error: " + pair.groupPath1 + ", " +
                        pair.groupPath2 + ", " + ex.Message );
                }
            }
        }

        #endregion

        #region Prviate Methods

        private Group FindGroup ( string groupPath )
        {
            Group curGroup = this.groups;
            string[] paths;
            paths = groupPath.Split( '\\' );
            foreach (string path in paths)
            {
                curGroup = curGroup.GetChildGroup( path );
                if (curGroup == null)
                {
                    Log.Write( "FindGroup: 错误的组路径被传入或不存在匹配组" );
                    return null;
                }
            }
            return curGroup;
        }

        private newIGameObj FindObj ( string objPath )
        {
            Group curGroup = this.groups;
            string[] paths;
            paths = objPath.Split( '\\' );
            for (int i = 0; i < paths.Length - 1; i++)
            {
                curGroup = curGroup.GetChildGroup( paths[i] );
                if (curGroup == null)
                {
                    Log.Write( "FindObj: 错误的组路径被传入或不存在匹配组" );
                    return null;
                }
            }
            if (curGroup != null && curGroup is TypeGroup)
            {
                return (curGroup as TypeGroup).GetObj( paths[paths.Length - 1] );
            }
            return null;
        }

        private delegate void ForEachTypeGroupHandler ( TypeGroup group );

        private static void ForEachTypeGroup ( Group group, ForEachTypeGroupHandler handler )
        {
            if (group is TypeGroup)
                handler( group as TypeGroup );

            foreach (KeyValuePair<string, Group> child in group.Childs)
            {
                ForEachTypeGroup( child.Value, handler );
            }
        }

        #endregion

    }
}
