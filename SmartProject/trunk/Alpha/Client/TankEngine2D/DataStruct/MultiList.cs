using System;
using System.Collections.Generic;
using System.Text;

namespace TankEngine2D.DataStructure
{
    /// <summary>
    /// һ����Hashʵ�ֵ�������
    /// 
    /// ������е�Ԫ�����ͼ̳���ĳ���ӿڣ����ࣩ�����ܻ�ȡ��������һ�������Ըýӿڣ����ࣩ���е����ĸ�����
    /// ��ԭ������Ԫ�������Ķ�ʱ��������ԭ����ʼ�ձ���һ�¡�
    /// 
    /// ���������е�ÿһ��Ԫ��ʹ��һ��string���͵�������ΪHash���������ͨ���������ﵽ���ٵĲ��ҡ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultiList<T> : IEnumerable<T> where T : class
    {
        Dictionary<string, T> dic;

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public MultiList ()
        {
            dic = new Dictionary<string, T>();
        }

        /// <summary>
        /// ���Ԫ��
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
        /// ɾ��Ԫ��
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
        /// ���Ԫ�ص�ֵ
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

        #region IEnumerable<T> ��Ա

        /// <summary>
        /// ���ö����
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

        #region IEnumerable ��Ա

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// ��ø���
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
    /// MultiList�ĸ���
    /// </summary>
    /// <typeparam name="CopyType"></typeparam>
    /// <typeparam name="T"></typeparam>
    public class MultiCopy<CopyType, T> : IEnumerable<CopyType>
        where T : class
        where CopyType : class
    {
        MultiList<T> mother;

        /// <summary>
        /// MultiList�ĸ���
        /// </summary>
        /// <param name="mother">ԭ����</param>
        public MultiCopy ( MultiList<T> mother )
        {
            this.mother = mother;
        }

        #region IEnumerable<CopyType> ��Ա

        /// <summary>
        /// ���ö����
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

        #region IEnumerable ��Ա

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
