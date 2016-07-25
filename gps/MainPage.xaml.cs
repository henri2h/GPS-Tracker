using System;
using System.Collections.Generic;
using System.IO;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.ApplicationModel.Background;
using System.Threading;
using Windows.UI.Core;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.UI;
using System.Xml;


// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace gps
{
    public sealed partial class MainPage : Page
    {
        string latitude = "test";
        string longitude = "test";
        double? speed = 0;
        double? maxSpeed = 0;
        string altitude = "test";
        string accuracy = "test";
        string output = "";
        string source = "";
        StringWriter sw = new StringWriter();
        int value = 0;
        DateTimeOffset date = DateTimeOffset.Now;

        BasicGeoposition position;
        BasicGeoposition position_old;
        Geoposition global;

        List<point> track = new List<point>();

        public MainPage()
        {
            this.InitializeComponent();
   

        }


        private Geolocator locator;
        private ObservableCollection<string> coordinates = new ObservableCollection<string>();

        //true if the localisation is enabled

        private void Locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {

            //get all the info
            global = args.Position;
            if (global != null)
            {
                position_old = position;
                position = args.Position.Coordinate.Point.Position;


                output = value.ToString();

                // login point
                point newPoint = new point();
                newPoint.track = value;

                pointDescription pointValue = new pointDescription();
                pointValue.altitude = args.Position.Coordinate.Point.Position.Altitude;
                pointValue.latitude = args.Position.Coordinate.Point.Position.Latitude;
                pointValue.longitude = args.Position.Coordinate.Point.Position.Longitude;

                pointValue.speed = args.Position.Coordinate.Speed * 3.6;
                pointValue.date = args.Position.Coordinate.Timestamp;

                newPoint.waypoint = pointValue;

                track.Add(newPoint);

                // set display
                altitude = newPoint.waypoint.altitude.ToString();
                latitude = newPoint.waypoint.latitude.ToString();
                longitude = newPoint.waypoint.longitude.ToString();
                speed = newPoint.waypoint.speed;
                date = newPoint.waypoint.date;

                accuracy = args.Position.Coordinate.Accuracy.ToString();
                source = args.Position.Coordinate.PositionSource.ToString();
                if (maxSpeed < speed) { maxSpeed = speed; }

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
        }
        private void Locator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            output = "status changed" + args.Status.ToString();
            var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                define();

            });

        }

        private void GetCoordinate()
        {

            locator = new Geolocator();
            locator.DesiredAccuracy = PositionAccuracy.High;
            locator.MovementThreshold = 1;
            locator.PositionChanged += Locator_PositionChanged;
            locator.StatusChanged += Locator_StatusChanged;
            coords.ItemsSource = coordinates;

            StartLocationExtensionSession();

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
            }

        }

        private void ExtendedExecutionSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
        //ExtendedExecutionSession sender, ExtensionRevokedEventArgs args)
        {
            //TODO: clean up session data

            StopLocationExtensionSession();
            StartLocationExtensionSession();
        }

        private void StopLocationExtensionSession()
        {
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
            tbLatitude.Text = latitude;
            tbLongitude.Text = longitude;
            tbAltitude.Text = altitude;
            tbSpeed.Text = speed.ToString();
            tbAccuracy.Text = accuracy;
            tbDate.Text = date.ToString();
            tbOutput.Text = output;
            tbSource.Text = source;
            tbMaxSpeed.Text = maxSpeed.ToString();
            value++;

            try { MapControl1.Center = global.Coordinate.Point; }
            catch { }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            choose();
        }


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
                await FileIO.WriteTextAsync(file, generateGPXOutput());
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
            /* MapControl1.Center =
                new Geopoint(new BasicGeoposition()
                {
                    Latitude = 46.604,
                    Longitude = 6.329
                });*/
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
        private string generateGPXOutput()
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "no");
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);


            XmlElement gpx = doc.CreateElement(string.Empty, "gpx", string.Empty);
            doc.AppendChild(gpx);

            XmlElement trk = doc.CreateElement(string.Empty, "trk", string.Empty);
            gpx.AppendChild(trk);

            XmlElement name = doc.CreateElement(string.Empty, "name", string.Empty);
            XmlText nameValue = doc.CreateTextNode("track : " + DateTime.Now.ToString());
            name.AppendChild(nameValue);
            trk.AppendChild(name);

            XmlElement trkseg = doc.CreateElement(string.Empty, "trkseg", string.Empty);
            trk.AppendChild(trkseg);

            foreach (point pointLocal in track)
            {
                //create point
                XmlElement trkpt = doc.CreateElement(string.Empty, "trkpt", string.Empty);
                //set attribute latitude and longitude
                XmlAttribute lat = doc.CreateAttribute("lat");
                lat.Value = pointLocal.waypoint.latitude.ToString();
                XmlAttribute lon = doc.CreateAttribute("lon");
                lon.Value = pointLocal.waypoint.longitude.ToString();
                // add attributes to the xml node
                trkpt.Attributes.Append(lat);
                trkpt.Attributes.Append(lon);
                // add the xml node to the xml document
                trkseg.AppendChild(trkpt);

                // add elevetion to the xml node
                XmlElement ele = doc.CreateElement(string.Empty, "ele", string.Empty);
                XmlText value = doc.CreateTextNode(pointLocal.waypoint.altitude.ToString());
                ele.AppendChild(value);
                trkpt.AppendChild(ele);
            }

            //returning the xml document to a string
            StringWriter output = new StringWriter();
            doc.Save(output);
            return output.ToString();

        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            GetCoordinate();

        }
    }

    public class pointDescription
    {
        //position
        public double altitude { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }

        //date
        public DateTimeOffset date { get; set; }

        // speed
        public double? speed { get; set; }
    }
    public class point
    {
        public int track { get; set; }
        public pointDescription waypoint { get; set; }
    }
}
