using System;
using System.Collections.Generic;
using System.Text;
using Platform.Senses.Vision;
using Microsoft.Xna.Framework;
using Platform.PhisicalCollision;
using GameBase.Graphics;
using Platform.Senses.Memory;
using GameBase.DataStructure;

namespace Platform.GameObjects.Tank.TankAIs
{
    public delegate void OnCollidedEventHandlerAI ( CollisionResult result, GameObjInfo objB );

    public interface IAIOrderServer
    {
        /// <summary>
        /// ��ÿɼ��������Ϣ
        /// </summary>
        /// <returns></returns>
        List<IEyeableInfo> GetEyeableInfo ();

        /// <summary>
        /// ���̹�˵ĵ�ǰλ��
        /// </summary>
        Vector2 Pos { get;}

        /// <summary>
        /// ���̹�˵�ǰ�ķ�λ��
        /// </summary>
        float Azi { get;}

        /// <summary>
        /// ���̹�˵�ǰ����ĵ�λ����
        /// </summary>
        Vector2 Direction { get;}

        /// <summary>
        /// ���̹��ǰ���ļ����ٶ�
        /// </summary>
        float MaxForwardSpeed { get;}

        /// <summary>
        /// ���̹�˺��˵ļ����ٶ�
        /// </summary>
        float MaxBackwardSpeed { get;}

        /// <summary>
        /// ���̹�˵ļ�����ת�ٶ�
        /// </summary>
        float MaxRotaSpeed { get;}

        /// <summary>
        /// ���̹�˵�ǰ��ǰ���ٶȣ����ֵΪ������ʾ̹�����ں���
        /// </summary>
        float ForwardSpeed { get; set;}

        /// <summary>
        /// ���̹�˵�ǰ����ת���ٶȡ����ȵ�λ��˳ʱ����תΪ����
        /// </summary>
        float TurnRightSpeed { get; set;}

        /// <summary>
        /// ���̹���״�İ뾶
        /// </summary>
        float RaderRadius { get;}

        /// <summary>
        /// ���̹���Žǵ�һ��
        /// </summary>
        float RaderAng { get;}

        /// <summary>
        /// ���̹���״ﵱǰ�����̹�˵ķ���
        /// </summary>
        float RaderAzi { get;}

        /// <summary>
        /// ���̹���״ﵱǰ�ķ�λ��
        /// </summary>
        float RaderAimAzi { get;}

        /// <summary>
        /// ���̹���״����ת�����ٶ�
        /// </summary>
        float MaxRotaRaderSpeed { get;}

        /// <summary>
        /// ��õ�ǰ̹���״����ת�ٶȣ�˳ʱ�뷽��Ϊ��
        /// </summary>
        float TurnRaderWiseSpeed { get; set;}

        /// <summary>
        /// ��ø�̹�˷����ڵ����ٶ�
        /// </summary>
        float ShellSpeed { get;}

        /// <summary>
        /// ���̹�˵Ŀ��
        /// </summary>
        float TankWidth { get;}

        /// <summary>
        /// ���̹�˵ĳ���
        /// </summary>
        float TankLength { get;}

        /// <summary>
        /// ��õ�ǰ̹�˿ɼ����б߽������
        /// </summary>
        EyeableBorderObjInfo[] EyeableBorderObjInfos { get;}

        /// <summary>
        /// ���㵼��ͼ
        /// </summary>
        /// <param name="selectFun">���������ɵ���ͼ��ʱ������Щ�б߽�����ĺ���</param>
        /// <param name="mapBorder">��ͼ�ı߽�</param>
        /// <param name="spaceForTank">����ײ��ı߽��Զ�ľ����ϱ�Ǿ�����</param>
        /// <returns></returns>
        NavigateMap CalNavigateMap ( NaviMapConsiderObj selectFun, Rectanglef mapBorder, float spaceForTank );

        /// <summary>
        /// ��̹�˷�����ײʱ
        /// </summary>
        event OnCollidedEventHandlerAI OnCollide;

        /// <summary>
        /// ��̹�˷����ص�ʱ
        /// </summary>
        event OnCollidedEventHandlerAI OnOverLap;
    }
}
