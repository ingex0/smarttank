using System;
using System.Collections.Generic;
using System.Text;
using Platform.GameObjects.Tank.TankControls;

namespace Platform.GameObjects.Tank.TankAIs
{
    [IAIAttribute( typeof( IAIOrderServerSinTur ), typeof( IAICommonServer ) )]
    public interface IAISinTur : IAI
    {

    }
}
