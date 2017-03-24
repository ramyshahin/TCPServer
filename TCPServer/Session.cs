using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace TCPServer
{
    // A single client-server session
    class Session
    {
        private TcpClient client;
        private Server server;

        private bool handshakeEst = false;
        private bool done = false;

        public Session(TcpClient c, Server s)
        {
            client = c;
            server = s;
#if DEBUG
            Console.WriteLine("New session");
#endif
        }

        ~Session()
        {
            client.Close();
        }

        protected Byte[] Str2Bytes(String s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

        protected String Bytes2Str(Byte[] bytes, Int32 count)
        {
            return Encoding.ASCII.GetString(bytes, 0, count);
        }

        protected void HandleCommand(String cmd, NetworkStream clientStream)
        {
            // before handshake is established, all other commands are invalid
            if (cmd != "HELO" && !handshakeEst)
            {
                SendReply("ERROR: HANDSHAKE REQUIRED", clientStream);
                return;
            }

            switch (cmd)
            {
                case "HELO":    // a single 'L' in the specs
                    server.Handshake();
                    handshakeEst = true;
                    SendReply("HI", clientStream);
                    break;
                case "COUNT":
                    SendReply(server.GetHandshakeCount().ToString(), clientStream);
                    break;
                case "CONNECTIONS":
                    SendReply(server.GetConnectionCount().ToString(), clientStream);
                    break;
                case "TERMINATE":
                    server.ConnectionClosed();
                    done = true;
                    SendReply("BYE", clientStream);
                    break;
                default:
                    SendReply("INVALID COMMAND", clientStream);
                    break;
            }
        }

        protected void SendReply(String reply, NetworkStream clientStream)
        {
            Byte[] bytes = Str2Bytes(reply);
            clientStream.Write(bytes, 0, bytes.Length);
        }

        public void Start()
        {
            // longest client command is 11 characters (22 bytes long)
            const int BUF_SIZE = 32;
            Byte[] buffer = new Byte[BUF_SIZE];

            NetworkStream clStream = client.GetStream();

            bool retrying = false;
            while (!done)
            {
                int bytesRead;
                try
                {
                    bytesRead = clStream.Read(buffer, 0, BUF_SIZE);
                }
                catch (Exception e)
                {
                    if (!retrying)
                    {
                        // retry once
                        retrying = true;
                    } else
                    {
                        // connection lost
                        done = true;
                    }
                    continue;
                }

                // binary to ASCII conversion (all commands are in ASCII,
                // so there is no need for UNICODE encoding
                String command = Bytes2Str(buffer, bytesRead);
                
#if DEBUG
                Console.WriteLine(command);
#endif

                HandleCommand(command, clStream);
            }

            clStream.Close();
        }
    }
}
