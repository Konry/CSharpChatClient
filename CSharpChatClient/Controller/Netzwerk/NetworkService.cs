using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CSharpChatClient
{
    public class NetworkService
    {
        TcpMessageClient tcpClient = null;
        TcpMessageServer tcpServer = null;
        BroadcastReceiver broadReceiver = null;
        BroadcastSender broadSender = null;
        private int tcpPort = -1;
        private User user = null;

        public NetworkService(User user)
        {
            this.user = user;
            Initialize();
        }

        private void Initialize()
        {
            Debug.WriteLine("Initialize Messagservice TcpMessageServer");
            tcpServer = new TcpMessageServer();
            Debug.WriteLine("initialize Messagservice TcpMessageClient");
            tcpClient = new TcpMessageClient();
            Debug.WriteLine("TestSelfconnect");
            TestSelfconnect();

            tcpClient.Send(tcpClient.client, "Test");

            broadReceiver = new BroadcastReceiver();

            broadSender = new BroadcastSender(user, tcpPort);
            broadSender.Start();
        }

        public void TestSelfconnect()
        {
            tcpClient.ConnectToIp(IPAddress.Parse("192.168.23.110"), Configuration.PORT_TCP[1]);
        }

        public void Send(User fromUser, User toUser, string message)
        {
            /* is there already an open port to the */

        }

        private string buildTCPMessage(User fromUser, User toUser, string message)
        {
            return "FROM:" + fromUser.name + ";TO:" + toUser.name + ";Mess:" + message;
        }
    }
}