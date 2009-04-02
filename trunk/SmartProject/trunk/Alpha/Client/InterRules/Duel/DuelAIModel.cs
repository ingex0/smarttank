using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SmartTank.AI;
using SmartTank.AI.AIHelper;
using TankEngine2D.DataStructure;
using TankEngine2D.Graphics;
using SmartTank.GameObjs;
using SmartTank.Senses.Vision;
using SmartTank.GameObjs.Tank.SinTur;

namespace InterRules.Duel
{
    [AIAttribute( "DuelAIModel", "SmartTank Team", "A Model for DuelAI", 2007, 11, 6 )]
    public class DuelAIModel : IDuelAI
    {
        IDuelAIOrderServer orderServer;
        AICommonServer commonServer;

        // ����ʹ�ø����ṩ�ĸ߲�ӿ�
        AIActionHelper action;

        Rectanglef mapBorder;

        public DuelAIModel ()
        {

        }

        #region IAI ��Ա

        public IAICommonServer CommonServer
        {
            set
            {
                commonServer = (AICommonServer)value;

                // ���Դ�CommonServer�л����Ϸ���뵱ǰ̹���޹ص�һЩ�������磺
                mapBorder = commonServer.MapBorder;
            }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = (IDuelAIOrderServer)value;
                // ��������¼�����
                orderServer.OnCollide += new OnCollidedEventHandlerAI( CollideHandler );

                // ���ʹ����AIActionHelper������Ҫ�ڴ˽��г�ʼ����
                action = new AIActionHelper( orderServer );
            }
        }

        #endregion

        #region IUpdater ��Ա

        public void Update ( float seconds )
        {
            // ���Դ�OrderServer�л��̹�˵�ǰ��״̬��������������£�
            Vector2 curPos = orderServer.Pos;

            if (orderServer.FireLeftCDTime <= 0)
                orderServer.Fire();

            orderServer.ForwardSpeed = orderServer.MaxForwardSpeed;

            // ����orderServer��GetEyeableInfo�����������Ұ�����壨�ж�̹�˵���Ϣ����
            List<IEyeableInfo> eyeableInfos = orderServer.GetEyeableInfo();
            if (eyeableInfos.Count != 0)
            {
                if (eyeableInfos[0].ObjInfo.ObjClass == "DuelTank" && eyeableInfos[0] is TankSinTur.TankCommonEyeableInfo)
                {
                    // �ɻ��Ŀ��̹�˵���Ϣ�����£�
                    Vector2 enemyPos = ((TankSinTur.TankCommonEyeableInfo)eyeableInfos[0]).Pos;
                    Vector2 enemyDirection = ((TankSinTur.TankCommonEyeableInfo)eyeableInfos[0]).Direction;

                }
            }


            /* ��AIActionHelper����ʹ�ò��ٸ߼����˶�ָ��
             * 
             * �б����£�
             * RotaToAziOrder           ��ת̹�˵��ƶ���λ��
             * RotaTurretToAziOrder
             * RotaTurretToPosOrder     ��ת̹������ֱ����׼�ƶ�λ��
             * RotaRaderToAziOrder
             * RotaRaderToPosOrder
             * MoveOrder                ��̹����ǰ������ƶ�һ�ξ���
             * MoveToPosDirectOrder     ֱ���ƶ����ƶ���
             * MoveCircleOrder          ����һ��Բ����·���ߵ�Ŀ���
             * 
             * Ҳ�����Լ���IActionOrder�ӿڼ̳��Լ��ĸ߼��˶�ָ�
             * */
            action.AddOrder( new OrderRotaRaderToPos( new Vector2( 100, 100 ) ) );
            action.AddOrder( new OrderMoveCircle( new Vector2( 100, 100 ), orderServer.MaxForwardSpeed, orderServer,
                delegate( IActionOrder order )
                {
                    orderServer.Fire();
                }, false ) );

            // ��ֹһ������
            action.StopRota();

            // ���ʹ��AIActionHelper�������������Update����
            action.Update( seconds );
        }

        #endregion

        void CollideHandler ( CollisionResult result, GameObjInfo objB )
        {
            // ͨ��objB����Ϣȷ����ײ������ࡣ
            if (objB.ObjClass == "Border")
            {
                // ����Լ��Ĵ�����
            }
            else if (objB.ObjClass == "ShellNormal")
            {
                // ����Լ��Ĵ�����
            }
            else if (objB.ObjClass == "DuelTank")
            {
                // ����Լ��Ĵ�����
            }

        }



        #region IAI ��Ա


        public void Draw ()
        {
            
        }

        #endregion
    }
}
