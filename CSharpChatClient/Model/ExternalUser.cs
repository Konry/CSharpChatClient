﻿using CSharpChatClient.Controller;
using System;
using System.ComponentModel;
using System.Net;

namespace CSharpChatClient.Model
{
    public class ExternalUser : User, INotifyPropertyChanged
    {
        private IPAddress ipAddress;
        private int port;

        public ExternalUser(string name) : base(name)
        {

        }

        public ExternalUser(User user) : this(user.Name)
        {
            this.Id = user.Id;
        }

        public ExternalUser(User user, IPAddress ipAddress, int port) : this(user.Name)
        {
            this.Id = user.Id;
            this.IpAddress = ipAddress;
            this.Port = port;
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

        public bool Equals(ExternalUser user)
        {
            return base.Equals(user);
        }


        /// <summary>
        /// Parses from a tcp connect <see cref="Message"/> the external user and their port and ip address
        /// </summary>
        /// <param name="message">Needs an TCP-connect message, more information at <see cref="Message"/></param>
        /// <returns>The external user out of this message</returns>
        public static ExternalUser ParseFromMessage(Message message)
        {
            try
            {
                ExternalUser exUser = new ExternalUser(message.FromUser);
                String[] split = message.MessageContent.Split(';');
                exUser.ipAddress = IPAddress.Parse(split[0]);
                exUser.port = int.Parse(split[1]);
                return exUser;
            } catch (FormatException fe)
            {
                Logger.LogException("FormatException while parsing a broadcast message. ", fe);
            } catch (ArgumentException ae)
            {
                Logger.LogException("ArgumentException while parsing a broadcast message. ", ae);
            } catch (OverflowException oe)
            {
                Logger.LogException("OverflowException while parsing a broadcast message. ", oe);
            }
            return null;
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
