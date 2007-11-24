using System;
using System.Collections.Generic;
using System.Text;
using Platform.Update;

namespace Platform.GameDraw.SceneEffects
{
    public interface IManagedEffect : IUpdater, IDrawableObj
    {
        bool IsEnd { get; }
    }
}
