﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server:");
            Server server = new Server();
            server.Start();
        }
    }
}
