using System;
using System.Collections.Generic;
using System.Text;
using SmartTank.Update;
using SmartTank.Draw;

namespace SmartTank.Effects.SceneEffects
{
    public interface IManagedEffect : IUpdater, IDrawableObj
    {
        bool IsEnd { get; }
    }
}
