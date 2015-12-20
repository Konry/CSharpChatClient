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
            Initialize();
        }

        ~TcpMessageServer()
        {
            if (socket != null)
            {
                socket.Close();
            }
            thread.Abort();
        }

        private void Initialize()
        {
        }

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

        public void Stop()
        {
            shouldStop = true;
            socket.Close();
            if (thread != null)
            {
                thread.Abort();
            }
        }

        /// <summary>
        /// Send a message to the socket of the given user and sends the message to it
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        public void Send(User user, string message)
        {
            foreach (UserConnection uc in netService.ConnectionList)
            {
                if (uc.User.Equals(user))
                {
                    Send(uc.Socket, message);
                    sendDone.Set();
                }
            }
        }

        /// <summary>
        /// Starts the listening to the port
        /// </summary>
        private void StartListening()
        {
            byte[] bytes = new Byte[1024];

            IPEndPoint localEndPoint = null;

            localEndPoint = new IPEndPoint(Configuration.localIpAddress, Configuration.selectedTcpPort);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Binds the socket to the local address
                socket.Bind(localEndPoint);
                socket.Listen(100);
                while (!shouldStop)
                {
                    connectDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    socket.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        socket);

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
                receiveDone.WaitOne();
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

                Logger.LogInfo("Server incoming " + content + " " + bytesRead);
                if (bytesRead > 0)
                {
                    content = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                    Logger.LogInfo(content);
                    netService.AnalyseIncomingContent(content, true, handle);

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
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            try
            {
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), handler);
            }
            catch (Exception ex)
            {
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
                sendDone.Set();
            }
            catch (Exception e)
            {
                Logger.LogException("Error in SendCallback ", e);
            }
        }
    }
}