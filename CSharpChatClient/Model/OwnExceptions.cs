using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient
{
    class ClientException : Exception
    {

        public ClientException()
        {

        }

        public ClientException(string message) : base(message)
        {

        }
        public ClientException(string message, Exception inner)
        : base(message, inner)
        {

        }

        public class PortIsNotFreeException : ClientException {
            public PortIsNotFreeException()
            {

            }

            public PortIsNotFreeException(string message) : base(message)
            {

            }

            public PortIsNotFreeException(string message, Exception inner)
        : base(message, inner)
            {

            }
        }
    }
}
