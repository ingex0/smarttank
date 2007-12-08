using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameObjects;
using Platform.PhisicalCollision;
using Platform.Shelter;
using GameBase.DataStructure;
using Platform.Senses.Vision;

namespace Platform.Scene
{
    public class TypeGroup<T> : Group
        where T : class, IGameObj
    {
        protected MultiList<T> objs;

        protected bool asPhi = false;
        protected bool asCol = false;
        protected bool asShe = false;
        protected bool asRaderOwner = false;
        protected bool asEyeable = false;

        public MultiList<T> Objs
        {
            get { return objs; }
        }

        public TypeGroup ( string name )
            : base( name )
        {
            this.objs = new MultiList<T>();

            Type type = typeof( T );
            foreach (Type intfce in type.GetInterfaces())
            {
                if (intfce == typeof( IPhisicalObj ))
                    this.asPhi = true;
                if (intfce == typeof( ICollideObj ))
                    this.asCol = true;
                if (intfce == typeof( IShelterObj ))
                    this.asShe = true;
                if (intfce == typeof( IRaderOwner ))
                    this.asRaderOwner = true;
                if (intfce == typeof( IEyeableObj ))
                    this.asEyeable = true;
            }
        }

        public bool AddObj ( T obj, string objName )
        {
            return objs.Add( objName, obj );
        }

        public bool DelObj ( string objName )
        {
            return objs.Remove( objName );
        }
    }
}
