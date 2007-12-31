using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TankEngine2D.DataStructure;
using Microsoft.Xna.Framework.Graphics;
using TankEngine2D.Helpers;
using TankEngine2D.Graphics;
using SmartTank.GameObjs;
using SmartTank.Draw;
using SmartTank.Senses.Vision;
using SmartTank.Senses.Memory;

namespace SmartTank.Shelter
{
    /* ��װһ�����ε��״
     * 
     * ��GPU������ڵ������״�ͼ�εĻ��ơ�
     * 
     * 
     * �״�ռ�(Rader Space)ָ����������Ϊr�ᣬ˳ʱ��Ϊ���������ļ�����ϵ����r��l��
     * 
     * �������λ������˵������ֱ�Ϊ��1��1���ͣ�1��-1����
     * 
     * ���ڴ˿ռ������������ݺ��ڵ��ļ��㡣
     * 
     * */

    public class Rader : IDrawableObj
    {
        #region Constants

        public static readonly float smallMapScale = (float)ShelterMgr.texSizeSmall / (float)ShelterMgr.texSize;

        #endregion

        #region Variables

        /// <summary>
        /// �нǵ�һ��
        /// </summary>
        float ang;

        /// <summary>
        /// �뾶
        /// </summary>
        float r;

        /// <summary>
        /// Բ��λ��
        /// </summary>
        Vector2 pos;

        /// <summary>
        /// ���ߵķ�λ��
        /// </summary>
        float azi;

        /// <summary>
        /// 
        /// </summary>
        Rectanglef bouBox;

        /// <summary>
        /// �״����ɫ
        /// </summary>
        Color color;

        /// <summary>
        /// ��ת�����ڼ�����ͼʱʹ�á�
        /// </summary>
        Matrix rotaMatrix;

        Texture2D texture;

        Texture2D texSmall;

        Color[] textureData;

        internal RaderDepthMap depthMap;

        internal RenderTarget2D target;
        internal RenderTarget2D targetSmall;

        int leftWaitframe = 0;

        // ��ǰ���ڵ���
        IShelterObj[] shelterObjs = new IShelterObj[0];

        ObjVisiBorder[] shelterVisiBorders = new ObjVisiBorder[0];

        EyeableBorderObjInfo[] eyeableBorderObjInfos = new EyeableBorderObjInfo[0];

        List<IEyeableInfo> curEyeableInfo = new List<IEyeableInfo>();

        ObjMemoryKeeper objMemoryKeeper;

        #endregion

        #region Properties

        public Vector2 Pos
        {
            get { return pos; }
            set
            {
                pos = value;
            }
        }

        public float X
        {
            get { return pos.X; }
            set
            {
                pos.X = value;
            }
        }

        public float Y
        {
            get { return pos.Y; }
            set
            {
                pos.Y = value;
            }
        }

        public float Azi
        {
            get { return azi; }
            set
            {
                azi = value;
            }
        }

        /// <summary>
        /// �нǵ�һ��
        /// </summary>
        public float Ang
        {
            get { return ang; }
            set { ang = value; }
        }

        /// <summary>
        /// �뾶
        /// </summary>
        public float R
        {
            get { return r; }
            set { r = value; }
        }

        public Rectanglef BoundBox
        {
            get { return bouBox; }
        }

        /// <summary>
        /// ���ߵ�λ����
        /// ʹ�ø����Ի�������Ǻ�����ֵ����ҪƵ�����á�
        /// </summary>
        public Vector2 N
        {
            get
            {
                return new Vector2( (float)Math.Sin( azi ), -(float)Math.Cos( azi ) );
            }
        }

        /// <summary>
        /// ��߽絥λ����
        /// ʹ�ø����Ի�������Ǻ�����ֵ����ҪƵ�����á�
        /// </summary>
        public Vector2 B
        {
            get
            {
                return new Vector2( (float)Math.Sin( azi - ang ), -(float)Math.Cos( azi - ang ) );
            }
        }

