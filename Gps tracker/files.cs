﻿using System;
using System.AppCore;
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
        // temp files and dir
        public static string getTempFile(string extention)
        {
            string file = Path.GetTempPath();
            string name = "current";
            int version = 0;
            while (File.Exists(file + name + version + extention))
            {
                version++;
            }
            return file + name + version + extention;
        }
        public static string getTempDir()
        {
            return Path.GetTempPath();
        }


        // save tempfile
        public static bool saveGPXTempFile(locator GPSLocator)
        {
            try
            {
                if (GPSLocator.track == null)
                {

                    Console.WriteLine("[TempFile] : The track file doesn't exist");
                }
                else
                {
                    File.WriteAllText(AppCore.Core.tempFile, gpx.generateGPXOutput(GPSLocator.track));

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
        public static async void saveFile(string extension, string content)
        {
            try
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
            catch (Exception ex)
            {
                ex.Source = "file.saveFile";
                ErrorMessage.printOut(ex);
            }
        }
        public static async void choose(MainPage page, locator GPXLocator)
        {
            try
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
                        page.output = "File " + file.Name + " was saved.";
                    }
                    else
                    {
                        page.output = "File " + file.Name + " couldn't be saved.";
                    }
                }
                else
                {
                    page.output = "Operation cancelled.";
                }
                page.unThreadUpdateUITextElement();
            }
            catch (Exception ex)
            {
                ex.Source = "file.choose";
                ErrorMessage.printOut(ex);
            }
        }
    }
}
