using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CSharpChatClient
{
    public class User
    {
        protected string _name { get; set; }
        protected long _id { get; set; }
        //public IPAddress ipAddress { get; set; }
        //public int port { get; set; }

        public User(string name)
        {
            this._name = name;
            this._id = -1;
            //this.ipAddress = IPAddress.Parse("127.0.0.1");
            //this.port = Configuration.DEFAULT_TCP_PORT;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public long Id
        {
            get { return _id; }
            set { _id = value; }
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
            if(this._name.Equals(user._name) && this._id.Equals(user._id))
            {
                return true;
            }
            return false;
        }
    }
}