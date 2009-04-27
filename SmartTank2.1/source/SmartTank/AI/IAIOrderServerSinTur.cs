using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Senses.Vision;
using Microsoft.Xna.Framework;
using SmartTank.Shelter;

namespace SmartTank.AI
{
    public interface IAIOrderServerSinTur : IAIOrderServer
    {
        Vector2 TurretAxePos { get;}

        float TurretAzi { get;}

        float TurretAimAzi { get;}

        float MaxRotaTurretSpeed { get;}

        float TurnTurretWiseSpeed { get; set;}

        float TurretLength { get;}

        float FireLeftCDTime { get;}

        void Fire ();

        event BorderObjUpdatedEventHandler onBorderObjUpdated;
    }
}
