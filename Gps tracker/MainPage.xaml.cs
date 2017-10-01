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
        Timer time;

        public MainPage()
        {
            try
            {
                this.InitializeComponent();

                Core.page = this;
                // intitalisation
                time = new Timer();

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
                ErrorMessage.PrintOut(ex);
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
                ErrorMessage.PrintOut(ex);
            }
        }
        //start
        private void BtStart_Click(object sender, RoutedEventArgs e)
        {
            if (Core.GPSLocator.recordingLocalisation == false)
            {
                Core.informations.startTravelTime = DateTime.Now;
                time.Start(Core.GPSLocator);
                Core.GPSLocator.recordingLocalisation = true;
            }
            else
            {
                Core.informations.endTravelTime = DateTime.Now;
                Core.informations.totalTravelDistance = 0;
                time.Stop();
                Core.GPSLocator.recordingLocalisation = false;
            }
            UpdateUITextElements();
        }

        void UpdateRecordingButton()
        {
            if (Core.GPSLocator.recordingLocalisation ==true)
            {
                UIAppBtRecord.Label = "Stop recording the track";
                UIAppBtRecord.Icon = new SymbolIcon(Symbol.Stop);
            }
            else
            {
                UIAppBtRecord.Label = "Start recording the track";
                UIAppBtRecord.Icon = new SymbolIcon(Symbol.Play);
            }
        }


        //save
        private void BtSave_Click(object sender, RoutedEventArgs e)
        {
            // UIMapView.UpdateUIAllMap(Core.GPSLocator.track.ToArray());
            try
            {
                Files.Choose(this, Core.GPSLocator);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not save file");
                ErrorMessage.PrintOut(ex, "Error in saving");
            }
        }

        private void BtUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateUITextElements();
        }

        public void UpdateUITextElements()
        {
            UITbInformations.updateUIInformations(Core.informations);
            UpdateRecordingButton();
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
                ErrorMessage.PrintOut(ex);
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
                ErrorMessage.PrintOut(ex);
            }
        }



        public static async void MessageBox(string msg)
        {
            try
            {
                var msgDlg = new Windows.UI.Popups.MessageDialog(msg)
                {
                    DefaultCommandIndex = 1
                };
                await msgDlg.ShowAsync();
            }
            catch
            {
                Console.WriteLine("Could show dialog");
            }
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
                ErrorMessage.PrintOut(ex);
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
                ErrorMessage.PrintOut(ex);
            }
        }
    }


}
