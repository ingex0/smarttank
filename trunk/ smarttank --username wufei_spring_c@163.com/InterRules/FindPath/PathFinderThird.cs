using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameObjects.Tank.TankAIs;
using Platform.AIHelper;
using Platform.Senses.Memory;
using GameBase.DataStructure;
using Microsoft.Xna.Framework;
using GameBase.Input;
using GameBase.Graphics;
using Microsoft.Xna.Framework.Graphics;
using GameBase.Helpers;

namespace InterRules.FindPath
{
    [AIAttribute( "PathFinder No.3", "SmartTank编写组", "利用导航图进行寻路的AI", 2007, 12, 3 )]
    class PathFinderThird : IAISinTur
    {
        IAIOrderServerSinTur orderServer;
        AICommonServer commonServer;
        AIActionHelper action;

        NavigateMap naviMap;
        Rectanglef mapSize;

        #region IAI 成员

        public IAICommonServer CommonServer
        {
            set
            {
                this.commonServer = (AICommonServer)value;
                this.mapSize = commonServer.MapBorder;
            }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                this.orderServer = (IAIOrderServerSinTur)value;
                action = new AIActionHelper( orderServer );
                orderServer.onBorderObjUpdated += new Platform.Shelter.BorderObjUpdatedEventHandler( OnBorderObjUpdated );
            }
        }

        #endregion

        #region Draw

        public void Draw ()
        {
            if (naviMap != null)
            {
                foreach (GraphPoint<NaviPoint> naviPoint in naviMap.Map)
                {
                    BasicGraphics.DrawPoint( naviPoint.value.Pos, 1f, Color.Yellow, 0.2f );

                    foreach (GraphPath<NaviPoint> path in naviPoint.neighbors)
                    {
                        BasicGraphics.DrawLine( naviPoint.value.Pos, path.neighbor.value.Pos, 3f, Color.Yellow, 0.2f );
                        FontManager.Draw( path.weight.ToString(), 0.5f * (naviPoint.value.Pos + path.neighbor.value.Pos), 0.5f, Color.Black, 0f, FontType.Comic );
                    }
                }

                foreach (Segment guardLine in naviMap.GuardLines)
                {
                    BasicGraphics.DrawLine( guardLine.startPoint, guardLine.endPoint, 3f, Color.Red, 0.1f, SpriteBlendMode.AlphaBlend );
                }

                foreach (Segment borderLine in naviMap.BorderLines)
                {
                    BasicGraphics.DrawLine( borderLine.startPoint, borderLine.endPoint, 3f, Color.Brown, 0.1f, SpriteBlendMode.AlphaBlend );
                }
            }

            if (this.path != null && this.path.Length > 0)
            {
                if (curPathIndex < path.Length)
                {
                    BasicGraphics.DrawLine( orderServer.Pos, path[curPathIndex].point.value.Pos, 5f, Color.Blue, 0f );

                    for (int i = curPathIndex; i < path.Length - 1; i++)
                    {
                        Vector2 start = path[i].point.value.Pos;
                        Vector2 end = path[i + 1].point.value.Pos;

                        BasicGraphics.DrawLine( start, end, 5f, Color.Blue, 0f );
                    }
                }
            }
        }

        #endregion

        #region IUpdater 成员

        Vector2 aimPos;
        bool hasOrder = false;

        NodeAStar[] path;
        int curPathIndex = 0;

        public void Update ( float seconds )
        {
            CheckOrder();
            CheckOrderFinish();

            action.Update( seconds );
        }

        private void CheckOrder ()
        {
            if (InputHandler.CurMouseRightBtnPressed)
            {
                if (mapSize.Contains( InputHandler.CurMousePosInLogic ))
                {
                    hasOrder = true;
                    aimPos = InputHandler.CurMousePosInLogic;
                    UpdateNaviMap();
                    CalPath();
                    MoveToNextKeyPoint();
                }
            }
        }

        private void MoveToNextKeyPoint ()
        {
            if (curPathIndex < path.Length)
            {
                while (curPathIndex < path.Length && Vector2.Distance( orderServer.Pos, path[curPathIndex].point.value.Pos ) < 0.3f * orderServer.TankLength)
                {
                    curPathIndex++;
                }
                if (curPathIndex < path.Length)
                    action.AddOrder( new OrderMoveToPosSmooth( path[curPathIndex].point.value.Pos, MathHelper.PiOver4, 0, 0,
                        delegate( IActionOrder order )
                        {
                            curPathIndex++;
                            MoveToNextKeyPoint();
                        }, false ) );
            }
        }

        private void CheckOrderFinish ()
        {
            if (hasOrder)
            {
                Vector2 destance = orderServer.Pos - aimPos;
                if (destance.LengthSquared() < 1)
                {
                    hasOrder = false;
                    orderServer.ForwardSpeed = 0;
                    orderServer.TurnRightSpeed = 0;
                    orderServer.TurnTurretWiseSpeed = 0;
                    orderServer.TurnRaderWiseSpeed = 0;
                    action.StopMove();
                    action.StopRota();
                    action.StopRotaRader();
                }
            }
        }

        void OnBorderObjUpdated ( EyeableBorderObjInfo[] borderObjInfos )
        {
            UpdateNaviMap();

            foreach (EyeableBorderObjInfo obj in orderServer.EyeableBorderObjInfos)
            {
                obj.UpdateConvexHall( orderServer.TankWidth );
            }

            if (hasOrder)
            {
                CalPath();
                MoveToNextKeyPoint();
            }
        }

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

        private void CalPath ()
        {
            if (naviMap == null)
                return;

            #region 复制导航图
            GraphPoint<NaviPoint>[] map = new GraphPoint<NaviPoint>[naviMap.Map.Length + 2];
            GraphPoint<NaviPoint>[] temp = GraphPoint<NaviPoint>.DepthCopy( naviMap.Map );
            for (int i = 0; i < temp.Length; i++)
            {
                map[i] = temp[i];
            }
            #endregion

            #region 将当前点和目标点加入到导航图中
            int prePointSum = temp.Length;
            Vector2 curPos = orderServer.Pos;
            GraphPoint<NaviPoint> curNaviPoint = new GraphPoint<NaviPoint>( new NaviPoint( null, -1, curPos ), new List<GraphPath<NaviPoint>>() );
            GraphPoint<NaviPoint> aimNaviPoint = new GraphPoint<NaviPoint>( new NaviPoint( null, -1, aimPos ), new List<GraphPath<NaviPoint>>() );
            AddCurPosToNaviMap( map, curNaviPoint, prePointSum, naviMap.GuardLines, naviMap.BorderLines );
            AddAimPosToNaviMap( map, aimNaviPoint, curNaviPoint, prePointSum, naviMap.GuardLines, naviMap.BorderLines );

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

            if (cur == null)
                return;

            Stack<NodeAStar> cahe = new Stack<NodeAStar>();
            while (cur.father != null)
            {
                cahe.Push( cur );
                cur = cur.father;
            }

            this.path = cahe.ToArray();
            curPathIndex = 0;

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

        private void UpdateNaviMap ()
        {
            this.naviMap = orderServer.CalNavigateMap( new NaviMapConsiderObj(
                delegate( EyeableBorderObjInfo obj )
                {
                    return true;
                } ), mapSize, orderServer.TankWidth * 0.6f );
        }


        #endregion
    }
}
