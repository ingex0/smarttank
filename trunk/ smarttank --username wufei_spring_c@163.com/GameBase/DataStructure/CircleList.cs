using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace GameBase.DataStructure
{
    #region Delegates

    public delegate void ForEachFunc<T> ( ref T type );
    public delegate bool FindFunc<T> ( T type );

    #endregion

    #region CircleListNode

    public class CircleListNode<T>
    {
        static readonly public CircleListNode<T> empty = new CircleListNode<T>();

        public CircleListNode ()
        {
        }
        public CircleListNode ( T value )
        {
            this.value = value;
        }

        public T value;
        public CircleListNode<T> pre;
        public CircleListNode<T> next;

        public CircleListNode<T> Clone ()
        {
            if (this.value is ICloneable)
            {
                return new CircleListNode<T>( (T)(this.value as ICloneable).Clone() );
            }
            else if (this.value is ValueType)
            {
                return new CircleListNode<T>( this.value );
            }
            else
            {
                throw new Exception( "the T should be a ICloneable or a ValueType!" );
            }
        }
    } 
    #endregion

    #region CircleList
    public class CircleList<T> : IEnumerable<T>
    {
        #region Emunerator
        private class CircleListEnumerator : IEnumerator<T>
        {
            public CircleListNode<T> curNode;
            CircleListNode<T> first;

            int EnumeLength = 0;

            public CircleListEnumerator ( CircleList<T> list )
            {
                this.first = list.first;
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

        #region Variable & Properties

        CircleListNode<T> first;
        public CircleListNode<T> First
        {
            get { return first; }
        }
        CircleListNode<T> last;
        public CircleListNode<T> Last
        {
            get { return last; }
        }

        bool linked = false;
        public bool isLinked
        {
            get { return linked; }
        }

        int length;
        public int Length
        {
            get { return length; }
        } 
        #endregion

        #region Methods

        public bool AddFirst ( CircleListNode<T> node )
        {
            if (first == null)
            {
                first = node;
                last = node;
            }
            else
            {
                node.next = first;
                first.pre = node;
                first = node;

                if (linked)
                {
                    first.pre = last;
                    last.next = first;
                }
            }
            length++;
            return true;
        }

        public bool AddFirst ( T value )
        {
            return AddFirst( new CircleListNode<T>( value ) );
        }

        public bool AddLast ( CircleListNode<T> node )
        {
            if (linked) return false;
            length++;
            if (first == null)
            {
                first = node;
                last = node;
            }
            else
            {
                last.next = node;
                node.pre = last;
                last = node;
            }

            return true;
        }

        public bool AddLast ( T value )
        {
            if (linked) return false;

            CircleListNode<T> node = new CircleListNode<T>( value );
            return AddLast( node );
        }

        public bool InsertAfter ( T value, CircleListNode<T> node )
        {
            CircleListNode<T> newNode = new CircleListNode<T>( value );
            newNode.next = node.next;

            if (node.next == null)
                return false;

            if (node == last)
                last = newNode;

            node.next.pre = newNode;
            node.next = newNode;
            newNode.pre = node;

            length++;

            return true;
        }


        public void LinkLastAndFirst ()
        {
            linked = true;

            last.next = first;
            first.pre = last;
        }

        public void ForEach ( ForEachFunc<T> func )
        {
            CircleListEnumerator iter = new CircleListEnumerator( this );
            while (iter.MoveNext())
            {
                func( ref iter.curNode.value );
            }
        }

        public CircleListNode<T> FindFirst ( FindFunc<T> func )
        {
            CircleListEnumerator iter = new CircleListEnumerator( this );
            while (iter.MoveNext())
            {
                if (func( iter.curNode.value ))
                {
                    return iter.curNode;
                }
            }
            return null;
        }

        public CircleListNode<T>[] FindAll ( FindFunc<T> func )
        {
            List<CircleListNode<T>> result = new List<CircleListNode<T>>();

            CircleListEnumerator iter = new CircleListEnumerator( this );
            while (iter.MoveNext())
            {
                if (func( iter.curNode.value ))
                {
                    result.Add( iter.curNode );
                }
            }
            return result.ToArray();
        }

        public int IndexOf ( CircleListNode<T> node )
        {
            CircleListEnumerator iter = new CircleListEnumerator( this );

            int result = -1;
            while (iter.MoveNext())
            {
                result++;
                if (iter.curNode == node)
                    return result;
            }
            return -1;
        }
        
        public T[] ToArray ( CircleListNode<T> startNode, bool forward )
        {
            if (startNode == null)
                throw new Exception( "The startNode is null!" );

            CircleListEnumerator iter = new CircleListEnumerator( this );

            bool startNodeInList = false;
            while (iter.MoveNext())
            {
                if (iter.curNode == startNode)
                {
                    startNodeInList = true;
                }
            }
            if (!startNodeInList)
                throw new Exception( "the startNode should in current List!" );

            if (!this.isLinked)
                this.LinkLastAndFirst();

            T[] result = new T[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                result[i] = startNode.value;
                if (forward)
                    startNode = startNode.next;
                else
                    startNode = startNode.pre;
            }

            return result;
        }

        public CircleList<T> Clone ()
        {
            CircleList<T> result = new CircleList<T>();
            CircleListEnumerator iter = new CircleListEnumerator( this );
            while (iter.MoveNext())
            {
                result.AddLast( iter.curNode.Clone() );
            }
            if (this.isLinked)
                result.LinkLastAndFirst();
            return result;
        } 

        #endregion

        #region IEnumerable<T> 成员

        public IEnumerator<T> GetEnumerator ()
        {
            return new CircleListEnumerator( this );
        }

        #endregion

        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        #endregion
    } 
    #endregion
}
