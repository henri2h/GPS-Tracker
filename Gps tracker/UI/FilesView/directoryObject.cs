using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gps_tracker.UI.FilesView
{
    public class directoryObject
    {
        public string name { get; set; }
        public string path { get; set; }
        public bool isDirectory { get; set; }
        public DateTime lastAccess { get; set; }
    }
}
