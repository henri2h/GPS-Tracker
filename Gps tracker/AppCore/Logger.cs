using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gps_tracker
{
    public class Logger
    {
        static String LogPath => Path.Combine(AppCore.Core.localFolder.Path, "logs");

        public static void LogLine(String subject, String content)
        {
            try
            {
                content += Environment.NewLine;

                if (Directory.Exists(LogPath)) { Directory.CreateDirectory(LogPath); }
                String fileName = Path.Combine(LogPath, subject + ".log");
                File.AppendAllText(fileName, content);
            }
            catch { }
        }
        public static void LogMain(String content)
        {
            LogLine("program", content);
        }
    }
}
