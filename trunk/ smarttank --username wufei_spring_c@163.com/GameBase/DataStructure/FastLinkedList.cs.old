using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using GameBase.Helpers;

namespace GameBase.DataStructure
{
    public class FastLinkedListNode<T>
    {
        static readonly public FastLinkedListNode<T> empty = new FastLinkedListNode<T>();

        public FastLinkedListNode ()
        {
        }
        public FastLinkedListNode ( T value )
        {
            this.value = value;
        }

        public T value;
        public FastLinkedListNode<T> pre;
        public FastLinkedListNode<T> next;
    }

    [Obsolete("没有经过完整的测试")]
    class FastLinkedList<T> : IEnumerable<T>
    {
        #region Emunerator
        private class FastLinkedListEnumerator : IEnumerator<T>
        {
            public FastLinkedListNode<T> curNode;
            FastLinkedListNode<T> head;

            int EnumeLength = 0;

            public FastLinkedListEnumerator ( FastLinkedList<T> list )
            {
                this.head = list.head;
                curNode = head;
                EnumeLength = list.length;
            }

            #region IEnumerator<T> 成员

            public T Current
            {
                get { return curNode.value; }
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
                    curNode = head;
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
                curNode = head;
            }

            #endregion


        }
        #endregion

        #region Variables

        FastLinkedListNode<T> head;
        FastLinkedListNode<T> last;
        int length;

        /// <summary>
        /// 为了使移除元素更高效而添加这个Hash表
        /// </summary>
        Dictionary<T, FastLinkedListNode<T>> ValueNodeTable;

        #endregion

        #region Properties

        FastLinkedListNode<T> First
        {
            get { return head.next; }
        }

        FastLinkedListNode<T> Last
        {
            get { return last; }
        }

        #endregion

        #region Construction

        public FastLinkedList ()
        {
            head = new FastLinkedListNode<T>();
            last = head;
            ValueNodeTable = new Dictionary<T, FastLinkedListNode<T>>();
        }

        #endregion

        #region Methods

        public bool AddLast ( FastLinkedListNode<T> node )
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
                Log.Write( "FastLinkedList : AddLast : ArgumentException : 当前键已存在." );
                return false;
            }
            return true;
        }

        public bool AddLast ( T value )
        {

            FastLinkedListNode<T> node = new FastLinkedListNode<T>( value );
            return AddLast( node );
        }

        public bool Remove ( FastLinkedListNode<T> node )
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
                FastLinkedListNode<T> node = ValueNodeTable[value];
                if (node != null)
                {
                    return Remove( node );
                }
            }
            catch (KeyNotFoundException)
            {
                Log.Write( "FastLinkedList: Remove : keyNotFoundException" );
            }
            return false;
        }

        public void ForEach ( ForEachFunc<T> func )
        {
            FastLinkedListEnumerator iter = new FastLinkedListEnumerator( this );
            while (iter.MoveNext())
            {
                func( ref iter.curNode.value );
            }
        }

        public FastLinkedListNode<T> FindFirst ( FindFunc<T> func )
        {
            FastLinkedListEnumerator iter = new FastLinkedListEnumerator( this );
            while (iter.MoveNext())
            {
                if (func( iter.curNode.value ))
                {
                    return iter.curNode;
                }
            }
            return null;
        }

        public FastLinkedListNode<T>[] FindAll ( FindFunc<T> func )
        {
            List<FastLinkedListNode<T>> result = new List<FastLinkedListNode<T>>();

            FastLinkedListEnumerator iter = new FastLinkedListEnumerator( this );
            while (iter.MoveNext())
            {
                if (func( iter.curNode.value ))
                {
                    result.Add( iter.curNode );
                }
            }
            return result.ToArray();
        }

        public int IndexOf ( FastLinkedListNode<T> node )
        {
            FastLinkedListEnumerator iter = new FastLinkedListEnumerator( this );

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



        #endregion

        #region IEnumerable<T> 成员

        public IEnumerator<T> GetEnumerator ()
        {
            return new FastLinkedListEnumerator( this );
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
