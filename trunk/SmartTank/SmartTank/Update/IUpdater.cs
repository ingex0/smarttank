using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.Update
{
    public interface IUpdater
    {
        void Update ( float seconds );
    }
}
