using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.Screens
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
