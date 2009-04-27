using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TankEngine2D.DataStructure;

namespace SmartTank.AI
{
    public class AICommonServer : IAICommonServer
    {
        Rectanglef mapBorder;

        public AICommonServer ( Rectanglef mapBorder )
        {
            this.mapBorder = mapBorder;
        }

        #region IAICommonServer ≥…‘±

        public Rectanglef MapBorder
        {
            get { return mapBorder; }
        }

        #endregion
    }
}
