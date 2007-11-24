using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.GameObjects
{
    /*
     *  ��ǰһЩ����������
     *
     *
     *  �������壺			���
     *
     *  ����				0.1
     *
     *  ����				0.15
     *  
     *  �ڵ�				0.4
     *
     *  ��������			0.5~0.65
     * 
	 *  ̹��			    0.6~0.65
     *
     *  ̹���״�			0.9
     *
     *  ����				0.95
     * 
     * */


    public static class LayerDepth
    {
        public static readonly float Mouse = 0.05f;

        public static readonly float Text = 0.1f;

        public static readonly float UI = 0.15f;

        public static readonly float EffectLow = 0.3f;

        public static readonly float Shell = 0.4f;

        public static readonly float TankTurret = 0.6f;

        public static readonly float TankBase = 0.65f;

        public static readonly float GroundObj = 0.66f;

        public static readonly float TankRader = 0.9f;

        public static readonly float BackGround = 0.95f;


    }
}
