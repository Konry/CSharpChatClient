using CSharpChatClient.Model;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CSharpChatClient.Controller.Network
{

    public class TcpMessageServer : TcpCommunication
    {
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public static ManualResetEvent serverBlock = new ManualResetEvent(false);
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
            if (Socket != null)
            {
                Socket.Close();
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
            Socket.Close();
            if (thread != null)
            {
                thread.Abort();
            }
        }

        public void Send(User user, string message)
        {
            foreach (UserConnection uc in netService.ConnectionList)
            {
                if (uc.user.Equals(user))
                {
                    Send(uc.socket, message);
                    sendDone.Set();
                }
            }
        }

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
                    serverBlock.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Socket.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        Socket);

                    // Wait until a connection is made before continuing.
                    serverBlock.WaitOne();
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
                    serverBlock.Set();

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

                Logger.LogInfo("Server incoming " + content + " " + bytesRead);
                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    //state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    content = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);

                    if (Message.IsNewContactMessage(content))
                    {
                        if (netService.AcceptIncomingConnectionFromServer())
                        {
                            netService.IncomingConnectionFromServer(Message.ParseNewContactMessage(content));
                            netService.AddSocketToList(handle, Message.ParseNewContactMessage(content));
                            Send(handle, Message.GenerateConnectMessage(Configuration.localUser, Configuration.localIpAddress, Configuration.selectedTcpPort));
                            sendDone.Set();
                        }
                    }
                    else
                    {
                        /* TODO Handle here the incoming data from an other client */
                        netService.IncomingMessageFromServer(Message.ParseTCPMessage(content));
                    }

                    // Echo the data back to the client. TODO optional, normally remove this.
                    //Send(handle, Message.GenerateConnectMessage(Configuration.localUser, Configuration.localIpAddress, Configuration.selectedTcpPort));
                    state.sb.Clear();

                    // Again ReceiveData

                    handle.BeginReceive(state.buffer, 0, TcpDataObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);

                }
                else
                {
                    /* socket closed*/
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
    }
}