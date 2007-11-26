using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameBase;
using Platform.Update;
using Platform.GameObjects;
using GameBase.DataStructure;
using GameBase.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Platform.GameDraw.UIElement
{
    public class Compass
    {
        Vector2 drawPos;           //绘制位置
        Vector2 cameraPos;         //Camera位置
        private float rota;         //指向
        Vector2 TestPos;

        public Vector2 DrawPos
        {
            get { return drawPos; }

            set { drawPos = value; }
        }
        public Vector2 CameraPos
        {
            get { return cameraPos; }
            set { cameraPos = value; }
        }
        public float Azi
        {
            get { return rota; }
            set { rota = value; }
        }

        public Compass(Vector2 drawPos)
        {
            this.drawPos = drawPos;
            this.rota = 0;
            if (Camera.CurCamera != null)
                this.cameraPos = Camera.CurCamera.CenterPos;
        }

        public void Update()
        {
            if (Camera.CurCamera != null)
                cameraPos = Camera.CurCamera.CenterPos;
        }

        public void Draw()
        {
            Vector2 drawPosInLogic = Coordin.LogicPos(drawPos);
            BasicGraphics.DrawLine(drawPosInLogic, drawPosInLogic - new Vector2(0, Coordin.LogicLength(50)), 3, Color.Red, 0);
        }

    }
}
