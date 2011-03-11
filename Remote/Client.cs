using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Remote
{
    class Client
    {
        Socket socket;
        const int SleepTime = 200;
        public Thread serverThread;

        public Client(Socket client_socket)
        {
            this.socket = client_socket;
            // Thread erzeugen
            serverThread = new Thread(new ThreadStart(Process));
            serverThread.Start();
        }
        void closeConnection()
        {
            serverThread.Abort();
            socket.Close();
            socket = null;
        }
        public void send(String message)
        {

        }
        public void recieve(Byte[] message)
        {

        }

        private void Process()
        {
            try
            {
                MemoryStream mem = new MemoryStream();// Empfangspuffer
                byte[] buffer = new byte[BufferSize];
                int TimeOut = 0;
                // 10 Sekunden Timeout
                while (TimeOut < (10 * 1000 / SleepTime))
                {
                    mem.Seek(0, SeekOrigin.Begin);
                    mem.SetLength(0);
                    while (socket.Available > 0)
                    {
                        //Byte[] buffer = new byte[bytesAvailable];
                        int bytesRead = socket.Receive(buffer, buffer.Length, SocketFlags.None);
                        if (bytesRead <= 0) continue;
                        mem.Write(buffer, 0, bytesRead);
                        // Alles zurücksetzen
                    }
                    if (mem.Length > 0)
                    {
                        if (mem.Length == 4)
                            if (System.Text.Encoding.ASCII.GetString(mem.ToArray(), 0, 4) == "quit")
                            {
                                closeConnection();
                                break;
                            }
                        // string message_back = mem.ToArray();
                        mem.Seek(0, SeekOrigin.Begin);
                        mem.SetLength(0);
                        TimeOut = 0;
                    }
                    else
                    {
                        TimeOut++;
                        Thread.Sleep(SleepTime);
                    }
                }
                closeConnection();
            }
            catch
            {

            }
        }

        public int BufferSize { get; set; }
    }
}
