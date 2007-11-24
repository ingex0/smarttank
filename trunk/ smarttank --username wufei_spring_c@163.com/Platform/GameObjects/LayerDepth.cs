using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.GameObjects
{
    /*
     *  当前一些绘制物的深度
     *
     *
     *  场景物体：			深度
     *
     *  文字				0.1
     *
     *  界面				0.15
     *  
     *  炮弹				0.4
     *
     *  地面物体			0.5~0.65
     * 
	 *  坦克			    0.6~0.65
     *
     *  坦克雷达			0.9
     *
     *  地面				0.95
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
