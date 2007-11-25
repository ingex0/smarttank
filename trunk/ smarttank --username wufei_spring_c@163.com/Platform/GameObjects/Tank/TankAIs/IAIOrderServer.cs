using System;
using System.Collections.Generic;
using System.Text;
using Platform.Senses.Vision;
using Microsoft.Xna.Framework;
using Platform.PhisicalCollision;
using GameBase.Graphics;
using Platform.Senses.Memory;
using GameBase.DataStructure;

namespace Platform.GameObjects.Tank.TankAIs
{
    public delegate void OnCollidedEventHandlerAI ( CollisionResult result, GameObjInfo objB );

    public interface IAIOrderServer
    {
        List<IEyeableInfo> GetEyeableInfo ();

        Vector2 Pos { get;}

        float Azi { get;}

        Vector2 Direction { get;}

        float MaxForwardSpeed { get;}

        float MaxBackwardSpeed { get;}

        float MaxRotaSpeed { get;}

        float ForwardSpeed { get; set;}

        float TurnRightSpeed { get; set;}

        float RaderRadius { get;}

        float RaderAng { get;}

        float RaderAzi { get;}

        float RaderAimAzi { get;}

        float MaxRotaRaderSpeed { get;}

        float TurnRaderWiseSpeed { get; set;}

        float ShellSpeed { get;}

        EyeableBorderObjInfo[] EyeableBorderObjInfos { get;}

        NavigateMap CalNavigateMap ( NaviMapConsiderObj selectFun, Rectanglef mapBorder, float spaceForTank );

        event OnCollidedEventHandlerAI OnCollide;

        event OnCollidedEventHandlerAI OnOverLap;
    }
}
