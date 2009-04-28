using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine.Effects
{
    /// <summary>
    /// ����֡����Ļ���
    /// </summary>
    public abstract class AnimatedSprite: IManagedEffect
    {

        #region Variables

        /// <summary>
        /// ����������ʱ��������¼�
        /// </summary>
        public event EventHandler OnStop;

        /// <summary>
        /// ��֡Ƶ��
        /// </summary>
        public float Interval = 1;

        /// <summary>
        /// ������֡����
        /// </summary>
        protected int mSumFrame;

        /// <summary>
        /// ��ǰ��֡��������0��ʼ
        /// </summary>
        protected int mCurFrameIndex;

        float timer = 0;

        bool started = false;

        bool end = false;

        int mShowedFrame;

        int mSumShowFrame = 0;

        bool mShowOnce = false;

        #endregion

        /// <summary>
        /// �����Ƿ��Ѿ���ʼ��ʾ
        /// </summary>
        public bool IsStart
        {
            get { return started; }
        }

        /// <summary>
        /// �����Ƿ��Ѿ�����
        /// </summary>
        public bool IsEnd
        {
            get { return end; }
        }

        #region Start

        /// <summary>
        /// ������0��ʼ�������Ŷ�����
        /// </summary>
        public void Start ()
        {
            Start( 0 );
        }

        /// <summary>
        /// ��ʼ�������Ŷ������ƶ���ʼ��֡����
        /// </summary>
        /// <param name="startFrame">��ʼ����֡����</param>
        public void Start ( int startFrame )
        {
            Start( startFrame, 0, false );
        }

        /// <summary>
        /// Start to Show the Cartoon on Screen,
        /// it will start at the startFrame index,
        /// and after passing sumShowFrame's number of frames, it will a stop automatically, and call OnStop Event.
        /// ��ʼ���Ŷ������ƶ����ſ�ʼ��������һ����ʾ����֡�����ƶ��Ƿ�ֻ����һ��
        /// </summary>
        /// <param name="startFrame">��ʼ����֡����</param>
        /// <param name="sumShowFrame">�ܹ���ʾ��֡��</param>
        /// <param name="showOnce">�Ƿ�ֻ��ʾһ��</param>
        public void Start ( int startFrame, int sumShowFrame, bool showOnce )
        {
            started = true;
            mCurFrameIndex = Math.Max( 0, Math.Min( mSumFrame, startFrame ) );
            mSumShowFrame = sumShowFrame;
            mShowedFrame = 0;
            mShowOnce = showOnce;
        }

        #endregion

        #region StopShow

        private void Stop()
        {
            mShowedFrame = 0;
            mCurFrameIndex = 0;
            started = false;
            if (OnStop != null)
                OnStop(this, null);
        }

        #endregion

        #region Draw Current Frame

        /// <summary>
        /// ���Ƶ�ǰ֡���ɼ̳������غ�ʵ��
        /// </summary>
        protected virtual void DrawCurFrame()
        {

        }

        #endregion



        #region IManagedEffect ��Ա


        public void Update(float seconds)
        {
            timer += seconds;
            if (timer >= Interval)
            {
                timer = 0;
                NextFrame();
            }
        }

        private void NextFrame()
        {
            if (mSumShowFrame != 0)
            {
                mShowedFrame++;
                if (mShowedFrame >= mSumShowFrame)
                {
                    Stop();
                    end = true;
                }
            }
            mCurFrameIndex++;
            if (mCurFrameIndex >= mSumFrame)
            {
                if (mShowOnce)
                {
                    Stop();
                    end = true;
                }
                else
                {
                    mCurFrameIndex -= mSumFrame;
                }
            }
        }

        #endregion

        #region IDrawableObj ��Ա

        public void Draw()
        {
            DrawCurFrame();
        }

        public abstract Microsoft.Xna.Framework.Vector2 Pos{ get; }

        #endregion
    }
}
