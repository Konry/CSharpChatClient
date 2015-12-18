using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CSharpChatClient
{
    /**
    * The Broadcast Sender sends a Live Message every x seconds;
    *
    */
    public class BroadcastSender 
    {
        private static System.Timers.Timer timer;
        private UdpClient udpClient = new UdpClient();
        IPEndPoint endpoint = null;

        public BroadcastSender()
        {
            Initialize();
            InitializeTimer();
        }

        private void Initialize()
        {
            endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), Configuration.PORT_UDP_BROADCAST);
        }

        ~BroadcastSender()
        {
            Stop();
        }

        public void Start()
        {
            timer.Enabled = true;
        }

        public void Stop()
        {
            timer.Enabled = false;
            SendMessage(false);
            udpClient.Close();
        }

        private void InitializeTimer()
        {
            timer = new System.Timers.Timer(Configuration.BROADCAST_TIMER_INTERVAL_MSEC);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            SendMessage(true);
        }

        private void SendMessage(bool online)
        {
            SendBroadcastMessage(Message.GenerateHeartbeatMessage(online));
        }

        private void SendBroadcastMessage(string message)
        {
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(message);
                udpClient.Send(bytes, bytes.Length, endpoint);
            }
            catch (System.ObjectDisposedException ode)
            {
                Debug.WriteLine("Catched ObjectDisposedException");
            }

        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            //Debug.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
            SendMessage(true);
        }
    }
}