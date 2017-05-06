using Gps_tracker.AppCore;
using Gps_tracker.UI.FilesView;
using System;
using System.AppCore;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Gps_tracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Information informations = new Information();
        timer time;

        public MainPage()
        {
            try
            {
                this.InitializeComponent();


                // intitalisation
                time = new timer();
                if (AppCore.Core.GPSLocator == null)
                {

                    AppCore.Core.GPSLocator = new locator(this);
                    AppCore.Core.GPSLocator.startLocalisation();
                }
                AppCore.Core.setTempFile();


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
                if (AppCore.Core.GPSLocator.global != null)
                {
                    UIMapView.centerMap(AppCore.Core.GPSLocator.global.Coordinate.Point);
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
            if (Core.GPSLocator.recordingLocalisation == false)
            {
                informations.startTravelTime = DateTime.Now;
                time.start(Core.GPSLocator);
                Core.GPSLocator.recordingLocalisation = true;
                btStart.Content = "Stop recording the track";
            }
            else
            {
                informations.endTravelTime = DateTime.Now;
                informations.totalTravelDistance = 0;
                time.stop();
                Core.GPSLocator.recordingLocalisation = false;
                btStart.Content = "Start recording the track";
            }
            updateUITextElements();
        }
        //save
        private void btSave_Click(object sender, RoutedEventArgs e) { files.choose(this, Core.GPSLocator); }

        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            updateUITextElements();
        }

        public void updateUITextElements()
        {
            UITbInformations.updateUIInformations(informations);
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

        public void unThreadUpdateUIMap()
        {
            try
            {
                var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { UIMapView.updateMap(Core.GPSLocator.track.ToArray()); });
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






        private void btMapupdate_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Map Updating : Size of track : " + Core.GPSLocator.track.ToArray().Length);
            UIMapView.updateUIAllMap(Core.GPSLocator.track.ToArray());
        }

        private void UITbSettings_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            bool canChange = rootFrame.Navigate(typeof(SettingsView));
        }

        private void UITbFiles_Click(object sender, RoutedEventArgs e)
        {

            Frame rootFrame = Window.Current.Content as Frame;
            bool canChange = rootFrame.Navigate(typeof(FileView));
        }
    }


}
