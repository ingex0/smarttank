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
        /// 获得组名
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// 获得子组
        /// </summary>
        public Dictionary<string, Group> Childs
        {
            get { return groups; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">组名</param>
        public Group ( string name )
        {
            this.name = name;
            groups = new Dictionary<string, Group>();
        }

        /// <summary>
        /// 获得子组
        /// </summary>
        /// <param name="groupName">组名</param>
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
        /// 添加子组
        /// </summary>
        /// <param name="group">要添加的子组</param>
        /// <returns></returns>
        public bool AddChildGroup ( Group group )
        {
            if (groups.ContainsKey( group.name ))
            {
                Log.Write( "在添加子组时，存在相同的子组名" + name + ", " + group.name );
                return false;
            }
            else
            {
                groups.Add( group.name, group );
                return true;
            }
        }

        /// <summary>
        /// 删除子组
        /// </summary>
        /// <param name="group">要删除的子组</param>
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
                Log.Write( "删除子组时，未找到相符子组" + name + ", " + group.name );
                return false;
            }
        }
        /// <summary>
        /// 删除子组
        /// </summary>
        /// <param name="groupName">要删除子组的组名</param>
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
                Log.Write( "删除子组时，未找到相符子组" + name + ", " + groupName );
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
