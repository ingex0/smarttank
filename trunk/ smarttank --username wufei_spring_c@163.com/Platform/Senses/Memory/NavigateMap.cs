using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameBase.Helpers;
using GameBase.DataStructure;
using Platform.Senses.Vision;

namespace Platform.Senses.Memory
{
    public struct NaviPoint
    {
        EyeableBorderObjInfo objInfo;
        int index;
        Vector2 pos;

        public EyeableBorderObjInfo ObjInfo
        {
            get { return objInfo; }
        }

        public int Index
        {
            get { return index; }
        }

        public Vector2 Pos
        {
            get { return pos; }
        }

        public NaviPoint ( EyeableBorderObjInfo objInfo, int index, Vector2 pos )
        {
            this.objInfo = objInfo;
            this.index = index;
            this.pos = pos;
        }
    }

    public class GuardConvex
    {
        public GraphPoint<NaviPoint>[] points;

        public GraphPoint<NaviPoint> this[int i]
        {
            get { return points[i]; }
        }

        public int Length
        {
            get { return points.Length; }
        }

        public GuardConvex ( GraphPoint<NaviPoint>[] points )
        {
            this.points = points;
        }

        public bool PointInConvex ( Vector2 p )
        {
            float sign = 0;
            bool first = true;
            for (int i = 0; i < points.Length; i++)
            {
                GraphPoint<NaviPoint> cur = points[i];
                GraphPoint<NaviPoint> next = points[(i + 1) % points.Length];

                float cross = MathTools.Vector2Cross( p - cur.value.Pos, next.value.Pos - p );

                if (first)
                {
                    sign = cross;
                    first = false;
                }
                else
                {
                    if (Math.Sign( sign ) != Math.Sign( cross ))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public class NavigateMap
    {
        GraphPoint<NaviPoint>[] naviGraph;

        List<Segment> guardLines;

        List<Segment> borderLines;

        List<GuardConvex> convexs;

        public GraphPoint<NaviPoint>[] Map
        {
            get { return naviGraph; }
        }

        public List<Segment> GuardLines
        {
            get { return guardLines; }
        }

        public List<Segment> BorderLines
        {
            get { return borderLines; }
        }

        public List<GuardConvex> Convexs
        {
            get { return convexs; }
        }

        public NavigateMap ( EyeableBorderObjInfo[] objInfos, Rectanglef mapBorder, float spaceForTank )
        {
            BuildMap( objInfos, mapBorder, spaceForTank );
        }

        public void BuildMap ( EyeableBorderObjInfo[] objInfos, Rectanglef mapBorder, float spaceForTank )
        {

            #region 对每一个BorderObj生成逻辑坐标上的凸包点集，并向外扩展

            borderLines = new List<Segment>();

            convexs = new List<GuardConvex>();
            foreach (EyeableBorderObjInfo obj in objInfos)
            {
                if (obj.ConvexHall == null || obj.IsDisappeared || obj.ConvexHall.Points == null)
                    continue;

                Matrix matrix = obj.EyeableInfo.CurTransMatrix;

                GraphPoint<NaviPoint>[] convexHall;

                List<BordPoint> bordPoints = obj.ConvexHall.Points;
                if (bordPoints.Count > 2)
                {
                    List<GraphPoint<NaviPoint>> list = new List<GraphPoint<NaviPoint>>();
                    for (int i = 0; i < bordPoints.Count; i++)
                    {
                        Vector2 lastPos = Vector2.Transform( ConvertHelper.PointToVector2( bordPoints[i - 1 < 0 ? bordPoints.Count - 1 : i - 1].p ), matrix );
                        Vector2 curPos = Vector2.Transform( ConvertHelper.PointToVector2( bordPoints[i].p ), matrix );
                        Vector2 nextPos = Vector2.Transform( ConvertHelper.PointToVector2( bordPoints[(i + 1) % bordPoints.Count].p ), matrix );

                        Vector2 v1 = curPos - lastPos;
                        Vector2 v2 = curPos - nextPos;
                        float ang = MathTools.AngBetweenVectors( v1, v2 );
                        if (ang >= MathHelper.PiOver2)
                        {
                            float halfDes = (float)(spaceForTank / Math.Sin( ang ));
                            Vector2 delta = halfDes * Vector2.Normalize( v1 ) + halfDes * Vector2.Normalize( v2 );
                            list.Add( new GraphPoint<NaviPoint>(
                                new NaviPoint( obj, bordPoints[i].index, curPos + delta ), new List<GraphPath<NaviPoint>>() ) );
                        }
                        else
                        {
                            v1.Normalize();
                            v2.Normalize();
                            Vector2 cenV = Vector2.Normalize( v1 + v2 );
                            Vector2 vertiV = new Vector2( cenV.Y, -cenV.X );
                            float ang2 = MathHelper.PiOver4 - 0.25f * ang;
                            float vertiL = (float)(spaceForTank * Math.Tan( ang2 ));

                            list.Add( new GraphPoint<NaviPoint>(
                                new NaviPoint( obj, bordPoints[i].index, curPos + spaceForTank * cenV + vertiL * vertiV ),
                                new List<GraphPath<NaviPoint>>() ) );
                            list.Add( new GraphPoint<NaviPoint>(
                                new NaviPoint( obj, bordPoints[i].index, curPos + spaceForTank * cenV - vertiL * vertiV ),
                                new List<GraphPath<NaviPoint>>() ) );
                        }

                        // 添加borderLine
                        borderLines.Add( new Segment( curPos, nextPos ) );
                    }
                    convexHall = list.ToArray();
                    convexs.Add( new GuardConvex( convexHall ) );
                }
                else if (bordPoints.Count == 2)
                {
                    convexHall = new GraphPoint<NaviPoint>[2];
                    Vector2 startPos = Vector2.Transform( ConvertHelper.PointToVector2( bordPoints[0].p ), matrix );
                    Vector2 endPos = Vector2.Transform( ConvertHelper.PointToVector2( bordPoints[1].p ), matrix );
                    Vector2 dir = endPos - startPos;
                    Vector2 normal = new Vector2( dir.Y, -dir.X );
                    normal.Normalize();
                    convexHall[0] = new GraphPoint<NaviPoint>(
                        new NaviPoint( obj, bordPoints[0].index, startPos + spaceForTank * normal ), new List<GraphPath<NaviPoint>>() );
                    convexHall[1] = new GraphPoint<NaviPoint>(
                        new NaviPoint( obj, bordPoints[1].index, endPos + spaceForTank * normal ), new List<GraphPath<NaviPoint>>() );

                    //if (float.IsNaN( convexHall[0].value.Pos.X ) || float.IsNaN( convexHall[1].value.Pos.X ))
                    //{

                    //}

                    // 添加borderLine
                    borderLines.Add( new Segment( startPos, endPos ) );

                    convexs.Add( new GuardConvex( convexHall ) );
                }

            }

            #endregion

            #region 得到警戒线

            guardLines = new List<Segment>();

            foreach (GuardConvex convex in convexs)
            {
                for (int i = 0; i < convex.points.Length; i++)
                {
                    guardLines.Add( new Segment( convex[i].value.Pos, convex[(i + 1) % convex.Length].value.Pos ) );

                    //if (float.IsNaN( convex[i].value.Pos.X ))
                    //{

                    //}
                }
            }

            mapBorder = new Rectanglef( mapBorder.X + spaceForTank, mapBorder.Y + spaceForTank,
                mapBorder.Width - 2 * spaceForTank, mapBorder.Height - 2 * spaceForTank );

            guardLines.Add( new Segment( mapBorder.UpLeft, mapBorder.UpRight ) );
            guardLines.Add( new Segment( mapBorder.UpRight, mapBorder.DownRight ) );
            guardLines.Add( new Segment( mapBorder.DownRight, mapBorder.DownLeft ) );
            guardLines.Add( new Segment( mapBorder.DownLeft, mapBorder.UpLeft ) );

            #endregion

            #region 检查凸包内部连线是否和警戒线相交,如不相交则连接该连线并计算权值

            foreach (GuardConvex convex in convexs)
            {
                for (int i = 0; i < convex.Length; i++)
                {
                    // 检查连线是否超出边界
                    if (!mapBorder.Contains( convex[i].value.Pos ))
                        continue;

                    Segment link = new Segment( convex[i].value.Pos, convex[(i + 1) % convex.Length].value.Pos );


                    bool isCross = false;
                    foreach (Segment guardLine in guardLines)
                    {
                        if (link.Equals( guardLine ))
                            continue;

                        if (Segment.IsCross( link, guardLine ))
                        {
                            isCross = true;
                            break;
                        }
                    }

                    if (!isCross)
                    {
                        float weight = Vector2.Distance( convex[i].value.Pos, convex[(i + 1) % convex.Length].value.Pos );
                        //if (float.IsNaN( weight ))
                        //{

                        //}
                        GraphPoint<NaviPoint>.Link( convex[i], convex[(i + 1) % convex.Length], weight );
                    }
                }
            }

            #endregion

            #region 检查凸包之间连线是否与警戒线以及边界线相交，如不相交则连接并计算权值

            for (int i = 0; i < convexs.Count - 1; i++)
            {
                for (int j = i + 1; j < convexs.Count; j++)
                {
                    foreach (GraphPoint<NaviPoint> p1 in convexs[i].points)
                    {
                        // 检查连线是否超出边界
                        if (!mapBorder.Contains( p1.value.Pos ))
                            continue;

                        foreach (GraphPoint<NaviPoint> p2 in convexs[j].points)
                        {
                            Segment link = new Segment( p1.value.Pos, p2.value.Pos );

                            bool isCross = false;
                            foreach (Segment guardLine in guardLines)
                            {
                                if (Segment.IsCross( link, guardLine ))
                                {
                                    isCross = true;
                                    break;
                                }
                            }
                            if (!isCross)
                            {
                                foreach (Segment borderLine in borderLines)
                                {
                                    if (Segment.IsCross( link, borderLine ))
                                    {
                                        isCross = true;
                                        break;
                                    }
                                }
                            }

                            if (!isCross)
                            {
                                float weight = Vector2.Distance( p1.value.Pos, p2.value.Pos );
                                //if (float.IsNaN( weight ))
                                //{

                                //}
                                GraphPoint<NaviPoint>.Link( p1, p2, weight );
                            }
                        }
                    }
                }
            }

            #endregion

            #region 整理导航图

            List<GraphPoint<NaviPoint>> points = new List<GraphPoint<NaviPoint>>();

            foreach (GuardConvex convex in convexs)
            {
                foreach (GraphPoint<NaviPoint> p in convex.points)
                {
                    points.Add( p );
                }
            }

            naviGraph = points.ToArray();

            #endregion
        }

        /// <summary>
        /// 检查点是否在生成的警戒线凸包之中
        /// </summary>
        /// <param name="p">要检查的点</param>
        /// <param name="convex">如果点处于某个凸包之中，返回这个凸包，否则为null</param>
        /// <returns></returns>
        public bool PointInConvexs ( Vector2 p, out GuardConvex convex )
        {
            foreach (GuardConvex conve in convexs)
            {
                if (conve.PointInConvex( p ))
                {
                    convex = conve;
                    return true;
                }
            }
            convex = null;
            return false;
        }

        /// <summary>
        /// 检查点是否在生成的警戒线凸包之中
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool PointInConvexs ( Vector2 p )
        {
            GuardConvex temp;
            return PointInConvexs( p, out temp );
        }

        #region 使用A*算法计算路径

        class NodeAStar
        {
            public GraphPoint<NaviPoint> point;
            public NodeAStar father;

            public float G;
            public float H;
            public float F;

            public NodeAStar ( GraphPoint<NaviPoint> point, NodeAStar father )
            {
                this.point = point;
                this.father = father;
                this.G = 0;
                this.H = 0;
                this.F = 0;
            }
        }

        /// <summary>
        /// 使用A*算法依据当前信息计算一条最短的路径。
        /// 注意，如果目标点在警戒线以内，会返回一条并非如你期望的路径。
        /// 所以请自行实现目标点无法到达时的处理逻辑。
        /// </summary>
        /// <param name="curPos"></param>
        /// <param name="aimPos"></param>
        /// <returns></returns>
        public NaviPoint[] CalPathUseAStar ( Vector2 curPos, Vector2 aimPos )
        {

            #region 复制导航图
            GraphPoint<NaviPoint>[] map = new GraphPoint<NaviPoint>[Map.Length + 2];
            GraphPoint<NaviPoint>[] temp = GraphPoint<NaviPoint>.DepthCopy( Map );
            for (int i = 0; i < temp.Length; i++)
            {
                map[i] = temp[i];
            }
            #endregion

            #region 将当前点和目标点加入到导航图中
            int prePointSum = temp.Length;
            GraphPoint<NaviPoint> curNaviPoint = new GraphPoint<NaviPoint>( new NaviPoint( null, -1, curPos ), new List<GraphPath<NaviPoint>>() );
            GraphPoint<NaviPoint> aimNaviPoint = new GraphPoint<NaviPoint>( new NaviPoint( null, -1, aimPos ), new List<GraphPath<NaviPoint>>() );
            AddCurPosToNaviMap( map, curNaviPoint, prePointSum, GuardLines, BorderLines );
            AddAimPosToNaviMap( map, aimNaviPoint, curNaviPoint, prePointSum, GuardLines, BorderLines );

            #endregion

            #region 计算最短路径，使用A*算法

            List<NodeAStar> open = new List<NodeAStar>();
            List<NodeAStar> close = new List<NodeAStar>();
            open.Add( new NodeAStar( curNaviPoint, null ) );

            NodeAStar cur = null;
            while (open.Count != 0)
            {
                cur = open[open.Count - 1];

                if (cur.point == aimNaviPoint)
                    break;

                open.RemoveAt( open.Count - 1 );
                close.Add( cur );

                foreach (GraphPath<NaviPoint> path in cur.point.neighbors)
                {
                    if (Contains( close, path.neighbor ))
                    {
                        continue;
                    }
                    else
                    {
                        NodeAStar inOpenNode;
                        if (Contains( open, path.neighbor, out inOpenNode ))
                        {
                            float G = cur.G + path.weight;
                            if (inOpenNode.G > G)
                            {
                                inOpenNode.G = G;
                                inOpenNode.F = G + inOpenNode.H;
                            }
                        }
                        else
                        {
                            NodeAStar childNode = new NodeAStar( path.neighbor, cur );
                            childNode.G = cur.G + path.weight;
                            childNode.H = Vector2.Distance( aimPos, childNode.point.value.Pos );
                            childNode.F = childNode.G + childNode.H;
                            SortInsert( open, childNode );
                        }
                    }
                }
            }

            //if (cur == null)
            //    return null;

            Stack<NodeAStar> cahe = new Stack<NodeAStar>();
            while (cur.father != null)
            {
                cahe.Push( cur );
                cur = cur.father;
            }

            NaviPoint[] result = new NaviPoint[cahe.Count];

            int j = 0;
            foreach (NodeAStar node in cahe)
            {
                result[j] = node.point.value;
                j++;
            }

            return result;

            #endregion
        }

        private void SortInsert ( List<NodeAStar> open, NodeAStar childNode )
        {
            int i = 0;
            while (i < open.Count && open[i].F > childNode.F)
            {
                i++;
            }
            if (i == open.Count)
                open.Add( childNode );
            else
                open.Insert( i, childNode );
        }

        private bool Contains ( List<NodeAStar> list, GraphPoint<NaviPoint> graphPoint, out NodeAStar findNode )
        {
            if ((findNode = list.Find( new Predicate<NodeAStar>(
                delegate( NodeAStar node )
                {
                    if (node.point == graphPoint)
                        return true;
                    else
                        return false;
                } ) )) == null)
                return false;
            else
                return true;
        }

        private bool Contains ( List<NodeAStar> list, GraphPoint<NaviPoint> graphPoint )
        {
            if (list.Find( new Predicate<NodeAStar>(
                delegate( NodeAStar node )
                {
                    if (node.point == graphPoint)
                        return true;
                    else
                        return false;
                } ) ) == null)
                return false;
            else
                return true;
        }

        private void AddCurPosToNaviMap ( GraphPoint<NaviPoint>[] map, GraphPoint<NaviPoint> curNaviP,
            int prePointSum, List<Segment> guardLines, List<Segment> borderLines )
        {
            map[prePointSum] = curNaviP;
            for (int i = 0; i < prePointSum; i++)
            {
                Segment seg = new Segment( curNaviP.value.Pos, map[i].value.Pos );

                bool cross = false;
                foreach (Segment guardLine in guardLines)
                {
                    if (Segment.IsCross( guardLine, seg ))
                    {
                        cross = true;
                        break;
                    }
                }

                if (!cross)
                {
                    foreach (Segment borderLine in borderLines)
                    {
                        if (Segment.IsCross( borderLine, seg ))
                        {
                            cross = true;
                            break;
                        }

                    }
                }

                if (!cross)
                {
                    float weight = Vector2.Distance( curNaviP.value.Pos, map[i].value.Pos );
                    GraphPoint<NaviPoint>.Link( map[i], curNaviP, weight );
                }
            }
        }

        private void AddAimPosToNaviMap ( GraphPoint<NaviPoint>[] map, GraphPoint<NaviPoint> aimNaviP, GraphPoint<NaviPoint> curNaviP,
            int prePointSum, List<Segment> guardLines, List<Segment> borderLines )
        {
            map[prePointSum + 1] = aimNaviP;
            for (int i = 0; i < prePointSum; i++)
            {
                Segment seg = new Segment( aimNaviP.value.Pos, map[i].value.Pos );

                bool cross = false;
                foreach (Segment guardLine in guardLines)
                {
                    if (Segment.IsCross( guardLine, seg ))
                    {
                        cross = true;
                        break;
                    }
                }

                if (!cross)
                {
                    foreach (Segment borderLine in borderLines)
                    {
                        if (Segment.IsCross( borderLine, seg ))
                        {
                            cross = true;
                            break;
                        }

                    }
                }

                if (!cross)
                {
                    float weight = Vector2.Distance( aimNaviP.value.Pos, map[i].value.Pos );
                    GraphPoint<NaviPoint>.Link( map[i], aimNaviP, weight );
                }
            }

            Segment curToAim = new Segment( curNaviP.value.Pos, aimNaviP.value.Pos );

            bool link = true;
            foreach (Segment guardLine in guardLines)
            {
                if (Segment.IsCross( guardLine, curToAim ))
                {
                    if (MathTools.Vector2Cross( guardLine.endPoint - guardLine.startPoint, curNaviP.value.Pos - guardLine.endPoint ) < 0)
                    {
                        link = false;
                        break;
                    }
                }
            }

            if (link)
            {
                foreach (Segment borderLine in borderLines)
                {
                    if (Segment.IsCross( borderLine, curToAim ))
                    {
                        link = false;
                        break;
                    }

                }
            }

            if (link)
            {
                float weight = Vector2.Distance( curNaviP.value.Pos, aimNaviP.value.Pos );
                GraphPoint<NaviPoint>.Link( curNaviP, aimNaviP, weight );
            }
        } 

        #endregion
    }
}
