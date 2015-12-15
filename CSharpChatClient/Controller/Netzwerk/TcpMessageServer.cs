using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CSharpChatClient
{
    public class TcpMessageServer
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private bool enabled = true;
        private Thread thread = null;
        private Socket server = null;

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
            Start();
            //thread = new Thread(StartListening);
            //thread.Start();
        }

        public void Start()
        {
            enabled = true;
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
            enabled = false;
        }

        private void StartListening()
        {
            Debug.WriteLine("Start Listening");
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            int index = 0;
            index = selectAvaiableConfigurationTCPPort();
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Configuration.PORT_TCP[index]);

            Configuration.localIpAddress = ipAddress;

            // Create a TCP/IP socket.
            server = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                server.Bind(localEndPoint);
                server.Listen(100);

                while (enabled)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Debug.WriteLine("Waiting for a connection...");
                    server.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        server);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("StartListening "+e.ToString());
            }
        }

        private int selectAvaiableConfigurationTCPPort()
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

        private void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            TcpConnectionObject state = new TcpConnectionObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, TcpConnectionObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            TcpConnectionObject state = (TcpConnectionObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the 
                    // client. Display it on the console.
                    Debug.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    // Echo the data back to the client.
                    Send(handler, content);
                }
                else {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, TcpConnectionObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }
        private static void Send(Socket handler, String data)
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

        private bool checkTcpPortAvaibility(int port)
        {
            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }
            if (isAvailable)
                Debug.WriteLine("Port: " + port + " is selected");
            return isAvailable;
        }

    }
}