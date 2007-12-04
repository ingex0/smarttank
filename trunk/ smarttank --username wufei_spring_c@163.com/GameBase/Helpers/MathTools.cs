using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using GameBase.DataStructure;

namespace GameBase.Helpers
{
    public static class MathTools
    {
        /// <summary>
        /// ������ֵ���㵽PI��-PI֮��
        /// </summary>
        /// <param name="ang"></param>
        /// <returns></returns>
        public static float AngTransInPI ( float ang )
        {
            float pi2 = 2 * MathHelper.Pi;
            while (ang > MathHelper.Pi)
            {
                ang -= pi2;
            }
            while (ang < -MathHelper.Pi)
            {
                ang += pi2;
            }
            return ang;
        }

        /// <summary>
        /// �ɷ���Ǽ�����÷���ĵ�λ�������������Ǻ�����
        /// </summary>
        /// <param name="azi"></param>
        /// <returns></returns>
        public static Vector2 NormalVectorFromAzi ( float azi )
        {
            return new Vector2( (float)Math.Sin( azi ), -(float)Math.Cos( azi ) );
        }

        /// <summary>
        /// ת��Ϊ���������
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static int Round ( float f )
        {
            return (int)Math.Round( f, MidpointRounding.AwayFromZero );
        }

        /// <summary>
        /// �Ƚ�������ֵ�Ƿ����
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool FloatEqual ( float a, float b, float error )
        {
            if (Math.Abs( a - b ) < error)
                return true;
            else
                return false;
        }

        /// <summary>
        /// ��鸡��ֵ�Ƿ�Ϊ��
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool FloatEqualZero ( float a, float error )
        {
            if (Math.Abs( a ) < error)
                return true;
            else
                return false;
        }

        /// <summary>
        /// ������������ķ�λ
        /// </summary>
        /// <param name="refPos"></param>
        /// <returns></returns>
        public static float AziFromRefPos ( Vector2 refPos )
        {
            float result = new float();
            if (refPos.Y == 0)
            {
                if (refPos.X >= 0)
                    result = MathHelper.PiOver2;
                else
                    result = -MathHelper.PiOver2;
            }
            else if (refPos.Y < 0)
            {
                result = (float)Math.Atan( -refPos.X / refPos.Y );
            }
            else
            {
                result = (float)Math.PI + (float)Math.Atan( -refPos.X / refPos.Y );
            }
            result = AngTransInPI( result );
            return result;
        }

        /// <summary>
        /// ����������д���
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static Line MidVerLine ( Vector2 point1, Vector2 point2 )
        {
            return new Line( 0.5f * (point1 + point2), Vector2.Normalize( new Vector2( -point2.Y + point1.Y, point2.X - point1.X ) ) );
        }

        /// <summary>
        /// ����ͨ�����㲢��ָ��ֱ�ߴ�ֱ��ֱ��
        /// </summary>
        /// <param name="line"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Line VerticeLine ( Line line, Vector2 point )
        {
            return new Line( point, Vector2.Normalize( new Vector2( -line.direction.Y, line.direction.X ) ) );
        }

        /// <summary>
        /// ������ֱ�ߵĽ��㣬����ֱ�ߴ��ڽ���ʱ����true
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="result"></param>
        /// <returns>����ֱ�ߴ��ڽ���ʱ����true</returns>
        public static bool InterPoint ( Line line1, Line line2, out Vector2 result )
        {
            if (Vector2.Normalize( line1.direction ) == Vector2.Normalize( line2.direction ) ||
                Vector2.Normalize( line1.direction ) == -Vector2.Normalize( line2.direction ))
            {
                result = Vector2.Zero;
                return false;
            }
            else
            {
                float k = ((line2.pos.X - line1.pos.X) * line2.direction.Y - (line2.pos.Y - line1.pos.Y) * line2.direction.X) /
                    (line1.direction.X * line2.direction.Y - line1.direction.Y * line2.direction.X);
                result = line1.pos + line1.direction * k;
                return true;
            }
        }

        /// <summary>
        /// ������������ļнǣ��Ի���Ϊ��λ
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        public static float AngBetweenVectors ( Vector2 vec1, Vector2 vec2 )
        {
            return (float)Math.Acos( Vector2.Dot( vec1, vec2 ) / (vec1.Length() * vec2.Length()) );
        }

        /// <summary>
        /// ����ά�����Ĳ�����������ʾ��������˳ʱ������
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        public static float Vector2Cross ( Vector2 vec1, Vector2 vec2 )
        {
            return vec1.X * vec2.Y - vec2.X * vec1.Y;
        }

        /// <summary>
        /// ����������������淴���ķ�������
        /// </summary>
        /// <param name="incident">��������</param>
        /// <param name="mirrorNormal">���淨����</param>
        /// <returns></returns>
        public static Vector2 ReflectVector ( Vector2 incident, Vector2 mirrorNormal )
        {
            mirrorNormal.Normalize();
            return incident - 2 * Vector2.Dot( incident, mirrorNormal ) * mirrorNormal;
        }


        public static Rectanglef BoundBox ( params Vector2[] containPoints )
        {
            Vector2 minPoint = containPoints[0];
            Vector2 maxPoint = containPoints[0];
            for (int i = 1; i < containPoints.Length; i++)
            {
                minPoint = Vector2.Min( minPoint, containPoints[i] );
                maxPoint = Vector2.Max( maxPoint, containPoints[i] );
            }

            return new Rectanglef( minPoint, maxPoint );
        }
    }
}
