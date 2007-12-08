using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameObjects;
using System.Xml;

namespace Platform.Scene
{
    public class Group
    {
        protected string name;

        protected List<Group> groups;
        
        public string Name
        {
            get { return name; }
        }

        public Group (string name)
        {
            this.name = name;
            groups = new List<Group>();
        }

        

        public bool AddChildGroup ( Group group )
        {
            return true;
        }

        public bool AddChildGroup ( string groupName )
        {
            return true;
        }

        public bool DelChildGroup ( string groupName )
        {
            return true;
        }

        public void Save ( XmlWriter writer )
        {

        }

        public static Group Load ( XmlReader reader )
        {
            return null;
        }
    }
}
