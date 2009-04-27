using System;
using System.Collections.Generic;
using System.Text;

namespace GameEngine.Graphics
{
    /// <summary>
    /// 可切帧对象的基类
    /// </summary>
    public class AnimatedSprite : IAnimated
    {

        #region Variables

        /// <summary>
        /// 当动画结束时引发这个事件
        /// </summary>
        public event EventHandler OnStop;

        /// <summary>
        /// 两次切帧之间相隔游戏绘制循环的个数
        /// </summary>
        public int Interval = 1;

        /// <summary>
        /// 动画的帧总数
        /// </summary>
        protected int mSumFrame;

        /// <summary>
        /// 当前的帧索引，从0开始
        /// </summary>
        protected int mCurFrameIndex;

        int timeTick = 0;

        bool started = false;

        bool end = false;

        int mShowedFrame;

        int mSumShowFrame = 0;

        bool mShowOnce = false;

        #endregion

        /// <summary>
        /// 动画是否已经开始显示
        /// </summary>
        public bool IsStart
        {
            get { return started; }
        }

        /// <summary>
        /// 动画是否已经结束
        /// </summary>
        public bool IsEnd
        {
            get { return end; }
        }

        #region Start

        /// <summary>
        /// 从索引0开始连续播放动画。
        /// </summary>
        public void Start ()
        {
            Start( 0 );
        }

        /// <summary>
        /// 开始连续播放动画，制定开始的帧索引
        /// </summary>
        /// <param name="startFrame">开始处的帧索引</param>
        public void Start ( int startFrame )
        {
            Start( startFrame, 0, false );
        }

        /// <summary>
        /// Start to Show the Cartoon on Screen,
        /// it will start at the startFrame index,
        /// and after passing sumShowFrame's number of frames, it will a stop automatically, and call OnStop Event.
        /// 开始播放动画，制定播放开始的索引和一共显示多少帧，并制定是否只播放一次
        /// </summary>
        /// <param name="startFrame">开始处的帧索引</param>
        /// <param name="sumShowFrame">总共显示的帧数</param>
        /// <param name="showOnce">是否只显示一次</param>
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

        /// <summary>
        /// 切换到下一帧
        /// </summary>
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

        /// <summary>
        /// 绘制当前帧
        /// </summary>
        public void DrawCurFrame ()
        {
            Draw();
            timeTick++;
            if (timeTick % Interval == 0)
            {
                NextFrame();
            }
        }

        /// <summary>
        /// 绘制当前帧，由继承类重载后实现
        /// </summary>
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
