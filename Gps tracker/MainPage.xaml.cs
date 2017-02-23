using Gps_tracker.AppCore;
using System;
using System.AppCore;
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
        public Information informations = new Information();
        timer time;
        public string tempFile = "";

        //UI
        public double? maxSpeed = 0;
        double mediumSpeed = 0;

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


                // intitalisation
                time = new timer();
                GPSLocator = new locator(this);
                Console.page = this;

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
        private void btCenter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GPSLocator.global != null)
                {
                    UIMapView.centerMap(GPSLocator.global.Coordinate.Point);
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
      
        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateUITextElements();
        }

        public void updateUITextElements()
        {
            UITbInformations.updateUIInformations(GPSLocator.currentPoint, totalDistance, date.ToString(), output, LocatorStatus, GPSLocator.currentPoint.speed, mediumSpeed, maxSpeed);
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




        public void unThreadUpdateUIMap()
        {
            try
            {
                var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { UIMapView.updateMap(GPSLocator.track.ToArray()); });
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.unThreadUpdateUIMap";
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
                    ConsoleView.WriteLine(text);
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
            UIMapView.updateUIAllMap(GPSLocator.track.ToArray());
        }

        private void UITbSettings_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            bool canChange = rootFrame.Navigate(typeof(SettingsView));
        }
    }


}
