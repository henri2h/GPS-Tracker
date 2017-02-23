using System;
using System.AppCore;
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
    public class timer
    {

        public delegate void endTimer();
        public static endTimer endtimer;

        public static Timer timerVar;
        static locator GPSLocator;
        public void start(locator gs)
        {
            GPSLocator = gs;
            timerVar = new Timer(timerCallback, null, 0, 10000);
            if (endtimer != null) { endtimer(); }
        }
        public void timerCallback(object tc)
        {
            try
            {
                bool ok = files.saveGPXTempFile( GPSLocator);
                if (!ok) { Console.WriteLine("TempFile : saveKO"); }
                else { Console.WriteLine("TempFile : saveOK"); }
            }
            catch (Exception ex)
            {
                ex.Source = "timer.timerCallback";
                ErrorMessage.printOut(ex, "error handeled");
            }
        }
        public void stop()
        {
            timerVar.Dispose();
        }

    }

}
