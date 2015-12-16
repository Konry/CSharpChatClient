using CSharpChatClient.Controller.Netzwerk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CSharpChatClient
{
    /**
    * The Broadcast Sender sends a Live Message every x seconds;
    *
    */
    public class BroadcastSender : IBroadcastInterface
    {
        private int PORT_NUMBER = Configuration.PORT_UDP_BROADCAST;
        private static System.Timers.Timer timer;

        public BroadcastSender()
        {
            InitializeTimer();
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
            SendBroadcastMessage(NetworkMessage.GenerateHeartbeatMessage(online));
        }

        private void SendBroadcastMessage(string message)
        {
            UdpClient client = new UdpClient();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), PORT_NUMBER);
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            client.Send(bytes, bytes.Length, endpoint);
            client.Close();
            //Debug.WriteLine("Broadcast Message %s is send!", message);
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            //Debug.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
            SendMessage(true);
        }
    }
}