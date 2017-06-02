using Gps_tracker.AppCore;
using System;
using System.AppCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Maps;

namespace Gps_tracker
{
    public class Locator
    {
        //postion
        public List<point> track = null;
        public point currentPoint = new point();

        //postion recorder
        BasicGeoposition position;
        BasicGeoposition position_old;

        public Geoposition global;
        public Geolocator geoLocator;

        public bool recordingLocalisation = false;
        public int value = 0;

        public string Status = "";
        internal bool SavedTimer;

        public Locator() { }

        //true if the localisation is enabled
        public bool StartLocalisation()
        {
            try
            {
                geoLocator = new Geolocator();

                track = new List<point>();
                geoLocator.DesiredAccuracy = PositionAccuracy.High;
                geoLocator.MovementThreshold = 5;

                geoLocator.PositionChanged += Locator_PositionChanged;
                geoLocator.StatusChanged += Locator_StatusChanged;


                Core.informations.output = "locator started";

                // start the extended session
                ExtendedSession.StartLocationExtensionSession();

                System.Diagnostics.Debug.WriteLine("Started locator");
                Console.WriteLine("Locator started : ");
                return true;
            }
            catch (Exception ex)
            {
                ex.Source = "locator.startLocalisation";
                ErrorMessage.PrintOut(ex);

                return false;
            }
        }
        void Locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            try
            {
                //get all the info
                global = args.Position;
                if (global != null)
                {

                    position_old = position;
                    position = args.Position.Coordinate.Point.Position;
                    Core.informations.output = value.ToString();


                    if (value > 2)
                    {
                        Core.informations.totalTravelDistance += GetDistanceBetweenPoints(position_old.Latitude, position_old.Longitude, position.Latitude, position.Longitude);
                    }

                    currentPoint = new point()
                    {
                        // ======= setup the variables ============
                        track = value,

                        altitude = args.Position.Coordinate.Point.Position.Altitude,
                        latitude = args.Position.Coordinate.Point.Position.Latitude,
                        longitude = args.Position.Coordinate.Point.Position.Longitude,

                        speed = args.Position.Coordinate.Speed,
                        date = args.Position.Coordinate.Timestamp,
                        accuracy = args.Position.Coordinate.Accuracy,
                        positionSource = args.Position.Coordinate.PositionSource
                    };

                    track.Add(currentPoint);

                    // update UI
                    UpdateUIMap();
                    Console.WriteLine("New point : " + value);

                    Core.informations.date = DateTime.Now;
                    Core.informations.currentPoint = currentPoint;
                    Core.informations.currentSpeed = currentPoint.speed.Value;
                    if (Core.informations.maxSpeed < currentPoint.speed) { Core.informations.maxSpeed = currentPoint.speed.Value; }

                    value++;
                    this.SavedTimer = false;
                    UpdateUI();

                }

            }
            catch (Exception ex)
            {
                ex.Source = "locator.Locator_PositionChanged";
                ErrorMessage.PrintOut(ex);
            }
        }
        void Locator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            try
            {
                Console.WriteLine("Locator status changed : ");
                Console.WriteLine("status changed" + args.Status.ToString());
                Console.WriteLine("Locator status : " + sender.LocationStatus.ToString());

                Core.informations.output = "status changed" + args.Status.ToString();
                System.Diagnostics.Debug.WriteLine(sender.LocationStatus.ToString());


                // tell the user
                Status = sender.LocationStatus.ToString();

                UpdateUI();
            }
            catch (Exception ex)
            {
                ex.Source = "locator.Locator_StatusChanged";
                ErrorMessage.PrintOut(ex);
            }


        }


        void UpdateUI()
        {
            try { Core.page.UnThreadUpdateUITextElement(); }
            catch (Exception ex)
            {
                ex.Source = "locator.updateUI";
                ErrorMessage.PrintOut(ex);
            }
        }
        void UpdateUIMap()
        {
            try { Core.page.UnThreadUpdateUIMap(); }
            catch (Exception ex)
            {
                ex.Source = "locator.updateUIMap";
                ErrorMessage.PrintOut(ex);
            }
        }

        public double GetDistanceBetweenPoints(double lat1, double long1, double lat2, double long2)
        {
            try
            {
                double distance = 0;

                double dLat = (lat2 - lat1) / 180 * Math.PI;
                double dLong = (long2 - long1) / 180 * Math.PI;

                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(lat2) * Math.Sin(dLong / 2) * Math.Sin(dLong / 2);
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

                //Calculate radius of earth
                // For this you can assume any of the two points.
                double radiusE = 6378135; // Equatorial radius, in metres
                double radiusP = 6356750; // Polar Radius

                //Numerator part of function
                double nr = Math.Pow(radiusE * radiusP * Math.Cos(lat1 / 180 * Math.PI), 2);
                //Denominator part of the function
                double dr = Math.Pow(radiusE * Math.Cos(lat1 / 180 * Math.PI), 2)
         + Math.Pow(radiusP * Math.Sin(lat1 / 180 * Math.PI), 2);
                double radius = Math.Sqrt(nr / dr);

                //Calaculate distance in metres.
                distance = radius * c;
                return distance;
            }
            catch (Exception ex)
            {
                ex.Source = "locator.getDistanceBetweenTwoPoints";
                ErrorMessage.PrintOut(ex);
                return 0;
            }
        }

    }
}
