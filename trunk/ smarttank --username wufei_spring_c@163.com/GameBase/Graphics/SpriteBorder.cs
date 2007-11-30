#define SHOWERROR

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using GameBase.DataStructure;


namespace GameBase.Graphics
{
    using BorderNode = CircleListNode<Border>;
    using BorderCircleList = CircleList<Border>;
    using GameBase.Helpers;

    public class BorderBulidException : Exception
    {
        public Point curPoint;
        public Point prePoint;
        public SpriteBorder.BorderMap borderMap;

        public BorderBulidException ( Point curPoint, Point prePoint, SpriteBorder.BorderMap borderMap )
        {
            this.curPoint = curPoint;
            this.prePoint = prePoint;
            this.borderMap = borderMap;
        }
    }

    /// <summary>
    /// ��ʾ����߽��ϵĵ�
    /// </summary>
    public class Border
    {
        /// <summary>
        /// ���øõ����ͼ����
        /// </summary>
        /// <param name="setp"></param>
        public Border ( Point setp )
        {
            p = setp;
        }

        /// <summary>
        /// �õ����ͼ����
        /// </summary>
        public Point p;

        /// <summary>
        /// ��øõ����ͼ����
        /// </summary>
        public Point Point
        {
            get { return p; }
        }

        /// <summary>
        /// ��øõ�����ڵ�
        /// </summary>
        public Point[] NeiborNodes
        {
            get
            {
                return new Point[4]
                    {
                        new Point(p.X - 1, p.Y),
                        new Point(p.X, p.Y - 1),
                        new Point(p.X, p.Y + 1),
                        new Point(p.X + 1, p.Y)//,
                        //new Point(p.X - 1, p.Y - 1),
                        //new Point(p.X + 1, p.Y - 1),
                        //new Point(p.X - 1, p.Y + 1),
                        //new Point(p.X + 1, p.Y + 1)
                    };
            }
        }
    }

    /// <summary>
    /// ��ʾ����ı߽�
    /// </summary>
    public class SpriteBorder
    {
        #region DataStruct Definition

        /// <summary>
        /// ��ԭͼ��һ�����ش��Ƿ������岿�ֵ�Alphaͨ����ֵ
        /// </summary>
        public const int minBlockAlpha = 10;

        public class BorderMap
        {
            bool[,] borderMap;

            public BorderMap ( int width, int height )
            {
                borderMap = new bool[height + 4, width + 4];
                borderMap.Initialize();
            }

            public bool this[int x, int y]
            {
                get
                {
                    if (x < -2 || x >= borderMap.GetLength( 1 ) ||
                        y < -2 || y >= borderMap.GetLength( 0 ))
                    {
                        throw new IndexOutOfRangeException();
                    }

                    return borderMap[y + 2, x + 2];
                }

                set
                {
                    if (x < -2 || x >= borderMap.GetLength( 1 ) ||
                        y < -2 || y >= borderMap.GetLength( 0 ))
                    {
                        throw new IndexOutOfRangeException();
                    }

                    borderMap[y + 2, x + 2] = value;
                }
            }

            public int Width
            {
                get { return borderMap.GetLength( 1 ); }
            }
            public int Height
            {
                get { return borderMap.GetLength( 0 ); }
            }

            #region Functions for test
            public void ShowDataToConsole ()
            {
                Console.WriteLine( "BorderMap:" );

                for (int y = -2; y < this.Height - 2; y++)
                {
                    for (int x = -2; x < this.Width - 2; x++)
                    {
                        Console.Write( this[x, y] ? "��" : "��" );
                    }
                    //Console.Write( "" + y );
                    Console.WriteLine();
                }
            }
            #endregion
        }

        private class TextureData
        {


            Color[] texData;

            int texWidth,
                texHeight;

            public TextureData ( Texture2D tex )
            {
                texData = new Color[tex.Width * tex.Height];
                tex.GetData<Color>( texData );
                texWidth = tex.Width;
                texHeight = tex.Height;
            }

            public bool this[int x, int y]
            {
                get
                {
                    if (x < 0 || x >= texWidth || y < 0 || y >= texHeight)
                        return false;
                    else
                        return texData[y * texWidth + x].A >= minBlockAlpha;
                }
            }
        }

        #endregion

        #region Variables

        List<BorderCircleList> borderCircles;

        TextureData texData;

        BorderMap borderMap;

        /// <summary>
        /// ���ڻ�ñ߽�ĸ������󣬲μ�InitialSurroundPoint������
        /// SetPrePointFirstTime������SurroundQueue�����Լ�BuildCircle������
        /// </summary>
        private CircleList<Point> SurroundPoint;

