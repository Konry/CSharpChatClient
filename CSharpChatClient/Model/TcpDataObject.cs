using System.Net.Sockets;

namespace CSharpChatClient
{
    public class TcpDataObject
    {
        // Client socket.
        public Socket workSocket = null;

        // Size of receive buffer.
        public const int BufferSize = 4096;

        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        //public StringBuilder sb = new StringBuilder();
    }
}