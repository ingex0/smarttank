using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Common.DataStructure
{
    #region Delegates

    /// <summary>
    /// �����������е�ÿ���������һ�δ���
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    public delegate void ForEachFunc<T> ( ref T type );

    /// <summary>
    /// ��������Ѱ����Ҫ�ҵĶ���
    /// ���ö���������Ҫ�Ķ���ʱ��������Ӧ����true
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public delegate bool FindFunc<T> ( T type );

    #endregion

    #region CircleListNode

    /// <summary>
    /// ������ڵ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircleListNode<T>
    {
        /// <summary>
        /// ��ʾһ��ֵΪ�յĽڵ�
        /// </summary>
        static readonly public CircleListNode<T> empty = new CircleListNode<T>();

        /// <summary>
        /// �չ��캯��
        /// </summary>
        public CircleListNode ()
        {
        }

        /// <summary>
        /// ��value��Ϊ�ýڵ��д����ֵ
        /// </summary>
        /// <param name="value"></param>
        public CircleListNode ( T value )
        {
            this.value = value;
        }

        /// <summary>
        /// �ڵ��д����ֵ
        /// </summary>
        public T value;

        /// <summary>
        /// �ýڵ��ǰһ���ڵ�
        /// </summary>
        public CircleListNode<T> pre;

        /// <summary>
        /// �ýڵ�ĺ�һ���ڵ�
        /// </summary>
        public CircleListNode<T> next;

        /// <summary>
        /// �Խڵ������ȸ��ơ�
        /// �ڵ��д����ֵ������ICloneable����ValueType�ſɵ��øú��������������쳣��
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
     * ��ƽ̨���������ͬС�ɡ�
     * ���Ǵ�����ͼ�߽������ݽṹ��ʹ����֮�侫ȷ�ĳ�ͻ����Ϊ���ܡ�
     * 
     * */


    /// <summary>
    /// ��������
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

            #region IEnumerator<T> ��Ա

            public T Current
            {
                get { return curNode.value; }
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

        #region Variable & Properties

        CircleListNode<T> first;
        /// <summary>
        /// ��ȡ�����еĵ�һ��Ԫ�أ�������Ϊ��ʱ����null
        /// </summary>
        public CircleListNode<T> First
        {
            get { return first; }
        }
        CircleListNode<T> last;
        /// <summary>
        /// ��ȡ�����е����һ��Ԫ�أ�������Ϊ��ʱ����null
        /// </summary>
        public CircleListNode<T> Last
        {
            get { return last; }
        }

        bool linked = false;
        /// <summary>
        /// ��ȡһ��ֵ����ʾ�������Ƿ��Ѿ���β������
        /// </summary>
        public bool isLinked
        {
            get { return linked; }
        }

        int length;

        /// <summary>
        /// ������Ԫ�ص�����
        /// </summary>
        public int Length
        {
            get { return length; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// ������ĵ�һ��Ԫ��ǰ����һ��Ԫ�ء�
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
        /// ������ĵ�һ��Ԫ��ǰ����һ��Ԫ�ء�
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddFirst ( T value )
        {
            return AddFirst( new CircleListNode<T>( value ) );
        }

        /// <summary>
        /// ��δ��β�������������������ĩβ����һ��Ԫ�ء�
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
        /// ��δ��β�������������������ĩβ����һ��Ԫ�ء�
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
        /// ��������ĳһ�ڵ�����һ���µĽڵ㡣
        /// </summary>
        /// <param name="value"></param>
        /// <param name="node">�����Ǹ������а����Ľڵ�</param>
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
        /// ���ӵ�һ�������һ���ڵ㣬ʹ�����Ϊһ��������
        /// </summary>
        public void LinkLastAndFirst ()
        {
            linked = true;

            last.next = first;
            first.pre = last;
        }

        /// <summary>
        /// �������е�ÿ��Ԫ��ִ��һ�β���
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
        /// �ñ����ķ�ʽ�����������ط���Ҫ��ĵ�һ���ڵ�
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
        /// �ñ����ķ�ʽ���������������Է���Ҫ��Ľڵ�
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
        /// �ñ����ķ�ʽ��øýڵ��������е�λ�á�
        /// ����ڵ㲻�������У�����-1
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
        /// ���������ʼ�ڵ㿪ʼ����ǰ������˳��ת����һ�������С�
        /// </summary>
        /// <param name="startNode">��ʼ�ڵ㣬�����������еĽڵ�</param>
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

        #region IEnumerable<T> ��Ա

        /// <summary>
        /// ��ø�����ĵ�����
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator ()
        {
            return new CircleListEnumerator( this );
        }

        #endregion

        #region IEnumerable ��Ա

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        #endregion
    }
    #endregion
}
