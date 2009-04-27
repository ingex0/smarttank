using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameEngine.Shelter;

namespace GameEngine.Senses.Vision
{
    public interface IEyeableObj
    {
        Vector2[] KeyPoints { get;}
        Matrix TransMatrix { get;}
        Vector2 Pos { get;}

        GetEyeableInfoHandler GetEyeableInfoHandler { get;set;}
    }

    public interface IEyeableInfo
    {
        Vector2 Pos { get;}
        Vector2[] CurKeyPoints { get;}
        Matrix CurTransMatrix { get;}
    }

    public class EyeableInfo : IEyeableInfo
    {
        Vector2 pos;
        Vector2[] curKeyPoints;
        Matrix curTransMatrix;

        static public IEyeableInfo GetEyeableInfoHandler ( IRaderOwner raderOwner, IEyeableObj obj )
        {
            return new EyeableInfo( obj );
        }

        public EyeableInfo ( IEyeableObj obj )
        {
            this.pos = obj.Pos;
            this.curTransMatrix = obj.TransMatrix;

            curKeyPoints = new Vector2[obj.KeyPoints.Length];
            for (int i = 0; i < obj.KeyPoints.Length; i++)
            {
                curKeyPoints[i] = Vector2.Transform( obj.KeyPoints[i], obj.TransMatrix );
            }
        }

        #region IEyeableInfo ³ÉÔ±

        public Vector2 Pos
        {
            get { return pos; }
        }

        public Vector2[] CurKeyPoints
        {
            get { return curKeyPoints; }
        }

        public Matrix CurTransMatrix
        {
            get { return curTransMatrix; }
        }

        #endregion
    }
}
