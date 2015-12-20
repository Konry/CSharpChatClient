using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CSharpChatClient.Controller.Network
{
    public class TcpMessageClient
    {
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private Socket socket = null;
        //private static ManualResetEvent sendDone = new ManualResetEvent(false);

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
            if (Socket != null)
            {
                try
                {
                    Socket.Disconnect(false);
                }
                catch (ObjectDisposedException ode)
                {
                    Logger.LogException("Connect ObjectDisposedException", ode);
                    /*throw away*/
                }
                Socket.Close();
            }
            if (thread != null)
            {
                thread.Abort();
            }
        }

        /// <summary>
        /// Disconnect from the current session and start an other one
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool ReConnect(IPAddress ipAddress, int port)
        {
            Disconnect();
            return Connect(ipAddress, port);
        }

        /// <summary>
        /// Disconnect from session, clean up socket
        /// </summary>
        public void Disconnect()
        {
            shouldStop = true;
            connected = false;
            if (Socket != null)
            {
                try
                {
                    Socket.Disconnect(true);
                }
                catch (ObjectDisposedException ode)
                {
                    Logger.LogException("ReConnect ObjectDisposedException", ode);
                    /*throw away*/
                }
            }
        }

        /// <summary>
        /// Sends a message to the given socket.
        /// </summary>
        /// <param name="message"></param>
        public void Send(string message)
        {
            Send(Socket, message);
            sendDone.Set();
        }

        private void SendConnectMessage(User user, IPAddress ipAddress, int port)
        {
            Logger.LogInfo("Send Connect Message " + Message.GenerateConnectMessage(user, ipAddress, port));
            Send(Socket, Message.GenerateConnectMessage(user, ipAddress, port));
            sendDone.Set();
        }

        /// <summary>
        /// Connects to the given address and port, blocking, by starting a new receiving thread
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public bool Connect(IPAddress ipAddress, int port)
        {
            try
            {
                if (connected)
                {
                    return true;
                }
                shouldStop = false;
                connected = true;
                IPEndPoint remoteEnd = new IPEndPoint(ipAddress, port);
                Socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                /* connect to the selected ipaddress */
                Socket.BeginConnect(remoteEnd, new AsyncCallback(ConnectCallback), Socket);
                bool ret = connectDone.WaitOne(2000, true);
                if (!ret)
                {
                    netService.CloseConnectionFromClient();
                }

                thread = new Thread(StartReceiving);
                /* Send a first message + TRY Connect to the remote */
                SendConnectMessage(Configuration.localUser, ipAddress, port);
                thread.Start();
                return true;
            }
            catch (System.ObjectDisposedException ode)
            {
                Logger.LogException("Connect ObjectDisposedException", ode);
            }
            catch (SocketException se)
            {
                Logger.LogException("Connect", se);
                return false;
            }
            return false;
        }

        /// <summary>
        /// Awaits the connection
        /// </summary>
        /// <param name="ar"></param>
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Logger.LogInfo("Socket connected to " + client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Logger.LogException("ConnectCallback", e);
            }
        }

        private void StartReceiving()
        {
            Receive(Socket);
        }

        private void Receive(Socket client)
        {
            try
            {
                //receiveDone.Reset();
                // Create the state object.
                TcpDataObject state = new TcpDataObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, TcpDataObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
                receiveDone.WaitOne();
            }
            catch (ObjectDisposedException ode)
            {
                Logger.LogException("Catched ObjectDisposedException", ode);
            }
            catch (Exception e)
            {
                Logger.LogException("Catched other exception", e);
            }
        }

        /// <summary>
        /// Analyises an incoming AsyncResult Object, resulting in the message.
        /// Informes networkService about the incoming 
        /// </summary>
        /// <param name="ar"></param>
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
                    content = Encoding.Unicode.GetString(state.buffer, 0, bytesRead);
                    Logger.LogInfo(content);

                    if (!content.Equals(""))
                    {
                        if (Message.IsTCPMessage(content))
                        {
                            netService.IncomingMessage(Message.ParseTCPMessage(content));
                        }
                        else if (Message.IsNewContactMessage(content))
                        {
                            netService.SetConnectionInformation(Message.ParseNewContactMessage(content));
                        } else if (Message.IsNotifyMessage(content))
                        {
                            netService.NoftifyFromCurrentUser(Message.ParseTCPNotifyMessage(content));
                        }
                    }
                    // Get the rest of the data.
                    //state.sb.Clear();
                    client.BeginReceive(state.buffer, 0, TcpDataObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else {
                    // All the data has arrived; Socket can be closed
                    Logger.LogInfo("TCP-Client - Socket closed by Server.");
                    netService.CloseConnectionFromClient();
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
            }
            catch (SocketException se)
            {
                netService.CloseConnectionFromClient();
                Logger.LogException("Socket from Remote has been closed abruptly.", se, Logger.LogState.INFO);
            }
            catch (Exception e)
            {
                Logger.LogException("ReceiveCallback ", e);
            }
        }

        /// <summary>
        /// Sends a message asynchrone to the selected socket handler.
        /// </summary>
        /// <param name="data">The string to send over the network</param>
        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using Unicode encoding.
            byte[] byteData = Encoding.Unicode.GetBytes(data);

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

        /// <summary>
        /// Sends the data to the remote device. Async function.
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Logger.LogInfo("Sent "+ bytesSent + " bytes to server.");

                // Signal that all bytes have been sent.
                sendDone.Set();
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