using System;
using System.Collections.Generic;
using System.Text;

namespace GameBase.DataStructure
{
    /// <summary>
    /// 表示有权无向图的路径
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct GraphPath<T>
    {
        /// <summary>
        /// 与当前节点通过该路径相连的节点
        /// </summary>
        public GraphPoint<T> neighbor;

        /// <summary>
        /// 路径上的权值
        /// </summary>
        public float weight;

        /// <summary>
        ///
        /// </summary>
        /// <param name="neighbor">与当前节点通过该路径相连的节点</param>
        /// <param name="weight">路径上的权值</param>
        public GraphPath ( GraphPoint<T> neighbor, float weight )
        {
            this.neighbor = neighbor;
            this.weight = weight;
        }
    }

    /// <summary>
    /// 有权无向图中的节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GraphPoint<T>
    {
        /// <summary>
        /// 节点中的值
        /// </summary>
        public T value;

        /// <summary>
        /// 通向相连节点的路径的列表
        /// </summary>
        public List<GraphPath<T>> neighbors;

        /// <summary>
        /// 空构造函数
        /// </summary>
        public GraphPoint ()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">节点中的值</param>
        /// <param name="neighbors">通向相连节点的路径的列表</param>
        public GraphPoint ( T value, List<GraphPath<T>> neighbors )
        {
            this.value = value;
            this.neighbors = neighbors;
        }

        /// <summary>
        /// 连接两个有权无向图节的节点
        /// </summary>
        /// <param name="p1">节点1</param>
        /// <param name="p2">节点2</param>
        /// <param name="weight">路径上的权重值</param>
        public static void Link ( GraphPoint<T> p1, GraphPoint<T> p2, float weight )
        {
            p1.neighbors.Add( new GraphPath<T>( p2, weight ) );
            p2.neighbors.Add( new GraphPath<T>( p1, weight ) );
        }
    }
}
