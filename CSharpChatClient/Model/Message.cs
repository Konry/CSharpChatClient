using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpChatClient
{
    public class Message
    {
        private string _MessageContent;
        private string _FromUser;
        private string _ToUser;
      
        public Message(String MessageContent)
        {
            this.MessageContent = MessageContent;
        }

        public string MessageContent
        {
            get
            {
                return _MessageContent;
            }

            set
            {
                _MessageContent = value;
            }
        }

        public string FromUser
        {
            get
            {
                return _FromUser;
            }

            set
            {
                _FromUser = value;
            }
        }

        public string ToUser
        {
            get
            {
                return _ToUser;
            }

            set
            {
                _ToUser = value;
            }
        }

    }
}