using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace CSharpChatClient
{
    public class Message
    {
        public string MessageContent { get; set; }
        public User FromUser { get; set; }
        public User ToUser { get; set; }

        public Message(string MessageContent)
        {
            this.MessageContent = MessageContent;
        }

        public Message(User from, User to, string MessageContent)
        {
            this.FromUser = from;
            this.ToUser = to;
            this.MessageContent = MessageContent;
        }

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
            return "" + message.FromUser.name + ";" + message.FromUser.id + ";" + message.ToUser.name + ";" + message.ToUser.id + ";" + message.MessageContent;
        }

        public static bool IsNewContact(string content)
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

        internal static Message ParseTCPMessage(string content)
        {
            string[] temp = content.Split(';');
            if (temp.Length != 5) {
                try {
                    User from = new User(temp[0]);
                    from.id = long.Parse(temp[1]);
                    User to = new User(temp[2]);
                    to.id = long.Parse(temp[3]);
                    return new Message(from, to, temp[4]);
                } catch (FormatException fe )
                {
                    Debug.WriteLine("Number has not the correct format! "+fe.StackTrace);
                }
                catch (OverflowException oe)
                {
                    Debug.WriteLine("Number overflow, is greater than long! " + oe.StackTrace);
                }
                catch (ArgumentException ae)
                {
                    Debug.WriteLine("Wrong argument for parsing! " + ae.StackTrace);
                }
            } 
            return null;
        }
    }
}