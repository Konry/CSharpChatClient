using CSharpChatClient.Model;
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

        private bool connected = false;
        private NetworkService netService = null;

        private Thread thread = null;

        public TcpMessageClient(NetworkService netService)
        {
            this.netService = netService;
            connected = false;
        }

        ~TcpMessageClient()
        {
            connected = false;
            if (socket != null)
            {
                try
                {
                    socket.Disconnect(false);
                }
                catch (ObjectDisposedException ode)
                {
                    Logger.LogException("Connect ObjectDisposedException", ode);
                    /*throw away*/
                }
                socket.Close();
            }
            if (thread != null)
            {
                thread.Abort();
            }
        }

        /// <summary>
        /// Connects to the given address and port, non blocking, by starting a new thread
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <exception cref="AlreadyConnectedException">Thrown when the connection is already existing.</exception>
        /// <exception cref="TimeoutException">Thrown when the connection could not be established.</exception>
        /// <exception cref="SocketException">Thrown when the connection could not be established.</exception>
        public void Connect(IPAddress ipAddress, int port)
        {
            if (connected)
            {
                throw new AlreadyConnectedException("Connection already established.");
            }
            thread = new Thread(delegate () { StartConnecting(ipAddress, port); });
            thread.Start();
        }

        /// <summary>
        /// Connects to 
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <exception cref="TimeoutException">Thrown when the connection could not be established.</exception>
        /// <exception cref="SocketException">Thrown when the connection could not be established.</exception>
        public void StartConnecting(IPAddress ipAddress, int port)
        {
            try
            {
                connected = true;
                IPEndPoint remoteEnd = new IPEndPoint(ipAddress, port);
                socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                /* connect to the selected ipaddress */
                socket.BeginConnect(remoteEnd, new AsyncCallback(ConnectCallback), socket);
                if (!Configuration.DEBUG_SESSION && !connectDone.WaitOne(5000, true))
                {
                    throw new TimeoutException("Connection could not be established.");
                }
                else
                {
                    connectDone.WaitOne();
                }

                /* Send a first message + TRY Connect to the remote */
                netService.SendConnectMessage(ExtendedUser.ConfigurationToExtendedUser());

                Receive(socket);
                if (!Configuration.DEBUG_SESSION && !receiveDone.WaitOne(5000, true))
                {
                    throw new TimeoutException("Message receive is interrupted.");
                }
                else
                {
                    receiveDone.WaitOne();
                }
            }
            catch (ObjectDisposedException ode)
            {
                Logger.LogException("Connect ObjectDisposedException", ode);
            }
            //catch (SocketException se)
            //{
            //    Logger.LogException("Connect", se);
            //}
        }

        /// <summary>
        /// Reconnects to the given address information, by close the current connection.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <exception cref="AlreadyConnectedException">Thrown when the connection is already existing.</exception>
        /// <exception cref="TimeoutException">Thrown when the connection could not be established.</exception>
        /// <exception cref="SocketException">Thrown when the connection could not be established.</exception>
        public void ReConnect(IPAddress ipAddress, int port)
        {
            Disconnect();
            Connect(ipAddress, port);
        }

        public void Disconnect()
        {
            connected = false;
            if (socket != null)
            {
                try
                {
                    socket.Disconnect(true);
                }
                catch (ObjectDisposedException ode)
                {
                    Logger.LogException("ReConnect ObjectDisposedException", ode);
                    /*throw away*/
                }
            }
        }

        public void Cancel()
        {
            if (socket != null)
            {
                socket.Close();
            }
            if (thread != null)
            {
                thread.Abort();
            }
        }

        /// <summary>
        /// Sends a message to the given socket.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="TimeoutException">Thrown when the send has reached timeout.</exception>
        public void Send(string message)
        {
            Send(socket, message);
            if (!Configuration.DEBUG_SESSION && !sendDone.WaitOne(5000, true))
            {
                throw new TimeoutException("Connection could not be established.");
            }
            else
            {
                sendDone.WaitOne();
            }
        }


        /// <summary>
        /// Awaits the connection
        /// </summary>
        /// <param name="ar"></param>
        /// <exception cref="SocketException">Thrown when the connection could not be established.</exception>
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
            catch (SocketException e)
            {
                throw new SocketException();
            }
            catch (Exception e)
            {
                Logger.LogException("ConnectCallback", e);
            }
        }

        /// <summary>
        /// Awaits message from the client.
        /// </summary>
        /// <param name="client">The socket of the client</param>
        /// <exception cref="TimeoutException">Thrown when the connection could not be established.</exception>
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
                if (!Configuration.DEBUG_SESSION && !receiveDone.WaitOne(5000, true))
                {
                    throw new TimeoutException("Receive is interrupted.");
                }
                else
                {
                    receiveDone.WaitOne();
                }
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
                    // There might be more data, so store the data received so far.
                    //state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    content = Encoding.ASCII.GetString(state.buffer, 0, bytesRead);
                    Logger.LogInfo(content);

                    netService.AnalyseIncomingContent(content);

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
                Disconnect();
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