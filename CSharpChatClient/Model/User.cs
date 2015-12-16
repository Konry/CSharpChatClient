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
        public long id { get; set; }
        //public IPAddress ipAddress { get; set; }
        //public int port { get; set; }

        public User(string name)
        {
            this.name = name;
            this.id = -1;
            //this.ipAddress = IPAddress.Parse("127.0.0.1");
            //this.port = Configuration.DEFAULT_TCP_PORT;
        }

        //public User(string name, IPAddress ipAddress)
        //{
        //    this.name = name;
        //    //this.ipAddress = ipAddress;
        //    //this.port = Configuration.DEFAULT_TCP_PORT;
        //}

        //public User(string name, IPAddress ipAddress, int port)
        //{
        //    this.name = name;
        //    this.ipAddress = ipAddress;
        //    this.port = port;
        //}

        public bool Equals(User user)
        {
            if(this.name.Equals(user.name) && this.id.Equals(user.id))
            {
                return true;
            }
            return false;
        }
    }
}