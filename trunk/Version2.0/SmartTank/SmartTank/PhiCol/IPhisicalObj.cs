using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.PhiCol
{
    /// <summary>
    /// ��ʾһ�����������
    /// </summary>
    public interface IPhisicalUpdater
    {
        /// <summary>
        /// ������һ������״̬��������Ч
        /// </summary>
        /// <param name="seconds"></param>
        void CalNextStatus ( float seconds );

        /// <summary>
        /// ��Ч��һ������״̬
        /// </summary>
        void Validated ();
    }

    /// <summary>
    /// ��ʾһ���ܸ�������״̬������
    /// </summary>
    public interface IPhisicalObj
    {
        /// <summary>
        /// �������״̬������
        /// </summary>
        IPhisicalUpdater PhisicalUpdater { get;}
    }
}