        /// <summary>
        /// �ұ߽絥λ����
        /// ʹ�ø����Ի�������Ǻ�����ֵ����ҪƵ�����á�
        /// </summary>
        public Vector2 U
        {
            get
            {
                return new Vector2( (float)Math.Sin( azi + ang ), -(float)Math.Cos( azi + ang ) );
            }
        }

        /// <summary>
        /// ��߽��
        /// ʹ�ø����Ի�������Ǻ�����ֵ����ҪƵ�����á�
        /// </summary>
        public Vector2 LeftP
        {
            get
            {
                return pos + B * r;
            }

        }

        /// <summary>
        /// �ұ߽��
        /// ʹ�ø����Ի�������Ǻ�����ֵ����ҪƵ�����á�
        /// </summary>
        public Vector2 RightP
        {
            get
            {
                return pos + U * r;
            }
        }

        /// <summary>
        /// �״����ɫ
        /// </summary>
        public Color RaderColor
        {
            get { return color; }
        }

        /// <summary>
        /// ��ת�����ڼ�����ͼʱʹ�á�
        /// ��ÿһ֡ʹ��ǰ����Ҫ����UpdateMatrix������
        /// </summary>
        internal Matrix RotaMatrix
        {
            get { return rotaMatrix; }
        }

        /// <summary>
        /// ��õ�ǰ�ڵ��״������
        /// </summary>
        public IShelterObj[] ShelterObjs
        {
            get { return shelterObjs; }
            set { shelterObjs = value; }
        }

        /// <summary>
        /// ��ȡ��ǰ�ɼ��ڵ�����Ŀɼ��߽���Ϣ
        /// </summary>
        internal ObjVisiBorder[] ShelterVisiBorders
        {
            get { return shelterVisiBorders; }
            set { shelterVisiBorders = value; }
        }

        //public ObjVisiBorder[] NonShelterVisiBorders
        //{
        //    get { return nonShelterVisiBorders; }
        //    set { nonShelterVisiBorders = value; }
        //}

        internal EyeableBorderObjInfo[] EyeableBorderObjInfos
        {
            get { return eyeableBorderObjInfos; }
            set { eyeableBorderObjInfos = value; }
        }

        public List<IEyeableInfo> CurEyeableObjs
        {
            get { return curEyeableInfo; }
            set { curEyeableInfo = value; }
        }

        /// <summary>
        /// ��ǰ��Ұ�е����ͼ��������˳ʱ�뷽������
        /// ֵΪ1��ʾ��ǰ�������ڵ���
        /// </summary>
        public float[] DepthMap
        {
            get { return depthMap.DepthMap; }
        }

        /// <summary>
        /// ���������������ı߽���Ϣ��
        /// ����ObjMemoryMgr�н����RaderOwnerע��Ϊ����߽���Ϣ�����顣
        /// </summary>
        public ObjMemoryKeeper ObjMemoryKeeper
        {
            get { return objMemoryKeeper; }
            set { objMemoryKeeper = value; }
        }

        #endregion

        #region Construction

        public Rader ( float ang, float r, Vector2 pos, float azi, Color color )
        {
            this.pos = pos;
            this.azi = azi;
            this.r = r;
            this.ang = ang;
            this.color = color;

            this.depthMap = new RaderDepthMap( ShelterMgr.gridSum );

            int texSize = ShelterMgr.texSize;
            int texSizeSmall = ShelterMgr.texSizeSmall;

            target = new RenderTarget2D( BaseGame.Device, texSize, texSize, 1, BaseGame.Device.PresentationParameters.BackBufferFormat );
            targetSmall = new RenderTarget2D( BaseGame.Device, texSizeSmall, texSizeSmall, 1, BaseGame.Device.PresentationParameters.BackBufferFormat );

            Camera.onCameraScaled += new EventHandler( Camera_onCameraScaled );
        }

        /// <summary>
        /// ��ֹ��������Ÿı��ʱ��ԭ�״�ͼ����Ȼ�����ơ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Camera_onCameraScaled ( object sender, EventArgs e )
        {
            leftWaitframe = 1;
        }

        #endregion

        #region Update

        public void Update ()
        {
            UpdateBoundRect();
            UpdateMatrix();
        }

        #endregion

