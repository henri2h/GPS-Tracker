using Gps_tracker.AppCore;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Gps_tracker.UI
{
    public sealed partial class InformationsView : UserControl
    {
        Information currentInfo = new Information();
        speedUnit SpeedUnit
        {
            get
            {
                return AppCore.Core.Settings.SpeedUnit;
            }
            set
            {
                AppCore.Core.Settings.SpeedUnit = value;
                //  AppCore.Core.saveSettings();
            }
        }

        public InformationsView()
        {
            this.InitializeComponent();
            updateUITextBox();
        }

        public void updateUIInformations(Information informations)
        {
            this.currentInfo = informations;
            //point currentPoint, double totalDistance, string date, string output, string Status, double? speed, double? mediumSpeed, double? maxSpeed
            try
            {
                updateUI();
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.updateUITextElements";
                ErrorMessage.printOut(ex);
            }
        }

        void updateUI()
        {
            updateUITextBox();
            updateSpeedUIElement();
            if (extendedSession.extendedSessionActive) { UITbExtendedSession.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Green); }
            else { UITbExtendedSession.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Red); }
        }
        void updateUITextBox()
        {
            try
            {
                updateTextBlock(tbDate, "Date : ", currentInfo.date.ToString());
                updateTextBlock(tbOutput, "Output : ", currentInfo.output);

                if (currentInfo.currentPoint != null)
                {
                    updateTextBlock(tbSource, "Postion source : ", currentInfo.currentPoint.positionSource.ToString());

                    updateTextBlock(tbLatitude, "Latitude : ", currentInfo.currentPoint.latitude.ToString());
                    updateTextBlock(tbLongitude, "Longitude : ", currentInfo.currentPoint.longitude.ToString());
                    updateTextBlock(tbAltitude, "Altitude : ", currentInfo.currentPoint.altitude.ToString());
                    updateTextBlock(tbAccuracy, "Accuracy : ", currentInfo.currentPoint.accuracy.ToString());
                }

                updateTextBlock(LocatorUITexBlock, "Locator status : ", currentInfo.Status);

                updateTextBlock(tbTotalDistance, "Total travel distance : ", currentInfo.totalTravelDistance.ToString());


                updateTextBlock(tbMemory, "Memory used : ", Windows.System.MemoryManager.AppMemoryUsage.ToString());
                updateTextBlock(tbMaxMemory, "Memory limit : ", Windows.System.MemoryManager.AppMemoryUsageLimit.ToString());
                updateTextBlock(tbMemoryLevel, "Memory level : ", Windows.System.MemoryManager.AppMemoryUsageLevel.ToString());
            }

            catch (Exception ex)
            {
                ex.Source = "MainPage.updateUITextBox";
                ErrorMessage.printOut(ex);
            }

        }


        void updateTextBlock(TextBlock tb, string helpString, string text)
        {
            try
            {
                if (text != "" && text != null)
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

        void updateSpeedUIElement()
        {
            sliderUnitSpeed.Header = "Speed unit : " + getSpeedStringUnit();
            // speed
            updateTextBlock(UISpeedTextBox, "Speed : ", getSpeedValueForUnit(currentInfo.currentSpeed));
            updateTextBlock(UIMediumSpeedTextBox, "Average speed : ", getSpeedValueForUnit(currentInfo.mediumSpeed));
            updateTextBlock(UIMaxSpeedTextBox, "Max speed : ", getSpeedValueForUnit(currentInfo.maxSpeed));
        }

        void updateSpeedUnit()
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

            updateSpeedUIElement();
        }


        // return speed unit values and calculate them
        string getSpeedValueForUnit(double? inputSpeed)
        {
            if (inputSpeed == null) { return null; }
            string outputSpeed = "";
            if (SpeedUnit == speedUnit.metersPerSecond) { outputSpeed = inputSpeed.ToString(); }
            else if (SpeedUnit == speedUnit.kmPerHour) { outputSpeed = (inputSpeed * 3.6).ToString(); }
            else if (SpeedUnit == speedUnit.milesPerHour) { outputSpeed = (inputSpeed / 1609.344 * 3600).ToString(); }
            else { return ""; }

            return outputSpeed + getSpeedStringUnit();
        }

        string getSpeedStringUnit()
        {
            if (SpeedUnit == speedUnit.metersPerSecond) { return "m/s"; }
            else if (SpeedUnit == speedUnit.kmPerHour) { return "km/h"; }
            else if (SpeedUnit == speedUnit.milesPerHour) { return "miles/h"; }
            else { return null; }
        }


        private void Slider_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            updateSpeedUnit();
        }
        private void Slider_PointerMoved(object sender, RangeBaseValueChangedEventArgs e)
        {
            updateSpeedUnit();
        }



    }
}
