using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gps_tracker
{

    /// <summary>
    /// This class is for the timer
    /// </summary>
    public static class timer
    {
        public static bool timerEnabled;

        public delegate void endTimer();
        public static endTimer endtimer;

        public static Timer timerVar;
        static locator GPSLocator;
        public static void start(locator gs)
        {
            GPSLocator = gs;
            timerVar = new Timer(timerCallback, null, 0, 10000);
            if (endtimer != null) { endtimer(); }
        }
        public static void timerCallback(object tc)
        {
            files.saveGPXTempFile(GPSLocator);
        }
        public static void stop()
        {
            timerVar.Dispose();
        }

    }

}
