using System;

namespace CSharpChatClient
{
    internal class ClientException : Exception
    {
        public ClientException() : base()
        {
        }

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