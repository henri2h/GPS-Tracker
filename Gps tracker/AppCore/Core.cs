using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Gps_tracker.AppCore
{
    public class Core
    {
        const string settingFileName = "settings.set";
        public static Setting settings { get; set; }
        public static string tempFile { get; set; }

        public static Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        public static Windows.Storage.StorageFolder tempFolder = Windows.Storage.ApplicationData.Current.TemporaryFolder;

        public static void startApp()
        {
            loadSettings();
        }

        public static async void loadSettings()
        {
            StorageFile file = await localFolder.GetFileAsync(settingFileName);
            if (file != null)
            {
                string content = await FileIO.ReadTextAsync(file);
                settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Setting>(content);
            }
            else
            {
                settings = new Setting();
                settings.SpeedUnit = speedUnit.metersPerSecond;
                settings.autoSave = true;
                settings.enhancedMode = true;

                saveSettings();
            }

        }

        public static async void saveSettings()
        {
            string text = Newtonsoft.Json.JsonConvert.SerializeObject(settings);
            StorageFile file = await localFolder.GetFileAsync(settingFileName);
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, text);
            }
        }

        public static void connectRemoteDebug()
        {
            // this is in order to debug the client remotely
            var _ = TCPClient.SocketClient.connect("10.0.0.3");
        }
        public static void getMemoryUsage()
        {
            ulong memUsage = Windows.System.MemoryManager.AppMemoryUsage;
        }

    }

}
