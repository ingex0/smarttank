using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TankEngine2D.Helpers;

namespace SmartTank.Effects
{
    /// <summary>
    /// ʵ��������Ч
    /// </summary>
    public static class Quake
    {
        #region Variables
        static bool sStarted = false;

        static float sStrength;

        static int sSumFrame;
        static int sRePlatformsFrame;

        static float sAx;
        static float sAy;
        static float sVx;
        static float sVy;
        static float sStrX;
        static float sStrY;
        static float sDeltaX;
        static float sDeltaY;

        static float sAtten;
        static float sCurRate;
        static float sCrest;

        static Rectangle orignScrnRect;
        #endregion

        #region Begin Quake

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="strengh">��ǿ��</param>
        /// <param name="sumFrame">�񶯳�����֡��</param>
        static public void BeginQuake ( float strengh, int sumFrame )
        {
            if (sumFrame <= 0)
                throw new Exception( "the value of sumFrame should biger than 0!" );

            strengh = Math.Abs( strengh );

            if (sStarted)
            {
                BaseGame.CoordinMgr.SetScreenViewRect( orignScrnRect );
            }

            sStarted = true;
            sStrength = strengh;
            sCurRate = 1f;

            sSumFrame = sumFrame;
            sRePlatformsFrame = sumFrame;
            orignScrnRect = BaseGame.CoordinMgr.ScrnViewRect;

            sStrX = strengh * RandomHelper.GetRandomFloat( -1f, 1f );
            sStrY = strengh * RandomHelper.GetRandomFloat( -1f, 1f );

            sAx = strengh * RandomHelper.GetRandomFloat( -0.7f, 0.7f );
            sAy = strengh * RandomHelper.GetRandomFloat( -0.7f, 0.7f );

            sVx = strengh * RandomHelper.GetRandomFloat( -0.7f, 0.7f );
            sVy = strengh * RandomHelper.GetRandomFloat( -0.7f, 0.7f );

            sAtten = 6 / (float)sumFrame;
            sCrest = 1;
        }
        #endregion

        #region Stop Quake

        /// <summary>
        /// ֹͣ��
        /// </summary>
        static public void StopQuake ()
        {
            sStarted = false;
            BaseGame.CoordinMgr.SetScreenViewRect( orignScrnRect );
        } 
        #endregion

        #region Update

        /// <summary>
        /// ���£��Ѿ���BaseGame�����
        /// </summary>
        static internal void Update ()
        {
            if (sStarted)
            {
                if (sRePlatformsFrame <= 0)
                {
                    sStarted = false;
                    BaseGame.CoordinMgr.SetScreenViewRect( orignScrnRect );
                    return;
                }
                else
                {
                    Next();
                    Rectangle curScrnViewRect =
                        new Rectangle( orignScrnRect.X + (int)sDeltaX,
                                        orignScrnRect.Y + (int)sDeltaY,
                                        orignScrnRect.Width,
                                        orignScrnRect.Height );

                    BaseGame.CoordinMgr.SetScreenViewRect( curScrnViewRect );
                    sRePlatformsFrame--;
                }
            }


        } 
        #endregion

        #region private Functions
        static void Next ()
        {
            sAx = -sStrX ;
            sVx += sAx;
            sStrX += sVx;

            sAy = -sStrY;
            sVy += sAy;
            sStrY += sVy;

            sCurRate -= sAtten;
            if (sCurRate < 0.2)
            {
                sCrest -= 0.167f;
                if (sCrest < 0) sCrest = 0;
                sCurRate = sCrest;
            }

            sDeltaX = sStrX * sCurRate;
            sDeltaY = sStrY * sCurRate;

        } 
        #endregion
    }
}
