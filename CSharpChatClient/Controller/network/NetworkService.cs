using CSharpChatClient.controller;
using CSharpChatClient.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

namespace CSharpChatClient
{
    public class NetworkService
    {
        public static CancellationTokenSource cts;

        TcpMessageClient tcpClient = null;
        TcpMessageServer tcpServer = null;

        BroadcastReceiver broadReceiver = null;
        BroadcastSender broadSender = null;

        private LinkedList<UserConnection> _connectionList = null;

        //private UserList onlineUserList = null;
        private ProgramController control = null;

        private bool _connectionOverClient = false;
        private bool _connectionOverServer = false;
        private Object incomingConnectionLock = new Object();

        public NetworkService(ProgramController control)
        {
            this.control = control;
            Initialize();
        }


        public bool ConnectionOverClient
        {
            get { return _connectionOverClient; }
            set { _connectionOverClient = value; }
        }

        public bool ConnectionOverServer
        {
            get { return _connectionOverServer; }
            set { _connectionOverServer = value; }
        }

        public LinkedList<UserConnection> ConnectionList
        {
            get { return _connectionList; }
            set { _connectionList = value; }
        }

        private void Initialize()
        {
            cts = new CancellationTokenSource();

            ConnectionList = new LinkedList<UserConnection>();

            FillNetworkConfiguration();
            if (Configuration.localUser == null)
            {
                User user = control.fileService.ReadUserCfgFile();
                if (user != null)
                {
                    Configuration.localUser = user;
                }
                else
                {
                    Configuration.localUser = new User("");
                }
            }

            tcpServer = new TcpMessageServer(this);
            tcpClient = new TcpMessageClient(this);

            try
            {
                broadReceiver = new BroadcastReceiver(this);
                broadSender = new BroadcastSender();
            }
            catch (Exception e)
            {

            }
        }

        public void Start()
        {
            try
            {
                broadReceiver.Start();
                broadSender.Start();
            }
            catch (Exception e)
            {

            }
            tcpServer.Start();
        }

        public void Stop()
        {
            try
            {
                broadReceiver.Stop();
                broadSender.Stop();
            }
            catch (Exception e)
            {

            }
            tcpServer.Stop();
            tcpClient.Cancel();
        }

        public void Send(Message message)
        {
            /* is there already an open port to the */
            foreach (UserConnection uc in ConnectionList)
            {
                if (message.ToUser.Equals(uc.user))
                {

                }
            }
        }

        private void FillNetworkConfiguration()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = GetLocalIPAddress(ipHostInfo);
            Configuration.localIpAddress = ipAddress;
            Configuration.selectedTcpPort = Configuration.PORT_TCP[GetAvaiableTCPPortIndex()];
        }

