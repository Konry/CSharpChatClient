using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient.Model
{
    public class MessageHistory
    {
        public LinkedList<Message> listOfMessages{ get; }

        public MessageHistory()
        {
            listOfMessages = new LinkedList<Message>();
        }

        public void FillUpMessageHistory(Message[] rawHistory)
        {
            foreach(Message message in rawHistory)
            {
                listOfMessages.AddLast(message);
            }
        }

        public void AddMessage(Message message)
        {
            listOfMessages.AddLast(message);
        }

        public void ClearHistory()
        {
            listOfMessages.Clear();
        }
        
    }
}
