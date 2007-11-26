using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameBase.Graphics
{
    /// <summary>
    /// �Ӷ��ͼ�н��������Ķ�����
    /// </summary>
    public class AnimatedSpriteSeries : AnimatedSprite, IDisposable
    {
        /// <summary>
        /// ��ǰ����ͼ�ļ����뵽Content���У��Ա����ȡ�����Ϸ��ͣ�͡�
        /// ��Դ���ɹ̶���ͷ���ֺ����ֺ�İ����������������
        /// </summary>
        /// <param name="assetHead">��Դ��·���Լ�ͷ����</param>
        /// <param name="firstNo">������ʼ������</param>
        /// <param name="sumFrame">ϵ����ͼ�ļ�������</param>
        static public void LoadResource ( string assetHead, int firstNo, int sumFrame )
        {
            try
            {
                for (int i = 0; i < sumFrame; i++)
                {
                    BaseGame.Content.Load<Texture2D>( assetHead + (firstNo + i) );
                }
            }
            catch (Exception)
            {
                throw new Exception( "���붯����Դ��������ͼƬ��Դ�Ƿ�����" );
            }
        }

        #region Variables

        Sprite[] mSprites;

        bool alreadyLoad = false;

        /// <summary>
        /// ��ȡ��ǰ֡��Sprite����
        /// </summary>
        public Sprite CurSprite
        {
            get { return mSprites[mCurFrameIndex]; }
        }

        /// <summary>
        /// ��ȡ����ÿһ֡��Sprite����
        /// </summary>
        public Sprite[] Sprites
        {
            get { return mSprites; }
        }

        #endregion

        #region Load Textures

        /// <summary>
        /// ���ļ��е���ϵ����ͼ��Դ��
        /// ��Դ���ɹ̶���ͷ���ֺ����ֺ�İ���������������ɡ�
        /// ������زĹܵ��е��룬��Ϊ�÷�������������Ϸ֡��ͣ�͡�
        /// </summary>
        /// <param name="path">��ͼ��Դ��·��</param>
        /// <param name="fileHeadName">ͷ����</param>
        /// <param name="extension">��չ��</param>
        /// <param name="firstNo">��һ����������</param>
        /// <param name="sumFrame">��������</param>
        /// <param name="supportInterDect">�Ƿ���ӳ�ͻ����֧��</param>
        public void LoadSeriesFromFiles ( string path, string fileHeadName, string extension, int firstNo, int sumFrame, bool supportInterDect )
        {
            if (alreadyLoad)
                throw new Exception( "�ظ����붯����Դ��" );

            alreadyLoad = true;

            mSprites = new Sprite[sumFrame];
            try
            {
                for (int i = 0; i < sumFrame; i++)
                {
                    mSprites[i] = new Sprite();
                    mSprites[i].LoadTextureFromFile( Path.Combine( path, fileHeadName + (i + firstNo) + extension ), supportInterDect );
                }
            }
            catch (Exception)
            {
                mSprites = null;
                alreadyLoad = false;

                throw new Exception( "���붯����Դ��������ͼƬ��Դ�Ƿ�����" );
            }

            mSumFrame = sumFrame;
            mCurFrameIndex = 0;
            AnimatedManager.Add( this );
        }

        /// <summary>
        /// ʹ���زĹܵ�������ͼ�ļ���
        /// ��Դ���ɹ̶���ͷ���ֺ����ֺ�İ����������������
        /// </summary>
        /// <param name="assetHead">��Դ��·���Լ�ͷ����</param>
        /// <param name="firstNo">������ʼ������</param>
        /// <param name="sumFrame">ϵ����ͼ�ļ�������</param>
        /// <param name="supportInterDect">�Ƿ��ṩ��ͻ����֧��</param>
        public void LoadSeriesFormContent ( string assetHead, int firstNo, int sumFrame, bool supportInterDect )
        {
            if (alreadyLoad)
                throw new Exception( "�ظ����붯����Դ��" );

            alreadyLoad = true;

            mSprites = new Sprite[sumFrame];

            try
            {
                for (int i = 0; i < sumFrame; i++)
                {
                    mSprites[i] = new Sprite();
                    mSprites[i].LoadTextureFromContent( assetHead + (firstNo + i), supportInterDect );
                }
            }
            catch (Exception)
            {
                mSprites = null;
                alreadyLoad = false;

                throw new Exception( "���붯����Դ��������ͼƬ��Դ�Ƿ�����" );
            }

            mSumFrame = sumFrame;
            mCurFrameIndex = 0;
            AnimatedManager.Add( this );
        }

        #endregion

        #region Dispose

        public void Dispose ()
        {
            foreach (Sprite sprite in mSprites)
            {
                sprite.Dispose();
            }
        }

        #endregion

        #region Set Sprites Parameters

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="pos"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="rata"></param>
        /// <param name="color"></param>
        /// <param name="layerDepth"></param>
        /// <param name="blendMode"></param>
        public void SetSpritesParameters ( Vector2 origin, Vector2 pos, float width, float height, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            foreach (Sprite sprite in mSprites)
            {
                sprite.SetParameters( origin, pos, width, height, rata, color, layerDepth, blendMode );
            }
        }

        /// <summary>
        /// ��������Sprite�Ĳ���
        /// </summary>
        /// <param name="origin">��ͼ�����ģ�����ͼ�����ʾ</param>
        /// <param name="pos">�߼�λ��</param>
        /// <param name="scale">��ͼ�����űȣ�= �߼�����/��ͼ���ȣ�</param>
        /// <param name="rata">�߼���λ��</param>
        /// <param name="color">������ɫ</param>
        /// <param name="layerDepth">��ȣ�1Ϊ��Ͳ㣬0Ϊ����</param>
        /// <param name="blendMode">���õĻ��ģʽ</param>
        public void SetSpritesParameters ( Vector2 origin, Vector2 pos, float scale, float rata, Color color, float layerDepth, SpriteBlendMode blendMode )
        {
            foreach (Sprite sprite in mSprites)
            {
                sprite.SetParameters( origin, pos, scale, rata, color, layerDepth, blendMode );
            }
        }

        #endregion

        #region Draw Current Frame

        /// <summary>
        /// ���Ƶ�ǰ����ͼ
        /// </summary>
        protected override void Draw ()
        {
            CurSprite.Draw();
        }

        #endregion


    }
}
