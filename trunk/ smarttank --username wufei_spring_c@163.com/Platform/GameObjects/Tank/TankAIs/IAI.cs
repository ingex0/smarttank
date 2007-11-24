using System;
using System.Collections.Generic;
using System.Text;
using Platform.Update;
using Platform.GameDraw;

namespace Platform.GameObjects.Tank.TankAIs
{
    public interface IAI : IUpdater
    {
        IAIOrderServer OrderServer { set;}
        IAICommonServer CommonServer { set;}

        void Draw ();
    }
}
