using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Configuration;

namespace Server
{
    class Program
    {
        static DataContext dataContext = null;
        static SqlConnection SQL_conection = null;

        static IPAddress client_ip = null;
        static IPEndPoint client_endPoint = null;
        static Socket client_socket = null;

        static void Main(string[] args)
        {
            //Thread for recieve data
            Thread searchThread = new Thread(new ThreadStart(GetAnswer));
            searchThread.IsBackground = true;
            searchThread.Start();

            SQL_conection = new SqlConnection();
            SQL_conection.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            Console.ReadKey();
        }

        private static void GetAnswer()
        {
            client_ip = IPAddress.Parse("127.0.0.1");
            client_endPoint = new IPEndPoint(client_ip, 1024);
            client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            client_socket.Connect(client_endPoint);

            try
            {
                
                try
                {
                    byte[] bytes = new byte[1024];
                    int l = 0;
                    do
                    {
                        l = client_socket.Receive(bytes);
                        Console.WriteLine(Encoding.UTF8.GetString(bytes, 0, l));
                    } while (l > 0);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
