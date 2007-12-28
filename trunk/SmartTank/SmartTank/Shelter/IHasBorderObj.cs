using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TankEngine2D.DataStructure;
using TankEngine2D.Graphics;

namespace SmartTank.Shelter
{
    public interface IHasBorderObj
    {
        CircleList<BorderPoint> BorderData { get;}
        
        /// <summary>
        /// ��ʱ����Ϊ��ͼ���굽��Ļ�����ת��������дCoordin���Ժ��޸�Ϊ��ͼ���굽�߼������ת����
        /// </summary>
        Matrix WorldTrans { get;}

        Rectanglef BoundingBox { get;}
    }
}
