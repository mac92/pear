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
using System.Threading;

namespace Remote
{
    public partial class Server : Form
    {
        public int serverListenPort = 8080;
        public const int sleepTime = 200;
        public IPAddress ipAddress = IPAddress.Any;
        bool server_is_running = false;
        private Thread mainThread;
        List<Client> ClientList;

        public Server()
        {
            InitializeComponent();
            ClientList = new List<Client>();
        }
        delegate void OutputUpdateDelegate(string log_message, int log_type = 0);
        public void update_log(string log_message, int log_type = 0)
        {
            if (richTextBox1.InvokeRequired)
                richTextBox1.Invoke(new OutputUpdateDelegate(OutputUpdateCallback),
                new object[] { log_message, log_type });
            else
                OutputUpdateCallback(log_message, log_type); //call directly
        }

        private void OutputUpdateCallback(string log_message, int log_type = 0)
        {
            string log_type_string = "";
            switch (log_type)
            {
                case 0:
                    log_type_string = "";
                    break;
                case 1:
                    log_type_string = "Error: ";
                    break;
                case 2:
                    log_type_string = "Warning: ";
                    break;
                case 3:
                    log_type_string = "Server: ";
                    break;
            }
            string log_cache = richTextBox1.Text;
            richTextBox1.Text = log_type_string + log_message + "\n";
            richTextBox1.Text += log_cache;
        }

        private void clean_up()
        {
            button1.Enabled = true;
            button1.Text = "Server starten";
            textBox1.Enabled = true;
            server_is_running = false;
            update_log("Server wurde gestoppt.",3);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (server_is_running == true)
            {
                // Stop Server
                clean_up();
            }
            else
            {
                button1.Text = "Server stoppen";
                textBox1.Enabled = false;
                server_is_running = true;
                // Start Server
                // Hauptthread wird instanziiert ...
                try
                {
                    mainThread.Abort();
                }
                catch
                {
                }
                mainThread = new Thread(new ThreadStart(this.start_server));
                // ... und gestartet
                mainThread.Start();
            }
        }

        private void start_server()
        {
            if (serverListenPort != 0)
            {
                TcpListener listener = new TcpListener(ipAddress, serverListenPort);
                try
                {
                    // Verbindungsannahme aktivieren
                    listener.Start();

                    update_log("Server auf Port " + serverListenPort.ToString() + " gestartet", 3);

                    // Warten auf Verbindung
                    update_log("Warte auf neue Verbindung.", 3);
                    while (!listener.Pending()) { Thread.Sleep(sleepTime); }
                    // Verbindung annehmen
                    Socket newSocket = listener.AcceptSocket();
                    // Mitteilung bzgl. neuer Clientverbindung
                    update_log("Neue Client-Verbindung (" +
                                "IP: " + newSocket.RemoteEndPoint + ", " +
                                "Port " + ((IPEndPoint)newSocket.LocalEndPoint).Port.ToString() + ")",3);
                    Client client_connection = new Client(newSocket);
                    ClientList.Add(client_connection);
                }
                catch(Exception exp)
                {
                    update_log(exp.Message, 1);
                }
            }
            else
            {
                update_log("Die Portnummer darf nicht 0 sein.",0);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                serverListenPort = Convert.ToInt32(((TextBox)sender).Text);
            }
            catch (Exception)
            {
                serverListenPort = 0;
                update_log("Der Port muss eine Ganzzahl sein.",0);
            }
        }
    }
}
