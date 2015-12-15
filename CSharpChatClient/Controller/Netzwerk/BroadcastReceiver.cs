using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace CSharpChatClient
{
    public class BroadcastReceiver
    {
        private int PORT_NUMBER = Configuration.PORT_UDP_BROADCAST;
        private UdpClient client;

        public BroadcastReceiver()
        {
            Initialize();
        }

        private void Initialize()
        {
            client = new UdpClient(PORT_NUMBER);
            client.BeginReceive(new AsyncCallback(receive), null);
        }

        private void receive(IAsyncResult res)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, PORT_NUMBER);
            byte[] received = client.EndReceive(res, ref RemoteIpEndPoint);

            //Process codes
            String s = Encoding.UTF8.GetString(received);

            client.BeginReceive(new AsyncCallback(receive), null);

            /*TODO Handle the output afterwarts -> Send to internal handler of date */

            //Debug.WriteLine(s);
        }
    }
}