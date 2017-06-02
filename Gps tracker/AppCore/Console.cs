using Gps_tracker.UI;
using System.Collections.Generic;
using System.Text;

namespace System.AppCore
{
    public class Console
    {
        public static bool active = true;
        public static List<ConsoleView> consoles = new List<ConsoleView>();
        public static void WriteLine(string Text)
        {
            if (active)
            {
                System.Diagnostics.Debug.WriteLine("[" + DateTime.Now.ToString() + "] : " + "[Console] : " + Text);
                foreach (ConsoleView cView in consoles)
                {
                    cView.WriteLine(Text);
                }
                var _ = Gps_tracker.TCPClient.SocketClient.WriteLine(Text);
            }
            else { System.Diagnostics.Debug.WriteLine("Console not enabled : " + Text); }
        }

        public static void setNewLine(string text)
        {
            string[] args = text.Split(' ');
            if (args[0] == "connect" && args.Length > 1)
            {
                var _ = Gps_tracker.TCPClient.SocketClient.Connect(args[1]);
            }
            else if (args[0] == "help") { showHelp(); }
            else if (args[0] == "connect")
            {
                var _ = Gps_tracker.TCPClient.SocketClient.Connect(Gps_tracker.TCPClient.SocketClient.host);
            }
            else if (args[0] == "send") { Gps_tracker.ErrorMessage.SendErrorMessages(); }
            else if (args[0] == "exRelaunch") { Gps_tracker.ExtendedSession.StartLocationExtensionSession(); }
            else
            {
                var _ = Gps_tracker.TCPClient.SocketClient.WriteLine(text);
            }

        }
        static void showHelp()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("===============");
            sb.AppendLine("Console Help : ");
            sb.AppendLine("===============");
            sb.AppendLine(" -- connect : connect to the debug server");
            sb.AppendLine(" -- connect <hostname> : connect to the debug server with the specified ip");
            sb.AppendLine(" -- send : send the errors messages to the client");
            sb.AppendLine(" -- exRelauch : relaunch the extended session");
            WriteLine(sb.ToString());
        }
    }
}