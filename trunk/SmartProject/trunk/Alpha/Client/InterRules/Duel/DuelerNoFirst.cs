using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SmartTank.AI;
using SmartTank.AI.AIHelper;
using TankEngine2D.DataStructure;
using TankEngine2D.Graphics;
using SmartTank.GameObjs;

namespace InterRules.Duel
{
    [AIAttribute( "Dueler No.1", "Wufei", "", 2007, 11, 7 )]
    class DuelerNoFirst : IDuelAI
    {
        IDuelAIOrderServer orderServer;
        AICommonServer commonServer;

        AIActionHelper action;

        Rectanglef mapBorder;

        ConsiderCenter considerCenter;

        public DuelerNoFirst ()
        {

        }

        #region IAI 成员

        public IAICommonServer CommonServer
        {
            set
            {
                commonServer = (AICommonServer)value;
                mapBorder = commonServer.MapBorder; ;

            }
        }

        public IAIOrderServer OrderServer
        {
            set
            {
                orderServer = (IDuelAIOrderServer)value;
                orderServer.OnCollide += new OnCollidedEventHandlerAI( CollideHandler );
                action = new AIActionHelper( orderServer );

                considerCenter = new ConsiderCenter( orderServer );
                considerCenter.AddConsider( new ConsiderAwayFromBorder( mapBorder, 30, 10 ), 5 );
                considerCenter.AddConsider( new ConsiderSearchEnemy( mapBorder, orderServer.RaderRadius ), 3 );
                considerCenter.AddConsider( new ConsiderRaderScan( mapBorder ), 3 );
                considerCenter.AddConsider( new ConsiderAwayFromEnemyTurret( mapBorder ), 4 );
                considerCenter.AddConsider( new ConsiderRaderLockEnemy(), 4 );
                considerCenter.AddConsider( new ConsiderKeepDistanceFromEnemy(), 4 );
                considerCenter.AddConsider( new ConsiderShootEnemy(), 5 );
            }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            considerCenter.Update( seconds );
        }

        #endregion

        void CollideHandler ( CollisionResult result, GameObjInfo objB )
        {
            if (objB.ObjClass == "Border")
            {
            }
            else if (objB.ObjClass == "ShellNormal")
            {
            }
            else if (objB.ObjClass == "DuelTank")
            {
            }

        }

        #region IAI 成员


        public void Draw ()
        {
            
        }

        #endregion
    }
}
