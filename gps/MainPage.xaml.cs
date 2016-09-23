using System;
using System.IO;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml.Controls.Maps;
using Windows.ApplicationModel.ExtendedExecution;
using System.Threading;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using Windows.UI;
using Windows.UI.Xaml;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace gps
{
    public sealed partial class MainPage : Page
    {
        //speed unit to display
        speedUnit speedunit = speedUnit.metersPerSecond;

        // saving
        static string tempFile = "";
        bool recordingLocalisation = false;

        //postion
        static List<point> track = new List<point>();

        point currentPoint = new point();

        // max
        //speed
        double? maxSpeed = 0;
        double mediumSpeed = 0;
        double mediumSpeedVertical = 0;

        //distance
        double totalDistance = 0;


        string output = "";
        string source = "";


        StringWriter sw = new StringWriter();
        int value = 0;
        DateTimeOffset date = DateTimeOffset.Now;

        //postion recorder
        BasicGeoposition position;
        BasicGeoposition position_old;

        Geoposition global;


        private Geolocator locator;
        private ObservableCollection<string> coordinates = new ObservableCollection<string>();


        public MainPage()
        {
            this.InitializeComponent();
            tempFile = getTempFile();
            messageBox("get temp file : " + tempFile);
            timer.start();

            startLocalisation();
        }

        private async void messageBox(string msg)
        {
            var msgDlg = new Windows.UI.Popups.MessageDialog(msg);
            msgDlg.DefaultCommandIndex = 1;
            await msgDlg.ShowAsync();
        }

        //true if the localisation is enabled
        private bool startLocalisation()
        {
            locator = new Geolocator();


            locator.DesiredAccuracy = PositionAccuracy.High;
            locator.MovementThreshold = 1;

            locator.PositionChanged += Locator_PositionChanged;
            locator.StatusChanged += Locator_StatusChanged;

            coords.ItemsSource = coordinates;

            StartLocationExtensionSession();

            //    System.Diagnostics.Debug.WriteLine("Failled to create extended session");


            define();
            return true;
        }

        private void Locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {

            //get all the info
            global = args.Position;
            if (global != null)
            {

                position_old = position;
                position = args.Position.Coordinate.Point.Position;
                output = value.ToString();

                if (value > 2)
                {
                    totalDistance += GetDistanceBetweenPoints(position_old.Latitude, position_old.Longitude, position.Latitude, position.Longitude);
                }
                
                // ======= setup the variables ============
                currentPoint.track = value;

                currentPoint.altitude = args.Position.Coordinate.Point.Position.Altitude;
                currentPoint.latitude = args.Position.Coordinate.Point.Position.Latitude;
                currentPoint.longitude = args.Position.Coordinate.Point.Position.Longitude;

                currentPoint.speed = args.Position.Coordinate.Speed;
                currentPoint.date = args.Position.Coordinate.Timestamp;
                currentPoint.accuracy = args.Position.Coordinate.Accuracy;
                currentPoint.positionSource = args.Position.Coordinate.PositionSource;



                // ===== save them if needed ===========
                if (recordingLocalisation)
                {
                    // login point
                    track.Add(currentPoint);
                    var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        define();
                        if (value > 2)
                        {
                            MapPolyline mapPolyline = new MapPolyline();
                            mapPolyline.Path = new Geopath(new List<BasicGeoposition>() {
                            new BasicGeoposition() {Latitude=position_old.Latitude, Longitude=position_old.Longitude},
                            new BasicGeoposition() {Latitude=position.Latitude, Longitude=position.Longitude}
                            });
                            mapPolyline.StrokeColor = Colors.Black;
                            mapPolyline.StrokeThickness = 3;
                            mapPolyline.StrokeDashed = true;

                            MapControl1.MapElements.Add(mapPolyline);
                        }
                    });
                }

                if (maxSpeed < currentPoint.speed) { maxSpeed = currentPoint.speed; }


            }
            
            value++;
        }
        private void Locator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            output = "status changed" + args.Status.ToString();
            var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                define();

            });

        }



        private ExtendedExecutionSession session;

        private async void StartLocationExtensionSession()
        {

            session = new ExtendedExecutionSession();
            session.Description = "Location Tracker";
            session.Reason = ExtendedExecutionReason.LocationTracking;
            session.Revoked += ExtendedExecutionSession_Revoked;
            var result = await session.RequestExtensionAsync();
            if (result == ExtendedExecutionResult.Denied)
            {
                output = "error";
                define();
                //TODO: handle denied
                //return false;

            }
            else
            {
                //return true;
            }

        }

        private void ExtendedExecutionSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
        //ExtendedExecutionSession sender, ExtensionRevokedEventArgs args)
        {
            messageBox("Extended session revoked");
            //TODO: clean up session data
            StopLocationExtensionSession();

        }

        private void StopLocationExtensionSession()
        {
            messageBox("Extended session stoped");
            //reinitialisze the session
            if (session != null)
            {
                session.Dispose();
                session = null;
            }

        }

        /// <summary>
        /// Update the UI
        /// </summary>
        private void define()
        {
            //localisation
            tbLatitude.Text = currentPoint.latitude.ToString();
            tbLongitude.Text = currentPoint.longitude.ToString();
            tbAltitude.Text = currentPoint.altitude.ToString();

            tbTotalDistance.Text = totalDistance.ToString();

            tbAccuracy.Text = currentPoint.accuracy.ToString();
            tbDate.Text = date.ToString();

            tbOutput.Text = output;
            tbSource.Text = source;

            // change the speed
            switch (speedunit)
            {
                case speedUnit.metersPerSecond:
                    tbSpeed.Text = currentPoint.speed.ToString() + "m/s";
                    tbMaxSpeed.Text = maxSpeed.ToString() + "m/s";
                    break;

                case speedUnit.kmPerHour:
                    tbSpeed.Text = (currentPoint.speed * 3.6).ToString() + "km/h";
                    tbMaxSpeed.Text = (maxSpeed * 3.6).ToString() + "km/h";
                    break;

                case speedUnit.milesPerHour:
                    tbSpeed.Text = (currentPoint.speed / 1609.344 * 3600).ToString() + "miles/h";
                    tbMaxSpeed.Text = (maxSpeed / 1609.344 * 3600).ToString() + "miles/h";
                    break;

                default:
                    break;
            }



            try { MapControl1.Center = global.Coordinate.Point; }
            catch { }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            messageBox("Page loaded");
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            choose();
        }


        /// <summary>
        /// choose and save file
        /// </summary>
        private async void choose()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".gpx" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "gps-track";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                await FileIO.WriteTextAsync(file, gpx.generateGPXOutput(track));
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    output = "File " + file.Name + " was saved.";
                }
                else
                {
                    output = "File " + file.Name + " couldn't be saved.";
                }
            }
            else
            {
                output = "Operation cancelled.";
            }
            define();
        }

        private void MapControl1_Loaded(object sender, RoutedEventArgs e)
        {
            MapControl1.ZoomLevel = 17;
        }

        private void btCenter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MapControl1.Center = global.Coordinate.Point;
            }
            catch { }
        }


        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            recordingLocalisation = true;
            define();

        }

        private string getTempFile()
        {
            string file = Path.GetTempPath();
            return file + "current.gpx";
        }


        public async static void saveGPX(Object stateInfo)
        {
            if (!File.Exists(tempFile))
            {
                File.Create(tempFile);
            }
            StorageFile file = await StorageFile.GetFileFromPathAsync(tempFile);

            await FileIO.WriteTextAsync(file, gpx.generateGPXOutput(track));
            System.Diagnostics.Debug.WriteLine("saved temporary file");
        }

        private void Slider_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            switch (sliderUnitSpeed.Value.ToString())
            {
                case "0":
                    speedunit = speedUnit.metersPerSecond;
                    sliderUnitSpeed.Header = "m/s";
                    break;

                case "1":
                    speedunit = speedUnit.kmPerHour;
                    sliderUnitSpeed.Header = "km/h";
                    break;

                case "2":
                    speedunit = speedUnit.milesPerHour;
                    sliderUnitSpeed.Header = "miles/h";
                    break;

            }
            define();
        }


        public double GetDistanceBetweenPoints(double lat1, double long1, double lat2, double long2)
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
    }

    public class point
    {
        public int track { get; set; }
        //position
        public double altitude { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double accuracy { get; set; }

        //date
        public DateTimeOffset date { get; set; }

        // speed
        public double? speed { get; set; }
        public PositionSource positionSource { get; set; }
    }



    /// <summary>
    /// This class is for the timer
    /// </summary>
    public static class timer
    {
        public static Timer timerVar;
        public static void start()
        {
            timerVar = new Timer(MainPage.saveGPX, null, 0, 100);
        }
        public static void stop()
        {
            timerVar.Dispose();
        }

    }

}
