using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using GameBase.Helpers;

namespace GameBase.DataStructure
{
    [Serializable]
    public class MultiLinkedListNode<T> where T : class
    {
        static readonly public MultiLinkedListNode<T> empty = new MultiLinkedListNode<T>();

        public MultiLinkedListNode ()
        {
        }
        public MultiLinkedListNode ( T value )
        {
            this.value = value;
        }

        public T value;
        public MultiLinkedListNode<T> pre;
        public MultiLinkedListNode<T> next;
    }

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

        public MultiLinkedList ()
        {
            head = new MultiLinkedListNode<T>();
            last = head;
            ValueNodeTable = new Dictionary<T, MultiLinkedListNode<T>>();
        }

        #endregion

        #region Methods

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

        public bool AddLast ( T value )
        {

            MultiLinkedListNode<T> node = new MultiLinkedListNode<T>( value );
            return AddLast( node );
        }

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

        public void ForEach ( ForEachFunc<T> func )
        {
            MultiLinkedListEnumerator iter = new MultiLinkedListEnumerator( this );
            while (iter.MoveNext())
            {
                func( ref iter.curNode.value );
            }
        }

        public MultiLinkedListNode<T> FindFirst ( FindFunc<T> func )
        {
            MultiLinkedListEnumerator iter = new MultiLinkedListEnumerator( this );
            while (iter.MoveNext())
            {
                if (func( iter.curNode.value ))
                {
                    return iter.curNode;
                }
            }
            return null;
        }

        public MultiLinkedListNode<T>[] FindAll ( FindFunc<T> func )
        {
            List<MultiLinkedListNode<T>> result = new List<MultiLinkedListNode<T>>();

            MultiLinkedListEnumerator iter = new MultiLinkedListEnumerator( this );
            while (iter.MoveNext())
            {
                if (func( iter.curNode.value ))
                {
                    result.Add( iter.curNode );
                }
            }
            return result.ToArray();
        }

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

    [Serializable]
    public class MultiListCopy<CopyType, T> : IEnumerable<CopyType>
        where T : class
        where CopyType : class
    {
        MultiLinkedList<T> list;

        public MultiListCopy ( MultiLinkedList<T> list )
        {
            this.list = list;
        }

        #region IEnumerable<CopyType> 成员

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
