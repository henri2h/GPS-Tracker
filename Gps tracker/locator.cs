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
    public class locator
    {

        public MainPage page;
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


        public locator(MainPage pages) { page = pages; }
        //true if the localisation is enabled
        public bool startLocalisation()
        {
            try
            {
                geoLocator = new Geolocator();

                track = new List<point>();
                geoLocator.DesiredAccuracy = PositionAccuracy.High;
                geoLocator.MovementThreshold = 1;

                geoLocator.PositionChanged += Locator_PositionChanged;
                geoLocator.StatusChanged += Locator_StatusChanged;


                page.informations.output = "locator started";

                // start the extended session
                extendedSession.StartLocationExtensionSession();

                System.Diagnostics.Debug.WriteLine("Started locator");
                Console.WriteLine("Locator started : ");
                return true;
            }
            catch (Exception ex)
            {
                ex.Source = "locator.startLocalisation";
                ErrorMessage.printOut(ex);

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
                    page.informations.output = value.ToString();
                    

                    if (value > 2)
                    {
                        page.informations.totalTravelDistance += GetDistanceBetweenPoints(position_old.Latitude, position_old.Longitude, position.Latitude, position.Longitude);
                    }
                    currentPoint = new point();
                    // ======= setup the variables ============
                    currentPoint.track = value;

                    currentPoint.altitude = args.Position.Coordinate.Point.Position.Altitude;
                    currentPoint.latitude = args.Position.Coordinate.Point.Position.Latitude;
                    currentPoint.longitude = args.Position.Coordinate.Point.Position.Longitude;

                    currentPoint.speed = args.Position.Coordinate.Speed;
                    currentPoint.date = args.Position.Coordinate.Timestamp;
                    currentPoint.accuracy = args.Position.Coordinate.Accuracy;
                    currentPoint.positionSource = args.Position.Coordinate.PositionSource;

                    track.Add(currentPoint);

                    // update UI
                    updateUIMap();
                    Console.WriteLine("New point : " + value);

                    page.informations.date = DateTime.Now;
                    page.informations.currentPoint = currentPoint;
                    if (page.informations.maxSpeed < currentPoint.speed) { page.informations.maxSpeed = currentPoint.speed.Value; }

                    value++;
                    updateUI();

                }

            }
            catch (Exception ex)
            {
                ex.Source = "locator.Locator_PositionChanged";
                ErrorMessage.printOut(ex);
            }
        }
        void Locator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            try
            {
                Console.WriteLine("Locator status changed : ");
                Console.WriteLine("status changed" + args.Status.ToString());
                Console.WriteLine("Locator status : " + sender.LocationStatus.ToString());

                page.informations.output = "status changed" + args.Status.ToString();
                System.Diagnostics.Debug.WriteLine(sender.LocationStatus.ToString());


                // tell the user
                Status = sender.LocationStatus.ToString();

                updateUI();
            }
            catch (Exception ex)
            {
                ex.Source = "locator.Locator_StatusChanged";
                ErrorMessage.printOut(ex);
            }


        }


        void updateUI()
        {
            try { page.unThreadUpdateUITextElement(); }
            catch (Exception ex)
            {
                ex.Source = "locator.updateUI";
                ErrorMessage.printOut(ex);
            }
        }
        void updateUIMap()
        {
            try { page.unThreadUpdateUIMap(); }
            catch (Exception ex)
            {
                ex.Source = "locator.updateUIMap";
                ErrorMessage.printOut(ex);
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
                ErrorMessage.printOut(ex);
                return 0;
            }
        }

    }
}