        #endregion

        #region Properties

        public BorderCircleList BorderCircle
        {
            get { return borderCircles[0]; }
        }

        #endregion

        #region Construction

        /// <summary>
        /// �����ƶ���ͼ�ı߽硣
        /// </summary>
        /// <param name="tex"></param>
        public SpriteBorder ( Texture2D tex )
        {
            InitialSurroundPoint();

            borderCircles = new List<BorderCircleList>();
            texData = new TextureData( tex );
            borderMap = new BorderMap( tex.Width, tex.Height );

            CreateBorderCircles( tex );
        }

        /// <summary>
        /// ����ָ����ͼ�ı߽磬Ϊ�˲��ԣ�����borderMap
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="borderMap"></param>
        public SpriteBorder ( Texture2D tex, out SpriteBorder.BorderMap borderMap )
            : this( tex )
        {
            borderMap = this.borderMap;
        }

        #region Private Functions

        const int minBorderListLength = 10;


        private void CreateBorderCircles ( Texture2D tex )
        {
            LinkedList<Border> leftBorderNodes = new LinkedList<Border>();

            for (int y = -1; y <= tex.Height; y++)
            {
                for (int x = -1; x <= tex.Width; x++)
                {
                    Border cur = new Border( new Point( x, y ) );
                    if (NodeIsBorder( cur ))
                    {
                        borderMap[x, y] = true;
                        leftBorderNodes.AddLast( cur );
                    }
                }
            }

            while (leftBorderNodes.Count != 0)
            {
                Border first = leftBorderNodes.First.Value;

                BorderCircleList list = BuildCircle( first );
                if (list.isLinked && list.Length >= minBorderListLength)
                {
                    borderCircles.Add( list );
                }

                // Delete Circles' node from LeftBorderNodes
                foreach (Border border in list)
                {
                    LinkedListNode<Border> node = leftBorderNodes.First;
                    for (int i = 0; i < leftBorderNodes.Count - 1; i++)
                    {
                        node = node.Next;
                        if (node.Previous.Value.p == border.p)
                        {
                            leftBorderNodes.Remove( node.Previous );
                        }
                    }
                    if (leftBorderNodes.Last != null && leftBorderNodes.Last.Value.p == border.p)
                        leftBorderNodes.RemoveLast();
                }
            }


        }

        private void InitialSurroundPoint ()
        {
            SurroundPoint = new CircleList<Point>();
            SurroundPoint.AddLast( new Point( -1, -1 ) );
            SurroundPoint.AddLast( new Point( 0, -1 ) );
            SurroundPoint.AddLast( new Point( 1, -1 ) );
            SurroundPoint.AddLast( new Point( 1, 0 ) );
            SurroundPoint.AddLast( new Point( 1, 1 ) );
            SurroundPoint.AddLast( new Point( 0, 1 ) );
            SurroundPoint.AddLast( new Point( -1, 1 ) );
            SurroundPoint.AddLast( new Point( -1, 0 ) );
            SurroundPoint.LinkLastAndFirst();
        }

        private bool NodeIsBlock ( Point node )
        {
            return texData[node.X, node.Y];
        }

