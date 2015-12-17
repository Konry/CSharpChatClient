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
        public string MessageType { get; set; }
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
            this.MessageType = "";
        }

        public Message(User from, User to, string MessageContent, string MessageType) : this(from, to, MessageContent)
        {
            this.MessageType = MessageType;
        }

        override
        public string ToString()
        {
            if ( FromUser == null && ToUser == null)
            {
                return "null,null - null,null:" + MessageContent;
            } else  if (ToUser == null)
            {
                return FromUser.Name + "," + FromUser.Id + " - null,null:" + MessageContent;
            } 
            return FromUser.Name + "," + FromUser.Id + " - " + ToUser.Name + "," + ToUser.Id + ":" + MessageContent;
        }

        public static string GenerateConnectMessage(User user, IPAddress ipAddress, int port)
        {
            return "TCPConnectTo;" + user.Name + ";" + user.Id + ";" + ipAddress.ToString() + ";" + port;
        }

        public static string GenerateHeartbeatMessage(bool online)
        {
            return GenerateHeartbeatMessage(Configuration.localUser, Configuration.localIpAddress, Configuration.selectedTcpPort, online);
        }

        public static string GenerateHeartbeatMessage(User user, IPAddress ipAddress, int port, bool online)
        {
            string modus = "";
            if (online)
            {
                modus += "Live";
            }
            else
            {
                modus += "Offline";
            }
            return "Heartbeat"+modus+";" + user.Name + ";" + user.Id + ";" + ipAddress.ToString() + ";" + port;
        }

        public static string GenerateTCPMessage(Message message)
        {
            return "TCPMessage;" + message.FromUser.Name + ";" + message.FromUser.Id + ";" + message.ToUser.Name + ";" + message.ToUser.Id + ";" + message.MessageContent;
            //return message.MessageContent;
        }

        public static bool IsNewContact(string content)
        {
            string[] stringArray = content.Split(';');
            if (stringArray[0].Equals("TCPConnectTo"))
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
                    from.Id = long.Parse(temp[2]);
                    User to = new User(temp[3]);
                    to.Id = long.Parse(temp[4]);
                    string message = "";
                    for (int i = 5; i < temp.Length; i++)
                    {
                        message += temp[i];
                    }
                    return new Message(from, to, message, temp[0]);
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

        internal static Message ParseBroadcastMessage(string content)
        {
            string[] temp = content.Split(';');
            Debug.WriteLine(content);
            if (temp.Length >= 5)
            {
                try
                {
                    User from = new User(temp[1]);
                    from.Id = long.Parse(temp[2]);
                    string message = temp[3] + ";"+ temp[4];
                    return new Message(from, null, message, temp[0]);
                }
                catch (FormatException fe)
                {
                    Debug.WriteLine("Number has not the correct format! " + fe.StackTrace);
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