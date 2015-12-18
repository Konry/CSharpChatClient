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
        private NetworkService netService = null;
        private UdpClient client = null;
        IPEndPoint remoteIpEndPoint = null;

        public BroadcastReceiver(NetworkService netService)
        {
            this.netService = netService;
        }

        private void Initialize()
        {
            client = new UdpClient(Configuration.PORT_UDP_BROADCAST);
            remoteIpEndPoint = new IPEndPoint(IPAddress.Any, Configuration.PORT_UDP_BROADCAST);
        }

        public void Start()
        {
            Initialize();
            try
            {
                client.BeginReceive(new AsyncCallback(receive), null);
            }
            catch (System.ObjectDisposedException ode)
            {
                Debug.WriteLine("Catched ObjectDisposedException");
            }
        }

        public void Stop()
        {
            client.Close();
        }

        private void receive(IAsyncResult res)
        {
            try
            {
                byte[] received = client.EndReceive(res, ref remoteIpEndPoint);

                //Process codes
                String s = Encoding.UTF8.GetString(received);

                netService.IncomingBroadcastMessage(Message.ParseNewContactMessage(s));

                client.BeginReceive(new AsyncCallback(receive), null);

                /*TODO Handle the output afterwarts -> Send to internal handler of date */

                //Debug.WriteLine(s);
            }
            catch (ObjectDisposedException ode)
            {
                Debug.WriteLine("Catched ObjectDisposedException");
                /*ignore object disposed exception*/
            }
        }
    }
}