        private bool NodeIsBorder ( Border node )
        {
            if (NodeIsBlock( node.p )) return false;

            foreach (Point p in node.NeiborNodes)
            {
                if (NodeIsBlock( p ))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// ����ʱ�뷽���ñ߽�Ȧ
        /// </summary>
        /// <param name="first"></param>
        /// <returns></returns>
        private BorderCircleList BuildCircle ( Border first )
        {
            BorderCircleList result = new BorderCircleList();

            BorderNode firstNode = new BorderNode( first );
            result.AddLast( firstNode );

            Point prePoint = new Point();
            Point curPoint = firstNode.value.p;

            try
            {
                //set the prePoint at the first time!
                SetPrePointFirstTime( firstNode.value.p, ref prePoint );


                bool linked = false;
                while (!linked)
                {
                    bool findNext = false;

                    foreach (Point p in SurroundQueue( curPoint, prePoint ))
                    {
                        if (borderMap[p.X, p.Y] && p != prePoint)
                        {
                            findNext = true;

                            if (p == firstNode.value.p)
                            {
                                linked = true;
                                result.LinkLastAndFirst();
                                break;
                            }

                            result.AddLast( new Border( p ) );
                            prePoint = curPoint;
                            curPoint = p;
                            break;
                        }
                    }
                    if (!findNext)
                    {
#if SHOWERROR
                        //ShowDataToConsole();
                        //ShowCurListResult( result );
#endif
                        throw new BorderBulidException( curPoint, prePoint, borderMap );
                    }
                }

            }
            // ����˴������쳣�������ǵ����ͼƬ��������Ҫ��
            // ��������д�ӡ��ͼƬ�Ͼ�������λ�á�
            // ��Ҫ�����޸�ͼƬ������ʹ�á�
            catch (BorderBulidException e)
            {
#if SHOWERROR
                //ShowDataToConsole();
                //ShowCurListResult( result );
#endif
                throw e;
                //return result;
            }
            return result;
        }



        private void SetPrePointFirstTime ( Point firPoint, ref Point prePoint )
        {
            CircleListNode<Point>[] surroundBorder =
                SurroundPoint.FindAll( delegate( Point p )
                    {
                        if (borderMap[p.X + firPoint.X, p.Y + firPoint.Y])
                            return true;
                        else
                            return false;
                    } );

            if (surroundBorder.Length == 2)
            {
                int IndexA = SurroundPoint.IndexOf( surroundBorder[0] );
                int IndexB = SurroundPoint.IndexOf( surroundBorder[1] );

                int BsubA = IndexB - IndexA;
                if (BsubA <= 4)
                {
                    Point refPos = surroundBorder[0].value;
                    prePoint = new Point( firPoint.X + refPos.X, firPoint.Y + refPos.Y );
                }
                else
                {
                    Point refPos = surroundBorder[1].value;
                    prePoint = new Point( firPoint.X + refPos.X, firPoint.Y + refPos.Y );
                }
            }
            //else if (surroundBorder.Length == 3)
            else
                throw new BorderBulidException( firPoint, prePoint, borderMap );
        }


        private Point[] SurroundQueue ( Point point, Point prePoint )
        {
            CircleList<Point> result = SurroundPoint.Clone();
            result.ForEach( delegate( ref Point p )
                {
                    p = new Point( p.X + point.X, p.Y + point.Y );
                } );

            CircleListNode<Point> find = result.FindFirst( delegate( Point p )
                {
                    if (p == prePoint)
                        return true;
                    else
                        return false;
                } );

            return result.ToArray( find, true );
        }

        #endregion

        #endregion

        #region Public Functions

        /// <summary>
        /// ���һ���߽�㴦�ķ�����
        /// </summary>
        /// <param name="node"></param>
        /// <param name="sumAverage"></param>
        /// <returns></returns>
        public Vector2 GetNormalVector ( BorderNode node, int sumAverage )
        {
            if (sumAverage < 1)
                sumAverage = 1;

            Vector2 sumTang = Vector2.Zero;
            BorderNode cur = node;
            for (int i = 0; i < sumAverage; i++)
            {
                cur = cur.pre;
            }
            for (int i = -sumAverage; i <= sumAverage; i++)
            {
                if (i == 0) i++;

                cur = cur.next;
                Point curPoint = cur.value.p;
                Point prePoint = cur.pre.value.p;
                Vector2 tang = new Vector2( curPoint.X - prePoint.X, curPoint.Y - prePoint.Y );
                tang.Normalize();
                tang *= Right( Math.Abs( i ), sumAverage );
                sumTang += tang;
            }
            Vector2 result = new Vector2( sumTang.Y, -sumTang.X );
            // sprite���л���б�׼�����˴��Ե�
            //result.Normalize();
            return result;
        }

        private float Right ( int destance, int sumAverage )
        {
            return sumAverage;// -destance;
        }

        #endregion

        #region Fuctions for test

        /// <summary>
        /// ����ǰ��������Ϣ���������̨�С�
        /// </summary>
        public void ShowDataToConsole ()
        {
            Console.WriteLine( "SpriteBorder class:" );
            Console.WriteLine();
            borderMap.ShowDataToConsole();
            Console.WriteLine();
            ShowCirclesData();
        }

        private void ShowCirclesData ()
        {
            foreach (BorderCircleList list in borderCircles)
            {
                BorderMap map = new BorderMap( borderMap.Width - 4, borderMap.Height - 4 );
                foreach (Border bord in list)
                {
                    Point p = bord.p;
                    map[p.X, p.Y] = true;
                }
                map.ShowDataToConsole();
                Console.WriteLine();
            }
        }

        private void ShowCurListResult ( BorderCircleList result )
        {
            BorderMap map = new BorderMap( borderMap.Width - 4, borderMap.Height - 4 );
            foreach (Border bord in result)
            {
                Point p = bord.p;
                map[p.X, p.Y] = true;
            }
            map.ShowDataToConsole();
            Console.WriteLine();
        }

        #endregion


    }
}
