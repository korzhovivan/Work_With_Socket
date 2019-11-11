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
using Server.Properties;

namespace Server
{
    class Program
    {
        static DataContext dataContext = null;
        static SqlConnection SQL_conection = null;

        static IPAddress client_ip = null;
        static IPEndPoint client_endPoint = null;
        static Socket client_socket = null;

        static IPAddress server_ip = null;
        static IPEndPoint server_endPoint = null;
        static Socket server_socket = null;


        static void Main(string[] args)
        {
            //Thread for recieve data
            Thread searchThread = new Thread(new ThreadStart(GetAnswer));
            searchThread.IsBackground = true;
            searchThread.Start();

            SQL_conection = new SqlConnection();
            SQL_conection.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            dataContext = new DataContext(SQL_conection);
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
                int recievedIndex_Int;
                string recievedIndex_String;

                try
                {
                    byte[] bytes = new byte[1024];
                    int l = 0;
                    
                        l = client_socket.Receive(bytes);
                        recievedIndex_String = Encoding.UTF8.GetString(bytes, 0, l);
                        recievedIndex_Int = Convert.ToInt32(recievedIndex_String);

                    var select = (from index in dataContext.GetTable<Index>()
                                  where index.Code == recievedIndex_Int
                                  select index).ToList<Index>(); //select streets
                    
                    foreach (var item in select)
                    {
                        Console.WriteLine(item.Street);
                    }

                    Thread searchThread = new Thread(new ParameterizedThreadStart(SendStreets))
                    {
                        IsBackground = true
                    };
                    searchThread.Start(select);

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

        private static void SendStreets(object obj)
        {
            Socket ns = null;

            server_ip = IPAddress.Parse("127.0.0.1");
            server_endPoint = new IPEndPoint(server_ip, 1025);
            server_socket  = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            server_socket.Bind(server_endPoint);
            
            try
            {
                server_socket.Listen(20);
                ns = server_socket.Accept();

                byte[] msg = new byte[1024];
                BinaryFormatter formatter = new BinaryFormatter();

                List<Index> streets = obj as List<Index>;

                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, streets);
                    msg = ms.ToArray();
                    ns.Send(msg);
                    ns.Shutdown(SocketShutdown.Both);
                    ns.Close();
                }



            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
