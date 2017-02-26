using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gps_tracker.AppCore
{
    public class Information
    {
        public DateTime date { get; set; }

        public double maxSpeed { get; set; }
        public double currentSpeed { get; set; }
        public double mediumSpeed
        {
            get
            {
                if (totalTravelTime != null)
                {
                    if (totalTravelTime.TotalSeconds != 0)
                    {
                        return totalTravelDistance / totalTravelTime.TotalSeconds;
                    }
                }
                return 0;
            }
        }


        public double totalTravelDistance { get; set; }
        public DateTime startTravelTime { get; set; }
        public DateTime endTravelTime { get; set; }
        public TimeSpan totalTravelTime
        {
            get
            {
                if (startTravelTime != null)
                {
                    if (endTravelTime != null && endTravelTime > startTravelTime)
                    {
                        return endTravelTime - startTravelTime;
                    }
                    else if (DateTime.Now > startTravelTime)
                    {
                        return DateTime.Now - startTravelTime;
                    }
                }
                return new TimeSpan();
            }
        }

        public string output { get; set; }
        public string Status { get; set; }

        public bool isExtendedSessionOn { get; set; }
        public point currentPoint { get; set; }
    }
}
