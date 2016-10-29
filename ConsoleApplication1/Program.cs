using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("============================");
                Console.WriteLine("     New socket server : ");
                Console.WriteLine("============================");
                TCPServer tc = new TCPServer();
                tc.listen();
                Console.WriteLine("Server created : ");
                while (true)
                {
                    // string output = "[" + DateTime.Now.ToShortTimeString() + "] : " + tc.readData();
                    string output = tc.readData();
                    Console.Write(output);
                    File.AppendAllText(@"D:\consoleOut.txt", output);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error happend :");
                Console.WriteLine(ex.Message);
                Console.WriteLine(Environment.NewLine);

            }
        }
    }
}
