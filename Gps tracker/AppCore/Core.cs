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
        public static Setting Settings { get; set; }
        public static string TempFile { get; set; }
        public static string SelectedFilePath { get; internal set; }

        public static Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        public static Windows.Storage.StorageFolder tempFolder = Windows.Storage.ApplicationData.Current.TemporaryFolder;
        internal static Locator GPSLocator;
        internal static Information informations;

        public static void StartApp()
        {
            LoadSettings();
        }
        public static void SetTempFile()
        {
            AppCore.Core.TempFile = Files.GetTempFile(".gpx");
            Console.WriteLine("Temp file : " + AppCore.Core.TempFile);
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
                    Settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Setting>(content);
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
            Settings = new Setting()
            {
                SpeedUnit = speedUnit.metersPerSecond,
                autoSave = true,
                enhancedMode = true
            };
            SaveSettings();
        }

        public static void SaveSettings()
        {
            Console.WriteLine("Going to save");
            string text = Newtonsoft.Json.JsonConvert.SerializeObject(Settings);
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

        public static void ConnectRemoteDebug()
        {
            // this is in order to debug the client remotely
            var _ = TCPClient.SocketClient.connect("10.0.0.3");
        }

        public static void GetMemoryUsage()
        {
            ulong memUsage = Windows.System.MemoryManager.AppMemoryUsage;
        }

    }

}
