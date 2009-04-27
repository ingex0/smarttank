using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
using Common.Helpers;
using Common.DataStructure;

namespace GameEngine.Senses.Memory
{
    public class ConvexHall
    {
        List<VisiBordPoint> convexPoints;

        public ConvexHall ( CircleList<VisiBordPoint> border, float minOptimizeDest )
        {
            BuildConvexHall( border, minOptimizeDest );
        }

        public List<VisiBordPoint> Points
        {
            get { return convexPoints; }
        }

        public void BuildConvexHall ( CircleList<VisiBordPoint> border, float minOptimizeDest )
        {
            #region 获得原始凸包
            if (border.Length < 2)
                return;

            Stack<VisiBordPoint> stack = new Stack<VisiBordPoint>();

            CircleListNode<VisiBordPoint> cur = border.First;
            stack.Push( cur.value );
            cur = cur.next;
            stack.Push( cur.value );
            cur = cur.next;
            for (int i = 2; i < border.Length; i++)
            {
                VisiBordPoint p1 = stack.Pop();

                VisiBordPoint p0 = stack.Peek();

                VisiBordPoint p2 = cur.value;

                if (CountWise( p1, p0, p2 ))
                {
                    stack.Push( p1 );
                    stack.Push( p2 );
                    cur = cur.next;
                }
                else
                {
                    if (stack.Count == 1)
                    {
                        stack.Push( p2 );
                        cur = cur.next;
                    }
                    else
                        i--;
                }
            }

            List<VisiBordPoint> templist = new List<VisiBordPoint>( stack );

            if (templist.Count > 3)
            {
                if (!CountWise( templist[0], templist[1], templist[templist.Count - 1] ))
                {
                    templist.RemoveAt( 0 );
                }
                if (!CountWise( templist[templist.Count - 1], templist[0], templist[templist.Count - 2] ))
                {
                    templist.RemoveAt( templist.Count - 1 );
                }
            }
            else if (templist.Count == 3)
            {
                if (!CountWise( templist[0], templist[1], templist[templist.Count - 1] ))
                {
                    templist.RemoveAt( 0 );
                    VisiBordPoint temp = templist[0];
                    templist[0] = templist[1];
                    templist[1] = temp;
                }
            }

            #endregion

            #region 对凸包进行化简

            if (templist.Count > 3)
            {
                for (int i = 0; i < templist.Count; i++)
                {
                    Vector2 p1 = ConvertHelper.PointToVector2( templist[i].p );
                    Vector2 p2 = ConvertHelper.PointToVector2( templist[(i + 1) % templist.Count].p );
                    Vector2 p3 = ConvertHelper.PointToVector2( templist[(i + 2) % templist.Count].p );
                    Vector2 p4 = ConvertHelper.PointToVector2( templist[(i + 3) % templist.Count].p );

                    if (Vector2.Distance( p2, p3 ) > minOptimizeDest)
                        continue;

                    Vector2 seg1 = p2 - p1;
                    Vector2 seg2 = p4 - p3;

                    float ang = (float)Math.Acos( Vector2.Dot( seg1, seg2 ) / (seg1.Length() * seg2.Length()) );
                    if (ang > MathHelper.PiOver2)
                        continue;

                    Line line1 = new Line( p1, seg1 );
                    Line line2 = new Line( p4, seg2 );

                    Vector2 interPoint;

                    if (!MathTools.InterPoint( line1, line2, out interPoint ))
                        continue;

                    templist[(i + 1) % templist.Count] =
                        new VisiBordPoint( templist[(i + 1) % templist.Count].index, ConvertHelper.Vector2ToPoint( interPoint ) );

                    templist.RemoveAt( (i + 2) % templist.Count );

                    i--;
                }
            }

            #endregion

            convexPoints = templist;
        }

        private bool CountWise ( VisiBordPoint p1, VisiBordPoint p0, VisiBordPoint p2 )
        {
            Vector2 v1 = new Vector2( p1.p.X - p0.p.X, p1.p.Y - p0.p.Y );
            Vector2 v2 = new Vector2( p2.p.X - p1.p.X, p2.p.Y - p1.p.Y );
            float cross = MathTools.Vector2Cross( v1, v2 );
            return cross < 0 || cross == 0 && Vector2.Dot( v1, v2 ) < 0;
        }
    }
}
