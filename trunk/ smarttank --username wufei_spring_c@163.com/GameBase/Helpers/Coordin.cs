using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using GameBase.DataStructure;
using GameBase.Helpers;


namespace GameBase
{
    public static class Coordin
    {
        #region Variables

        static private Rectangle clientRect;

        //static private Rectanglef logicViewRect;

        static private Vector2 logicCenter;

        static private Vector2 scrnCenter;

        static private float rota;

        static private Matrix rotaMatrix;
        static private Matrix rotaMatrixInvert;


        static private Rectangle gameViewRect;

        static private float scale;

        static bool screenViewRectSeted = false;

        #endregion

        #region Properties
        //static public Rectanglef LogicRect
        //{
        //    get { return logicViewRect; }
        //}

        static public Rectangle ScrnViewRect
        {
            get { return gameViewRect; }
        }

        static public int ClientWidth
        {
            get { return clientRect.Width; }
        }

        static public int ClientHeight
        {
            get { return clientRect.Height; }
        }

        static public int ViewWidth
        {
            get { return gameViewRect.Width; }
        }

        static public int ViewHeight
        {
            get { return gameViewRect.Height; }
        }

        static public Vector2 TexelSize
        {
            get { return new Vector2( 1 / scale, 1 / scale ); }
        }

        static public float Rota
        {
            get { return rota; }
        }

        static public Matrix RotaMatrixFromLogicToScrn
        {
            get { return rotaMatrixInvert; }
        }

        static public Matrix RotaMatrixFromScrnToLogic
        {
            get { return rotaMatrix; }
        }


        #endregion

        #region SetFunctions Called By Platform

        static public void SetClientRect ( Rectangle rect )
        {
            clientRect = rect;
        }

        static public void SetScreenViewRect ( Rectangle rect )
        {
            gameViewRect = rect;
            scrnCenter = new Vector2( rect.X + 0.5f * rect.Width, rect.Y + 0.5f * rect.Height );
            screenViewRectSeted = true;
        }

        static public void SetCamera ( float setScale, Vector2 centerLogicPos, float setRota )
        {
            if (!screenViewRectSeted)
                throw new Exception( "you should call SetScreenViewRect before SetLogicViewRect" );

            scale = setScale;
            rota = setRota;
            rotaMatrix = Matrix.CreateRotationZ( rota );
            rotaMatrixInvert = Matrix.CreateRotationZ( -rota );
            logicCenter = centerLogicPos;
        }

        #endregion

        #region HelpFunctions

        static public float LogicLength ( int scrnLength )
        {
            return scrnLength / scale;
        }

        static public float LogicLength ( float scrnLength )
        {
            return scrnLength / scale;
        }

        static public int ScrnLength ( float logicLength )
        {
            return MathTools.Round( logicLength * scale );
        }

        static public float ScrnLengthf ( float logicLength )
        {
            return logicLength * scale;
        }

        static public Vector2 LogicPos ( Vector2 screenPos )
        {
            return Vector2.Transform( screenPos - scrnCenter, rotaMatrix ) / scale + logicCenter;
        }

        static public Vector2 ScreenPos ( Vector2 logicPos )
        {
            return Vector2.Transform( logicPos - logicCenter, rotaMatrixInvert ) * scale + scrnCenter;
        }

        static public Vector2 LogicVector ( Vector2 screenVector )
        {
            return new Vector2( Coordin.LogicLength( screenVector.X ), Coordin.LogicLength( screenVector.Y ) );
        }

        //public static Rectangle ScreenRectangle ( Rectanglef logicRect )
        //{
        //    return new Rectangle( ScrnX( logicRect.X ), ScrnY( logicRect.Y ), ScrnLength( logicRect.Width ), ScrnLength( logicRect.Height ) );
        //}

        #endregion

    }
}
