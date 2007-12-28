using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SmartTank.GameObjects;
using SmartTank.Shelter;

namespace SmartTank.Senses.Vision
{
    public interface IEyeableObj
    {
        Vector2[] KeyPoints { get;}
        Matrix TransMatrix { get;}
        Vector2 Pos { get;}
        GameObjInfo ObjInfo { get;}

        GetEyeableInfoHandler GetEyeableInfoHandler { get;set;}
    }

    public interface IEyeableInfo
    {
        GameObjInfo ObjInfo { get;}
        Vector2 Pos { get;}
        Vector2[] CurKeyPoints { get;}
        Matrix CurTransMatrix { get;}
    }

    public class EyeableInfo : IEyeableInfo
    {
        GameObjInfo objInfo;
        Vector2 pos;
        Vector2[] curKeyPoints;
        Matrix curTransMatrix;

        static public IEyeableInfo GetEyeableInfoHandler ( IRaderOwner raderOwner, IEyeableObj obj )
        {
            return new EyeableInfo( obj );
        }

        public EyeableInfo ( IEyeableObj obj )
        {
            this.objInfo = obj.ObjInfo;
            this.pos = obj.Pos;
            this.curTransMatrix = obj.TransMatrix;

            curKeyPoints = new Vector2[obj.KeyPoints.Length];
            for (int i = 0; i < obj.KeyPoints.Length; i++)
            {
                curKeyPoints[i] = Vector2.Transform( obj.KeyPoints[i], obj.TransMatrix );
            }
        }

        #region IEyeableInfo ³ÉÔ±

        public GameObjInfo ObjInfo
        {
            get { return objInfo; }
        }

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
