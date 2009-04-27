using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameEngine.PhiCol;
using GameEngine.Shelter;
using Common.DataStructure;

namespace GameEngine.Senses.Memory
{
    public struct VisiBordPoint
    {
        public int index;
        public Point p;

        public VisiBordPoint ( int index, Point p )
        {
            this.index = index;
            this.p = p;
        }
    }

    public class ObjVisiBorder
    {
        IHasBorderObj obj;
        CircleList<VisiBordPoint> visiBorder;

        internal IHasBorderObj Obj
        {
            get { return obj; }
        }

        public CircleList<VisiBordPoint> VisiBorder
        {
            get { return visiBorder; }
        }


        public ObjVisiBorder ( IHasBorderObj obj, CircleList<VisiBordPoint> visiBorder )
        {
            this.obj = obj;
            this.visiBorder = visiBorder;
            this.visiBorder.LinkLastAndFirst();
        }

        //public ObjVisiBorder ( IGameObj obj )
        //{
        //    this.obj = obj;
        //    this.visiBorder = new CircleList<BordPoint>();
        //}

        internal bool Combine ( CircleList<VisiBordPoint> borderB )
        {
            CircleListNode<VisiBordPoint> curA = this.visiBorder.First;

            int iA = 0;
            bool borderUpdated = false;

            if (curA.value.index > borderB.First.value.index)
            {
                this.visiBorder.AddFirst( borderB.First.value );
                curA = this.visiBorder.First;
                borderUpdated = true;
            }

            bool objborderChanged = false;

            foreach (VisiBordPoint pB in borderB)
            {
                while (curA.value.index < pB.index && iA < this.visiBorder.Length)
                {
                    curA = curA.next;
                    iA++;
                }
                if (curA.value.index == pB.index)
                {
                    if (curA.value.p != pB.p)
                    {
                        objborderChanged = true;
                        break;
                    }
                }
                else
                {
                    borderUpdated = true;
                    this.visiBorder.InsertAfter( pB, curA.pre );
                    curA = curA.pre;
                }
            }

            if (objborderChanged)
            {
                this.visiBorder = borderB;
            }

            #region Check Code
            //CircleListNode<BordPoint> checkCur = this.visiBorder.First;

            //int lastIndex = -1;

            //for (int i = 0; i < this.visiBorder.Length; i++)
            //{
            //    if (checkCur.value.index <= lastIndex)
            //    {
            //        Console.WriteLine( "this.visiBorder :" );
            //        this.ShowInfo();
            //        Console.WriteLine();
            //        Console.WriteLine( "borderB :" );
            //        foreach (BordPoint p in borderB)
            //        {
            //            Console.WriteLine( p.index );
            //        }

            //        throw new Exception();
            //    }

            //    lastIndex = checkCur.value.index;
            //    checkCur = checkCur.next;
            //}
            #endregion

            return borderUpdated;
        }

        internal void ShowInfo ()
        {
            CircleListNode<VisiBordPoint> checkCur = this.visiBorder.First;
            for (int i = 0; i < this.visiBorder.Length; i++)
            {
                Console.WriteLine( checkCur.value.index );
                checkCur = checkCur.next;
            }
        }
    }
}
