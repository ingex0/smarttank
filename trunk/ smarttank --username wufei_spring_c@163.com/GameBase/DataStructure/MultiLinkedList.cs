using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using GameBase.Helpers;

namespace GameBase.DataStructure
{
    /*
     * һ���ܹ����ж�����ݵ���ʽ������
     * 
     * ������е�Ԫ�ؼ̳���ĳ���ӿڣ����ࣩ�����ܻ�ȡ�������һ�������Ըýӿڣ����ࣩ���е����ĸ�����
     * 
     * ��ԭ������Ԫ�������Ķ�ʱ��������ԭ����ʼ�ձ���һ�¡�
     * 
     * */

    /// <summary>
    /// �����������Ľڵ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MultiLinkedListNode<T> where T : class
    {
        static readonly public MultiLinkedListNode<T> empty = new MultiLinkedListNode<T>();

        /// <summary>
        /// �չ��캯��
        /// </summary>
        public MultiLinkedListNode ()
        {
        }
        /// <summary>
        /// ���ýڵ��е�ֵ
        /// </summary>
        /// <param name="value"></param>
        public MultiLinkedListNode ( T value )
        {
            this.value = value;
        }

        /// <summary>
        /// �ڵ��ֵ
        /// </summary>
        public T value;

        /// <summary>
        /// �ýڵ��ǰһ���ڵ�
        /// </summary>
        public MultiLinkedListNode<T> pre;

        /// <summary>
        /// �ýڵ�ĺ�һ���ڵ�
        /// </summary>
        public MultiLinkedListNode<T> next;
    }

