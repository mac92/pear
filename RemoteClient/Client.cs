using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Security;

namespace Remote_Client
{
    public partial class Client : Form
    {
        public Socket socket = null;

        public Client()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client_connect(textBox1.Text, Convert.ToInt32(textBox2.Text));
        }

        private void client_connect(String Address, int port)
        {
            try
            {
                IPHostEntry hostInfo = Dns.GetHostByName(Address);
                System.Net.IPEndPoint ep = new System.Net.IPEndPoint(hostInfo.AddressList[0], port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ep);
            }
            catch (SecurityException exp)
            {
                MessageBox.Show(exp.Message);
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }
    }
}
