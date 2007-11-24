using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using InterRules.Duel;
using Platform.GameObjects.Tank.TankAIs;

namespace TestAI
{
    [AIAttribute( "TestAI No.1", "Wufei", "Test DI", 2007, 11, 15 )]
    public class TestAIFirst : IDuelAI
    {

        public DuelAIOrderServer orderServer;

        #region IAI 成员

        public Platform.GameObjects.Tank.TankAIs.IAICommonServer CommonServer
        {
            set { }
        }

        public Platform.GameObjects.Tank.TankAIs.IAIOrderServer OrderServer
        {
            set { orderServer = (DuelAIOrderServer)value; }
        }

        #endregion

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            orderServer.ForwardSpeed = orderServer.MaxForwardSpeed;
        }

        #endregion

        #region IAI 成员


        public void Draw ()
        {
            
        }

        #endregion
    }
}
