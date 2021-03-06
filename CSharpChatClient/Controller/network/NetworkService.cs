﻿using CSharpChatClient.controller;
using CSharpChatClient.Controller;
using CSharpChatClient.Controller.Network;
using CSharpChatClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

        public delegate void DisconnectDelegate();
        public delegate void NoConnectionDelegate();

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

        private void FillNetworkConfiguration()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = GetLocalIPAddress(ipHostInfo);
            Configuration.localIpAddress = ipAddress;
            Configuration.selectedTcpPort = GetAvaiableTCPPort();
        }

        /// <summary>
        /// Send the username Change to the remote connection
        /// </summary>
        /// <param name="message"></param>
        internal void RenameUsernameNotifyRemote(Message message)
        {
            SendNotify(message);
        }


        /// <summary>
        /// Starts TCP-Listening Server
        /// </summary>
        public void Start()
        {
            tcpServer.Start();
        }

        /// <summary>
        /// Stops the server totally and disconnect all connections.
        /// </summary>
        public void Stop()
        {
            Message message = new Message(Configuration.localUser, null, "UserDisconnect");
            if (ConnectionOverClient)
            {
                tcpClient.Send(Message.GenerateTCPNotify(message));
            }
            else if (connectionOverServer)
            {
                tcpServer.Send(control.GraphicControl.CurrentlyActiveChatUser, Message.GenerateTCPNotify(message));
            }
            tcpServer.Stop();
            tcpClient.Disconnect();
        }


        /// <summary>
        /// Sends the given message to the current open connection
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal bool SendMessage(Message message)
        {
            if (ConnectionOverClient)
            {
                Logger.LogInfo("Send Tcp Message " + message);
                tcpClient.Send(Message.GenerateTCPMessage(message));
            }
            else if (ConnectionOverServer)
            {
                tcpServer.Send(message.ToUser, Message.GenerateTCPMessage(message));
            }
            else
            {
                Logger.LogError("There is currently no connection!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Send a notfiy message to the current open connection
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal bool SendNotify(Message message)
        {
            if (ConnectionOverClient)
            {
                tcpClient.Send(Message.GenerateTCPNotify(message));
            }
            else if (ConnectionOverServer)
            {
                tcpServer.Send(message.ToUser, Message.GenerateTCPNotify(message));
            }
            else
            {
                Logger.LogError("There is currently no connection!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reads the first local ip-address adapter id
        /// </summary>
        /// <param name="host"></param>
        /// <exception cref="Exception">Throws a new address is not found exception. </exception>
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

        /// <summary>
        /// Checks if there is a free port between two numbers, sets also the input values into bounds of 0 - 65535
        /// </summary>
        /// <returns>The next fee tcp port.</returns>
        internal static int GetAvaiableTCPPort(int portStartIndex = 51110, int portEndIndex = 51150)
        {
            if (portStartIndex < 0)
            {
                portStartIndex = 0;
            }
            else if (portEndIndex > 65535)
            {
                portStartIndex = 65535;
            }
            if (portEndIndex < 0)
            {
                portEndIndex = 0;
            }
            else if (portEndIndex > 65535)
            {
                portEndIndex = 65535;
            }
            else if (portEndIndex < portStartIndex)
            {
                portEndIndex = portStartIndex;
            }

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
            List<int> usedPorts = tcpEndPoints.Select(p => p.Port).ToList<int>();
            int unusedPort = 51110;
            for (int portIndex = portStartIndex; portIndex < portEndIndex; portIndex++)
            {
                if (!usedPorts.Contains(portIndex))
                {
                    Logger.LogInfo("Unused Port: " + unusedPort);
                    unusedPort = portIndex;
                    break;
                }
            }
            return unusedPort;
        }        

        /// <summary>
        /// Manages an incoming message from the client. Send message to graphic controller
        /// </summary>
        /// <param name="content"></param>
        internal void IncomingMessage(Message content)
        {
            if (content.FromUser.Equals(control.GraphicControl.CurrentlyActiveChatUser) ||
                control.GraphicControl.CurrentlyActiveChatUser == null)
            {
                control.GraphicControl.ReceiveMessage(content);
            }
        }

        /// <summary>
        /// Check if there is a session to a client or server active.
        /// </summary>
        /// <returns>true for active session</returns>
        internal bool HasIncomingConnection()
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

        /// <summary>
        /// Closes the current connection from the server. Remove the used socket from the connection list.
        /// </summary>
        internal void CloseConnectionFromServer()
        {
            if (ConnectionOverServer)
            {
                RemoveCurrentActiveUserSocketFromList();

                control.GraphicControl.CurrentlyActiveChatUser = null;
                ConnectionOverServer = false;
            }
        }


        /// <summary>
        /// Closes an active connection from the server, update all neccessary modules
        /// </summary>
        internal void CloseConnectionFromClient()
        {
            if (ConnectionOverClient)
            {
                tcpClient.Disconnect();
                control.GraphicControl.CurrentlyActiveChatUser = null;
                ConnectionOverClient = false;
            }
        }

        /// <summary>
        /// Informs the graphical interface about a new currently active user
        /// </summary>
        /// <param name="message"></param>
        internal void NoftifyFromCurrentUser(Message message)
        {
            if (message.MessageContent.StartsWith("UserDisconnect"))
            {
                if (connectionOverClient)
                {
                    CloseConnectionFromClient();
                }
                else if (connectionOverServer)
                {
                    CloseConnectionFromServer();
                }
            } else if (message.MessageContent.StartsWith("Rename:"))
            {
                control.GraphicControl.InitiateCurrentlyActiveUser(message);
            }
        }

        /// <summary>
        /// Notifys the Gui Thread when there is a new connection established.
        /// </summary>
        /// <param name="tcpAcceptMessage"></param>
        internal void IncomingConnectionFromServer(Message tcpAcceptMessage)
        {
            lock (incomingConnectionLock)
            {
                if (!ConnectionOverClient && !ConnectionOverServer)
                {
                    ConnectionOverServer = true;
                    control.GraphicControl.CurrentlyActiveChatUser = new ExtendedUser(tcpAcceptMessage.FromUser);
                }
            }
        }

        /// <summary>
        /// Connect to the given address external user, who has the address informations inside.
        /// </summary>
        /// <param name="exUser"></param>
        internal bool ManualConnectToExUser(ExtendedUser exUser)
        {
            bool success = false;
            lock (incomingConnectionLock)
            {
                if (control.GraphicControl.CurrentlyActiveChatUser != null && exUser.Equals(control.GraphicControl.CurrentlyActiveChatUser)) { return false; }
                if (!ConnectionOverServer)
                {
                    try {
                        success = tcpClient.ReConnect(exUser.IpAddress, exUser.Port);
                        if (success)
                        {
                            ConnectionOverClient = true;
                        }
                    }catch(SocketException ex)
                    {
                        CloseConnectionFromClient();
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Disconnects from the current user
        /// </summary>
        /// <param name="ex"></param>
        internal void ManualDisconnectFromExUser(ExtendedUser ex)
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

        /// <summary>
        /// Adds the socket to the connection list, if not exisiting
        /// </summary>
        /// <param name="message"></param>
        /// <param name="handle"></param>
        internal void AddSocketToList(Socket handle, Message message)
        {
            /* Add the new contact to the connection list */
            bool isAlreadyInList = false;
            ExtendedUser exUser = ExtendedUser.ParseFromMessage(message);

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
                Logger.LogInfo("Add user " + (message.FromUser == null) + " " + (message.ToUser == null) + " ");
                ConnectionList.AddLast(new UserConnection(exUser, handle));
            }
        }

        /// <summary>
        /// Remove from the UserconnectionList the active user
        /// </summary>
        private void RemoveCurrentActiveUserSocketFromList()
        {
            bool isInside = false;
            UserConnection toRemove = null;
            foreach (UserConnection ex in ConnectionList)
            {
                Logger.LogInfo("CloseConnectionFromServer " + ex.User.Name);
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
    }
}