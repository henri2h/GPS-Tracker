using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Gps_tracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //local
        public static MainPage mainpage;
        public static string tempFile = "";

        //UI
        public static double? maxSpeed = 0;
        double mediumSpeed = 0;
        speedUnit SpeedUnit = speedUnit.metersPerSecond;

        public static double totalDistance = 0;

        public static string output = "";
        public static string LocatorStatus = "";


        StringWriter sw = new StringWriter();

        DateTimeOffset date = DateTimeOffset.Now;

        // intialize the locator
        locator GPSLocator = new locator();

        public static ObservableCollection<string> coordinates = new ObservableCollection<string>();


        public MainPage()
        {
            try
            {
                this.InitializeComponent();
                mainpage = this;

                tempFile = files.getTempFile();
                Console.WriteLine("Temp file : " + tempFile);
                System.Diagnostics.Debug.WriteLine("get temp file : " + tempFile);

                GPSLocator.startLocalisation();
                updateUITextElements();
            }
            catch (Exception ex)
            {
                string err = ErrorMessage.getErrorString(ex);
                Console.WriteLine(err);
                tempFile = files.getTempFile();
                File.WriteAllText(tempFile + ".err", err);
                files.saveFile(".err", err + Environment.NewLine + tempFile);
            }
        }

        //intialisation
        private void MapControl1_Loaded(object sender, RoutedEventArgs e) { updateUIMap(); }
        private void btCenter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MapControl1.Center = GPSLocator.global.Coordinate.Point;
                MapControl1.ZoomLevel = 18;
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ErrorMessage.getErrorString(ex)); }
        }
        //start
        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            if (GPSLocator.recordingLocalisation == false)
            {
                //  timer.start(GPSLocator);
                GPSLocator.recordingLocalisation = true;
                btStart.Content = "Stop recording the track";
            }
            else
            {
                //timer.stop();
                GPSLocator.recordingLocalisation = false;
                btStart.Content = "Start recording the track";
            }
            updateUITextElements();
        }
        //save
        private void btSave_Click(object sender, RoutedEventArgs e) { files.choose(GPSLocator); }
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
        public void unThreadUpdateUITextElement() { var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { updateUITextElements(); }); }
        public void updateUITextElements()
        {
            updateUITextBox();
            updateSpeedUIElement();
        }

        public void updateUITextBox()
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


        public void updateTextBlock(TextBlock tb, string helpString, string text)
        {
            if (text != "" || text != null)
            {
                tb.Visibility = Visibility.Visible;
                tb.Text = helpString + text;
            }
            else { tb.Visibility = Visibility.Collapsed; }
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


        public void unThreadUpdateUIMap() { var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { updateUIMap(); }); }
        public void updateUIMap()
        {
            point oldPoint = null;
            foreach (point pointElement in GPSLocator.track)
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

        public static async void messageBox(string msg)
        {
            var msgDlg = new Windows.UI.Popups.MessageDialog(msg);
            msgDlg.DefaultCommandIndex = 1;
            await msgDlg.ShowAsync();
        }

        private void btMapupdate_Click(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine("Map should be updated");
            Console.WriteLine("Size of track : " + GPSLocator.track.ToArray().Length);
            updateUIMap();
        }

        public void WriteLine(string text)
        {
            var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ConsoleUIWriteBox.Text += text + Environment.NewLine;
                ConsoleUITextBoxScroll.ChangeView(ConsoleUITextBoxScroll.ScrollableHeight, 0, ConsoleUITextBoxScroll.ZoomFactor);
            });
        }


    }


}
