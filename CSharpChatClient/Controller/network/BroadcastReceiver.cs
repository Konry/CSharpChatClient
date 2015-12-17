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
        private NetworkService netService = null;
        private UdpClient client = null;

        public BroadcastReceiver(NetworkService netService)
        {
            this.netService = netService;
        }

        private void Initialize()
        {
            client = new UdpClient(PORT_NUMBER);
        }

        public void Start()
        {
            Initialize();
            client.BeginReceive(new AsyncCallback(receive), null);
        }

        public void Stop()
        {
            client.Close();
        }

        private void receive(IAsyncResult res)
        {
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, PORT_NUMBER);
            byte[] received = client.EndReceive(res, ref RemoteIpEndPoint);

            //Process codes
            String s = Encoding.UTF8.GetString(received);

            netService.IncomingBroadcastMessage(Message.ParseBroadcastMessage(s));

            client.BeginReceive(new AsyncCallback(receive), null);

            /*TODO Handle the output afterwarts -> Send to internal handler of date */

            //Debug.WriteLine(s);
        }
    }
}