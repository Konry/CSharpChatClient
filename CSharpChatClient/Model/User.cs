using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CSharpChatClient
{
    public class User
    {
        public string name { get; set; }
        public IPAddress ipAddress { get; set; }
        public int port { get; set; }

        public User(string name)
        {
            this.name = name;
            this.port = Configuration.DEFAULT_TCP_PORT;
        }

        public User(string name, IPAddress ipAddress)
        {
            this.name = name;
            this.ipAddress = ipAddress;
            this.port = Configuration.DEFAULT_TCP_PORT;
        }

        public User(string name, IPAddress ipAddress, int port)
        {
            this.name = name;
            this.ipAddress = ipAddress;
            this.port = port;
        }
    }
}