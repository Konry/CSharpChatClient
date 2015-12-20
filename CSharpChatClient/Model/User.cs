using System;

namespace CSharpChatClient
{
    public class User
    {
        protected string name = "";
        protected long id = -1;

        public User(string name)
        {
            this.name = name;
            this.id = -1;
        }

        public User(string name, long id)
        {
            this.name = name;
            this.id = id;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Check the equality of the user with an other one, depending on their name and their id
        /// </summary>
        /// <param name="user"></param>
        /// <returns>true for equality</returns>
        public bool Equals(User user)
        {
            if (this.name.Equals(user.name) && this.id.Equals(user.id))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Generates a long int as random user id
        /// </summary>
        /// <returns></returns>
        public static long GenerateUserID()
        {
            Random random = new Random();
            byte[] buffer = new byte[8];
            random.NextBytes(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Generates a name of type "Namenslos "+ number from 1 to 999
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomNameless()
        {
            Random random = new Random();
            return "Namenslos" + random.Next(1, 999);
        }
    }
}