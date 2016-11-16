using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace ConsoleApplication1
{
    public class TCPServer
    {
        Socket soc;
        TcpListener tcpListener;
        public void listen()
        {
            System.Net.IPEndPoint end = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 78);
            tcpListener = new TcpListener(end);

            StreamSocketListener l = new StreamSocketListener();

            tcpListener.Start();
            Console.WriteLine("Socket listener started");

            soc = tcpListener.AcceptSocket();

            Console.WriteLine("Client connected : " + soc.RemoteEndPoint.AddressFamily.ToString());

        }

        public string readData()
        {
            bool noData = false;

            StringBuilder sb = new StringBuilder();

            byte[] sizeBuff = new byte[4];
            //int size = soc.Receive(sizeBuff);



            //     uint toRead = BitConverter.ToUInt32(sizeBuff, 0);
            uint toRead = 800;
            if (toRead != 0)
            {
                noData = false;


                uint mainBuffSize = toRead / 8;
                uint rest = toRead % 8;



                if (rest != 0) mainBuffSize += 1;
                //buffersSize.Add(rest);

                Debug.Write("[" + DateTime.Now.ToShortTimeString() + "] : " + toRead.ToString() + " ; " + mainBuffSize + "|" + rest + " : ");


                byte[] buff = new byte[mainBuffSize];
                int read = soc.Receive(buff, Convert.ToInt32(mainBuffSize), SocketFlags.None);

                Debug.Write("notRead : " + (Convert.ToInt32(mainBuffSize) - read) + " : ");

                byte[] endB = new byte[read];
                for (int i = 0; i < read; i++)
                {
                    endB[i] = buff[i];
                    Debug.Write(endB[i] + " ");
                }

                Debug.WriteLine("");

                string end = UTF8Encoding.UTF8.GetString(endB);
                sb.Append(end);


                return sb.ToString();

            }
            else
            {
                if (!noData) Console.WriteLine("No data");
                noData = true;
                return "";
            }


        }
    }
}

