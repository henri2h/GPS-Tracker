using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Gps_tracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        timer time;
        public string tempFile = "";

        //UI
        public double? maxSpeed = 0;
        double mediumSpeed = 0;
        speedUnit SpeedUnit = speedUnit.metersPerSecond;

        public double totalDistance = new double();
        public string output = "";

        public string LocatorStatus = "";


        StringWriter sw = new StringWriter();

        DateTimeOffset date = DateTimeOffset.Now;

        // intialize the locator
        locator GPSLocator;

        public static ObservableCollection<string> coordinates = new ObservableCollection<string>();


        public MainPage()
        {
            try
            {
                this.InitializeComponent();

                // place the mapServiceToken here, you can get one at https://www.bingmapsportal.com/
                MapControl1.MapServiceToken = "";

                // intitalisation
                time = new timer(this);
                GPSLocator = new locator(this);
                Console.page = this;

                // this is in order to debug the client remotely
                var _ = TCPClient.SocketClient.connect("10.0.0.3");


                tempFile = files.getTempFile(".gpx");
                Console.WriteLine("Temp file : " + tempFile);

                GPSLocator.startLocalisation();
                updateUITextElements();
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.constructor";
                ErrorMessage.printOut(ex);
            }
        }

        //intialisation
        private void MapControl1_Loaded(object sender, RoutedEventArgs e) { updateUIMap(); }
        private void btCenter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GPSLocator.global != null)
                {
                    MapControl1.Center = GPSLocator.global.Coordinate.Point;
                    MapControl1.ZoomLevel = 16;
                }
            }
            catch (Exception ex)
            {
                ex.Source = "MaiPage.btCenter_Click";
                ErrorMessage.printOut(ex);
            }
        }
        //start
        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            if (GPSLocator.recordingLocalisation == false)
            {
                time.start(GPSLocator);
                GPSLocator.recordingLocalisation = true;
                btStart.Content = "Stop recording the track";
            }
            else
            {
                time.stop();
                GPSLocator.recordingLocalisation = false;
                btStart.Content = "Start recording the track";
            }
            updateUITextElements();
        }
        //save
        private void btSave_Click(object sender, RoutedEventArgs e) { files.choose(this, GPSLocator); }
        //slider
        private void Slider_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            updateUITextElements();
        }
        private void Slider_PointerMoved(object sender, RangeBaseValueChangedEventArgs e)
        {
            updateUITextElements();
        }
        private void sliderUnitSpeed_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            updateSpeedUIElement();
        }
        //update

        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateUITextElements();
        }

        //============ UI ===============
        public void unThreadUpdateUITextElement()
        {
            try
            {
                var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { updateUITextElements(); });
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.unThreadUpdateUITextElement";
                ErrorMessage.printOut(ex);
            }
        }
        public void updateUITextElements()
        {
            try
            {
                updateUITextBox();
                updateSpeedUIElement();
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.updateUITextElements";
                ErrorMessage.printOut(ex);
            }
        }

        public void updateUITextBox()
        {
            try
            {
                updateTextBlock(tbDate, "Date : ", date.ToString());
                updateTextBlock(tbOutput, "Output : ", output);

                updateTextBlock(tbSource, "Postion source : ", GPSLocator.currentPoint.positionSource.ToString());
                updateTextBlock(LocatorUITexBlock, "Locator status : ", GPSLocator.Status);

                updateTextBlock(tbTotalDistance, "Total travel distance : ", totalDistance.ToString());

                updateTextBlock(tbLatitude, "Latitude : ", GPSLocator.currentPoint.latitude.ToString());
                updateTextBlock(tbLongitude, "Longitude : ", GPSLocator.currentPoint.longitude.ToString());
                updateTextBlock(tbAltitude, "Altitude : ", GPSLocator.currentPoint.altitude.ToString());
                updateTextBlock(tbAccuracy, "Accuracy : ", GPSLocator.currentPoint.accuracy.ToString());
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.updateUITextBox";
                ErrorMessage.printOut(ex);
            }

        }


        public void updateTextBlock(TextBlock tb, string helpString, string text)
        {
            try
            {
                if (text != "" || text != null)
                {
                    tb.Visibility = Visibility.Visible;
                    tb.Text = helpString + text;
                }
                else { tb.Visibility = Visibility.Collapsed; }
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.updateTextBlock";
                ErrorMessage.printOut(ex);
            }
        }

        public void updateSpeedUnit()
        {
            switch (sliderUnitSpeed.Value.ToString())
            {
                case "0":
                    SpeedUnit = speedUnit.metersPerSecond;

                    break;

                case "1":
                    SpeedUnit = speedUnit.kmPerHour;
                    break;

                case "2":
                    SpeedUnit = speedUnit.milesPerHour;
                    break;

            }
        }
        public void updateSpeedUIElement()
        {
            updateSpeedUnit();

            sliderUnitSpeed.Header = "Speed unit : " + getSpeedStringUnit();
            // speed
            updateTextBlock(UISpeedTextBox, "Speed : ", getSpeedValueForUnit(GPSLocator.currentPoint.speed));
            updateTextBlock(UIMediumSpeedTextBox, "Average speed : ", getSpeedValueForUnit(mediumSpeed));
            updateTextBlock(UIMaxSpeedTextBox, "Max speed : ", getSpeedValueForUnit(maxSpeed));
        }

        // return speed unit values and calculate them
        public string getSpeedValueForUnit(double? inputSpeed)
        {
            if (inputSpeed == null) { return null; }
            string outputSpeed = "";
            if (SpeedUnit == speedUnit.metersPerSecond) { outputSpeed = inputSpeed.ToString(); }
            else if (SpeedUnit == speedUnit.kmPerHour) { outputSpeed = (inputSpeed * 3.6).ToString(); }
            else if (SpeedUnit == speedUnit.milesPerHour) { outputSpeed = (inputSpeed / 1609.344 * 3600).ToString(); }
            else { return ""; }

            return outputSpeed + getSpeedStringUnit();
        }
        public string getSpeedStringUnit()
        {
            if (SpeedUnit == speedUnit.metersPerSecond) { return "m/s"; }
            else if (SpeedUnit == speedUnit.kmPerHour) { return "km/h"; }
            else if (SpeedUnit == speedUnit.milesPerHour) { return "miles/h"; }
            else { return null; }
        }


        public void unThreadUpdateUIMap()
        {
            try
            {
                var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { updateUIMap(); });
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.unThreadUpdateUIMap";
                ErrorMessage.printOut(ex);
            }
        }
        public void updateUIMap()
        {
            try
            {

                point[] points = GPSLocator.track.ToArray();
                if (points.Length > 0)
                {
                    point oldPoint = points[points.Length - 2];
                    point current = points[points.Length - 1];

                    if (oldPoint != null)
                    {
                        setUIMapSegement(
                            new BasicGeoposition() { Latitude = oldPoint.latitude, Longitude = oldPoint.longitude },
                            new BasicGeoposition() { Latitude = current.latitude, Longitude = current.longitude }
                        );


                    }
                }

            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.updateUIMap";
                ErrorMessage.printOut(ex);
            }
        }
        public void setUIMapSegement(BasicGeoposition start, BasicGeoposition end)
        {
            Windows.UI.Xaml.Controls.Maps.MapPolyline mapPolyline = new Windows.UI.Xaml.Controls.Maps.MapPolyline();
            mapPolyline.Path = new Geopath(new List<BasicGeoposition>() { start, end });

            mapPolyline.StrokeColor = Colors.Black;
            mapPolyline.StrokeThickness = 3;
            mapPolyline.StrokeDashed = true;
            MapControl1.MapElements.Add(mapPolyline);
        }

        /// <summary>
        /// Refresh all the map
        /// </summary>
        public void updateUIAllMap()
        {
            MapControl1.MapElements.Clear();
            try
            {
                point oldPoint = null;
                point[] points = GPSLocator.track.ToArray();
                foreach (point pointElement in points)
                {
                    if (oldPoint != null)
                    {
                        Windows.UI.Xaml.Controls.Maps.MapPolyline mapPolyline = new Windows.UI.Xaml.Controls.Maps.MapPolyline();
                        mapPolyline.Path = new Geopath(new List<BasicGeoposition>() {
                            new BasicGeoposition() {Latitude=oldPoint.latitude, Longitude=oldPoint.longitude},
                            new BasicGeoposition() {Latitude=pointElement.latitude, Longitude=pointElement.longitude}
                             });
                        mapPolyline.StrokeColor = Colors.Black;
                        mapPolyline.StrokeThickness = 3;
                        mapPolyline.StrokeDashed = true;
                        MapControl1.MapElements.Add(mapPolyline);
                    }

                    oldPoint = pointElement;
                }

            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.updateUIMap";
                ErrorMessage.printOut(ex);
            }
        }
        public static async void messageBox(string msg)
        {
            var msgDlg = new Windows.UI.Popups.MessageDialog(msg);
            msgDlg.DefaultCommandIndex = 1;
            await msgDlg.ShowAsync();
        }



        public void WriteLine(string text)
        {
            try
            {
                var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ConsoleUIWriteBox.Text += text + Environment.NewLine;
                    //  ConsoleUITextBoxScroll.ChangeView(ConsoleUITextBoxScroll.ScrollableHeight, 0, ConsoleUITextBoxScroll.ZoomFactor);
                });
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.WriteLine (Console)";
                ErrorMessage.printOut(ex);
            }
        }

        private void ConsoleUIbtReturn_Click(object sender, RoutedEventArgs e)
        {
            string comm = ConsoleUIReadBox.Text;
            var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Console.setNewLine(comm); });
            ConsoleUIReadBox.Text = "";
        }
        private void btMapupdate_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Map Updating : Size of track : " + GPSLocator.track.ToArray().Length);
            updateUIAllMap();
        }
    }


}
