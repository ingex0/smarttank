using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.PhiCol;
using SmartTank.Shelter;
using SmartTank.Draw;
using SmartTank.Update;
using SmartTank.Senses.Vision;

namespace SmartTank.Scene
{
    public interface ISceneKeeper
    {
        void RegistPhiCol ( PhiColMgr manager );

        void RegistShelter ( ShelterManager manager );

        void RegistDrawables ( DrawManager manager );

        void RegistUpdaters ( UpdateManager manager );

        void RegistVision ( VisionManager manager );
    }
}
