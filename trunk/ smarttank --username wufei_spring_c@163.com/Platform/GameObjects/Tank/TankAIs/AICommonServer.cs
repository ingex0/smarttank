using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Platform.GameObjects.Tank.TankAIs
{
    public class AICommonServer:IAICommonServer
    {
        Vector2 mapSize;

        public AICommonServer ( Vector2 mapSize )
        {
            this.mapSize = mapSize;
        }

        #region IAICommonServer ≥…‘±

        public Vector2 MapSize
        {
            get { return mapSize; }
        }

        #endregion
    }
}
