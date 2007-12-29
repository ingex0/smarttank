using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.GameObjs.Tank.SinTur
{
    public class TankLimitSinTur
    {
        public float MaxForwardSpeed;
        public float MaxBackwardSpeed;
        public float MaxTurnAngleSpeed;
        public float MaxTurretAngleSpeed;
        public float MaxRaderAngleSpeed;

        public TankLimitSinTur ( float maxForwardSpeed, float maxBackwardSpeed, float maxTurnAngleSpeed, float maxTurretAngleSpeed, float maxRaderAngleSpeed )
        {
            this.MaxForwardSpeed = maxForwardSpeed;
            this.MaxBackwardSpeed = maxBackwardSpeed;
            this.MaxTurnAngleSpeed = maxTurnAngleSpeed;
            this.MaxTurretAngleSpeed = maxTurretAngleSpeed;
            this.MaxRaderAngleSpeed = maxRaderAngleSpeed;
        }

        public float LimitSpeed ( float speed )
        {
            return Math.Min( MaxForwardSpeed, Math.Max( -MaxBackwardSpeed, speed ) );
        }

        public float LimitAngleSpeed ( float angleSpeed )
        {
            return Math.Min( MaxTurnAngleSpeed, Math.Max( -MaxTurnAngleSpeed, angleSpeed ) );
        }

        public float LimitTurretAngleSpeed ( float turretAngleSpeed )
        {
            return Math.Min( MaxTurretAngleSpeed, Math.Max( -MaxTurretAngleSpeed, turretAngleSpeed ) );
        }

        internal float LimitRaderAngleSpeed ( float raderAngleSpeed )
        {
            return Math.Min( MaxRaderAngleSpeed, Math.Max( -MaxRaderAngleSpeed, raderAngleSpeed ) );
        }
    }
}