    /// <summary>
    /// һ���ܹ����ж�����ݵ���ʽ������
    /// ������е�Ԫ�ؼ̳���ĳ���ӿڣ����ࣩ�����ܻ�ȡ�������һ�������Ըýӿڣ����ࣩ���е����ĸ�����
    /// ��ԭ������Ԫ�������Ķ�ʱ��������ԭ����ʼ�ձ���һ�¡�
    /// ������ͬʱ����һ��Hash����ʹ����Ч�ʸ��ߣ���Ҳʹ�����в����нڵ�ֵ��ͬ�Ľڵ㡣
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MultiLinkedList<T> : IEnumerable<T> where T : class
    {
        #region Emunerator
        private class MultiLinkedListEnumerator : IEnumerator<T>
        {
            public MultiLinkedListNode<T> curNode;
            MultiLinkedListNode<T> first;

            int EnumeLength = 0;

            public MultiLinkedListEnumerator ( MultiLinkedList<T> list )
            {
                this.first = list.First;
                EnumeLength = list.length;
            }

            #region IEnumerator<T> ��Ա

            public T Current
            {
                get { return curNode != null ? curNode.value : null; }
            }

            #endregion

            #region IDisposable ��Ա

            public void Dispose ()
            {

            }

            #endregion

            #region IEnumerator ��Ա

            object IEnumerator.Current
            {
                get { return curNode; }
            }

            public bool MoveNext ()
            {
                if (--EnumeLength < 0) return false;

                if (curNode == null)
                {
                    curNode = first;
                    return true;
                }
                else if (curNode.next != null)
                {
                    curNode = curNode.next;
                    return true;
                }
                else
                    return false;
            }

            public void Reset ()
            {
                curNode = first;
            }

            #endregion
        }
        #endregion

        #region Variables

        MultiLinkedListNode<T> head;
        MultiLinkedListNode<T> last;
        int length;

        /// <summary>
        /// Ϊ��ʹ�Ƴ�Ԫ�ظ���Ч��������Hash��
        /// </summary>
        Dictionary<T, MultiLinkedListNode<T>> ValueNodeTable;

        #endregion

        #region Properties

        MultiLinkedListNode<T> First
        {
            get { return head.next; }
        }

        MultiLinkedListNode<T> Last
        {
            get { return last; }
        }

        #endregion

        #region Construction

        /// <summary>
        /// ����һ��������
        /// </summary>
        public MultiLinkedList ()
        {
            head = new MultiLinkedListNode<T>();
            last = head;
            ValueNodeTable = new Dictionary<T, MultiLinkedListNode<T>>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// �������ĩβ����һ���µĽڵ�
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool AddLast ( MultiLinkedListNode<T> node )
        {
            try
            {
                ValueNodeTable.Add( node.value, node );
                length++;

                last.next = node;
                node.pre = last;
                last = node;
            }
            catch (ArgumentException)
            {
                Log.Write( "MultiLinkedList : AddLast : ArgumentException : ��ǰ���Ѵ���." );
                return false;
            }
            return true;
        }

        /// <summary>
        /// �������ĩβ����һ���µĽڵ�
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddLast ( T value )
        {

            MultiLinkedListNode<T> node = new MultiLinkedListNode<T>( value );
            return AddLast( node );
        }

        /// <summary>
        /// �Ƴ������еĽڵ�
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Remove ( MultiLinkedListNode<T> node )
        {
            if (node != null && node.pre != null)
            {
                node.pre.next = node.next;
                if (node.next != null)
                    node.next.pre = node.pre;
                if (last == node)
                    last = node.pre;
                ValueNodeTable.Remove( node.value );
                length--;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// �����������Ƿ���ֵΪvalue�Ľڵ㣬����У���ɾ���ýڵ㡣��û�У�����־�ϼ�¼��
        /// ���ҹ���ʹ��Hash��
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove ( T value )
        {
            try
            {
                MultiLinkedListNode<T> node = ValueNodeTable[value];
                if (node != null)
                {
                    return Remove( node );
                }
            }
            catch (KeyNotFoundException)
            {
                Log.Write( "MultiLinkedList: Remove : keyNotFoundException" );
            }
            return false;
        }

        /// <summary>
        /// ��ÿ��Ԫ��ִ��һ�β���
        /// </summary>
        /// <param name="func"></param>
        public void ForEach ( ForEachFunc<T> func )
        {
            MultiLinkedListEnumerator iter = new MultiLinkedListEnumerator( this );
            while (iter.MoveNext())
            {
                func( ref iter.curNode.value );
            }
        }

        /// <summary>
        /// �ñ����ķ�����ýڵ��������е�λ�á�
        /// ����ڵ㲻�������У�����-1
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int IndexOf ( MultiLinkedListNode<T> node )
        {
            MultiLinkedListEnumerator iter = new MultiLinkedListEnumerator( this );

            int result = -1;
            while (iter.MoveNext())
            {
                result++;
                if (iter.curNode == node)
                    return result;
            }
            return -1;
        }

        /// <summary>
        /// �������е�ֵת����һ�������С�
        /// </summary>
        /// <returns></returns>
        public T[] ToArray ()
        {
            T[] result = new T[this.length];

            int i = 0;
            foreach (T value in this)
            {
                result[i] = value;
                i++;
            }

            return result;
        }

        /// <summary>
        /// ��ø������һ���������ø�����һ������ֵ�Ľӿڣ����ࣩ�Ŀɵ������󣬲�ʼ����ԭ������һ�¡�
        /// </summary>
        /// <typeparam name="CopyType">�������������ͣ�������������ֵ�Ľӿڻ���</typeparam>
        /// <returns></returns>
        public IEnumerable<CopyType> GetConvertList<CopyType> () where CopyType : class
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


            IEnumerable<CopyType> copy = new MultiListCopy<CopyType, T>( this );
            return copy;
        }

        #endregion

        #region IEnumerable<T> ��Ա

        /// <summary>
        /// �������ĵ�����
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator ()
        {
            return new MultiLinkedListEnumerator( this );
        }

        #endregion

        #region IEnumerable ��Ա

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// �����������ĸ���
    /// </summary>
    /// <typeparam name="CopyType"></typeparam>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MultiListCopy<CopyType, T> : IEnumerable<CopyType>
        where T : class
        where CopyType : class
    {
        MultiLinkedList<T> list;

        /// <summary>
        /// ��ԭ����������ø���
        /// </summary>
        /// <param name="list"></param>
        public MultiListCopy ( MultiLinkedList<T> list )
        {
            this.list = list;
        }

        #region IEnumerable<CopyType> ��Ա

        /// <summary>
        /// ��õ�����
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CopyType> GetEnumerator ()
        {
            foreach (T obj in list)
            {
                yield return obj as CopyType;
            }
        }

        #endregion

        #region IEnumerable ��Ա

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
