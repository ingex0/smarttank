using System;
using System.Collections.Generic;
using System.Text;

namespace GameBase.DataStructure
{
    public struct GraphPath<T>
    {
        public GraphPoint<T> neighbor;
        public float weight;

        public GraphPath ( GraphPoint<T> neighbor, float weight )
        {
            this.neighbor = neighbor;
            this.weight = weight;
        }
    }

    public class GraphPoint<T>
    {
        public T value;
        public List<GraphPath<T>> neighbors;

        public GraphPoint ()
        {

        }

        public GraphPoint ( T value, List<GraphPath<T>> neighbors )
        {
            this.value = value;
            this.neighbors = neighbors;
        }

        public static void Link ( GraphPoint<T> p1, GraphPoint<T> p2, float weight )
        {
            p1.neighbors.Add( new GraphPath<T>( p2, weight ) );
            p2.neighbors.Add( new GraphPath<T>( p1, weight ) );
        }
    }
}
