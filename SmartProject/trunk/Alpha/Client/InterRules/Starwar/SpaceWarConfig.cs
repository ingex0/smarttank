using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace InterRules.Starwar
{
    class SpaceWarConfig
    {
        public static float SpeedMax;
        public static float SpeedAccel;       // 加速度
        public static float SpeedDecay;       // 在无输入状态下每秒降低的速度百分比，实际是积分过程 
        public static float StillSpeedScale; // 硬直状态下移动速度的缩减系数

        public static int IniHP;
        public static float ShootCD;
        public static int HitbyShellDamage;
        public static int HitbyObjExceptShell;
        public static float StillTime;
        public static float WTFTime;
        public static int ScoreByHit;

        public static int ShootEndDest;

        public static int ShellSpeed;
        public static int ShellLiveTime;

        public static int GoldScore;
        public static float GoldLiveTime;

        public static float RockCreateTime;
        public static float RockMinSpeed;
        public static float RockMaxSpeed;
        public static float RockMaxAziSpeed;

        static public void LoadConfig()
        {
            FileStream confile = File.OpenRead("Content\\Rules\\SpaceWar\\WarShipConfig.txt");
            StreamReader SR = new StreamReader(confile);
            string line = "";
            try
            {
                while (!SR.EndOfStream)
                {
                    line = SR.ReadLine();

                    if (line.StartsWith("SpeedMax=")) SpaceWarConfig.SpeedMax = float.Parse(line.Substring(9));
                    else if (line.StartsWith("SpeedAccel=")) SpaceWarConfig.SpeedAccel = float.Parse(line.Substring(11));
                    else if (line.StartsWith("SpeedDecay=")) SpaceWarConfig.SpeedDecay = float.Parse(line.Substring(11));
                    else if (line.StartsWith("StillSpeedScale=")) SpaceWarConfig.StillSpeedScale = float.Parse(line.Substring(16));
                    else if (line.StartsWith("IniHP=")) SpaceWarConfig.IniHP = int.Parse(line.Substring(6));
                    else if (line.StartsWith("ShootCD=")) SpaceWarConfig.ShootCD = float.Parse(line.Substring(8));
                    else if (line.StartsWith("HitbyShellDamage=")) SpaceWarConfig.HitbyShellDamage = int.Parse(line.Substring(17));
                    else if (line.StartsWith("HitbyObjDamage=")) SpaceWarConfig.HitbyObjExceptShell = int.Parse(line.Substring(15));
                    else if (line.StartsWith("StillTime=")) SpaceWarConfig.StillTime = float.Parse(line.Substring(10));
                    else if (line.StartsWith("WTFTime=")) SpaceWarConfig.WTFTime = float.Parse(line.Substring(8));
                    else if (line.StartsWith("ScoreByHit=")) SpaceWarConfig.ScoreByHit = int.Parse(line.Substring(11));
                    else if (line.StartsWith("ShootEndDest=")) SpaceWarConfig.ShootEndDest = int.Parse(line.Substring(13));
                    else if (line.StartsWith("ShellSpeed=")) SpaceWarConfig.ShellSpeed = int.Parse(line.Substring(11));
                    else if (line.StartsWith("ShellLiveTime=")) SpaceWarConfig.ShellLiveTime = int.Parse(line.Substring(14));
                    else if (line.StartsWith("GoldScore=")) SpaceWarConfig.GoldScore = int.Parse(line.Substring(10));
                    else if (line.StartsWith("GoldLiveTime=")) SpaceWarConfig.GoldLiveTime = float.Parse(line.Substring(13));
                    else if (line.StartsWith("RockCreateTime=")) SpaceWarConfig.RockCreateTime = float.Parse(line.Substring(15));
                    else if (line.StartsWith("RockMinSpeed=")) SpaceWarConfig.RockMinSpeed = float.Parse(line.Substring(13));
                    else if (line.StartsWith("RockMaxSpeed=")) SpaceWarConfig.RockMaxSpeed = float.Parse(line.Substring(13));
                    else if (line.StartsWith("RockMaxAziSpeed=")) SpaceWarConfig.RockMaxAziSpeed = float.Parse(line.Substring(16));
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("WarShipConfig.txt配置文件格式错误 ： " + line + "  " + ex.ToString());
            }

        }

    }
}
