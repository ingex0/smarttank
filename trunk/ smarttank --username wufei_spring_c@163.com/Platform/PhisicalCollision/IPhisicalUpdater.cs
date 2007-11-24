using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameObjects;

namespace Platform.PhisicalCollision
{
    public interface IPhisicalUpdater
    {
        void CalNextStatus ( float seconds );
        void Validated ();
    }

    public interface IPhisicalObj
    {
        IPhisicalUpdater PhisicalUpdater { get;}
    }
}
