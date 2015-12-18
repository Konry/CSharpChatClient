using System.Runtime.CompilerServices;
using CSharpChatClient.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CSharpChatClient
{

    public class TcpMessageServer
    {
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);

        public static ManualResetEvent serverBlock = new ManualResetEvent(false);
        private Thread thread = null;
        private volatile bool shouldStop;
        private NetworkService netService = null;

        private Socket server = null;
        //private Socket client = null;

        public TcpMessageServer(NetworkService netService)
        {
            this.netService = netService;
            Initialize();
        }

        ~TcpMessageServer()
        {
            if (server != null)
            {
                server.Close();
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
            server.Close();
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
                }
            }
        }

        private void StartListening()
        {
            Debug.WriteLine("Start Listening");
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            IPEndPoint localEndPoint = null;


            localEndPoint = new IPEndPoint(Configuration.localIpAddress, Configuration.selectedTcpPort);
                Debug.WriteLine("Continue;");

            // Create a TCP/IP socket.

            try
            {
                //while (!shouldStop)
                // {
                server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    server.Bind(localEndPoint);
                    server.Listen(100);
                }
                catch (SocketException e)
                {
                    localEndPoint = new IPEndPoint(Configuration.localIpAddress, Configuration.PORT_TCP[1]);
                    Configuration.selectedTcpPort = Configuration.PORT_TCP[1];
                    server.Bind(localEndPoint);
                    server.Listen(100);
                }
                Debug.WriteLine(Configuration.selectedTcpPort + " checkTcpPortAvaibility " + NetworkService.checkTcpPortAvaibility(Configuration.selectedTcpPort));


                // Set the event to nonsignaled state.
                serverBlock.Reset();

                // Start an asynchronous socket to listen for connections.
                Debug.WriteLine("Waiting for a connection...");
                server.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    server);

                // Wait until a connection is made before continuing.
                serverBlock.WaitOne();
                //}

            }
            catch (Exception e)
            {
                Debug.WriteLine("StartListening " + e.ToString());
            }
            Debug.WriteLine("END Listening");
        }



        private void AcceptCallback(IAsyncResult ar)
        {
            Debug.WriteLine("Accept");
            try
            {
                while (!shouldStop)
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
                }

            }
            catch (ObjectDisposedException ode)
            {
                Debug.WriteLine("Catched ObjectDisposedException");
                /*ignore object disposed exception*/
            }
            catch (Exception e) when (e is ArgumentException || e is SocketException || e is InvalidOperationException || e is NotSupportedException)
            {
                Debug.WriteLine("Catched other specific exception");
            }
            Debug.WriteLine("Socket Closed");
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

                if (handle == null)
                {
                    Debug.WriteLine("Socket closed");
                }

                int bytesRead = state.workSocket.EndReceive(ar);

                //Debug.WriteLine("Incoming " + content + " "+ bytesRead);
                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read 
                    // more data.
                    content = state.sb.ToString();

                    if (Message.IsNewContact(content))
                    {
                        if (netService.AcceptIncomingConnectionFromServer())
                        {
                            netService.IncomingConnectionFromServer(Message.ParseNewContactMessage(content));
                            netService.AddSocketToList(handle, Message.ParseNewContactMessage(content));
                            Send(handle, Message.GenerateConnectMessage(Configuration.localUser, Configuration.localIpAddress, Configuration.selectedTcpPort));
                            server = handle;
                        }
                        else
                        {
                            Debug.WriteLine("Verbindung schon vorhanden.");
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
                    Debug.WriteLine("End of Socket");
                    netService.CloseConnectionFromServer();
                    return;
                }
            }
            catch (ObjectDisposedException ode)
            {
                Debug.WriteLine("Catched ObjectDisposedException " + ode.StackTrace);
                //    /*ignore object disposed exception*/
            }
            catch (SocketException se)
            {
                Debug.WriteLine("Catched SocketException " + se.StackTrace);
            }
            catch (NullReferenceException se)
            {
                Debug.WriteLine("Catched System.NullReferenceException " + se.StackTrace);
            }
        }

        private static void Send(Socket handler, string data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Debug.WriteLine("Sent {0} bytes to client.", bytesSent);

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}