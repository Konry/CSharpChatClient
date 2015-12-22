using CSharpChatClient.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CSharpChatClient.Controller.Network
{
    public class TcpMessageServer
    {
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);

        private Socket socket = null;

        private Thread thread = null;
        private volatile bool shouldStop;
        private NetworkService netService = null;

        //private Socket client = null;

        public TcpMessageServer(NetworkService netService)
        {
            this.netService = netService;
        }

        ~TcpMessageServer()
        {
            if (Socket != null)
            {
                Socket.Close();
            }
            thread.Abort();
        }


        /// <summary>
        /// Starts the server thread
        /// </summary>
        public void Start()
        {
            shouldStop = false;
            if (thread == null)
            {
                thread = new Thread(StartListening);
                thread.Start();
            }
            else
            {
                thread.Start();
            }
        }

        /// <summary>
        /// Stops the server permanently
        /// </summary>
        public void Stop()
        {
            shouldStop = true;
            try
            {
                Socket.Close();
                if (thread != null)
                {
                    thread.Abort();
                }
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Send a message to the socket of the given user and sends the message to it
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        public void Send(User user, string message)
        {
            try
            {
                foreach (UserConnection uc in netService.ConnectionList)
                {
                    if (uc.User.Equals(user))
                    {
                        Logger.LogInfo("CloseConnectionFromServer " + uc.User.Name);
                        Send(uc.Socket, message);
                        sendDone.Set();
                    }
                }
            }
            catch (NullReferenceException ex) { }
        }

        /// <summary>
        /// Starts the listening to the port
        /// </summary>
        private void StartListening()
        {
            byte[] bytes = new Byte[1024];

            IPEndPoint localEndPoint = null;

            localEndPoint = new IPEndPoint(Configuration.localIpAddress, Configuration.selectedTcpPort);
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Binds the socket to the local address
                Socket.Bind(localEndPoint);
                Socket.Listen(100);
                while (!shouldStop)
                {
                    // Set the event to nonsignaled state.
                    connectDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Socket.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        Socket);

                    // Wait until a connection is made before continuing.
                    connectDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Logger.LogException("StartListening ", e);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                // Signal the main thread to continue.
                connectDone.Set();

                // Get the socket that handles the client request.
                Socket listener = (Socket)ar.AsyncState;
                Socket handle = listener.EndAccept(ar);

                // Create the state object.
                TcpDataObject state = new TcpDataObject();
                state.workSocket = handle;

                handle.BeginReceive(state.buffer, 0, TcpDataObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                //receiveDone.WaitOne();
            }
            catch (ObjectDisposedException ode)
            {
                Logger.LogException("Catched ObjectDisposedException", ode);
            }
            catch (Exception e) when (e is ArgumentException || e is SocketException || e is InvalidOperationException || e is NotSupportedException)
            {
                Logger.LogException("Catched other specific exception", e);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            try
            {
                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                TcpDataObject state = (TcpDataObject)ar.AsyncState;
                Socket handle = state.workSocket;

                int bytesRead = state.workSocket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    content = Encoding.Unicode.GetString(state.buffer, 0, bytesRead);

                    Logger.LogInfo("Server incoming " + content + " " + bytesRead);
                    if (Message.IsNewContactMessage(content))
                    {
                        if (netService.HasIncomingConnection())
                        {
                            netService.IncomingConnectionFromServer(Message.ParseNewContactMessage(content));
                            netService.AddSocketToList(handle, Message.ParseNewContactMessage(content));
                            Send(handle, Message.GenerateConnectMessage(Configuration.localUser, Configuration.localIpAddress, Configuration.selectedTcpPort));
                            sendDone.Set();
                        }
                    }
                    else if (Message.IsTCPMessage(content))
                    {
                        /* TODO Handle here the incoming data from an other client */
                        netService.IncomingMessage(Message.ParseTCPMessage(content));
                    }
                    else if (Message.IsNotifyMessage(content))
                    {
                        netService.NoftifyFromCurrentUser(Message.ParseTCPNotifyMessage(content));
                    }

                    // Echo the data back to the client. TODO optional, normally remove this.
                    //Send(handle, Message.GenerateConnectMessage(Configuration.localUser, Configuration.localIpAddress, Configuration.selectedTcpPort));
                    //state.sb.Clear();

                    // Again ReceiveData

                    handle.BeginReceive(state.buffer, 0, TcpDataObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
                }
                else
                {
                    // socket closed
                    Logger.LogInfo("TCP-Server - Socket closed by Client.");
                    netService.CloseConnectionFromServer();
                    receiveDone.Set();
                }
            }
            catch (ObjectDisposedException ode)
            {
                Logger.LogException("Catched ReadCallback ", ode);
            }
            catch (SocketException se)
            {
                Logger.LogException("Catched ReadCallback ", se);
            }
            catch (NullReferenceException nre)
            {
                Logger.LogException("Catched NullReferenceException ", nre);
            }
        }

        /// <summary>
        /// Sends a message to the selected socket handler.
        /// </summary>
        /// <param name="data">The string to send over the network</param>
        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.Unicode.GetBytes(data);

            // Begin sending the data to the remote device.
            try
            {
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), handler);
            }
            catch (Exception ex)
            {
                netService.CloseConnectionFromClient();
                Logger.LogException("Message can not be send!", ex, Logger.LogState.FATAL);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Logger.LogInfo("Sent " + bytesSent + " bytes to server.");

                // Signal that all bytes have been sent.
                sendDone.WaitOne();
            }
            catch (Exception e)
            {
                Logger.LogException("Error in SendCallback ", e);
            }
        }

        public Socket Socket
        {
            get { return socket; }
            set { socket = value; }
        }
    }
}