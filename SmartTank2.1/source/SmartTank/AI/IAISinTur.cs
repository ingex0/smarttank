using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.AI
{
    [IAIAttribute( typeof( IAIOrderServerSinTur ), typeof( IAICommonServer ) )]
    public interface IAISinTur : IAI
    {

    }
}
