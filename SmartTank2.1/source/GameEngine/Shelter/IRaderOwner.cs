using System;
using System.Collections.Generic;
using System.Text;
using GameEngine.Senses.Vision;
using GameEngine.Senses.Memory;

namespace GameEngine.Shelter
{
    public delegate void BorderObjUpdatedEventHandler ( EyeableBorderObjInfo[] borderObjInfos );

    public interface IRaderOwner
    {
        Rader Rader { get;}
        void BorderObjUpdated ( EyeableBorderObjInfo[] borderObjInfo );
    }
}
