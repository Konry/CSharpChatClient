using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient.Model
{
    public class ExternalUser : User, INotifyPropertyChanged
    {
        private IPAddress _ipAddress { get; set; }
        private int _port { get; set; }

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
            get { return _port; }
            set { _port = value; NotifyPropertyChanged("Port"); }
        }
        public IPAddress IpAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; NotifyPropertyChanged("IpAddress"); }
        }
        public new string Name
        {
            get { return _name; }
            set { _name = value; NotifyPropertyChanged("Name"); }
        }
        public new long Id
        {
            get { return _id; }
            set { _id = value; NotifyPropertyChanged("Id"); }
        }

        public bool Equals(ExternalUser user)
        {
            return base.Equals(user);
        }

        public static ExternalUser ParseFromMessage(Message message)
        {
            try
            {
                ExternalUser exUser = new ExternalUser(message.FromUser);
                String[] split = message.MessageContent.Split(';');
                exUser._ipAddress = IPAddress.Parse(split[0]);
                exUser._port = int.Parse(split[1]);
                return exUser;
            } catch (FormatException fe)
            {
                Debug.WriteLine("FormatException while parsing a broadcast message." + fe.StackTrace);
            } catch (ArgumentException ae)
            {
                Debug.WriteLine("ArgumentException while parsing a broadcast message." + ae.StackTrace);
            } catch (OverflowException oe)
            {
                Debug.WriteLine("OverflowException while parsing a broadcast message." + oe.StackTrace);
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
