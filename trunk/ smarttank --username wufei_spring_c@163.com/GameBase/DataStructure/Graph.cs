using System;
using System.Collections.Generic;
using System.Text;

namespace GameBase.DataStructure
{
    /// <summary>
    /// ��ʾ��Ȩ����ͼ��·��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct GraphPath<T>
    {
        /// <summary>
        /// �뵱ǰ�ڵ�ͨ����·�������Ľڵ�
        /// </summary>
        public GraphPoint<T> neighbor;

        /// <summary>
        /// ·���ϵ�Ȩֵ
        /// </summary>
        public float weight;

        /// <summary>
        ///
        /// </summary>
        /// <param name="neighbor">�뵱ǰ�ڵ�ͨ����·�������Ľڵ�</param>
        /// <param name="weight">·���ϵ�Ȩֵ</param>
        public GraphPath ( GraphPoint<T> neighbor, float weight )
        {
            this.neighbor = neighbor;
            this.weight = weight;
        }
    }

    /// <summary>
    /// ��Ȩ����ͼ�еĽڵ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GraphPoint<T>
    {
        /// <summary>
        /// ����һ��ͼ������ͼ�Ľڵ㣬�������ƽڵ��е�ֵ��
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        static public GraphPoint<T>[] DepthCopy ( GraphPoint<T>[] graph )
        {
            GraphPoint<T>[] result = new GraphPoint<T>[graph.Length];

            Dictionary<GraphPoint<T>, int> indexCahe = new Dictionary<GraphPoint<T>, int>();

            for (int i = 0; i < graph.Length; i++)
            {
                result[i] = new GraphPoint<T>( graph[i].value, new List<GraphPath<T>>() );
                indexCahe.Add( graph[i], i );
            }

            for (int i = 0; i < graph.Length; i++)
            {
                foreach (GraphPath<T> path in graph[i].neighbors)
                {
                    Link( result[indexCahe[path.neighbor]], result[i], path.weight );
                }
            }

            return result;
        }

        /// <summary>
        /// �ڵ��е�ֵ
        /// </summary>
        public T value;

        /// <summary>
        /// ͨ�������ڵ��·�����б�
        /// </summary>
        public List<GraphPath<T>> neighbors;

        /// <summary>
        /// �չ��캯��
        /// </summary>
        public GraphPoint ()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">�ڵ��е�ֵ</param>
        /// <param name="neighbors">ͨ�������ڵ��·�����б�</param>
        public GraphPoint ( T value, List<GraphPath<T>> neighbors )
        {
            this.value = value;
            this.neighbors = neighbors;
        }

        /// <summary>
        /// ����������Ȩ����ͼ�ڵĽڵ�
        /// </summary>
        /// <param name="p1">�ڵ�1</param>
        /// <param name="p2">�ڵ�2</param>
        /// <param name="weight">·���ϵ�Ȩ��ֵ</param>
        public static void Link ( GraphPoint<T> p1, GraphPoint<T> p2, float weight )
        {
            p1.neighbors.Add( new GraphPath<T>( p2, weight ) );
            p2.neighbors.Add( new GraphPath<T>( p1, weight ) );
        }
    }


}
