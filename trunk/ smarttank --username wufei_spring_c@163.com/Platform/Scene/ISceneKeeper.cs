using System;
using System.Collections.Generic;
using System.Text;
using Platform.PhisicalCollision;
using Platform.Shelter;
using Platform.GameDraw;
using Platform.Update;
using Platform.Senses.Vision;

namespace Platform.Scene
{
    public interface ISceneKeeper
    {
        void RegistPhiCol ( PhiColManager manager );

        void RegistShelter ( ShelterManager manager );

        void RegistDrawables ( DrawManager manager );

        void RegistUpdaters ( UpdateManager manager );

        void RegistVision ( VisionManager manager );
    }
}
