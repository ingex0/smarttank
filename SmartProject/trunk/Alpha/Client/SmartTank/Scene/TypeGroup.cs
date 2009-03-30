using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.GameObjs;
using SmartTank.PhiCol;
using SmartTank.Shelter;
using TankEngine2D.DataStructure;
using SmartTank.Senses.Vision;

namespace SmartTank.Scene
{
    public abstract class TypeGroup : Group
    {
        public TypeGroup ( string groupName )
            : base( groupName )
        {
        }

        public abstract bool AddObj ( IGameObj obj );
        public abstract bool DelObj ( string name );
        public abstract bool DelObj ( IGameObj obj );
        public abstract IGameObj GetObj ( string name );

        public abstract IEnumerable<CopyType> GetEnumerableCopy<CopyType> () where CopyType : class;
    }

    public class TypeGroup<T> : TypeGroup
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

        public bool AddObj ( T obj )
        {
            return objs.Add( obj.Name, obj );
        }

        public override bool DelObj ( string objName )
        {
            return objs.Remove( objName );
        }

        public override bool DelObj ( IGameObj obj )
        {
            return DelObj( obj.Name );
        }

        public override bool AddObj ( IGameObj obj )
        {
            return AddObj( (T)obj );
        }

        public override IGameObj GetObj ( string name )
        {
            return objs[name];
        }

        public override IEnumerable<CopyType> GetEnumerableCopy<CopyType> ()
        {
            return objs.GetCopy<CopyType>();
        }
    }
}
