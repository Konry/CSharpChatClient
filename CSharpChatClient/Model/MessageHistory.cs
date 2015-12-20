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

        /// <summary>
        /// Fills the list up with a amount of messages
        /// </summary>
        /// <param name="rawHistory"></param>
        public void FillUpMessageHistory(Message[] rawHistory)
        {
            foreach (Message message in rawHistory)
            {
                listOfMessages.AddLast(message);
            }
        }

        /// <summary>
        /// Adds a message to the history
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(Message message)
        {
            listOfMessages.AddLast(message);
            stringBuilder.Append(message.FromUser.Name + ": " + message.MessageContent + "\r\n");
        }

        /// <summary>
        /// Clears the content of the objects which containing the message history
        /// </summary>
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