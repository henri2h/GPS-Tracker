using Gps_tracker.AppCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Gps_tracker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : Page
    {
        public SettingsView()
        {
            this.InitializeComponent();
            LoadSettings();
        }
        public void LoadSettings()
        {
            UISwitchExtended.IsOn = Core.Settings.enhancedMode;
            UISwitchSaving.IsOn = Core.Settings.autoSave;
            if (Core.Settings.Hostname != null)
                UITbHostname.Text = Core.Settings.Hostname;
        }
        public void SaveSettings()
        {
            Core.Settings.enhancedMode = UISwitchExtended.IsOn;
            Core.Settings.autoSave = UISwitchSaving.IsOn;
            Core.Settings.Hostname = UITbHostname.Text;
            Core.SaveSettings();
        }

        private void UITbSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();

            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.GoBack();
        }

        private void UIBtConnect_Click(object sender, RoutedEventArgs e)
        {

        }
        private void RetrunToHomeView_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            bool canChange = rootFrame.Navigate(typeof(MainPage));
        }
    }
}
