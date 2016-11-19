namespace System
{
    internal class Console
    {
        public static Gps_tracker.MainPage page;
        public static bool active = true;
        internal static void WriteLine(string Text)
        {
            if (active)
            {
                System.Diagnostics.Debug.WriteLine("[Console] : " + Text);
                page.WriteLine(Text);
                var _ = Gps_tracker.TCPClient.SocketClient.writeLine(Text);
            }
            else { System.Diagnostics.Debug.WriteLine("Console not enabled : " + Text); }
        }
        internal static void setNewLine(string text)
        {
            string[] args = text.Split(' ');
            if (args[0] == "connect" && args.Length > 1)
            {
                var _ = Gps_tracker.TCPClient.SocketClient.connect(args[1]);
            }
            else if (args[0] == "connect")
            {
                var _ = Gps_tracker.TCPClient.SocketClient.connect(Gps_tracker.TCPClient.SocketClient.host);
            }
            else if (args[0] == "send") { Gps_tracker.ErrorMessage.sendErrorMessages(); }
            else if (args[0] == "exRelaunch") { Gps_tracker.extendedSession.StartLocationExtensionSession(); }
            else
            {
                var _ = Gps_tracker.TCPClient.SocketClient.writeLine(text);
            }

        }
    }
}