using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using GameBase.Helpers;

namespace GameBase.DataStructure
{
    /*
     * 一个能够具有多重身份的链式容器。
     * 
     * 如果其中的元素继承于某个接口（或父类），就能获取该链表的一个可以以该接口（或父类）进行迭代的副本。
     * 
     * 当原链表中元素有所改动时，副本和原链表始终保持一致。
     * 
     * */

    /// <summary>
    /// 多重身份链表的节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MultiLinkedListNode<T> where T : class
    {
        static readonly public MultiLinkedListNode<T> empty = new MultiLinkedListNode<T>();

        /// <summary>
        /// 空构造函数
        /// </summary>
        public MultiLinkedListNode ()
        {
        }
        /// <summary>
        /// 设置节点中的值
        /// </summary>
        /// <param name="value"></param>
        public MultiLinkedListNode ( T value )
        {
            this.value = value;
        }

        /// <summary>
        /// 节点的值
        /// </summary>
        public T value;

        /// <summary>
        /// 该节点的前一个节点
        /// </summary>
        public MultiLinkedListNode<T> pre;

        /// <summary>
        /// 该节点的后一个节点
        /// </summary>
        public MultiLinkedListNode<T> next;
    }

    /// <summary>
    /// 一个能够具有多重身份的链式容器。
    /// 如果其中的元素继承于某个接口（或父类），就能获取该链表的一个可以以该接口（或父类）进行迭代的副本。
    /// 当原链表中元素有所改动时，副本和原链表始终保持一致。
    /// 该容器同时保管一个Hash表。以使查找效率更高，但也使容器中不能有节点值相同的节点。
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

            #region IEnumerator<T> 成员

            public T Current
            {
                get { return curNode != null ? curNode.value : null; }
            }

            #endregion

            #region IDisposable 成员

            public void Dispose ()
            {

            }

            #endregion

            #region IEnumerator 成员

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
        /// 为了使移除元素更高效而添加这个Hash表
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
        /// 构建一个空链表
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
        /// 在链表的末尾加入一个新的节点
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
                Log.Write( "MultiLinkedList : AddLast : ArgumentException : 当前键已存在." );
                return false;
            }
            return true;
        }

        /// <summary>
        /// 在链表的末尾加入一个新的节点
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddLast ( T value )
        {

            MultiLinkedListNode<T> node = new MultiLinkedListNode<T>( value );
            return AddLast( node );
        }

        /// <summary>
        /// 移除链表中的节点
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
        /// 查找链表中是否含有值为value的节点，如果有，则删除该节点。若没有，在日志上记录。
        /// 查找过程使用Hash表。
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
        /// 对每个元素执行一次操作
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
        /// 用遍历的方法获得节点在链表中的位置。
        /// 如果节点不在链表中，返回-1
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
        /// 将链表中的值转换到一个数组中。
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
        /// 获得该链表的一个副本，该副本是一个链表值的接口（或父类）的可迭代对象，并始终与原链表保持一致。
        /// </summary>
        /// <typeparam name="CopyType">副本迭代的类型，必须是链表中值的接口或父类</typeparam>
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

        #region IEnumerable<T> 成员

        /// <summary>
        /// 获得链表的迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator ()
        {
            return new MultiLinkedListEnumerator( this );
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// 多重身份链表的副本
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
        /// 用原链表来创造该副本
        /// </summary>
        /// <param name="list"></param>
        public MultiListCopy ( MultiLinkedList<T> list )
        {
            this.list = list;
        }

        #region IEnumerable<CopyType> 成员

        /// <summary>
        /// 获得迭代器
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

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
