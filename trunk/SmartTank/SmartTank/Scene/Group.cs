using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.GameObjs;
using System.Xml;
using TankEngine2D.Helpers;

namespace SmartTank.Scene
{
    public class Group
    {
        protected string name;

        protected Dictionary<string, Group> groups;

        /// <summary>
        /// �������
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// �������
        /// </summary>
        public Dictionary<string, Group> Childs
        {
            get { return groups; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">����</param>
        public Group ( string name )
        {
            this.name = name;
            groups = new Dictionary<string, Group>();
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="groupName">����</param>
        /// <returns></returns>
        public Group GetChildGroup ( string groupName )
        {
            if (groups.ContainsKey( groupName ))
            {
                return groups[groupName];
            }
            return null;
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <param name="group">Ҫ��ӵ�����</param>
        /// <returns></returns>
        public bool AddChildGroup ( Group group )
        {
            if (groups.ContainsKey( group.name ))
            {
                Log.Write( "���������ʱ��������ͬ��������" + name + ", " + group.name );
                return false;
            }
            else
            {
                groups.Add( group.name, group );
                return true;
            }
        }

        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="group">Ҫɾ��������</param>
        /// <returns></returns>
        public bool DelChildGroup ( Group group )
        {
            if (groups.ContainsKey( group.name ) && groups[group.name] == group)
            {
                groups.Remove( group.name );
                return true;
            }
            else
            {
                Log.Write( "ɾ������ʱ��δ�ҵ��������" + name + ", " + group.name );
                return false;
            }
        }
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="groupName">Ҫɾ�����������</param>
        /// <returns></returns>
        public bool DelChildGroup ( string groupName )
        {
            if (groups.ContainsKey( groupName ))
            {
                groups.Remove( groupName );
                return true;
            }
            else
            {
                Log.Write( "ɾ������ʱ��δ�ҵ��������" + name + ", " + groupName );
                return false;
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="writer"></param>
        //public void Save ( XmlWriter writer )
        //{

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="reader"></param>
        ///// <returns></returns>
        //public static Group Load ( XmlReader reader )
        //{
        //    return null;
        //}
    }
}
