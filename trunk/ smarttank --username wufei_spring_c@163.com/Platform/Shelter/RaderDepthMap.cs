using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameBase;
using GameBase.Graphics;
using GameBase.DataStructure;
using GameBase.Helpers;
using Platform.GameObjects;
using Platform.Senses.Memory;

namespace Platform.Shelter
{
    public partial class ShelterManager
    {
        class RaderDepthMap
        {
            #region Constants

            readonly float scale;

            readonly int minSize = 10;

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
            BordPoint[] objBordIndexMap;


            IShelterObj[] curShelters;

            ObjVisiBorder[] curObjVisiBorders;

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

            public RaderDepthMap ( int gridSum )
            {
                depthMap = new float[gridSum];
                scale = (float)gridSum / 2f;
                ClearMap( 1 );

                objMap = new IShelterObj[gridSum];
                objBordIndexMap = new BordPoint[gridSum];
            }

            /// <summary>
            /// 通过栅格逻辑长度和雷达的参数构造对象
            /// </summary>
            /// <param name="gridlength">栅格参考长度</param>
            /// <param name="raderAng">雷达张角的一半</param>
            /// <param name="raderR">雷达半径</param>
            public RaderDepthMap ( float gridlength, float raderAng, float raderR )
            {
                float length = 2 * raderAng * raderR;
                int gridSum = (int)(length / gridlength);
                if (gridSum < minSize)
                    gridSum = minSize;

                depthMap = new float[gridSum];
                scale = (float)gridSum / 2f;

                ClearMap( 1 );

                objMap = new IShelterObj[gridSum];
                objBordIndexMap = new BordPoint[gridSum];
            }

            public void ClearMap ( float value )
            {
                for (int i = 0; i < depthMap.Length; i++)
                {
                    depthMap[i] = value;
                }
            }

            #endregion

            #region Methods

            public void ApplyShelterObj ( Rader rader, IShelterObj obj )
            {
                Matrix worldMatrix = obj.WorldTrans;

                CircleList<Border> border = obj.BorderData;
                CircleListNode<Border> lastB = border.First.pre;
                Vector2 pInRader = rader.TranslateToRaderSpace( Vector2.Transform( ConvertHelper.PointToVector2( lastB.value.p ), worldMatrix ) );
                int lastIndex = IndexOf( pInRader.X );
                float lastDepth = pInRader.Y;

                CircleListNode<Border> cur = border.First;
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

            public Texture2D GetMapTexture ()
            {
                Texture2D result = new Texture2D( BaseGame.Device, depthMap.Length, 1, 1, ResourceUsage.None, SurfaceFormat.Alpha8 );

                byte[] data = new byte[depthMap.Length];
                for (int i = 0; i < depthMap.Length; i++)
                {
                    data[i] = (byte)(depthMap[i] * 255);
                }
                result.SetData<byte>( data );
                return result;
            }

            public IShelterObj[] GetShelters ()
            {
                return curShelters;
            }

            public ObjVisiBorder[] GetSheltersVisiBorder ()
            {
                return curObjVisiBorders;
            }

            public void CalSheltersVisiBorder ()
            {
                Dictionary<IShelterObj, List<BordPoint>> temp = new Dictionary<IShelterObj, List<BordPoint>>();

                int index = 0;
                foreach (IShelterObj obj in objMap)
                {
                    if (obj != null)
                    {
                        if (!temp.ContainsKey( obj ))
                        {
                            temp.Add( obj, new List<BordPoint>() );
                        }
                        temp[obj].Add( objBordIndexMap[index] );
                    }
                    index++;
                }

                curObjVisiBorders = new ObjVisiBorder[temp.Count];

                curShelters = new IShelterObj[temp.Count];

                int i = 0;
                foreach (KeyValuePair<IShelterObj, List<BordPoint>> pair in temp)
                {
                    pair.Value.Sort(
                        delegate( BordPoint p1, BordPoint p2 )
                        {
                            if (p1.index < p2.index)
                                return -1;
                            else if (p1.index == p2.index)
                                return 0;
                            else
                                return 1;
                        } );

                    CircleList<BordPoint> points = new CircleList<BordPoint>();

                    foreach (BordPoint p in pair.Value)
                    {
                        if (points.Last == null || points.Last.value.index != p.index)
                            points.AddLast( p );
                    }

                    curObjVisiBorders[i] = new ObjVisiBorder( (Platform.PhisicalCollision.IHasBorderObj)pair.Key, points );
                    curShelters[i] = pair.Key;

                    i++;
                }
            }

            #endregion

            #region Private Functions

            private int IndexOf ( float x )
            {
                return (int)((x + 1) * scale);
            }

            private void SetValueAtIndex ( int index, float value, IShelterObj obj, int bordIndex, Point bordp )
            {
                if (index < 0 || index >= depthMap.Length)
                    return;

                if (value < depthMap[index])
                {
                    depthMap[index] = value;
                    objMap[index] = obj;
                    objBordIndexMap[index] = new BordPoint( bordIndex, bordp );
                }
            }

            #endregion
        }
    }
}
