using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameBase.DataStructure;

namespace Platform.PhisicalCollision
{
    public interface IHasBorderObj
    {
        CircleList<GameBase.Graphics.Border> BorderData { get;}
        
        /// <summary>
        /// ��ʱ����Ϊ��ͼ���굽��Ļ�����ת��������дCoordin���Ժ��޸�Ϊ��ͼ���굽�߼������ת����
        /// </summary>
        Matrix WorldTrans { get;}

        Rectanglef BoundingBox { get;}
    }
}
