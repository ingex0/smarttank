using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmartTank.PhiCol
{
    public interface ISyncable
    {
        void SetServerStatue(Vector2 serPos, Vector2 serVel, float serAzi, float serAziVel, float syncTime);
    }
}
