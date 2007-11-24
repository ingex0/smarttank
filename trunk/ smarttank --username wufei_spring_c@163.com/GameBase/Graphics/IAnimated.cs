using System;
using System.Collections.Generic;
using System.Text;

namespace GameBase.Graphics
{
    interface IAnimated
    {
        bool IsStart { get;}
        bool IsEnd { get;}
        void DrawCurFrame ();
    }
}
