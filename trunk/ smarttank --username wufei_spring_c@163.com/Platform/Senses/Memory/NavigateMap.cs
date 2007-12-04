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

    public class NavigateMap
    {
        GraphPoint<NaviPoint>[] naviGraph;

        List<Segment> guardLines;

        List<Segment> borderLines;

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

        public NavigateMap ( EyeableBorderObjInfo[] objInfos, Rectanglef mapBorder, float spaceForTank )
        {
            BuildMap( objInfos, mapBorder, spaceForTank );
        }

        public void BuildMap ( EyeableBorderObjInfo[] objInfos, Rectanglef mapBorder, float spaceForTank )
        {

            #region 对每一个BorderObj生成逻辑坐标上的凸包点集，并向外扩展

            borderLines = new List<Segment>();

            List<GraphPoint<NaviPoint>[]> convexs = new List<GraphPoint<NaviPoint>[]>();
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
                    convexs.Add( convexHall );
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

                    convexs.Add( convexHall );
                }

            }

            #endregion

            #region 得到警戒线

            guardLines = new List<Segment>();

            foreach (GraphPoint<NaviPoint>[] convex in convexs)
            {
                for (int i = 0; i < convex.Length; i++)
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

            foreach (GraphPoint<NaviPoint>[] convex in convexs)
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
                    foreach (GraphPoint<NaviPoint> p1 in convexs[i])
                    {
                        // 检查连线是否超出边界
                        if (!mapBorder.Contains( p1.value.Pos ))
                            continue;

                        foreach (GraphPoint<NaviPoint> p2 in convexs[j])
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

            foreach (GraphPoint<NaviPoint>[] convex in convexs)
            {
                foreach (GraphPoint<NaviPoint> p in convex)
                {
                    points.Add( p );
                }
            }

            naviGraph = points.ToArray();

            #endregion
        }

    }
}
