using System;
using System.Collections.Generic;
using System.Text;
using Platform.Update;

namespace Platform.UpdateManage
{
    /*
     * 需要进一步的完善，添加Interval功能
     * 
     * */

    public delegate void OnTimerCallHandler ();

    public class GameTimer : IUpdater
    {

        static List<GameTimer> sTimers = new List<GameTimer>();

        static public void UpdateTimers ( float seconds )
        {
            for (int i = 0; i < sTimers.Count; i++)
            {
                sTimers[i].Update( seconds );
                if (sTimers[i].isEnd)
                {
                    sTimers.Remove( sTimers[i] );
                    i--;
                }
            }
        }

        static public void ClearAllTimer ()
        {
            sTimers.Clear();
        }

        float curTime = 0;

        float callTime;

        bool isEnd = false;

        public event EventHandler onCall;

        OnTimerCallHandler handler;

        public GameTimer ( float callTime )
        {
            this.callTime = Math.Max( 0, callTime );

            sTimers.Add( this );
        }


        public GameTimer ( float callTime, OnTimerCallHandler handler )
        {
            this.callTime = Math.Max( 0, callTime );

            this.handler = handler;

            sTimers.Add( this );
        }



        #region IUpdater 成员

        public void Update ( float seconds )
        {
            curTime += seconds;

            if (curTime >= callTime)
            {
                if (onCall != null)
                    onCall( this, EventArgs.Empty );

                if (handler != null)
                    handler();

                isEnd = true;
            }
        }

        #endregion
    }
}
