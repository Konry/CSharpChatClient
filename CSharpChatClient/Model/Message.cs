using CSharpChatClient.Controller;
using System;
using System.Net;

namespace CSharpChatClient
{
    /// <summary>
    /// The Message is used for message transfer in the application and between different applications.
    /// </summary>
    public class Message
    {
        private string messageType;
        private string messageContent;
        private User fromUser;
        private User toUser;

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

        public static string GenerateConnectMessage(User user, IPAddress ipAddress, int port)
        {
            return "TCPConnectTo;" + user.Name + ";" + user.Id + ";" + ipAddress.ToString() + ";" + port;
        }

        public static string GenerateTCPMessage(Message message)
        {
            return "TCPMessage;" + message.FromUser.Name + ";" + message.FromUser.Id + ";" + message.ToUser.Name + ";" + message.ToUser.Id + ";" + message.MessageContent;
        }

        public static string GenerateTCPNotify(Message message)
        {
            return "TCPNotify;" + message.FromUser.Name + ";" + message.FromUser.Id + ";" + message.MessageContent;
        }

        /// <summary>
        /// Check if the message begins with TCPConnectTo and has more than 5 elements seperated by semicolon
        /// </summary>
        public static bool IsNewContactMessage(string content)
        {
            if (content.StartsWith("TCPConnectTo;"))
            {
                string[] temp = content.Split(';');
                if (temp.Length >= 5)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if the message begins with TCPMessage and has more than 6 elements seperated by semicolon
        /// </summary>
        public static bool IsTCPMessage(string content)
        {
            if (content.StartsWith("TCPMessage;"))
            {
                string[] temp = content.Split(';');
                if (temp.Length >= 6)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsNotifyMessage(string content)
        {
            if (content.StartsWith("TCPNotify;"))
            {
                string[] temp = content.Split(';');
                if (temp.Length >= 3)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Parses a string to a message, awaits a correct formated string, can be tested with method <see cref="IsTCPMessage"/>.
        /// </summary>
        ///
        /// <param name="content">a string formed like
        /// TCPMessage;user.name;user.id;user.name;user.id;message
        /// </param>
        /// <returns>A new message object or null when exception occurs</returns>
        internal static Message ParseTCPMessage(string content)
        {
            string[] temp = content.Split(';');
            if (temp.Length >= 6)
            {
                try
                {
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
                }
                catch (Exception ex)
                {
                    Logger.LogException("ParseTCPMessage", ex);
                }
            }
            return null;
        }

        /// <summary>
        /// Reads a message out of a well formated NewContactMessage, test before if string is correct with method <see cref="IsNewContactMessage"/>.
        /// Format for a new contact message: TCPConnectTo;username;userid;ipaddress;port
        /// </summary>
        /// <param name="content">A well formated </param>
        /// <returns></returns>
        internal static Message ParseNewContactMessage(string content)
        {
            string[] temp = content.Split(';');
            if (temp.Length >= 5)
            {
                try
                {
                    User from = new User(temp[1]);
                    from.Id = long.Parse(temp[2]);
                    string message = temp[3] + ";" + temp[4];
                    return new Message(from, null, message, temp[0]);
                }
                catch (Exception ex)
                {
                    Logger.LogException("ParseNewContactMessage", ex);
                }
            }
            return null;
        }

        internal static Message ParseTCPNotifyMessage(string content)
        {
            string[] temp = content.Split(';');
            if (temp.Length >= 5)
            {
                try
                {
                    User from = new User(temp[1]);
                    from.Id = long.Parse(temp[2]);
                    string message = "";
                    for (int i = 3; i < temp.Length; i++)
                    {
                        message += temp[i];
                    }
                    return new Message(from, null, message, temp[0]);
                }
                catch (Exception ex)
                {
                    Logger.LogException("ParseTCPMessage", ex);
                }
            }
            return null;
        }

        override
        public string ToString()
        {
            try
            {
                if (FromUser == null && ToUser == null)
                {
                    return "null,null - null,null:" + MessageContent;
                }
                else if (ToUser == null)
                {
                    return FromUser.Name + "," + FromUser.Id + " - null,null:" + MessageContent;
                }
                return FromUser.Name + "," + FromUser.Id + " - " + ToUser.Name + "," + ToUser.Id + ":" + MessageContent;
            }
            catch (Exception ex)
            {
                Logger.LogException("Nullpointer in ToString", ex);
            }
            return "";
        }

        public string MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }

        public string MessageContent
        {
            get { return messageContent; }
            set { messageContent = value; }
        }

        public User FromUser
        {
            get { return fromUser; }
            set { fromUser = value; }
        }

        public User ToUser
        {
            get { return toUser; }
            set { toUser = value; }
        }
    }
}