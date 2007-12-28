using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using SmartTank.GameObjects;
using TankEngine2D.DataStructure;

namespace SmartTank.Draw
{
    /*
     * 需要实现场景物体的跟踪。
     * 
     * 视域的缩放动画等效果。
     * 
     * 
     * */
    public class Camera 
    {
        static Camera curCamera;
        static public event EventHandler onCameraScaled;

        static public Camera CurCamera
        {
            get
            {
                return curCamera;
            }
        }

        const float defaultMinScale = 0.8f;
        const float defaultMaxScale = 6f;

        float scale;
        Vector2 centerPos;

        bool enabled = false;

        IGameObj focusObj;
        bool focusing = false;
        bool focusAzi = false;

        public float minScale = defaultMinScale;
        public float maxScale = defaultMaxScale;

        float rota;

        //public Rectanglef LogicViewRect
        //{
        //    get
        //    {
        //        return Coordin.LogicRect;
        //    }
        //}


        public Vector2 CenterPos
        {
            get { return centerPos; }
            set
            {
                centerPos = value;
                SubmitChange();
            }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                scale = Math.Min( Math.Max( minScale, scale ), maxScale );
                if (curCamera == this && onCameraScaled != null)
                    onCameraScaled( this, EventArgs.Empty );
                SubmitChange();
            }
        }

        public float Azi
        {
            get { return rota; }
            set
            {
                rota = value;
                SubmitChange();
            }
        }

        public Camera ( float scale, Vector2 centerPos, float rota )
        {
            this.scale = scale;
            this.centerPos = centerPos;
            this.rota = rota;
        }

        public void Enable ()
        {
            if (curCamera != null)
                curCamera.Disable();
            curCamera = this;
            enabled = true;
            SubmitChange();
        }

        private void SubmitChange ()
        {
            BaseGame.CoordinMgr.SetCamera( scale, centerPos, rota );
        }

        public void Disable ()
        {
            enabled = false;
        }

        public void Move ( Vector2 deltaVector )
        {
            centerPos += Vector2.Transform( deltaVector, BaseGame.CoordinMgr.RotaMatrixFromScrnToLogic );
            SubmitChange();
        }

        /// <summary>
        /// 缩放镜头
        /// </summary>
        /// <param name="rate">正数表示放大百分比，负数表示缩小百分比</param>
        public void Zoom ( float rate )
        {
            scale *= (1 + rate);
            scale = Math.Min( Math.Max( minScale, scale ), maxScale );
            SubmitChange();
            if (curCamera == this && onCameraScaled != null)
                onCameraScaled( this, EventArgs.Empty );
        }

        /// <summary>
        /// 旋转镜头
        /// </summary>
        /// <param name="ang"></param>
        public void Rota ( float ang )
        {
            rota += ang;
            SubmitChange();
        }

        public void Focus ( IGameObj obj, bool focusAzi )
        {
            focusing = true;
            focusObj = obj;
            this.focusAzi = focusAzi;
        }

        public void FocusCancel ()
        {
            focusing = false;
        }

        #region IUpdater 成员

        public void Update ( float seconds )
        {
            if (!enabled)
                return;

            if (focusing)
            {
                centerPos = centerPos * 0.95f + focusObj.Pos * 0.05f;
                if (focusAzi)
                    rota = rota * 0.95f + focusObj.Azi * 0.05f;
                SubmitChange();
            }

        }

        #endregion
    }
}
