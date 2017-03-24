using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;
using System.Text;

namespace UnitTests
{
    [TestClass]
    public class TServer
    {
        private String serverIP = "127.0.0.1"; // localhost
        private Int32 serverPort = 55555;
        private TcpClient client;

        protected Byte[] Str2Bytes(String s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

        protected String Bytes2Str(Byte[] bytes, Int32 count)
        {
            return Encoding.ASCII.GetString(bytes, 0, count);
        }

        public TServer()
        {
            client = new TcpClient(serverIP, serverPort);
            client.ReceiveTimeout = 5000; // 5000 milliseconds
        }

        protected String SendCommand(String cmd)
        {
            Assert.AreEqual(client.Connected, true);

            Byte[] bytes = Str2Bytes(cmd);
            NetworkStream clStream = client.GetStream();

            clStream.Write(bytes, 0, bytes.Length);

            const int BUF_LEN = 1024;
            Byte[] buffer = new Byte[BUF_LEN];
            Int32 bytesRead = clStream.Read(buffer, 0, BUF_LEN);

            return Bytes2Str(buffer, bytesRead);
        }

        [TestMethod]
        public void BeforeHandshake()
        {
            string response = SendCommand("COUNT");
            Assert.AreEqual(response, "ERROR: HANDSHAKE REQUIRED");
        }

        [TestMethod]
        public void InvalidCommand()
        {
            string response = SendCommand("HELO");
            Assert.AreEqual(response, "HI");
            response = SendCommand("BLABLABLA");
            Assert.AreEqual(response, "INVALID COMMAND");
        }

        [TestMethod]
        public void Handshake()
        {
            String response = SendCommand("HELO");  // with a single 'L' in the specs
            Assert.AreEqual(response, "HI");
        }

        [TestMethod]
        public void Count()
        {
            string response = SendCommand("HELO");
            Assert.AreEqual(response, "HI");

            response = SendCommand("COUNT");
            int initCount = int.Parse(response);

            response = SendCommand("HELO");
            response = SendCommand("COUNT");
            int nextCount = int.Parse(response);
            Assert.AreEqual(initCount+1, nextCount);
        }
    }
}
