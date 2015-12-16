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
            return "TCPMessage;" + message.FromUser.name + ";" + message.FromUser.id + ";" + message.ToUser.name + ";" + message.ToUser.id + ";" + message.MessageContent;
            //return message.MessageContent;
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

        public static bool IsTCPMessage(string content)
        {
            if (content.StartsWith("TCPMessage;"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Parses a string to a message, awaits a correct formated string
        /// </summary>
        /// 
        /// <param name="content">a string formed like 
        /// TCPMessage;user.name;user.id;user.name;user.id;message 
        /// </param>
        /// <returns>A new message object or null when exception occurs</returns>
        internal static Message ParseTCPMessage(string content)
        {
            string[] temp = content.Split(';');
            Debug.WriteLine(content);
            if (temp.Length >= 6) {
                try {
                    User from = new User(temp[1]);
                    from.id = long.Parse(temp[2]);
                    User to = new User(temp[3]);
                    to.id = long.Parse(temp[4]);
                    string message = "";
                    for (int i = 5; i < temp.Length; i++)
                    {
                        message += temp[i];
                    }
                    return new Message(from, to, temp[5]);
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
                catch (IndexOutOfRangeException ioore)
                {
                    Debug.WriteLine("Sorry, big mistake! " + ioore.StackTrace);
                }
            } 
            return null;
        }
    }
}