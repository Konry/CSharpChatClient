﻿using System.Net;
using static CSharpChatClient.Controller.Logger;

namespace CSharpChatClient
{

    /// <summary>
    /// Hard coded configurations, ports and more...
    /// </summary>
    public struct Configuration
    {
        public static int[] PORT_TCP = {51110, 51111, 51112, 51113, 51114, 51115};
        public const int DEFAULT_TCP_PORT = 51110;
        public const int BROADCAST_TIMER_INTERVAL_MSEC = 10000;

        public static User localUser = null;
        public static IPAddress localIpAddress = null;
        public static int selectedTcpPort = -1;

        public static LogState logLevel = LogState.INFO;
    }
}