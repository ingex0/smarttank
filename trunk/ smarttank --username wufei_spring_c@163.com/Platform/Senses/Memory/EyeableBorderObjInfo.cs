using System;
using System.Collections.Generic;
using System.Text;
using Platform.Senses.Vision;
using Platform.PhisicalCollision;
using Microsoft.Xna.Framework;

namespace Platform.Senses.Memory
{
    public class EyeableBorderObjInfo
    {
        IEyeableInfo eyeableInfo;
        ObjVisiBorder border;
        bool isDisappeared = false;

        ConvexHall convexHall;

        public EyeableBorderObjInfo ( IEyeableInfo eyeableInfo, ObjVisiBorder border )
        {
            this.eyeableInfo = eyeableInfo;
            this.border = border;
        }

        public ObjVisiBorder Border
        {
            get { return border; }
        }

        public IEyeableInfo EyeableInfo
        {
            get { return eyeableInfo; }
        }

        public bool IsDisappeared
        {
            get { return isDisappeared; }
        }

        public void UpdateConvexHall ( float minOptimizeDest )
        {
            Vector2 transV = Vector2.Transform( Vector2.UnitX, eyeableInfo.CurTransMatrix );
            Vector2 transZero = Vector2.Transform( Vector2.Zero, eyeableInfo.CurTransMatrix );
            float scale = (transV - transZero).Length();
            float minOptimizeDestInTexSpace = minOptimizeDest / scale;

            if (convexHall == null)
                convexHall = new ConvexHall( border.VisiBorder, minOptimizeDestInTexSpace );
            else
                convexHall.BuildConvexHall( border.VisiBorder, minOptimizeDestInTexSpace );
        }

        public ConvexHall ConvexHall
        {
            get { return convexHall; }
        }

        internal IHasBorderObj Obj
        {
            get { return border.Obj; }
        }

        internal bool Combine ( EyeableBorderObjInfo borderObjInfo )
        {
            bool objUpdated = false;

            if (borderObjInfo.eyeableInfo.Pos != this.eyeableInfo.Pos)
                objUpdated = true;

            this.eyeableInfo = borderObjInfo.eyeableInfo;
            if (this.border.Combine( borderObjInfo.border.VisiBorder ))
                objUpdated = true;

            isDisappeared = false;

            return objUpdated;
        }

        internal void SetIsDisappeared ( bool isDisappeared )
        {
            this.isDisappeared = isDisappeared;
        }


    }
}
