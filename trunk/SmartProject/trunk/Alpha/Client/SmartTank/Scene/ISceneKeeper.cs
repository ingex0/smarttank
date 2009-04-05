using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.PhiCol;
using SmartTank.Shelter;
using SmartTank.Draw;
using SmartTank.Update;
using SmartTank.Senses.Vision;
using SmartTank.GameObjs;

namespace SmartTank.Scene
{
    public interface ISceneKeeper
    {
        void RegistPhiCol ( PhiColMgr manager );

        void RegistShelter ( ShelterMgr manager );

        void RegistDrawables ( DrawMgr manager );

        void RegistUpdaters ( UpdateMgr manager );

        void RegistVision ( VisionMgr manager );

        IGameObj GetGameObj(string objPath);

        bool AddGameObj(string groupPath, IGameObj obj);

        bool DelGameObj(string objPath);
    }
}
