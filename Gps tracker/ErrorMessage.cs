using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gps_tracker
{
    public class ErrorMessage
    {
        public static string getErrorString(Exception ex)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine("Message : " + ex.Message);
            output.AppendLine("Inner exception" + ex.InnerException);
            output.AppendLine("Source : " + ex.Source);
            output.AppendLine("Data : " + ex.Data);
            output.AppendLine("Stack Trace : " + ex.StackTrace);

            return output.ToString();
        }
    }
}
