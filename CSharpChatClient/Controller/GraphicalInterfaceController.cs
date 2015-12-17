using CSharpChatClient.controller;
using CSharpChatClient.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient.Controller
{
    public class GraphicalInterfaceController
    {
        private ChatForm chatForm;
        private ProgramController programControl = null;
        private NetworkService networkService = null;

        private User currentlyActiveChatUser = null;
        private MessageHistory messageHistory = null;

        private HashSet<ExternalUser> onlineChatPartner = null;
        //private 

        public GraphicalInterfaceController(ProgramController programControl, ChatForm chatForm, NetworkService networkService)
        {
            this.programControl = programControl;
            this.chatForm = chatForm;
            this.networkService = networkService;
            Initialize(); 
        }

        private void Initialize()
        {
            messageHistory = new MessageHistory();
            onlineChatPartner = new HashSet<ExternalUser>();

            /* TODO load history from file */
            //messageHistory.FillUpMessageHistory(programControl.fileService.ReadHistoryFile());
        }

        internal void ChangeUsername(string username)
        {
            if (Configuration.localUser == null)
            {
                if (username.Equals(""))
                {
                    username = GenerateRandomName();
                } 
                Configuration.localUser = new User(username);
            }
            else
            {
                if (username.Equals("") && Configuration.localUser.Name.Equals(""))
                {
                    username = GenerateRandomName();
                }
                else if(username.Equals(""))
                {
                    username = Configuration.localUser.Name;
                }
                Configuration.localUser.Name = username;
            }
            chatForm.UpdateUsernameLabel(username);
            programControl.fileService.UpdateUserName();
        }

        private static string GenerateRandomName()
        {
            Random random = new Random();
            return "Namenslos" + random.Next(1, 999);
        }

        internal void SendMessage(string text)
        {
            Message message = new Message(Configuration.localUser, currentlyActiveChatUser, text);

            networkService.SendMessage(message);
            messageHistory.AddMessage(message);
        }

        internal void ReceiveMessage(Message message)
        {
            if(message.FromUser.Equals(currentlyActiveChatUser)){
                messageHistory.AddMessage(message);
            } else
            {
                Debug.WriteLine("Fehler: Die letzte Nachricht war nicht vom aktuellen Benutzer");
            }
        }

        internal void BroadcastRemove(ExternalUser exUser)
        {
            bool updateGui = onlineChatPartner.Remove(exUser);
            if (updateGui)
            {
                ExternalUser[] namesOfOnlineChatPartner = new ExternalUser[onlineChatPartner.Count];
                int index = 0;
                foreach (ExternalUser ex in onlineChatPartner)
                {
                    namesOfOnlineChatPartner[index++] = ex;
                }
                chatForm.UpdateListOfClients(namesOfOnlineChatPartner);
            }
        }

        internal void BroadcastAdd(ExternalUser exUser)
        {
            bool updateGui = onlineChatPartner.Add(exUser);
            if (updateGui)
            {
                ExternalUser[] namesOfOnlineChatPartner = new ExternalUser[onlineChatPartner.Count];
                int index = 0;
                foreach (ExternalUser ex in onlineChatPartner)
                {
                    namesOfOnlineChatPartner[index++] = ex;
                }
                chatForm.UpdateListOfClients(namesOfOnlineChatPartner);
            }

            Debug.WriteLine("ADD broadcast " + updateGui);
            //onlineChatPartner.Remove
        }
    }
}
