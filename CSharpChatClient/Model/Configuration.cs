using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace CSharpChatClient
{
    public struct Configuration
    {

        public static int[] PORT_TCP = {51110, 51112, 51113, 51114, 51115};
        public static int DEFAULT_TCP_PORT = 51110;
        public static int PORT_UDP_BROADCAST = 51111;
        public static int BROADCAST_TIMER_INTERVAL_MSEC = 10000;

        public static IPAddress localIpAddress = null;
        public static int selectedTcpPort = -1;

    }
}