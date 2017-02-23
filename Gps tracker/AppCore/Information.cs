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
        public double mediumSpeed { get; set; }


        public double totalTravelDistance { get; set; }

        public string output { get; set; }
        public string Status { get; set; }

        public bool isExtendedSessionOn { get; set; }
        public point currentPoint { get; set; }
    }
}
