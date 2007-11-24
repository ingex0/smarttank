using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Platform.GameObjects.Tank.TankAIs;
using Platform.AIHelper;

namespace InterRules.Duel
{
    [AIAttribute( "Dueler No.1", "Wufei", "", 2007, 11, 7 )]
    class DuelerNoFirst : IDuelAI
    {
        DuelAIOrderServer orderServer;
        AICommonServer commonServer;

        AIActionHelper action;

        Vector2 mapSize;

        ConsiderCenter considerCenter;

        public DuelerNoFirst ()
        {

        }

        #region IAI ��Ա

        public Platform.GameObjects.Tank.TankAIs.IAICommonServer CommonServer
        {
            set
            {
                commonServer = (AICommonServer)value;
                mapSize = commonServer.MapSize;

            }
        }

        public Platform.GameObjects.Tank.TankAIs.IAIOrderServer OrderServer
        {
            set
            {
                orderServer = (DuelAIOrderServer)value;
                orderServer.OnCollide += new OnCollidedEventHandlerAI( CollideHandler );
                action = new AIActionHelper( orderServer );

                considerCenter = new ConsiderCenter( orderServer );
                considerCenter.AddConsider( new ConsiderAwayFromBorder( mapSize, 30, 10 ), 5 );
                considerCenter.AddConsider( new ConsiderSearchEnemy( mapSize, orderServer.RaderRadius ), 3 );
                considerCenter.AddConsider( new ConsiderRaderScan( mapSize ), 3 );
                considerCenter.AddConsider( new ConsiderAwayFromEnemyTurret( mapSize ), 4 );
                considerCenter.AddConsider( new ConsiderRaderLockEnemy(), 4 );
                considerCenter.AddConsider( new ConsiderKeepDistanceFromEnemy(), 4 );
                considerCenter.AddConsider( new ConsiderShootEnemy(), 5 );
            }
        }

        #endregion

        #region IUpdater ��Ա

        public void Update ( float seconds )
        {
            considerCenter.Update( seconds );
        }

        #endregion

        void CollideHandler ( GameBase.Graphics.CollisionResult result, Platform.GameObjects.GameObjInfo objB )
        {
            if (objB.Name == "Border")
            {
            }
            else if (objB.Name == "ShellNormal")
            {
            }
            else if (objB.Name == "DuelTank")
            {
            }

        }

        #region IAI ��Ա


        public void Draw ()
        {
            
        }

        #endregion
    }
}
