using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Update;
using SmartTank.Draw;

namespace SmartTank.AI
{
    public interface IAI : IUpdater
    {
        IAIOrderServer OrderServer { set;}
        IAICommonServer CommonServer { set;}

        void Draw ();
    }
}
