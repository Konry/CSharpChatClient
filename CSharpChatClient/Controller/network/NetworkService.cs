using CSharpChatClient.controller;
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

        internal void RenameUsernameNotifyRemote(Message message)
        {
            SendNotifyMessage(message);
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
            tcpClient.Cancel();
        }

        private void FillNetworkConfiguration()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = GetLocalIPAddress(ipHostInfo);
            Configuration.localIpAddress = ipAddress;
            Configuration.selectedTcpPort = GetAvaiableTCPPort();
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

        internal bool SendNotifyMessage(Message message)
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

        internal bool SendConnectMessage(ExtendedUser localExtendedUser)
        {
            if (ConnectionOverClient)
            {
                // No need to send connection message
                tcpClient.Send(Message.GenerateConnectMessage(localExtendedUser));
            }
            else if (ConnectionOverServer)
            {
                Logger.LogInfo("Send connect Message");
                tcpServer.Send(control.GraphicControl.CurrentlyActiveChatUser, Message.GenerateConnectMessage(localExtendedUser));
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

        internal void IncomingMessage(Message content)
        {
            if (content.FromUser.Equals(control.GraphicControl.CurrentlyActiveChatUser) ||
                control.GraphicControl.CurrentlyActiveChatUser == null)
            {
                control.GraphicControl.ReceiveMessage(content);
            }
        }

        internal bool AcceptIncomingConnection()
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


        /// <summary>
        /// Closes the current connection from the client.
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
        /// Closes an active connection from the server.
        /// </summary>
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

        internal void IncomingNotifyMessage(Message message)
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
            }
            else if (message.MessageContent.StartsWith("Rename:"))
            {
                control.GraphicControl.InitiateCurrentlyActiveUser(message);
            }
            else if (message.MessageContent.StartsWith("Recject"))
            {
                control.GraphicControl.InformUser("Verbindung nicht möglich, da bereits im Chat");
            }
        }


        /// <summary>
        /// Informs the graphical interface about a new currently active user
        /// </summary>
        /// <param name="message"></param>
        private void IncomingNewContactMessage(Message message)
        {
            control.GraphicControl.InitiateCurrentlyActiveUser(message);
        }

        /// <summary>
        /// Notifys the Gui Thread when there is a new connection established.
        /// </summary>
        /// <param name="tcpAcceptMessage"></param>
        private void IncomingConnectionFromServer(Message tcpAcceptMessage)
        {
            lock (incomingConnectionLock)
            {
                if (!ConnectionOverClient && !ConnectionOverServer)
                {
                    ConnectionOverServer = true;
                    control.GraphicControl.CurrentlyActiveChatUser = ExtendedUser.ParseFromMessage(tcpAcceptMessage);
                }
            }
        }

        /// <summary>
        /// Connect to the given address external user, who has the address informations inside.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        internal bool ManualConnectToExUser(ExtendedUser exUser)
        {
            bool success = false;
            lock (incomingConnectionLock)
            {
                if (control.GraphicControl.CurrentlyActiveChatUser != null && exUser.Equals(control.GraphicControl.CurrentlyActiveChatUser)) { return false; }
                if (!ConnectionOverServer)
                {
                    try
                    {
                        tcpClient.ReConnect(exUser.IpAddress, exUser.Port);
                        ConnectionOverClient = true;
                        SendConnectMessage(ExtendedUser.ConfigurationToExtendedUser());
                    }
                    catch (TimeoutException ex)
                    {
                        control.GraphicControl.InformUser("Verbindung unterbrochen. "+ex.Message);
                        ConnectionOverClient = false;
                    }
                    catch (AlreadyConnectedException ex)
                    {
                        control.GraphicControl.InformUser("Verbindung besteht bereits. " + ex.Message);
                    } catch(SocketException ex)
                    {
                        control.GraphicControl.InformUser("Verbindung nicht möglich " + ex.Message);
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
        /// The content analyser, which get the message and the handling informations out of the string.
        /// </summary>
        /// <param name="content">Stream message</param>
        /// <param name="server"></param>
        /// <param name="handle">Socket needed if there if server is true</param>
        /// <exception cref="ArgumentNullException">When the server variable is true, but there is no socket</exception>
        public void AnalyseIncomingContent(string content, bool server = false, Socket handle = null)
        {
            Logger.LogInfo("Incoming " + content);
            if (Message.IsTCPMessage(content))
            {
                IncomingMessage(Message.ParseTCPMessage(content));
            }
            else if (Message.IsNewContactMessage(content))
            {
                Message contactMessage = Message.ParseNewContactMessage(content);
                if (!AcceptIncomingConnection())
                {
                    SendNotifyMessage(new Message(contactMessage.ToUser, null, "Reject"));
                }
                else {
                    IncomingNewContactMessage(contactMessage);
                    if (server)
                    {
                        if (handle == null)
                        {
                            throw new ArgumentNullException("If the server is true the handle can not be null");
                        }
                        IncomingConnectionFromServer(contactMessage);
                        AddSocketToList(contactMessage, handle);
                        SendConnectMessage(ExtendedUser.ConfigurationToExtendedUser());
                    }
                    else
                    {
                        ConnectionOverClient = true;
                    }
                }
            }
            else if (Message.IsNotifyMessage(content))
            {
                IncomingNotifyMessage(Message.ParseTCPNotifyMessage(content));
            }
        }

        /// <summary>
        /// Adds the socket to the connection list, if not exisiting
        /// </summary>
        /// <param name="message"></param>
        /// <param name="handle"></param>
        private void AddSocketToList(Message message, Socket handle)
        {
            bool alreadyInList = false;
            ExtendedUser exUser = ExtendedUser.ParseFromMessage(message);
            foreach (UserConnection uc in ConnectionList)
            {
                if (uc.Equals(exUser))
                {
                    alreadyInList = true;
                    break;
                }
            }
            if (!alreadyInList)
            {
                ConnectionList.AddLast(new UserConnection(exUser, handle));
            }
        }
    }
}