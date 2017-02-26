using System;
using System.AppCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gps_tracker.UI.FilesView
{
    public class FilesManager
    {
        public static List<directoryObject> listFiles(string path)
        {
            List<directoryObject> files = new List<directoryObject>();

            if (path == "" || Directory.Exists(path) == false)
            {
                path = AppCore.Core.localFolder.Path;
            }
            Console.WriteLine("Reading path : " + path);
            path = Path.GetFullPath(path);
            Console.WriteLine("After : " + path);

            foreach (string file in Directory.EnumerateFileSystemEntries(path))
            {
                directoryObject newFile = new directoryObject();

                FileAttributes attr = File.GetAttributes(file);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    newFile.isDirectory = true;
                    newFile.name = Path.GetDirectoryName(file);
                }
                else
                {
                    newFile.isDirectory = false;
                    newFile.name = Path.GetFileName(file);
                }


                newFile.lastAccess = File.GetLastAccessTime(file);
                newFile.path = file;


                files.Add(newFile);
            }
            return files;
        }
    }
}
