using System;

namespace CSharpChatClient
{
    internal class ClientException : Exception
    {
        public ClientException(string message) : base(message)
        {
        }

        public ClientException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }

    public class PortIsNotFreeException : Exception
    {
        public PortIsNotFreeException(string message) : base(message)
        {
        }

        public PortIsNotFreeException(string message, Exception inner)
    : base(message, inner)
        {
        }
    }

    public class AlreadyConnectedException : Exception
    {
        public AlreadyConnectedException(string message) : base(message)
        {
        }

        public AlreadyConnectedException(string message, Exception inner)
    : base(message, inner)
        {
        }
    }
}