using System.Runtime.CompilerServices;
using CSharpChatClient.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

[assembly: InternalsVisibleTo("TestTcpMessageServer")]
namespace CSharpChatClient
{
    
    public class TcpMessageServer
    {
        public static ManualResetEvent serverBlock = new ManualResetEvent(false);
        private Thread thread = null;
        private volatile bool shouldStop;

        private Socket server = null;
        //private Socket client = null;

        private LinkedList<UserConnection> connectionList = null;

        public TcpMessageServer()
        {
            Initialize();
        }

        ~TcpMessageServer()
        {
            if (server != null)
            {
                server.Close();
            }
        }

        private void Initialize()
        {
            connectionList = new LinkedList<UserConnection>();
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
        }

        public void Send(User user, string message)
        {
            foreach (UserConnection uc in connectionList)
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

            
            IPEndPoint localEndPoint = new IPEndPoint(Configuration.localIpAddress, Configuration.selectedTcpPort);

            // Create a TCP/IP socket.
            server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Bind(localEndPoint);
                server.Listen(100);

                while (!shouldStop)
                {
                    // Set the event to nonsignaled state.
                    serverBlock.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Debug.WriteLine("Waiting for a connection...");
                    server.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        server);

                    // Wait until a connection is made before continuing.
                    serverBlock.WaitOne();
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("StartListening " + e.ToString());
            }
        }

 

        private void AcceptCallback(IAsyncResult ar)
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

        private void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            TcpDataObject state = (TcpDataObject)ar.AsyncState;
            Socket handle = state.workSocket;

            int bytesRead = handle.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();

                if (!Message.IsNewContact(content)) {
                    /* TODO Handle here the incoming data from an other client */
                    NetworkService.IncomingMessageFromServer(Message.ParseTCPMessage(content));
                } else
                {
                    AddSocketToList(handle);
                }

                // Echo the data back to the client. TODO optional, normally remove this.
                Send(handle, content);

                // Again ReceiveData
                handle.BeginReceive(state.buffer, 0, TcpDataObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
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

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private void AddSocketToList(Socket handle)
        {
            /* Add the new contact to the connection list */
            bool isAlreadyInList = false;
            foreach (UserConnection uc in connectionList)
            {
                if (uc.socket.Equals(handle))
                {
                    isAlreadyInList = true;
                    break;
                }
            }
            if (!isAlreadyInList)
            {
                connectionList.AddLast(new UserConnection(new User("temporary"), handle));
            }
        }

        
    }
}