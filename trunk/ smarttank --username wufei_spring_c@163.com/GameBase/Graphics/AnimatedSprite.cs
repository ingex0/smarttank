using System;
using System.Collections.Generic;
using System.Text;

namespace GameBase.Graphics
{
    public class AnimatedSprite : IAnimated
    {

        #region Variables

        public event EventHandler OnStop;

        public int Interval = 1;

        protected int mSumFrame;

        protected int mCurFrameIndex;

        int timeTick = 0;

        bool started = false;

        bool end = false;

        int mShowedFrame;

        int mSumShowFrame = 0;

        bool mShowOnce = false;

        #endregion

        public bool IsStart
        {
            get { return started; }
        }

        public bool IsEnd
        {
            get { return end; }
        }

        #region Start

        public void Start ()
        {
            Start( 0 );
        }

        public void Start ( int startFrame )
        {
            Start( startFrame, 0, false );
        }

        /// <summary>
        /// Start to Show the Cartoon on Screen,
        /// it will start at the startFrame index,
        /// and after passing sumShowFrame's number of frames, it will a stop automatically, and call OnStop Event.
        /// </summary>
        /// <param name="startFrame"></param>
        /// <param name="sumShowFrame"></param>
        public void Start ( int startFrame, int sumShowFrame, bool showOnce )
        {
            started = true;
            mCurFrameIndex = Math.Max( 0, Math.Min( mSumFrame, startFrame ) );
            mSumShowFrame = sumShowFrame;
            mShowedFrame = 0;
            mShowOnce = showOnce;
        }

        #endregion

        #region Next Frame
        public void NextFrame ()
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

        #region Draw Current Frame
        public void DrawCurFrame ()
        {
            Draw();
            timeTick++;
            if (timeTick % Interval == 0)
            {
                NextFrame();
            }
        }

        protected virtual void Draw ()
        {

        }

        #endregion

        #region StopShow
        private void Stop ()
        {
            mShowedFrame = 0;
            mCurFrameIndex = 0;
            started = false;
            if (OnStop != null)
                OnStop( this, null );
        }
        #endregion
    }
}
