using Server.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PostClient
{
    public partial class Form1 : Form
    {
        //this socket info
        IPAddress client_ip = null;
        IPEndPoint client_endPoint = null;
        Socket client_socket = null;

        List<Index> streets = null;

        //another socket info
        IPAddress server_ip = null;
        IPEndPoint server_endPoint = null;
        Socket server_socket = null;



        public Form1()
        {
            InitializeComponent();

            //To send
            client_ip = IPAddress.Parse("127.0.0.1");
            client_endPoint = new IPEndPoint(client_ip, 1024);
            client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            client_socket.Bind(client_endPoint);
            client_socket.Listen(20);

            

            string[] indexList = new string[]
            {
                "50007",
                "50034",
                "50041",
                "50000",
                "50480",
                "50089",
            };
            comboBox_Indexes.Items.AddRange(indexList);

            
        }
        //Thread for sending data
        private void btn_search_Click(object sender, EventArgs e)
        {
            Thread searchThread = new Thread(new ParameterizedThreadStart(Search))
            {
                IsBackground = true
            };
            searchThread.Start(comboBox_Indexes.SelectedItem.ToString());
            
        }

        private void Search(object index)
        {
            Socket server_socket = null;
            try
            {
                server_socket = client_socket.Accept();
                byte[] msg = new byte[1024];
                
                server_socket.Send(Encoding.UTF8.GetBytes((string)index));
                
                
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void GetAnswer()
        {
            // To recieve
            server_ip = IPAddress.Parse("127.0.0.1");
            server_endPoint = new IPEndPoint(server_ip, 1025);
            server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            server_socket.Connect(server_endPoint);

            try
            {
                byte[] bytes = new byte[1024];
                int len;
                do
                {
                    len = server_socket.Receive(bytes);
                    Array.Resize<byte>(ref bytes, bytes.Length + 1024);

                } while (server_socket.Available > 0);

                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    streets = (List<Index>)formatter.Deserialize(ms);
                    //SetStreets(streets);
                    foreach (Index item in streets)
                    {
                        MessageBox.Show(item.Street);
                    }
                   
                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Thread for recieve data
            Thread searchThread = new Thread(new ThreadStart(GetAnswer));
            searchThread.IsBackground = true;
            searchThread.Start();
        }

        //public void SetStreets(List<Index> streets)
        //{
        //    if (listView_Streets.InvokeRequired)
        //    {
        //        listView_Streets.Invoke(new Action<List<Index>>(SetStreets), streets);
        //    }
        //    else
        //        foreach (Index item in streets)
        //        {
        //            listView_Streets.Items.Add(item.Street);
        //        }
        //}
    }
}
