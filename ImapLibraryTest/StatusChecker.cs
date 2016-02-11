using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ImapLibraryTest
{
    class StatusChecker
    {
        private int invokeCount;
        private int maxCount;

        public StatusChecker(int count)
        {
            invokeCount = 0;
            maxCount = count;
        }

        // This method is called by the timer delegate.
        public void CheckStatus(Object stateInfo)
        {
            //AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            //Console.WriteLine("{0} Checking status {1,2}.",
            //    DateTime.Now.ToString("h:mm:ss.fff"),
            //    (++invokeCount));

            //if (invokeCount == maxCount)
            //{
            //    // Reset the counter and signal Main.
            //    invokeCount = 0;
            //    autoEvent.Set();
            //}
        }
    }
}
