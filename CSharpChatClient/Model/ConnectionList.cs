using System.Net.Sockets;

namespace CSharpChatClient.Model
{
    /// <summary>
    /// Connects an external user with an open socket to the external user.
    /// </summary>
    public class UserConnection
    {
        private ExtendedUser user;
        private Socket socket;

        public UserConnection(ExtendedUser user, Socket socket)
        {
            this.user = user;
            this.socket = socket;
        }

        public ExtendedUser User
        {
            get { return user; }
            set { user = value; }
        }

        public Socket Socket
        {
            get { return socket; }
            set { socket = value; }
        }
    }
}