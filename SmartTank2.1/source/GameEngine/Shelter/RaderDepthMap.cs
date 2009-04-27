using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameEngine.Graphics;
using Common.DataStructure;
using Common.Helpers;
using GameEngine.Senses.Memory;

namespace GameEngine.Shelter
{

    public class RaderDepthMap
    {
        #region Constants

        readonly float scale;

        //readonly int minSize = 10;

        #endregion

        #region Variables

        float[] depthMap;

        /*
         * 需要为TankAI提供当前遮挡它的物体信息。
         * 
         * 故加入此成员。
         * 
         * */
        IShelterObj[] objMap;


        /// <summary>
        /// 缓存物体的可见边界。
        /// </summary>
        VisiBordPoint[] objBordIndexMap;


        IShelterObj[] curShelters;

        ObjVisiBorder[] curObjVisiBorders;

        Texture2D mapTex;

        #endregion

        #region Properties

        public int Size
        {
            get { return depthMap.Length; }
        }

        public float this[float l]
        {
            get
            {
                int index = IndexOf( l );
                if (index < 0 || index >= Size)
                    return 1;
                else
                    return depthMap[index];
            }
            set
            {
                int index = IndexOf( l );
                if (index < 0 || index >= Size)
                    return;
                else
                    depthMap[index] = value;
            }
        }

        public float[] DepthMap
        {
            get { return depthMap; }
        }

        #endregion

        #region Construction

        public RaderDepthMap( int gridSum )
        {
            depthMap = new float[gridSum];
            scale = (float)gridSum / 2f;
            ClearMap( 1 );

            objMap = new IShelterObj[gridSum];
            objBordIndexMap = new VisiBordPoint[gridSum];

            mapTex = new Texture2D( BaseGame.Device, ShelterMgr.gridSum, 1, 1, TextureUsage.None, SurfaceFormat.Alpha8 );
        }

        public void ClearMap( float value )
        {
            for (int i = 0; i < depthMap.Length; i++)
            {
                depthMap[i] = value;
            }
        }

        #endregion

        #region Methods

        public void ApplyShelterObj( Rader rader, IShelterObj obj )
        {
            Matrix worldMatrix = obj.WorldTrans;

            CircleList<BorderPoint> border = obj.BorderData;
            CircleListNode<BorderPoint> lastB = border.First.pre;
            Vector2 pInRader = rader.TranslateToRaderSpace( Vector2.Transform( ConvertHelper.PointToVector2( lastB.value.p ), worldMatrix ) );
            int lastIndex = IndexOf( pInRader.X );
            float lastDepth = pInRader.Y;

            CircleListNode<BorderPoint> cur = border.First;
            for (int i = 0; i < border.Length; i++)
            {
                pInRader = rader.TranslateToRaderSpace( Vector2.Transform( ConvertHelper.PointToVector2( cur.value.p ), worldMatrix ) );
                int curIndex = IndexOf( pInRader.X );
                float curDepth = pInRader.Y;
                if (pInRader.X >= -1.1f && pInRader.X <= 1.3f)
                {
                    //this[pInRader.X] = Math.Min( this[pInRader.X], pInRader.Y );
                    SetValueAtIndex( curIndex, pInRader.Y, obj, i, cur.value.p );

                    if (curIndex - lastIndex > 1)
                    {
                        int overIndex = lastIndex + 1;
                        while (overIndex != curIndex)
                        {
                            float lerp = MathHelper.Lerp( lastDepth, curDepth, (curIndex - overIndex) / (curIndex - lastIndex) );
                            SetValueAtIndex( overIndex, lerp, obj, i, cur.value.p );
                            overIndex++;
                        }
                    }
                    else if (curIndex - lastIndex < -1)
                    {
                        int overIndex = lastIndex - 1;
                        while (overIndex != curIndex)
                        {
                            float lerp = MathHelper.Lerp( lastDepth, curDepth, (curIndex - overIndex) / (curIndex - lastIndex) );
                            SetValueAtIndex( overIndex, lerp, obj, i, cur.value.p );
                            overIndex--;
                        }
                    }
                }
                lastIndex = curIndex;
                lastDepth = curDepth;

                cur = cur.next;
            }
        }

        public Texture2D GetMapTexture()
        {
            byte[] data = new byte[depthMap.Length];
            for (int i = 0; i < depthMap.Length; i++)
            {
                data[i] = (byte)(depthMap[i] * 255);
            }
            BaseGame.Device.Textures[0] = null;
            mapTex.SetData<byte>( data );
            return mapTex;
        }

        public IShelterObj[] GetShelters()
        {
            return curShelters;
        }

        public ObjVisiBorder[] GetSheltersVisiBorder()
        {
            return curObjVisiBorders;
        }

        public void CalSheltersVisiBorder()
        {
            Dictionary<IShelterObj, List<VisiBordPoint>> temp = new Dictionary<IShelterObj, List<VisiBordPoint>>();

            int index = 0;
            foreach (IShelterObj obj in objMap)
            {
                if (obj != null)
                {
                    if (!temp.ContainsKey( obj ))
                    {
                        temp.Add( obj, new List<VisiBordPoint>() );
                    }
                    temp[obj].Add( objBordIndexMap[index] );
                }
                index++;
            }

            curObjVisiBorders = new ObjVisiBorder[temp.Count];

            curShelters = new IShelterObj[temp.Count];

            int i = 0;
            foreach (KeyValuePair<IShelterObj, List<VisiBordPoint>> pair in temp)
            {
                pair.Value.Sort(
                    delegate( VisiBordPoint p1, VisiBordPoint p2 )
                    {
                        if (p1.index < p2.index)
                            return -1;
                        else if (p1.index == p2.index)
                            return 0;
                        else
                            return 1;
                    } );

                CircleList<VisiBordPoint> points = new CircleList<VisiBordPoint>();

                foreach (VisiBordPoint p in pair.Value)
                {
                    if (points.Last == null || points.Last.value.index != p.index)
                        points.AddLast( p );
                }

                curObjVisiBorders[i] = new ObjVisiBorder( (IHasBorderObj)pair.Key, points );
                curShelters[i] = pair.Key;

                i++;
            }
        }

        #endregion

        #region Private Functions

        private int IndexOf( float x )
        {
            return (int)((x + 1) * scale);
        }

        private void SetValueAtIndex( int index, float value, IShelterObj obj, int bordIndex, Point bordp )
        {
            if (index < 0 || index >= depthMap.Length)
                return;

            if (value < depthMap[index])
            {
                depthMap[index] = value;
                objMap[index] = obj;
                objBordIndexMap[index] = new VisiBordPoint( bordIndex, bordp );
            }
        }

        #endregion
    }

}
