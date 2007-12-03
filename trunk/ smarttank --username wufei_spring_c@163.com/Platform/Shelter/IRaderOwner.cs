using System;
using System.Collections.Generic;
using System.Text;
using Platform.Senses.Vision;
using Platform.Senses.Memory;

namespace Platform.Shelter
{
    public delegate void BorderObjUpdatedEventHandler ( EyeableBorderObjInfo[] borderObjInfos );

    public interface IRaderOwner
    {
        Rader Rader { get;}
        void BorderObjUpdated ( EyeableBorderObjInfo[] borderObjInfo );
    }
}
