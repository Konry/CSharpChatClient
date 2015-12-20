using CSharpChatClient.Controller.Network;
using NUnit.Framework;
using System.Net;
using System.Net.Sockets;

namespace CSharpChatClient.Test
{
    [TestFixture]
    internal class TestTcpMessageServer
    {
        [Test]
        public void TestReconnectClient()
        {
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 53333);

            TcpMessageServer server = new TcpMessageServer(null);
            TcpClient client = new TcpClient();
            client.Connect(endpoint);
        }
    }
}