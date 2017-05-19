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
        timer time;

        public MainPage()
        {
            try
            {
                this.InitializeComponent();

                Core.page = this;
                // intitalisation
                time = new timer();

                if (Core.informations == null)
                {
                    Core.informations = new Information();
                }

                if (AppCore.Core.GPSLocator == null)
                {
                    AppCore.Core.GPSLocator = new Locator();
                    AppCore.Core.GPSLocator.StartLocalisation();
                }
                AppCore.Core.SetTempFile();

                if (AppCore.Core.GPSLocator.global != null)
                {
                    UIMapView.CenterMap(AppCore.Core.GPSLocator.global.Coordinate.Point);
                }

                UpdateUITextElements();

            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.constructor";
                ErrorMessage.printOut(ex);
            }
        }

        //intialisation
        private void BtCenter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AppCore.Core.GPSLocator.global != null)
                {
                    UIMapView.CenterMap(AppCore.Core.GPSLocator.global.Coordinate.Point);
                }
            }
            catch (Exception ex)
            {
                ex.Source = "MaiPage.btCenter_Click";
                ErrorMessage.printOut(ex);
            }
        }
        //start
        private void BtStart_Click(object sender, RoutedEventArgs e)
        {
            if (Core.GPSLocator.recordingLocalisation == false)
            {
                Core.informations.startTravelTime = DateTime.Now;
                time.start(Core.GPSLocator);
                Core.GPSLocator.recordingLocalisation = true;
                UIAppBtRecord.Label = "Stop recording the track";
                UIAppBtRecord.Icon = new SymbolIcon(Symbol.Stop);
            }
            else
            {
                Core.informations.endTravelTime = DateTime.Now;
                Core.informations.totalTravelDistance = 0;
                time.stop();
                Core.GPSLocator.recordingLocalisation = false;
                UIAppBtRecord.Label = "Start recording the track";
                UIAppBtRecord.Icon = new SymbolIcon(Symbol.Play);
            }
            UpdateUITextElements();
        }
        //save
        private void BtSave_Click(object sender, RoutedEventArgs e)
        {

            UIMapView.UpdateUIAllMap(Core.GPSLocator.track.ToArray());
            Files.Choose(this, Core.GPSLocator);
        }

        private void BtUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateUITextElements();
        }

        public void UpdateUITextElements()
        {
            UITbInformations.updateUIInformations(Core.informations);
        }

        //============ UI ===============
        public void UnThreadUpdateUITextElement()
        {
            try
            {
                var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { UpdateUITextElements(); });
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.unThreadUpdateUITextElement";
                ErrorMessage.printOut(ex);
            }
        }

        public void UnThreadUpdateUIMap()
        {
            try
            {
                var _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { UIMapView.UpdateMap(Core.GPSLocator.track.ToArray()); });
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.unThreadUpdateUIMap";
                ErrorMessage.printOut(ex);
            }
        }



        public static async void MessageBox(string msg)
        {
            var msgDlg = new Windows.UI.Popups.MessageDialog(msg)
            {
                DefaultCommandIndex = 1
            };
            await msgDlg.ShowAsync();
        }

        private void BtMapupdate_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Map Updating : Size of track : " + Core.GPSLocator.track.ToArray().Length);
            UIMapView.UpdateUIAllMap(Core.GPSLocator.track.ToArray());
        }

        private void UITbSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Frame rootFrame = Window.Current.Content as Frame;
                bool canChange = rootFrame.Navigate(typeof(SettingsView));
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.UITbSettings_Click";
                ErrorMessage.printOut(ex);
            }
        }

        private void UITbFiles_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Frame rootFrame = Window.Current.Content as Frame;
                bool canChange = rootFrame.Navigate(typeof(FileView));
            }
            catch (Exception ex)
            {
                ex.Source = "MainPage.UITbFiles_Click";
                ErrorMessage.printOut(ex);
            }
        }
    }


}
