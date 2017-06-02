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
    public class Timer
    {

        public delegate void endTimer();
        public static endTimer endtimer;

        public static System.Threading.Timer timerVar;
        static Locator GPSLocator;
        public void Start(Locator gs)
        {
            GPSLocator = gs;
            timerVar = new System.Threading.Timer(TimerCallback, null, 0, 10000);
            endtimer?.Invoke();
        }
        public void TimerCallback(object tc)
        {
            try
            {
                bool ok = Files.SaveGPXTempFile(GPSLocator);
                if (!ok) { Console.WriteLine("TempFile : saveKO"); }
                else { Console.WriteLine("TempFile : saveOK"); }
            }
            catch (Exception ex)
            {
                ex.Source = "timer.timerCallback";
                ErrorMessage.PrintOut(ex, "error handeled");
            }
        }
        public void Stop()
        {
            timerVar.Dispose();
        }

    }

}
