using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Common.DataStructure
{
    #region Delegates

    /// <summary>
    /// 对数据容器中的每个对象进行一次处理。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    public delegate void ForEachFunc<T> ( ref T type );

    /// <summary>
    /// 在容器中寻找想要找的对象。
    /// 当该对象是你需要的对象时，代理函数应返回true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public delegate bool FindFunc<T> ( T type );

    #endregion

    #region CircleListNode

    /// <summary>
    /// 环链表节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircleListNode<T>
    {
        /// <summary>
        /// 表示一个值为空的节点
        /// </summary>
        static readonly public CircleListNode<T> empty = new CircleListNode<T>();

        /// <summary>
        /// 空构造函数
        /// </summary>
        public CircleListNode ()
        {
        }

        /// <summary>
        /// 将value作为该节点中储存的值
        /// </summary>
        /// <param name="value"></param>
        public CircleListNode ( T value )
        {
            this.value = value;
        }

        /// <summary>
        /// 节点中储存的值
        /// </summary>
        public T value;

        /// <summary>
        /// 该节点的前一个节点
        /// </summary>
        public CircleListNode<T> pre;

        /// <summary>
        /// 该节点的后一个节点
        /// </summary>
        public CircleListNode<T> next;

        /// <summary>
        /// 对节点进行深度复制。
        /// 节点中储存的值必须是ICloneable或是ValueType才可调用该函数，否则会出现异常。
        /// </summary>
        /// <returns></returns>
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

    /*
     * 对平台而言意义非同小可。
     * 它是储存贴图边界点的数据结构，使精灵之间精确的冲突检测成为可能。
     * 
     * */


    /// <summary>
    /// 环形链表。
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
        /// <summary>
        /// 获取链表中的第一个元素，当链表为空时返回null
        /// </summary>
        public CircleListNode<T> First
        {
            get { return first; }
        }
        CircleListNode<T> last;
        /// <summary>
        /// 获取链表中的最后一个元素，当链表为空时返回null
        /// </summary>
        public CircleListNode<T> Last
        {
            get { return last; }
        }

        bool linked = false;
        /// <summary>
        /// 获取一个值，表示该链表是否已经首尾相连。
        /// </summary>
        public bool isLinked
        {
            get { return linked; }
        }

        int length;

        /// <summary>
        /// 链表中元素的数量
        /// </summary>
        public int Length
        {
            get { return length; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// 在链表的第一个元素前插入一个元素。
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 在链表的第一个元素前插入一个元素。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddFirst ( T value )
        {
            return AddFirst( new CircleListNode<T>( value ) );
        }

        /// <summary>
        /// 在未首尾相连的条件下在链表的末尾加入一个元素。
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 在未首尾相连的条件下在链表的末尾加入一个元素。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddLast ( T value )
        {
            if (linked) return false;

            CircleListNode<T> node = new CircleListNode<T>( value );
            return AddLast( node );
        }

        /// <summary>
        /// 在链表中某一节点后插入一个新的节点。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="node">必须是该链表中包含的节点</param>
        /// <returns></returns>
        public bool InsertAfter ( T value, CircleListNode<T> node )
        {
            CircleListNode<T> newNode = new CircleListNode<T>( value );
            newNode.next = node.next;

            if (node.next == null)
                return false;

            if (node == last)
                last = newNode;

            if (node.next != null)
                node.next.pre = newNode;
            node.next = newNode;
            newNode.pre = node;

            length++;

            return true;
        }

        /// <summary>
        /// 链接第一个和最后一个节点，使链表成为一个环链表。
        /// </summary>
        public void LinkLastAndFirst ()
        {
            linked = true;

            last.next = first;
            first.pre = last;
        }

        /// <summary>
        /// 对链表中的每个元素执行一次操作
        /// </summary>
        /// <param name="func"></param>
        public void ForEach ( ForEachFunc<T> func )
        {
            CircleListEnumerator iter = new CircleListEnumerator( this );
            while (iter.MoveNext())
            {
                func( ref iter.curNode.value );
            }
        }

        /// <summary>
        /// 用遍历的方式查找链表，返回符合要求的第一个节点
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 用遍历的方式查找链表，返回所以符合要求的节点
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 用遍历的方式获得该节点在链表中的位置。
        /// 如果节点不在链表中，返回-1
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 将链表从起始节点开始，向前或向后的顺序转换到一个数组中。
        /// </summary>
        /// <param name="startNode">起始节点，必须是链表中的节点</param>
        /// <param name="forward"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 获得该链表的迭代器
        /// </summary>
        /// <returns></returns>
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
