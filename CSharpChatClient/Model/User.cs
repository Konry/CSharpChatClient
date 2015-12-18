using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace CSharpChatClient
{
    public class User
    {
        protected string name { get; set; }
        protected long id { get; set; }

        public User(string name)
        {
            this.name = name;
            this.id = -1;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Check the equality of the user with an other one, depending on their name and their id
        /// </summary>
        /// <param name="user"></param>
        /// <returns>true for equality</returns>
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