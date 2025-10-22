using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyServer
{
    class Program
    {
        static void Main(string[] args, TcpUserServer server)
        {
            server.Start();
        }
    }
}

