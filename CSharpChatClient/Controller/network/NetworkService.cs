using CSharpChatClient.controller;
using CSharpChatClient.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

        LinkedList<UserConnection> connectionList = null;

        //private UserList onlineUserList = null;
        private ProgramController control = null;

        public NetworkService(ProgramController control)
        {
            this.control = control;
            Initialize();
        }

        private void Initialize()
        {
            FillNetworkConfiguration();
            if (Configuration.localUser == null)
            {
                Configuration.localUser = new User("");
            }

            tcpServer = new TcpMessageServer();
            tcpClient = new TcpMessageClient();
            //TestSelfconnect();

            //tcpClient.Send("Test");

            broadReceiver = new BroadcastReceiver();
            broadSender = new BroadcastSender();
            broadSender.Start();
        }

        public void Send(Message message)
        {
            /* is there already an open port to the */
            foreach(UserConnection uc in connectionList)
            {
                //if()
            }
        }

        private string buildTCPMessage(User fromUser, User toUser, string message)
        {
            return "FROM:" + fromUser.name + ";TO:" + toUser.name + ";Mess:" + message;
        }

        private void FillNetworkConfiguration()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = GetLocalIPAddress(ipHostInfo);
            Configuration.localIpAddress = ipAddress;
            Configuration.selectedTcpPort = Configuration.PORT_TCP[GetAvaiableTCPPortIndex()];
        }

        internal void SendMessage(Message text)
        {

        }

        private IPAddress GetLocalIPAddress(IPHostEntry host)
        {
            int index = 0;
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
                index++;
            }
            throw new Exception("Local IP Address Not Found!");
        }

        private int GetAvaiableTCPPortIndex()
        {
            int index;
            for (index = 0; index < Configuration.PORT_TCP.Length; index++)
            {
                if (checkTcpPortAvaibility(Configuration.PORT_TCP[index]))
                {
                    break;
                }
            }
            return index;
        }

        private bool checkTcpPortAvaibility(int port)
        {
            bool isAvailable = true;

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnectionInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnectionInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }
            if (isAvailable)
            {
                Debug.WriteLine("Port: " + port + " is selected");
            }
            return isAvailable;
        }

        internal static void IncomingMessageFromServer(Message content)
        {
            throw new NotImplementedException();
        }
    }
}