using CSharpChatClient.controller;
using CSharpChatClient.Controller;
using CSharpChatClient.Controller.Network;
using CSharpChatClient.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace CSharpChatClient
{
    public class NetworkService
    {
        private ProgramController control = null;

        private TcpMessageClient tcpClient = null;
        private TcpMessageServer tcpServer = null;

        private LinkedList<UserConnection> connectionList = null;

        private bool connectionOverClient = false;
        private bool connectionOverServer = false;

        private Object incomingConnectionLock = new Object();

        public NetworkService(ProgramController control)
        {
            this.control = control;
            Initialize();
        }

        private void Initialize()
        {
            ConnectionList = new LinkedList<UserConnection>();

            FillNetworkConfiguration();
            if (Configuration.localUser == null)
            {
                User user = control.FileService.ReadUserCfgFile();
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
        }

        public bool ConnectionOverClient
        {
            get { return connectionOverClient; }
            set { connectionOverClient = value; }
        }

        public bool ConnectionOverServer
        {
            get { return connectionOverServer; }
            set { connectionOverServer = value; }
        }

        public LinkedList<UserConnection> ConnectionList
        {
            get { return connectionList; }
            set { connectionList = value; }
        }

        public void Start()
        {
            tcpServer.Start();
        }

        public void Stop()
        {
            tcpServer.Stop();
            tcpClient.Cancel();
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
                Logger.LogInfo("Send Tcp Message " + text);
                tcpClient.Send(Message.GenerateTCPMessage(text));
            }
            else if (ConnectionOverServer)
            {
                tcpServer.Send(exUser, Message.GenerateTCPMessage(text));
            }
            else
            {
                Logger.LogError("There is currently no connection!");
                return false;
            }
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <exception cref="Exception">Throws a </exception>
        /// <returns></returns>
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
                if (CheckTcpPortAvailability(Configuration.localIpAddress, Configuration.PORT_TCP[index]))
                {
                    Debug.WriteLine("Select index " + index);
                    break;
                }
            }
            return index;
        }

        internal static bool CheckTcpPortAvailability(IPAddress ipAddress, int port)
        {
            bool availability = false;
            try
            {
                System.Net.Sockets.Socket sock = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                sock.Connect(ipAddress, port);
                if (sock.Connected == true)  // Port is in use and connection is successful
                    availability = true;
                sock.Close();
                return true;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (ex.ErrorCode == 10061)  // Port is unused and could not establish connection 
                {
                    Debug.WriteLine("Port is Open!");
                    return true;
                }
                else
                    Debug.WriteLine(ex.Message);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentOutOfRangeException ||
            ex is ObjectDisposedException || ex is NotSupportedException || ex is ArgumentException ||
            ex is InvalidOperationException)
            {
                Debug.WriteLine(ex.Message);
            }
            return availability;
        }

        internal void IncomingMessageFromServer(Message content)
        {
            Debug.WriteLine("Incoming IncomingMessageFromServer " + content.FromUser.Equals(control.GraphicControl.CurrentlyActiveChatUser) + " " + control.GraphicControl.CurrentlyActiveChatUser == null);

            if (content.FromUser.Equals(control.GraphicControl.CurrentlyActiveChatUser) || control.GraphicControl.CurrentlyActiveChatUser == null)
            {
                control.GraphicControl.ReceiveMessage(content);
            }
        }

        internal void IncomingMessageFromClient(Message content)
        {
            //Debug.WriteLine("Incoming IncomingMessageFromClient "+ content.FromUser.Equals(control.graphicControl.CurrentlyActiveChatUser)+" "+ control.graphicControl.CurrentlyActiveChatUser == null);
            if (content.FromUser.Equals(control.GraphicControl.CurrentlyActiveChatUser) ||
                control.GraphicControl.CurrentlyActiveChatUser == null)
            {
                control.GraphicControl.ReceiveMessage(content);
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
            control.GraphicControl.InitiateCurrentlyActiveUser(message);
        }

        internal void CloseConnectionFromServer()
        {
            if (ConnectionOverServer)
            {
                bool isInside = false;
                UserConnection toRemove = null;
                foreach (UserConnection ex in ConnectionList)
                {
                    if (ex.Equals(control.GraphicControl.CurrentlyActiveChatUser))
                    {
                        isInside = true;
                        toRemove = ex;
                    }
                }
                if (isInside)
                {
                    ConnectionList.Remove(toRemove);
                }

                control.GraphicControl.CurrentlyActiveChatUser = null;
                ConnectionOverServer = false;
            }
        }

        internal void CloseConnectionFromClient()
        {
            control.GraphicControl.CurrentlyActiveChatUser = null;
            ConnectionOverClient = false;
        }

        internal void IncomingBroadcastMessage(Message content)
        {
            ExternalUser exUser = ExternalUser.ParseFromMessage(content);
            if (exUser != null && content.MessageType.StartsWith("HeartbeatLive"))
            {
                control.GraphicControl.BroadcastAdd(exUser);
            }
            else if (exUser != null && content.MessageType.StartsWith("HeartbeatOffline"))
            {
                control.GraphicControl.BroadcastRemove(exUser);
            }
        }

        internal void IncomingConnectionFromServer(Message tcpAcceptMessage)
        {
            lock (incomingConnectionLock)
            {
                if (!ConnectionOverClient && !ConnectionOverServer)
                {
                    ConnectionOverServer = true;
                    control.GraphicControl.CurrentlyActiveChatUser = new ExternalUser(tcpAcceptMessage.FromUser);
                }
            }
        }

        internal bool ManualConnectToExUser(ExternalUser ex)
        {
            bool success = false;
            lock (incomingConnectionLock)
            {
                if (control.GraphicControl.CurrentlyActiveChatUser != null && ex.Equals(control.GraphicControl.CurrentlyActiveChatUser)) { return false; }
                if (!ConnectionOverServer)
                {
                    success = tcpClient.ReConnect(ex.IpAddress, ex.Port);
                    if (success)
                    {
                        ConnectionOverClient = true;
                    }
                }
            }
            return success;
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