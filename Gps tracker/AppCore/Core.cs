using System;
using System.AppCore;
using System.Collections.Generic;
using System.IO;
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
        public static string selectedFilePath { get; internal set; }

        public static Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        public static Windows.Storage.StorageFolder tempFolder = Windows.Storage.ApplicationData.Current.TemporaryFolder;
        internal static Locator GPSLocator;
        internal static Information informations;

        public static void startApp()
        {
            LoadSettings();
        }
        public static void setTempFile()
        {
            AppCore.Core.tempFile = Files.GetTempFile(".gpx");
            Console.WriteLine("Temp file : " + AppCore.Core.tempFile);
        }

        public static async void LoadSettings()
        {
            try
            {
                string filePath = Path.Combine(localFolder.Path, settingFileName);
                if (File.Exists(filePath))
                {
                    StorageFile file = await localFolder.GetFileAsync(settingFileName);
                    string content = await FileIO.ReadTextAsync(file);
                    settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Setting>(content);
                    Console.WriteLine("File exist");
                }
                else { CreateSettings(); }

            }
            catch
            {
                CreateSettings();
            }

        }
        static void CreateSettings()
        {
            Console.WriteLine("File didn't exist");
            settings = new Setting()
            {
                SpeedUnit = speedUnit.metersPerSecond,
                autoSave = true,
                enhancedMode = true
            };
            SaveSettings();
        }

        public static async void SaveSettings()
        {
            Console.WriteLine("Going to save");
            string text = Newtonsoft.Json.JsonConvert.SerializeObject(settings);
            string filePath = Path.Combine(localFolder.Path, settingFileName);
            File.WriteAllText(filePath, text);

            /*await localFolder.CreateFileAsync(settingFileName, CreationCollisionOption.ReplaceExisting);
             StorageFile file = await localFolder.GetFileAsync(settingFileName);

             if (file != null)
             {
                 await FileIO.WriteTextAsync(file, text);
             }*/

            Console.WriteLine("Setting saved");
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
