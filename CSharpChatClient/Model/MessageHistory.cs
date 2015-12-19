using System.Collections.Generic;
using System.Text;

namespace CSharpChatClient.Model
{
    public class MessageHistory
    {
        private LinkedList<Message> listOfMessages;
        private StringBuilder stringBuilder;

        public MessageHistory()
        {
            listOfMessages = new LinkedList<Message>();
            stringBuilder = new StringBuilder();
        }

        public void FillUpMessageHistory(Message[] rawHistory)
        {
            foreach (Message message in rawHistory)
            {
                listOfMessages.AddLast(message);
            }
        }

        public void AddMessage(Message message)
        {
            listOfMessages.AddLast(message);
            stringBuilder.Append(message.FromUser.Name + ": " + message.MessageContent + "\r\n");
        }

        public void ClearHistory()
        {
            listOfMessages.Clear();
            stringBuilder.Clear();
        }

        public LinkedList<Message> ListOfMessages
        {
            get { return listOfMessages; }
        }

        public StringBuilder StringBuilder
        {
            get { return stringBuilder; }
        }
    }
}
