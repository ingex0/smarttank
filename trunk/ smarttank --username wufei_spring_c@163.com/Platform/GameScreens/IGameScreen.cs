using System;
using System.Collections.Generic;
using System.Text;
using Platform.Update;

namespace Platform.GameScreens
{
    public interface IGameScreen
    {
        /// <summary>
        /// ����Ҫ�˳�����Ļʱ����true
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        bool Update ( float second );

        void Render ();
    }
}
