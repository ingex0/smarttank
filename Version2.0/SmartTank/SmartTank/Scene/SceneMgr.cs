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
     * �Գ���·����˵����
     * 
     * ��TankGroup��SceneMgr��ֱ������֮һ��
     * 
     * ����һ����ΪTank3������λ��TankGroup��AlliedGroup�У�
     * ��������·��Ϊ"TankGroup\\AlliedGroup\\Tank3"
     * 
     * AlliedGroup���·����Ϊ"TankGroup\\AlliedGroup"��
     * 
     * */

    public class SceneMgr : ISceneKeeper
    {
        #region Serialize

        /// <summary>
        /// ���л�SceneMgr
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="sceneMgr"></param>
        /// <returns></returns>
        public static bool Save( Stream stream, SceneMgr sceneMgr )
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
        /// �����л�
        /// </summary>
        /// <param name="sceneFilePath"></param>
        /// <returns></returns>
        public static SceneMgr Load( string sceneFilePath )
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
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public SceneMgr()
        {
            phiGroups = new List<string>();
            colSinGroups = new List<string>();
            lapSinGroups = new List<string>();
            colPairGroups = new List<Pair>();
            lapPairGroups = new List<Pair>();
            shelterGroups = new List<MulPair>();
            visionGroups = new List<MulPair>();
            groups = new Group( "" );
        }

        #region Groups and Objs

        protected Group groups;

        /// <summary>
        /// ���һ����
        /// </summary>
        /// <param name="groupPath"></param>
        /// <returns></returns>
        public Group GetGroup( string groupPath )
        {
            return FindGroup( groupPath );
        }

        /// <summary>
        /// ���һ��������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="groupPath"></param>
        /// <returns></returns>
        public TypeGroup<T> GetTypeGroup<T>( string groupPath ) where T : class, IGameObj
        {
            Group result = FindGroup( groupPath );
            if (result != null && result is TypeGroup<T>)
                return result as TypeGroup<T>;
            else
                return null;
        }

        /// <summary>
        /// �ƶ���������Ĵ��·������ó�������
        /// </summary>
        /// <param name="objPath">����ĳ���·��</param>
        /// <returns></returns>
        public IGameObj GetGameObj( string objPath )
        {
            return FindObj( objPath );
        }

        /// <summary>
        /// �����
        /// </summary>
        /// <param name="fatherPath">��ӵ��ĸ����·���������ʾ��ӵ�������</param>
        /// <param name="group">Ҫ��ӵ���</param>
        /// <returns></returns>
        public bool AddGroup( string fatherPath, Group group )
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
        /// �������
        /// </summary>
        /// <param name="groupPath">Ҫ��ӵ������·�����������ΪTypeGroup</param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool AddGameObj( string groupPath, IGameObj obj )
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
        /// ͬʱ��Ӷ��ͬ������
        /// </summary>
        /// <param name="groupPath"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public bool AddGameObj( string groupPath, params IGameObj[] objs )
        {
            bool success = true;
            foreach (IGameObj obj in objs)
            {
                success = success && AddGameObj( groupPath, obj );
            }
            return success;
        }

        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="objPath">�����·��</param>
        /// <returns></returns>
        public bool DelGameObj( string objPath )
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
        /// ɾ������
        /// </summary>
        /// <param name="groupPath">�����������·��</param>
        /// <param name="objName">���������</param>
        /// <returns></returns>
        public bool DelGameObj( string groupPath, string objName )
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
        /// ��ʾ���Ԫ��ϵ
        /// </summary>
        public struct Pair
        {
            public string groupPath1;
            public string groupPath2;

            public Pair( string groupPath1, string groupPath2 )
            {
                this.groupPath1 = groupPath1;
                this.groupPath2 = groupPath2;
            }
        }
        /// <summary>
        /// ��ʾһ�Զ�Ĺ�ϵ
        /// </summary>
        public struct MulPair
        {
            public string groupPath;
            public List<string> groupPaths;

            public MulPair( string groupPath, List<string> groupPaths )
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

        private List<MulPair> visionGroups;

        /// <summary>
        /// ��ע������������
        /// </summary>
        public List<string> PhiGroups
        {
            get { return phiGroups; }
        }
        /// <summary>
        /// ��ע��ĵ�����ײ��
        /// </summary>
        public List<string> ColSinGroups
        {
            get { return colSinGroups; }
        }
        /// <summary>
        /// ��ע��ĵ����ص���
        /// </summary>
        public List<string> LapSinGroups
        {
            get { return LapSinGroups; }
        }
        /// <summary>
        /// ��ע�����ײ���
        /// </summary>
        public List<Pair> ColPairGroups
        {
            get { return colPairGroups; }
        }
        /// <summary>
        /// ��ע����ص����
        /// </summary>
        public List<Pair> LapPairGroups
        {
            get { return lapPairGroups; }
        }
        /// <summary>
        /// ��ע����ڵ���ϵ
        /// </summary>
        public List<MulPair> ShelterGroups
        {
            get { return shelterGroups; }
        }
        /// <summary>
        /// ��ע��Ŀ��ӹ�ϵ
        /// </summary>
        public List<MulPair> VisionGroups
        {
            get { return visionGroups; }
        }

        /// <summary>
        /// ע�Ἰ����������ײ��ϵ����
        /// </summary>
        /// <param name="groups"></param>
        public void AddColMulGroups( params string[] groups )
        {
            for (int i = 0; i < groups.Length - 1; i++)
            {
                for (int j = i + 1; j < groups.Length; j++)
                {
                    ForEachTypeGroup( FindGroup( groups[i] ), new ForEachTypeGroupHandler(
                        delegate( TypeGroup typeGroup1 )
                        {
                            ForEachTypeGroup( FindGroup( groups[j] ), new ForEachTypeGroupHandler(
                                delegate( TypeGroup typeGroup2 )
                                {
                                    colPairGroups.Add( new Pair( typeGroup1.Path, typeGroup2.Path ) );
                                } ) );
                        } ) );
                }
            }
        }
        /// <summary>
        /// ע�Ἰ���������ص���ϵ����
        /// </summary>
        /// <param name="groups"></param>
        public void AddLapMulGroups( params string[] groups )
        {
            for (int i = 0; i < groups.Length - 1; i++)
            {
                for (int j = i + 1; j < groups.Length; j++)
                {
                    lapPairGroups.Add( new Pair( groups[i], groups[j] ) );
                }
            }
        }
        /// <summary>
        /// ע���ڵ���ϵ
        /// </summary>
        /// <param name="raderOwnerGroup"></param>
        /// <param name="shelterGroups"></param>
        public void AddShelterMulGroups( string raderOwnerGroup, params string[] shelterGroups )
        {
            this.shelterGroups.Add( new MulPair( raderOwnerGroup, new List<string>( shelterGroups ) ) );
        }

        /// <summary>
        /// ��ƽ̨�����ע���������
        /// </summary>
        /// <param name="manager"></param>
        public void RegistPhiCol( SmartTank.PhiCol.PhiColMgr manager )
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
        /// ��ƽ̨�����ע���ڵ���ϵ
        /// </summary>
        /// <param name="manager"></param>
        public void RegistShelter( SmartTank.Shelter.ShelterMgr manager )
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
        /// ��ƽ̨�����ע��ɻ�����
        /// </summary>
        /// <param name="manager"></param>
        public void RegistDrawables( SmartTank.Draw.DrawMgr manager )
        {
            ForEachTypeGroup( groups, new ForEachTypeGroupHandler(
                delegate( TypeGroup typeGroup )
                {
                    manager.AddGroup( typeGroup.GetEnumerableCopy<IDrawableObj>() );
                } ) );
        }
        /// <summary>
        /// ��ƽ̨�����ע��ɸ�����
        /// </summary>
        /// <param name="manager"></param>
        public void RegistUpdaters( SmartTank.Update.UpdateMgr manager )
        {
            ForEachTypeGroup( groups, new ForEachTypeGroupHandler(
                delegate( TypeGroup typeGroup )
                {
                    manager.AddGroup( typeGroup.GetEnumerableCopy<IUpdater>() );
                } ) );
        }
        /// <summary>
        /// ��ƽ̨�����ע����ӹ�ϵ
        /// </summary>
        /// <param name="manager"></param>
        public void RegistVision( SmartTank.Senses.Vision.VisionMgr manager )
        {
            foreach (MulPair pair in visionGroups)
            {
                try
                {
                    //TypeGroup group1 = FindGroup( pair.groupPath ) as TypeGroup;
                    //TypeGroup group2 = FindGroup( pair.groupPath2 ) as TypeGroup;
                    //manager.AddVisionGroup(
                    //    group1.GetEnumerableCopy<IRaderOwner>(),
                    //    group2.GetEnumerableCopy<IEyeableObj>() );
                    List<IEnumerable<IEyeableObj>> EyeableGroups = new List<IEnumerable<IEyeableObj>>();

                    foreach (string eyeGroupPath in pair.groupPaths)
                    {
                        TypeGroup shelGroup = FindGroup( eyeGroupPath ) as TypeGroup;
                        EyeableGroups.Add( shelGroup.GetEnumerableCopy<IEyeableObj>() );
                    }

                    TypeGroup group = FindGroup( pair.groupPath ) as TypeGroup;

                    manager.AddVisionGroup(
                        group.GetEnumerableCopy<IRaderOwner>(),
                        EyeableGroups.ToArray() );
                }
                catch (Exception ex)
                {
                    Log.Write( "RegistVisionGroup error: " + pair.groupPath + ", " +
                        pair.groupPath.ToString() + ", " + ex.Message );
                }
            }
        }

        #endregion

        #region Prviate Methods

        private Group FindGroup( string groupPath )
        {
            if (groupPath == string.Empty)
                return this.groups;

            Group curGroup = this.groups;
            string[] paths;
            paths = groupPath.Split( '\\' );
            foreach (string path in paths)
            {
                curGroup = curGroup.GetChildGroup( path );
                if (curGroup == null)
                {
                    Log.Write( "FindGroup: �������·��������򲻴���ƥ����" );
                    return null;
                }
            }
            return curGroup;
        }

        private IGameObj FindObj( string objPath )
        {
            Group curGroup = this.groups;
            string[] paths;
            paths = objPath.Split( '\\' );
            for (int i = 0; i < paths.Length - 1; i++)
            {
                curGroup = curGroup.GetChildGroup( paths[i] );
                if (curGroup == null)
                {
                    Log.Write( "FindObj: �������·��������򲻴���ƥ����" );
                    return null;
                }
            }
            if (curGroup != null && curGroup is TypeGroup)
            {
                return (curGroup as TypeGroup).GetObj( paths[paths.Length - 1] );
            }
            return null;
        }

        private delegate void ForEachTypeGroupHandler( TypeGroup group );

        private static void ForEachTypeGroup( Group group, ForEachTypeGroupHandler handler )
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
