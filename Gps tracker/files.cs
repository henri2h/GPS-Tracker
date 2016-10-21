using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;

namespace Gps_tracker
{
    public class files
    {
        public static string getTempFile()
        {
            string file = Path.GetTempPath();
            return file + "current.gpx";
        }
        // save tempfile
        public async static void saveGPXTempFile(locator GPSLocator)
        {
            try
            {
                if (GPSLocator.track == null) { System.Diagnostics.Debug.WriteLine("error, the track file doesn't exist"); }
                if (!File.Exists(MainPage.tempFile))
                {
                    File.Create(MainPage.tempFile);
                }
                StorageFile file = await StorageFile.GetFileFromPathAsync(MainPage.tempFile);

                await FileIO.WriteTextAsync(file, gpx.generateGPXOutput(GPSLocator.track));
                System.Diagnostics.Debug.WriteLine("saved temporary file");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error");
                System.Diagnostics.Debug.WriteLine(ErrorMessage.getErrorString(ex));
            }
        }
        public static async void saveFile(string extension, string content)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { extension });
            savePicker.SuggestedFileName = "error";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                await FileIO.WriteTextAsync(file, content);
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    MainPage.messageBox("File " + file.Name + " was saved.");
                    Console.WriteLine("File " + file.Name + " was saved.");
                }
                else
                {
                    MainPage.messageBox("File " + file.Name + " couldn't be saved.");
                    Console.WriteLine("File " + file.Name + " couldn't be saved.");
                }
            }
            else
            {
                MainPage.messageBox("Operation cancelled.");
                Console.WriteLine("Operation cancelled");
            }
        }
        public static async void choose(locator GPXLocator)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Plain Text", new List<string>() { ".gpx" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "gps-track";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                await FileIO.WriteTextAsync(file, gpx.generateGPXOutput(GPXLocator.track));
                // Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status == FileUpdateStatus.Complete)
                {
                    MainPage.output = "File " + file.Name + " was saved.";
                }
                else
                {
                    MainPage.output = "File " + file.Name + " couldn't be saved.";
                }
            }
            else
            {
                MainPage.output = "Operation cancelled.";
            }
            MainPage.mainpage.unThreadUpdateUITextElement();
        }
    }
}