        internal bool SendMessage(Message text, ExternalUser exUser)
        {
            if (ConnectionOverClient)
            {
                tcpClient.Send(Message.GenerateTCPMessage(text));
            }
            else if (ConnectionOverServer)
            {
                tcpServer.Send(exUser, Message.GenerateTCPMessage(text));
            }
            else
            {
                Debug.WriteLine("There is currently no connection");
                return false;
            }
            return true;
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

        internal static int GetAvaiableTCPPortIndex()
        {
            int index = 0;
            for (index = 0; index < Configuration.PORT_TCP.Length; index++)
            {
                if (checkTcpPortAvaibility(Configuration.PORT_TCP[index]))
                {
                    return index;
                    Debug.WriteLine("Select index " + index);
                    //if (index >= Configuration.PORT_TCP.Length)
                    //{
                    //    return 0;
                    //}
                    //else
                    //{
                    //    return index;
                    //}
                }
            }
            return index;
        }

        internal static bool checkTcpPortAvaibility(int port)
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

        internal void IncomingMessageFromServer(Message content)
        {
            Debug.WriteLine("Incoming IncomingMessageFromServer " + content.FromUser.Equals(control.graphicControl.CurrentlyActiveChatUser) + " " + control.graphicControl.CurrentlyActiveChatUser == null);

            if (content.FromUser.Equals(control.graphicControl.CurrentlyActiveChatUser) || control.graphicControl.CurrentlyActiveChatUser == null)
            {
                control.graphicControl.ReceiveMessage(content);
            }
        }

        internal void IncomingMessageFromClient(Message content)
        {
            //Debug.WriteLine("Incoming IncomingMessageFromClient "+ content.FromUser.Equals(control.graphicControl.CurrentlyActiveChatUser)+" "+ control.graphicControl.CurrentlyActiveChatUser == null);
            if (content.FromUser.Equals(control.graphicControl.CurrentlyActiveChatUser) ||
                control.graphicControl.CurrentlyActiveChatUser == null)
            {
                control.graphicControl.ReceiveMessage(content);
            }
        }

        internal bool AcceptIncomingConnectionFromServer()
        {
            lock (incomingConnectionLock)
            {
                if (!ConnectionOverClient && !ConnectionOverServer)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal void SetConnectionInformation(Message message)
        {
            control.graphicControl.InitiateCurrentlyActiveUser(message);
        }

        internal void CloseConnectionFromServer()
        {
            bool isInside = false;
            UserConnection toRemove = null;
            foreach (UserConnection ex in ConnectionList)
            {
                if (ex.Equals(control.graphicControl.CurrentlyActiveChatUser))
                {
                    isInside = true;
                    toRemove = ex;
                }
            }
            if (isInside)
            {
                ConnectionList.Remove(toRemove);
            }

            control.graphicControl.CurrentlyActiveChatUser = null;
            ConnectionOverServer = false;
        }

        internal void CloseConnectionFromClient()
        {
            control.graphicControl.CurrentlyActiveChatUser = null;
            ConnectionOverClient = false;
        }

        internal void IncomingBroadcastMessage(Message content)
        {
            ExternalUser exUser = ExternalUser.ParseFromMessage(content);
            if (exUser != null && content.MessageType.StartsWith("HeartbeatLive"))
            {
                control.graphicControl.BroadcastAdd(exUser);
            }
            else if (exUser != null && content.MessageType.StartsWith("HeartbeatOffline"))
            {
                control.graphicControl.BroadcastRemove(exUser);
            }
        }

        internal void IncomingConnectionFromServer(Message tcpAcceptMessage)
        {
            lock (incomingConnectionLock)
            {
                if (!ConnectionOverClient && !ConnectionOverServer)
                {
                    ConnectionOverServer = true;
                    control.graphicControl.CurrentlyActiveChatUser = new ExternalUser(tcpAcceptMessage.FromUser);
                }
            }
        }

        internal void ManualConnectToExUser(ExternalUser ex)
        {
            lock (incomingConnectionLock)
            {
                if (control.graphicControl.CurrentlyActiveChatUser != null && ex.Equals(control.graphicControl.CurrentlyActiveChatUser)) { return; }
                if (!ConnectionOverServer)
                {
                    tcpClient.ReConnect(ex.IpAddress, ex.Port);
                    ConnectionOverClient = true;
                }
            }
        }

        internal void ManuelDisconnectFromExUser(ExternalUser ex)
        {
            lock (incomingConnectionLock)
            {
                if (ConnectionOverClient)
                {
                    tcpClient.Disconnect();
                    ConnectionOverClient = false;
                }
            }
        }

        //internal void RemoveSocketToList(Socket handle)
        //{
        //    /* Add the new contact to the connection list */
        //    bool isInList = false;
        //    ExternalUser exUser = ExternalUser.ParseFromMessage(message);
        //    foreach (UserConnection uc in ConnectionList)
        //    {
        //        if (uc.Equals(exUser))
        //        {
        //            isInList = true;
        //            break;
        //        }
        //    }
        //    if (isInList)
        //    {
        //        ConnectionList.AddLast(new UserConnection(exUser, handle));

        //    }
        //}

        internal void AddSocketToList(Socket handle, Message message)
        {
            /* Add the new contact to the connection list */
            bool isAlreadyInList = false;
            ExternalUser exUser = ExternalUser.ParseFromMessage(message);
            foreach (UserConnection uc in ConnectionList)
            {
                if (uc.Equals(exUser))
                {
                    isAlreadyInList = true;
                    break;
                }
            }
            if (!isAlreadyInList)
            {
                ConnectionList.AddLast(new UserConnection(exUser, handle));

            }
        }
    }
}