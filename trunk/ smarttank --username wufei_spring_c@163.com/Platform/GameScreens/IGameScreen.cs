using System;
using System.Collections.Generic;
using System.Text;
using Platform.Update;

namespace Platform.GameScreens
{
    public interface IGameScreen
    {
        /// <summary>
        /// 当需要退出该屏幕时返回true
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        bool Update ( float second );

        void Render ();
    }
}
