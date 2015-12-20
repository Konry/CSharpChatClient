using CSharpChatClient.Controller;
using System;
using System.ComponentModel;
using System.Net;

namespace CSharpChatClient.Model
{
    /// <summary>
    /// The ExtendedUser extends the User by the address information and port information.
    /// </summary>
    public class ExtendedUser : User, INotifyPropertyChanged
    {
        private IPAddress ipAddress;
        private int port;

        public ExtendedUser(string name) : base(name)
        {
        }

        public ExtendedUser(User user) : this(user.Name)
        {
            this.Id = user.Id;
        }

        public ExtendedUser(User user, IPAddress ipAddress, int port) : this(user.Name)
        {
            this.Id = user.Id;
            this.IpAddress = ipAddress;
            this.Port = port;
        }

        /// <summary>
        /// Checks if username and id are equal
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool Equals(ExtendedUser user)
        {
            return base.Equals(user);
        }

        /// <summary>
        /// Parses from a tcp connect <see cref="Message"/> the external user and their port and ip address
        /// </summary>
        /// <param name="message">Needs an TCP-connect message, more information at <see cref="Message"/></param>
        /// <returns>The external user out of this message</returns>
        public static ExtendedUser ParseFromMessage(Message message)
        {
            try
            {
                Logger.LogFatal(message.MessageContent);
                ExtendedUser exUser = new ExtendedUser(message.FromUser);
                String[] split = message.MessageContent.Split(';');
                exUser.ipAddress = IPAddress.Parse(split[0]);
                exUser.port = int.Parse(split[1]);
                return exUser;
            }
            catch (Exception ex)
            {
                Logger.LogException("Exception while parsing a message. ", ex);
            }
            return null;
        }

        public int Port
        {
            get { return port; }
            set { port = value; NotifyPropertyChanged("Port"); }
        }

        public IPAddress IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; NotifyPropertyChanged("IpAddress"); }
        }

        public new string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged("Name"); }
        }

        public new long Id
        {
            get { return id; }
            set { id = value; NotifyPropertyChanged("Id"); }
        }

        /// <summary>
        /// Creates an extendedUser out of the configuration information
        /// </summary>
        /// <returns></returns>
        public static ExtendedUser ConfigurationToExtendedUser()
        {
            ExtendedUser exUser = new ExtendedUser(Configuration.localUser);
            exUser.ipAddress = Configuration.localIpAddress;
            exUser.port = Configuration.selectedTcpPort;
            return exUser;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}