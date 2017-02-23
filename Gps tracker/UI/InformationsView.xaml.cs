using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Gps_tracker.UI
{
    public sealed partial class InformationsView : UserControl
    {
        speedUnit SpeedUnit
        {
            get
            {
                return AppCore.Core.settings.SpeedUnit;
            }
            set
            {
                AppCore.Core.settings.SpeedUnit = value;
                AppCore.Core.saveSettings();
            }
        }

        public InformationsView()
        {
            this.InitializeComponent();
        }
        public void updateUIInformations(point currentPoint, double totalDistance, string date, string output, string Status, double? speed, double? mediumSpeed, double? maxSpeed)
        {
            try
            {
                updateUITextBox(currentPoint, totalDistance, date, output, Status);
                updateSpeedUIElement(speed, mediumSpeed, maxSpeed);
                if (extendedSession.extendedSessionActive) { UITbExtendedSession.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Green); }
                else { UITbExtendedSession.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Red); }
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.updateUITextElements";
                ErrorMessage.printOut(ex);
            }
        }

        public void updateUITextBox(point currentPoint, double totalDistance, string date, string output, string Status)
        {
            try
            {
                updateTextBlock(tbDate, "Date : ", date);
                updateTextBlock(tbOutput, "Output : ", output);

                updateTextBlock(tbSource, "Postion source : ", currentPoint.positionSource.ToString());
                updateTextBlock(LocatorUITexBlock, "Locator status : ", Status);

                updateTextBlock(tbTotalDistance, "Total travel distance : ", totalDistance.ToString());

                updateTextBlock(tbLatitude, "Latitude : ", currentPoint.latitude.ToString());
                updateTextBlock(tbLongitude, "Longitude : ", currentPoint.longitude.ToString());
                updateTextBlock(tbAltitude, "Altitude : ", currentPoint.altitude.ToString());
                updateTextBlock(tbAccuracy, "Accuracy : ", currentPoint.accuracy.ToString());
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

        public void updateSpeedUIElement(double? speed, double? mediumSpeed, double? maxSpeed)
        {
            updateSpeedUnit();

            sliderUnitSpeed.Header = "Speed unit : " + getSpeedStringUnit();
            // speed
            updateTextBlock(UISpeedTextBox, "Speed : ", getSpeedValueForUnit(speed));
            updateTextBlock(UIMediumSpeedTextBox, "Average speed : ", getSpeedValueForUnit(mediumSpeed));
            updateTextBlock(UIMaxSpeedTextBox, "Max speed : ", getSpeedValueForUnit(maxSpeed));
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
        /*
        private void sliderUnitSpeed_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            updateSpeedUIElement();
        }
        //update
    */


        //slider
        private void Slider_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            //updateUITextElements();
        }
        private void Slider_PointerMoved(object sender, RangeBaseValueChangedEventArgs e)
        {
        //();
        }



    }
}
