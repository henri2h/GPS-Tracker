using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Windows.Storage.Streams;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.Security.Cryptography;
using System.AppCore;

namespace Gps_tracker
{
    class TCPClient
    {
        public class SocketClient
        {
            static StreamSocket socket;
            static bool isEnabled = false;
            public static string host;
            public static bool isUsed = false;

            public static async Task Connect(string hostG)
            {
                try
                {
                    host = hostG;
                    string port = "78";

                    socket = new StreamSocket();
                    HostName hostName = new HostName(host);


                    // Connect to the server
                    await socket.ConnectAsync(hostName, port);
                    Console.WriteLine("Connected to : " + socket.Information.RemoteHostName.DisplayName);
                    isEnabled = true;

                    await WriteLine("Connected");
                }
                catch (Exception exception)
                {
                    exception.Source = "TCPClient.connect";
                    isEnabled = false;
                    ErrorMessage.printOut(exception);
                }

            }
            public static async Task WriteLine(string text)
            {
                try
                {
                    if (isEnabled)
                    {
                        while (isUsed)
                        {
                            System.Diagnostics.Debug.WriteLine("Used");
                        }

                        isUsed = true;
                        DataWriter writer = new DataWriter(socket.OutputStream)
                        {
                            UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8,
                            ByteOrder = ByteOrder.LittleEndian
                        };
                        text += Environment.NewLine;

                        uint size = writer.MeasureString(text);
                        //  writer.WriteUInt32(size);
                        uint storedUint = await writer.StoreAsync();

                        writer.WriteString(text);
                        uint end = await writer.StoreAsync();

                        await writer.FlushAsync();
                        writer.DetachBuffer();
                        writer.DetachStream();
                        writer.Dispose();
                        isUsed = false;
                    }
                    else { }
                }
                catch (Exception ex)
                {
                    ex.Source = "TCPClient.WriteLine";
                    isEnabled = false;
                    ErrorMessage.printOut(ex);
                }
            }
        }


    }
}

