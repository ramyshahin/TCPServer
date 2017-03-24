using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCPServer
{
    class Server
    {
        private Int32 serverPort = 55555;
        private TcpListener listener;

        // server state: counters
        private int handshakeCount;

        public void Handshake()
        {
            Interlocked.Increment(ref handshakeCount);
        }

        public int GetHandshakeCount()
        {
            return handshakeCount;
        }

        protected async Task StartSession(TcpClient client)
        {
            Session session = new Session(client, this);
            Task.Run(() => session.Start());
        }

        protected async void ServerLoop()
        {
            while (true)
            {
                Console.WriteLine("starting");
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    await StartSession(client);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e);
                }
                Console.WriteLine("---");
            } // while
        }

        public void Start()
        {
            
            try {
                listener.Start();

                ServerLoop();
            } catch (Exception e) {
                Console.WriteLine("Exception: " + e);
            } finally {
                Console.WriteLine("stopping");
                listener.Stop();
            }
        }

        public Server()
        {
            try {
                // listening on all localhost network interfaces if more
                // than one exist
                listener = new TcpListener(IPAddress.Any, serverPort);
            } catch (Exception e) {
                Console.WriteLine("TcpListener construction failed: " + e);
            }
        }
    }
}
