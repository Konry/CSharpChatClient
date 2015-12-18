using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient.Model
{
    class UserList
    {
        public LinkedList<ExternalUser> userList { get; }

        public UserList()
        {
            userList = new LinkedList<ExternalUser>();
        }

        public bool AddUser(ExternalUser user)
        {
            foreach (ExternalUser u in userList)
            {
                if(user.Equals(u))
                {
                    return false;
                }
            }
            userList.AddLast(user);
            OnPropertyChanged("userList");
            return true;
        }

        public bool RemoveUser(ExternalUser user)
        {
            foreach (ExternalUser u in userList)
            {
                if (user.Equals(u))
                {
                    userList.Remove(user);
                    return true;
                }
            }
            OnPropertyChanged("userList");
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
