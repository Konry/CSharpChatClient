using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient.Controller.Netzwerk
{
    class NetworkMessage
    {
        public static string GenerateConnectMessage(User user, IPAddress ipAddress, int port)
        {
            return "TRYConnectTo;" + user.name + ";" + user.id + ";" + ipAddress.ToString() + ";" + port;
        }

        //public static string GenerateConnectMessage(User user)
        //{
        //    return GenerateConnectMessage(user, user.ipAddress, user.port);
        //}
        public static string GenerateHeartbeatMessage(bool online)
        {
            return GenerateHeartbeatMessage(Configuration.localUser, Configuration.localIpAddress, Configuration.selectedTcpPort, online);
        }

        public static string GenerateHeartbeatMessage(User user, IPAddress ipAddress, int port, bool online)
        {
            string temp = "Heartbeat;" + user.name + ";" + user.id + ";" + ipAddress.ToString() + ";" + port + ";";
            if (online)
            {
                return temp + "live";
            }
            else
            {
                return temp + "offline";
            }
        }

        public static string GenerateTCPMessage(Message message)
        {
            return "from:" + message.FromUser + ";to:" + message.ToUser + ";" + message.MessageContent;
        }

        public static bool isNewContact(string content)
        {
            string[] stringArray = content.Split(';');
            if (stringArray[0].Equals("TRYConnectTo"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
