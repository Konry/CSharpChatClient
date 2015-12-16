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

        public TcpMessageClient()
        {
            connected = false;
        }

        ~TcpMessageClient()
        {
            if (client != null)
            {
                client.Close();
            }
        }

        public void Connect(IPAddress ipAddress, int port)
        {
            Disconnect();
            IPEndPoint remoteEnd = new IPEndPoint(ipAddress, port);
            client = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            /* connect to the selected ipaddress */
            client.BeginConnect(remoteEnd, new AsyncCallback(ConnectCallback), client);
            connectDone.WaitOne();
            connected = true;

            /* Send a first message + TRY Connect to the remote */
            SendConnectMessage(Configuration.localUser, ipAddress, port);

            /* Receive loop ? hopefully */
            Receive(client);
        }

        public void Disconnect()
        {
            connected = false;
        }

        public void Send(string message)
        {
            Send(client, message);
        }

        private void SendConnectMessage(User user, IPAddress ipAddress, int port)
        {
            Debug.WriteLine(Message.GenerateConnectMessage(user, ipAddress, port));
            Send(client, Message.GenerateConnectMessage(user, ipAddress, port));
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

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                TcpDataObject state = new TcpDataObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, TcpDataObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error in Receive " + e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
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

                    // Get the rest of the data.
                    if (connected)
                    {
                        client.BeginReceive(state.buffer, 0, TcpDataObject.BufferSize, 0,
                            new AsyncCallback(ReceiveCallback), state);
                    }
                }
                else {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        Debug.WriteLine(state.sb.ToString());
                    }
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