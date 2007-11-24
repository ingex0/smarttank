using System;
using System.Collections.Generic;
using System.Text;

namespace GameBase.DataStructure.test
{
    public class CircleListTest
    {
        class A : ICloneable
        {
            public A ( int x )
            {
                this.x = x;
            }
            public int x;

            #region ICloneable ≥…‘±

            public object Clone ()
            {
                return new A( this.x );
            }

            #endregion
        }

        static public void Run ()
        {
            CircleList<int> circleInt = new CircleList<int>();
            circleInt.AddLast( 1 );
            circleInt.AddLast( 2 );
            circleInt.AddLast( 3 );
            circleInt.AddLast( 4 );
            circleInt.AddLast( 5 );
            circleInt.AddLast( 6 );
            circleInt.AddLast( 7 );
            circleInt.LinkLastAndFirst();

            Console.WriteLine( "circleInt:" );
            foreach (int i in circleInt)
            {
                Console.WriteLine( i );
            }
            CircleList<int> circleclone = circleInt.Clone();

            Console.WriteLine( "circleInt Foreach *=2 ." );

            circleInt.ForEach( delegate( ref int i )
                {
                    i = i * 2;
                } );

            Console.WriteLine( "circleInt:" );
            foreach (int i in circleInt)
            {
                Console.WriteLine( i );
            }

            foreach (int i in circleclone)
            {
                Console.WriteLine( i );
            }

            CircleList<A> circleA = new CircleList<A>();
            circleA.AddLast( new A( 1 ) );
            circleA.AddLast( new A( 2 ) );
            circleA.AddLast( new A( 3 ) );
            circleA.AddLast( new A( 4 ) );
            circleA.AddLast( new A( 5 ) );

            Console.WriteLine( "circleA:" );
            foreach (A a in circleA)
            {
                Console.WriteLine( a.x );
            }

            CircleList<A> circleB = circleA.Clone();

            Console.WriteLine( "circleB.ForEach *= 2." );
            circleB.ForEach( delegate( ref A a )
            {
                a.x *= 2;
            } );

            Console.WriteLine( "circleA:" );
            foreach (A a in circleA)
            {
                Console.WriteLine( a.x );
            }

            Console.WriteLine( "circleB:" );
            foreach (A a in circleB)
            {
                Console.WriteLine( a.x );
            }

            CircleListNode<A> find = circleA.FindFirst( delegate( A a )
                {
                    if (a.x == 3)
                        return true;
                    else
                        return false;
                } );

            A[] Aarray = circleA.ToArray( find, false );

            Console.WriteLine("Aarray:");
            foreach (A a in Aarray)
            {
                Console.WriteLine( a.x );
            }

            Console.WriteLine();
            Console.WriteLine(circleA.IndexOf( find ));
            Console.WriteLine( find.value.x );

            CircleListNode<A>[] findall = circleA.FindAll( delegate( A a )
                {
                    if (a.x == 3 || a.x == 4)
                        return true;
                    else
                        return false;
                } );
            Console.WriteLine( "FindAll:" );
            foreach (CircleListNode<A> node in findall)
            {
                Console.WriteLine( node.value.x );
            }
        }
    }
}