        #region If Obj in RaderRect

        private void UpdateBoundRect ()
        {
            float len = r / (float)Math.Cos( ang );
            Vector2 CenterP = pos + N * len;
            Vector2 min = Vector2.Min( Vector2.Min( Vector2.Min( pos, LeftP ), RightP ), CenterP );
            Vector2 max = Vector2.Max( Vector2.Max( Vector2.Max( pos, LeftP ), RightP ), CenterP );

            bouBox = new Rectanglef( min.X, min.Y, max.X - min.X, max.Y - min.Y );
        }

        /// <summary>
        /// �ڵ��øú���֮ǰ��ȷ����ǰ֡�ڵ��ù�UpdateBoundRect()������
        /// </summary>
        /// <param name="objBoundBox"></param>
        /// <returns></returns>
        public bool InRaderRect ( Rectanglef objBoundBox )
        {
            return bouBox.Intersects( objBoundBox );
        }

        #endregion

        #region Translations

        private void UpdateMatrix ()
        {
            rotaMatrix = Matrix.CreateRotationZ( -azi + BaseGame.CoordinMgr.Rota );
        }

        internal Vector2 TranslateToRaderSpace ( Vector2 pInWorld )
        {
            Vector2 result;
            Vector2 V = pInWorld - pos;

            float aziP;
            if (V.Y == 0)
            {
                if (V.X == 0)
                    return Vector2.Zero;
                else if (V.X > 0)
                    aziP = MathHelper.PiOver2;
                else
                    aziP = -MathHelper.PiOver2;
            }
            else if (V.Y < 0)
            {
                aziP = -(float)Math.Atan( V.X / V.Y );
            }
            else
            {
                aziP = MathHelper.Pi - (float)Math.Atan( V.X / V.Y );
            }
            result.X = MathTools.AngTransInPI( aziP - azi ) / ang;
            result.Y = V.Length() / r;

            return result;
        }

        #endregion

        #region Draw

        internal void UpdateTexture()
        {
            texture = target.GetTexture();
            texSmall = targetSmall.GetTexture();
            UpdateTexData();

            depthMap.CalSheltersVisiBorder();
            this.ShelterObjs = depthMap.GetShelters();
            this.ShelterVisiBorders = depthMap.GetSheltersVisiBorder();

        }

        public void Draw ()
        {
            if (leftWaitframe != 0)
            {
                leftWaitframe--;
                return;
            }
            if (texture != null)
            {
                BaseGame.SpriteMgr.alphaSprite.Draw( texture, BaseGame.CoordinMgr.ScreenPos( pos ), null,
                    Color.White, 0, new Vector2( 0.5f * ShelterMgr.texSize, 0.5f * ShelterMgr.texSize ), BaseGame.CoordinMgr.ScrnLengthf( r + r ) / (float)ShelterMgr.texSize, SpriteEffects.None, LayerDepth.TankRader );
            }
        }

        #endregion

        #region Dectection

        private void UpdateTexData ()
        {
            if (texSmall != null)
            {
                try
                {
                    textureData = new Color[texSmall.Width * texSmall.Height];
                    texSmall.GetData<Color>( textureData );
                }
                catch (Exception)
                {

                }
            }
        }

        /*
         * ʹ��һ���ߴ��С����ͼ�ж������Ƿ����״��С�
         * 
         * */
        public bool PointInRader ( Vector2 point )
        {
            if (textureData == null)
                return false;

            if (texture == null)
                return false;

            if (!bouBox.Contains( point ))
                return false;

            Vector2 texPos = (Vector2.Transform( (point - pos), Matrix.CreateRotationZ( -BaseGame.CoordinMgr.Rota ) ) + new Vector2( r, r )) * (float)(texture.Width) * smallMapScale * 0.5f / r;

            if (texPos.X < 0 || texPos.X >= texSmall.Width || texPos.Y < 0 || texPos.Y >= texSmall.Height)
                return false;

            int index = (int)(texPos.X) + texSmall.Width * (int)(texPos.Y);
            if (textureData[index].A > 0)
                return true;
            else
                return false;
        }

        #endregion



        
    }

}
