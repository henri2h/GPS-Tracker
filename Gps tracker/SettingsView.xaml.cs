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
            loadSettings();
        }
        public void loadSettings()
        {
            UISwitchExtended.IsOn = AppCore.Core.settings.enhancedMode;
            UISwitchSaving.IsOn = AppCore.Core.settings.autoSave;
            UITbHostname.Text = AppCore.Core.settings.Hostname;
        }
        public void saveSettings()
        {
            AppCore.Core.settings.enhancedMode = UISwitchExtended.IsOn;
            AppCore.Core.settings.autoSave = UISwitchSaving.IsOn;
            AppCore.Core.settings.Hostname = UITbHostname.Text;
            AppCore.Core.SaveSettings();
        }

        private void UITbSave_Click(object sender, RoutedEventArgs e)
        {
            saveSettings();

            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.GoBack();
        }

        private void UIBtConnect_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
