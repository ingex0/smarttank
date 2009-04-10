using System;
using System.Collections.Generic;
using System.Text;

namespace TankEngine2D.DataStructure
{
    /// <summary>
    /// 一个用Hash实现的容器。
    /// 
    /// 如果其中的元素类型继承于某个接口（或父类），就能获取该容器的一个可以以该接口（或父类）进行迭代的副本。
    /// 当原容器中元素有所改动时，副本和原容器始终保持一致。
    /// 
    /// 容器对其中的每一个元素使用一个string类型的名称作为Hash表的主键，通过该主键达到快速的查找。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiList<T> : IEnumerable<T> where T : class
    {
        Dictionary<string, T> dic;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public MultiList ()
        {
            dic = new Dictionary<string, T>();
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add ( string name, T value )
        {
            if (dic.ContainsKey( name ))
                return false;
            else
            {
                dic.Add( name, value );
                return true;
            }
        }

        /// <summary>
        /// 删除元素
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Remove ( string name )
        {
            if (dic.ContainsKey( name ))
            {
                dic.Remove( name );
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 获得元素的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public T this[string name]
        {
            get
            {
                try
                {
                    if (dic.ContainsKey(name))
                        return dic[name];
                    else
                        return null;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        #region IEnumerable<T> 成员

        /// <summary>
        /// 获得枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator ()
        {
            foreach (KeyValuePair<string, T> pair in dic)
            {
                yield return pair.Value;
            }
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// 获得副本
        /// </summary>
        /// <typeparam name="CopyType"></typeparam>
        /// <returns></returns>
        public MultiCopy<CopyType, T> GetCopy<CopyType> () where CopyType : class
        {
            Type copyType = typeof( CopyType );
            Type valueType = typeof( T );
            if (copyType.IsInterface)
            {
                if (valueType.GetInterface( copyType.Name ) == null)
                    throw new Exception( "Can not convert ValueType to CopyType!" );
            }
            else
            {
                if (!typeof( T ).IsSubclassOf( typeof( CopyType ) ))
                    throw new Exception( "Can not convert ValueType to CopyType!" );
            }

            MultiCopy<CopyType,T> copy = new MultiCopy<CopyType,T>( this );
            return copy;
        }
    }

    /// <summary>
    /// MultiList的副本
    /// </summary>
    /// <typeparam name="CopyType"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class MultiCopy<CopyType, T> : IEnumerable<CopyType>
        where T : class
        where CopyType : class
    {
        MultiList<T> mother;

        /// <summary>
        /// MultiList的副本
        /// </summary>
        /// <param name="mother">原容器</param>
        public MultiCopy ( MultiList<T> mother )
        {
            this.mother = mother;
        }

        #region IEnumerable<CopyType> 成员

        /// <summary>
        /// 获得枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CopyType> GetEnumerator ()
        {
            foreach (T value in mother)
            {
                yield return value as CopyType;
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
}
