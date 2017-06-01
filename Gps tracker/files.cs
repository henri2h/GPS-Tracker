using Gps_tracker.AppCore;
using System;
using System.AppCore;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;

namespace Gps_tracker
{
    public class Files
    {
        // temp files and dir
        public static string GetTempFile(string extention)
        {
            return GetTempFile("gpxFiles", extention);
        }
        public static string GetErrorTempFile()
        {
            return GetTempFile("errors", ".err");
        }

        public static string GetTempFile(string dir, string extention)
        {
            string directory = Path.Combine(AppCore.Core.localFolder.Path, dir);
            if (Directory.Exists(directory) == false) { Directory.CreateDirectory(directory); }

            string name = "current";
            int version = 0;
            while (File.Exists(Path.Combine(directory, name + version + extention)))
            {
                version++;
            }
            return Path.Combine(directory, name + version + extention);
        }
        // temp files and dir
        public static string GetFileName(string directoryName, string filename, string extention)
        {
            string directory = Path.Combine(AppCore.Core.localFolder.Path, directoryName);
            if (Directory.Exists(directory) == false) { Directory.CreateDirectory(directory); }

            int version = 0;
            while (File.Exists(Path.Combine(directory, filename + version + extention)))
            {
                version++;
            }
            return Path.Combine(directory, filename + version + extention);
        }

        public static string GetTempDir()
        {
            return Path.GetTempPath();
        }


        // save tempfile
        public static bool SaveGPXTempFile(Locator GPSLocator)
        {
            try
            {
                if (GPSLocator.track == null)
                {

                    Console.WriteLine("[TempFile] : The track file doesn't exist");
                }
                else
                {
                    File.WriteAllText(AppCore.Core.TempFile, gpx.generateGPXOutput(GPSLocator.track));
                    File.WriteAllText(AppCore.Core.TempFile + "JSON", Newtonsoft.Json.JsonConvert.SerializeObject(GPSLocator.track));
                    return true;
                }
            }
            catch (FileLoadException fex)
            {
                // we didn't catch this erro because if it happend, it make no difference because we are trying to save a temporary file.
                Console.WriteLine("[TempFile] : " + fex.Message);
                return false;
            }
            return false;
        }
        public static async void SaveFile(string extension, string content)
        {
            try
            {
                FileSavePicker savePicker = new FileSavePicker()
                {
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                };
                // Dropdown of file types the user can save the file as
                savePicker.FileTypeChoices.Add("GPX format", new List<string>() { extension });
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
                        MainPage.MessageBox("File " + file.Name + " was saved.");
                        Console.WriteLine("File " + file.Name + " was saved.");
                    }
                    else
                    {
                        MainPage.MessageBox("File " + file.Name + " couldn't be saved.");
                        Console.WriteLine("File " + file.Name + " couldn't be saved.");
                    }
                }
                else
                {
                    MainPage.MessageBox("Operation cancelled.");
                    Console.WriteLine("Operation cancelled");
                }
            }
            catch (Exception ex)
            {
                ex.Source = "file.saveFile";
                ErrorMessage.printOut(ex);
            }
        }
        public static async void Choose(MainPage page, Locator GPXLocator)
        {
            try
            {
                FileSavePicker savePicker = new FileSavePicker()
                {
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                };
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
                        Core.informations.output = "File " + file.Name + " was saved.";
                        Console.WriteLine("File " + file.Name + " was saved.");
                    }
                    else
                    {
                        Core.informations.output = "File " + file.Name + " couldn't be saved.";
                        Console.WriteLine("File " + file.Name + " couldn't be saved.");
                    }
                }
                else
                {
                    Core.informations.output = "Operation cancelled.";
                    Console.WriteLine("Operation cancelled");
                }
                page.UnThreadUpdateUITextElement();
            }
            catch (Exception ex)
            {
                ex.Source = "file.choose";
                ErrorMessage.printOut(ex);
            }
        }
    }
}
