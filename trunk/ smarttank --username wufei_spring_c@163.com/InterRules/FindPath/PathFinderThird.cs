using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameObjects.Tank.TankAIs;
using Platform.AIHelper;
using Platform.Senses.Memory;
using GameBase.DataStructure;
using Microsoft.Xna.Framework;
using GameBase.Input;

namespace InterRules.FindPath
{
    [AIAttribute( "PathFinder No.3", "SmartTank��д��", "���õ���ͼ����Ѱ·��AI", 2007, 12, 3 )]
    class PathFinderThird : IAISinTur
    {
        IAIOrderServerSinTur orderServer;
        AICommonServer commonServer;
        AIActionHelper action;

        NavigateMap naviMap;
        Rectanglef mapSize;

        #region IAI ��Ա

        public IAICommonServer CommonServer
        {
            set
            {
                this.commonServer = (AICommonServer)value;
                this.mapSize = commonServer.MapBorder;
            }
        }

        public void Draw ()
        {

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

        Vector2 aimPos;
        bool hasOrder = false;


        #region IUpdater ��Ա

        public void Update ( float seconds )
        {
            CheckOrder();
            CheckOrderFinish();
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
                }
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
            // ���µ���ͼ�����¼���·����
            UpdateNaviMap();
            CalPath();
        }

        private void CalPath ()
        {
            if (naviMap == null)
                return;

            // ���Ƶ���ͼ
            GraphPoint<NaviPoint>[] map = new GraphPoint<NaviPoint>[naviMap.Map.Length + 2];
            GraphPoint<NaviPoint>[] temp = GraphPoint<NaviPoint>.DepthCopy( naviMap.Map );
            for (int i = 0; i < temp.Length; i++)
            {
                map[i] = temp[i];
            }

            // ����ǰ���Ŀ�����뵽����ͼ��
            int curPointSum = temp.Length;
            Vector2 curPos = orderServer.Pos;

            AddPointToNaviMap( map, curPos, curPointSum, naviMap.GuardLines );
            curPointSum++;
            AddPointToNaviMap( map, aimPos, curPointSum, naviMap.GuardLines );

            // �������·��


        }

        private void AddPointToNaviMap ( GraphPoint<NaviPoint>[] map, Vector2 newPoint, int curPointSum, List<Segment> guardLines )
        {
            GraphPoint<NaviPoint> newNaviP = new GraphPoint<NaviPoint>( new NaviPoint( null, -1, newPoint ), new List<GraphPath<NaviPoint>>() );

            map[curPointSum] = newNaviP;
            for (int i = 0; i < curPointSum; i++)
            {
                Segment seg = new Segment( newPoint, map[i].value.Pos );

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
                    float weight = Vector2.Distance( newPoint, map[i].value.Pos );
                    GraphPoint<NaviPoint>.Link( map[i], newNaviP, weight );
                }
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
