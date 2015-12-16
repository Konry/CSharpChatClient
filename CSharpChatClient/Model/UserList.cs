using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient.Model
{
    class UserList
    {
        public LinkedList<User> userList { get; }

        public UserList()
        {
            userList = new LinkedList<User>();
        }

        public bool AddUser(User user)
        {
            foreach ( User u in userList)
            {
                if(user.Equals(u))
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
                if (user.Equals(u))
                {
                    userList.Remove(user);
                    return true;
                }
            }
            return false;
        }
    }
}
