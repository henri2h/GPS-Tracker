using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gps_tracker.AppCore
{
    public class Logger
    {

        static String LogPath => Path.Combine(AppCore.Core.localFolder.Path, "logs");

        public static void LogLine(String subject, String content)
        {
            try
            {
                content += Environment.NewLine;

                if (!Directory.Exists(LogPath)) Directory.CreateDirectory(LogPath);
                String fileName = Path.Combine(LogPath, subject + ".log");
                if (!File.Exists(fileName)) File.Create(fileName);
                File.AppendAllText(fileName, content);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ErrorMessage.GetErrorString(ex));
            }
        }
        public static void LogMain(String content)
        {
            LogLine("program", content);
        }

        public static void CleanLogs()
        {
            try
            {
                if (Directory.Exists(LogPath))
                {
                    String NewDir = GetTempFile("OldLogs_");
                    Directory.CreateDirectory(NewDir);
                    foreach (String path in Directory.GetFiles(LogPath))
                    {
                        string dir = Path.Combine(NewDir, Path.GetFileName(path));
                        File.Move(path, dir);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ErrorMessage.GetErrorString(ex));
            }

        }


        public static string GetTempFile(string name)
        {
            int version = 0;
            while (Directory.Exists(Path.Combine(LogPath, name + version)))
            {
                version++;
            }
            return Path.Combine(LogPath, name + version);
        }

        internal static void DeleteErrors()
        {
            String p = Path.Combine(AppCore.Core.localFolder.Path, "errors");
            if (Directory.Exists(p)) {
                foreach (String f in Directory.EnumerateFiles(p))
                {
                    try { File.Delete(f); }
                    catch
                    {
                        LogMain("Could not delete " + f);
                    }
                }
            }
        }
        internal static void DeleteLogs()
        {
            String p = Path.Combine(AppCore.Core.localFolder.Path, "logs");
            if (Directory.Exists(p))
            {
                foreach (String f in Directory.EnumerateFiles(p))
                {
                    try { File.Delete(f); }
                    catch
                    {
                        LogMain("Could not delete " + f);
                    }
                }
            }
        }

    }
}
