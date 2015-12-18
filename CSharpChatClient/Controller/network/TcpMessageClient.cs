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
    public class TcpMessageClient
    {
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        public Socket client { get; set; }
        private bool connected = false;
        private NetworkService netService = null;

        private Thread thread = null;
        private volatile bool shouldStop = false;

        public TcpMessageClient(NetworkService netService)
        {
            this.netService = netService;
            connected = false;
        }

        ~TcpMessageClient()
        {
            connected = false;
            if (client != null)
            {
                try
                {
                    client.Disconnect(false);
                }
                catch (ObjectDisposedException ode)
                {
                    Debug.WriteLine("Catched ObjectDisposedException");
                    /*throw away*/
                }
                client.Close();
            }
            if (thread != null)
            {
                thread.Abort();
            }
        }

        public void Connect(IPAddress ipAddress, int port)
        {
            try
            {
                if (connected)
                {
                    return;
                }
                shouldStop = false;
                IPEndPoint remoteEnd = new IPEndPoint(ipAddress, port);
                client = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                /* connect to the selected ipaddress */
                client.BeginConnect(remoteEnd, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
                connected = true;

                thread = new Thread(StartReceiving);
                /* Send a first message + TRY Connect to the remote */
                SendConnectMessage(Configuration.localUser, ipAddress, port);
                thread.Start();
            }
            catch (System.ObjectDisposedException ode)
            {
                Debug.WriteLine("Catched ObjectDisposedException");
            }
        }

        public void ReConnect(IPAddress ipAddress, int port)
        {
            Disconnect();
            Connect(ipAddress, port);
        }

        public void Disconnect()
        {
            shouldStop = true;
            connected = false;
            if (client != null)
            {
                try
                {
                    client.Disconnect(true);
                }
                catch (ObjectDisposedException ode)
                {
                    Debug.WriteLine("Catched ObjectDisposedException");
                    /*throw away*/
                }
            }
        }

        public void Cancel()
        {
            shouldStop = true;
            if (client != null)
            {
                client.Close();
            }
            if (thread != null)
            {
                thread.Abort();
            }
        }

        public void Send(string message)
        {
            Send(client, message);
        }

        private void SendConnectMessage(User user, IPAddress ipAddress, int port)
        {
            Debug.WriteLine("Send Connect Message " + Message.GenerateConnectMessage(user, ipAddress, port));
            Send(client, Message.GenerateConnectMessage(user, ipAddress, port));
            Debug.WriteLine("Send Connect Message DONE");
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Debug.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        private void StartReceiving()
        {
            while (!shouldStop)
            {
                Receive(client);
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                receiveDone.Reset();
                // Create the state object.
                TcpDataObject state = new TcpDataObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, TcpDataObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                receiveDone.WaitOne();
            }
            catch (ObjectDisposedException ode)
            {
                Debug.WriteLine("Catched ObjectDisposedException");
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Receive " + e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                TcpDataObject state = (TcpDataObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    content = state.sb.ToString();
                    Debug.WriteLine(content);

                    if (!content.Equals(""))
                    {
                        if (Message.IsTCPMessage(content))
                        {
                            netService.IncomingMessageFromClient(Message.ParseTCPMessage(content));
                        } else if (Message.IsNewContact(content))
                        {
                            netService.SetConnectionInformation(Message.ParseNewContactMessage(content));
                        }
                    }
                    // Get the rest of the data.
                    state.sb.Clear();
                    client.BeginReceive(state.buffer, 0, TcpDataObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else {
                    // All the data has arrived; Socket can be closed

                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in ReceiveCallback " + e.ToString());
            }
        }

        private void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Debug.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in SendCallback " + e.ToString());
            }
        }
    }
}