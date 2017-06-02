using System;
using System.AppCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Gps_tracker
{
    public class ErrorMessage
    {
        public static string GetErrorString(Exception ex)
        {
            StringBuilder output = new StringBuilder();

            output.AppendLine("New error : ");
            output.AppendLine("Message : " + ex.Message);
            output.AppendLine("Inner exception : " + ex.InnerException);
            output.AppendLine("Source : " + ex.Source);
            output.AppendLine("Data : " + ex.Data);
            output.AppendLine("Stack Trace : " + ex.StackTrace);

            return output.ToString();
        }
        public static void PrintOut(Exception ex, string customMessage = "")
        {
            // add a way to print custom messages
            if(customMessage != ""){
                Console.WriteLine(customMessage);
            }
            string err = GetErrorString(ex);
            Console.WriteLine(err);
            string tempFile = Files.GetErrorTempFile();

            File.AppendAllText(tempFile, err);
            //  files.saveFile(".err", err + Environment.NewLine + tempFile);
        }
        public static void SaveOut(Exception ex)
        {
            //just to save without printing the error
            string err = GetErrorString(ex);
            string tempFile = Files.GetTempFile(".err");

            File.AppendAllText(tempFile, err);
            //  files.saveFile(".err", err + Environment.NewLine + tempFile);
        }

        public static void SendErrorMessages()
        {
            StringBuilder sb = new StringBuilder();
            string tempDir = Files.GetTempDir();
            string[] localFiles = Directory.GetFiles(tempDir);
            foreach (string localFile in localFiles)
            {
                sb.AppendLine("==========================");
                sb.AppendLine("New File = ");
                sb.AppendLine("===========");
                sb.AppendLine("Name : [" + localFile + "]");

                try { sb.AppendLine(File.ReadAllText(localFile)); }
                catch { sb.AppendLine("Cannot read the file"); }

                sb.AppendLine("==========================");

                sb.AppendLine();
            }
            var _ = TCPClient.SocketClient.WriteLine(sb.ToString());
        }
    }
}
