using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Gps_tracker
{
    public class point
    {

        public int track { get; set; }
        //position
        public double altitude { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        /// <summary>
        /// accuracy in m
        /// </summary>
        public double accuracy { get; set; }

        //date
        public DateTimeOffset date { get; set; }

        // speed
        /// <summary>
        /// Speed in m/s
        /// </summary>
        public double? speed { get; set; }
        public PositionSource positionSource { get; set; }


    }
}
