using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankEngine2D.Helpers;

namespace SmartTank.Effects.TextEffects
{
    /// <summary>
    /// ����Ч���Ĺ�����
    /// </summary>
    public static class TextEffect
    {
        static LinkedList<ITextEffect> sEffects = new LinkedList<ITextEffect>();

        /// <summary>
        /// ���Ƶ�ǰ����Ч���岢��������
        /// </summary>
        static public void Draw ()
        {
            LinkedListNode<ITextEffect> node;
            for (node = sEffects.First; node != null; )
            {
                if (node.Value.Ended)
                {
                    node = node.Next;
                    if (node != null)
                        sEffects.Remove( node.Previous );
                    else
                        sEffects.RemoveLast();
                }
                else
                {
                    node.Value.Draw();
                    node = node.Next;
                }
            }
        }

        /// <summary>
        /// ָ���߼��������һ��RiseFade������Ч��
        /// ע�⣬������������а������ģ�����ѡ����������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">��ʾ���߼�λ��</param>
        /// <param name="Scale">��С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">�������</param>
        /// <param name="fontType">����</param>
        /// <param name="existedFrame">��������Ļ�е�ʱ��ѭ������</param>
        /// <param name="step">ÿ��ʱ��ѭ���У������ĸ߶ȣ�������Ϊ��λ</param>
        static public void AddRiseFade ( string text, Vector2 pos, float Scale, Color color, float layerDepth,string fontType, float existedFrame, float step )
        {
            sEffects.AddLast( new FadeUpEffect( text, true, pos, Scale, color, layerDepth, fontType, existedFrame, step ) );
        }

        /// <summary>
        /// ����Ļ���������һ��RiseFade������Ч��
        /// ע�⣬������������а������ģ�����ѡ����������
        /// </summary>
        /// <param name="text">��������</param>
        /// <param name="pos">��ʾ����Ļλ��</param>
        /// <param name="Scale">��С</param>
        /// <param name="color">��ɫ</param>
        /// <param name="layerDepth">�������</param>
        /// <param name="fontType">����</param>
        /// <param name="existedFrame">��������Ļ�е�ʱ��ѭ������</param>
        /// <param name="step">ÿ��ʱ��ѭ���У������ĸ߶ȣ�������Ϊ��λ</param>
        static public void AddRiseFadeInScrnCoordin ( string text, Vector2 pos, float Scale, Color color, float layerDepth, string fontType, float existedFrame, float step )
        {
            sEffects.AddLast( new FadeUpEffect( text, false, pos, Scale, color, layerDepth, fontType, existedFrame, step ) );
        }

        /// <summary>
        /// ������е�������Ч
        /// </summary>
        public static void Clear ()
        {
            sEffects.Clear();
        }

    }

    /// <summary>
    /// ����������Ч��һ�㷽��
    /// </summary>
    public interface ITextEffect
    {
        /// <summary>
        /// ���Ʋ������Լ�
        /// </summary>
        void Draw ();

        /// <summary>
        /// �Ƿ��Ѿ�����
        /// </summary>
        bool Ended { get;}
    }
    
}
