using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient.Model
{
    class UserList
    {
        LinkedList<User> userList = null;

        public UserList()
        {
            userList = new LinkedList<User>();
        }

        public bool AddUser(User user)
        {
            foreach ( User u in userList)
            {
                if(user.name == u.name && user.ipAddress == u.ipAddress)
                {
                    return false;
                }
            }
            userList.AddLast(user);
            return true;
        }

        public bool RemoveUser(User user)
        {
            foreach (User u in userList)
            {
                if (user.name == u.name && user.ipAddress == u.ipAddress)
                {
                    userList.Remove(user);
                    return true;
                }
            }
            return false;
        }
    }
}
