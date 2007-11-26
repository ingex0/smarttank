using System;
using System.Collections.Generic;
using System.Text;

namespace GameBase.Graphics
{
    /// <summary>
    /// �������֡����
    /// </summary>
    interface IAnimated
    {
        /// <summary>
        /// ��ȡ�Ƿ��Ѿ���ʼ��ʾ
        /// </summary>
        bool IsStart { get;}

        /// <summary>
        /// ��ȡ�����Ƿ��Ѿ�����
        /// </summary>
        bool IsEnd { get;}

        /// <summary>
        /// ���Ƶ�ǰ֡����׼����һ֡
        /// </summary>
        void DrawCurFrame ();
    }
}
