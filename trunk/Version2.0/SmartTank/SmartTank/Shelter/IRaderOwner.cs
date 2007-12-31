using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Senses.Vision;
using SmartTank.Senses.Memory;

namespace SmartTank.Shelter
{
    public delegate void BorderObjUpdatedEventHandler ( EyeableBorderObjInfo[] borderObjInfos );

    public interface IRaderOwner
    {
        Rader Rader { get;}
        void BorderObjUpdated ( EyeableBorderObjInfo[] borderObjInfo );
    }
}
