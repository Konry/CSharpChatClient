using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient.Model
{
    class UserConnection
    {
        public User user
        {
            get; set;
        }

        public Socket socket
        {
            get; set;
        }

        public UserConnection(User user, Socket socket)
        {
            this.user = user;
            this.socket = socket;
        }
    }
}
