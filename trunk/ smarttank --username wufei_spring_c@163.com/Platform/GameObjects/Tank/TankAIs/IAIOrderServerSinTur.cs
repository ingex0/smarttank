using System;
using System.Collections.Generic;
using System.Text;
using Platform.Senses.Vision;
using Microsoft.Xna.Framework;

namespace Platform.GameObjects.Tank.TankAIs
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
    }
}
