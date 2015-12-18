using CSharpChatClient.controller;
using CSharpChatClient.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSharpChatClient.Controller
{
    public class GraphicalInterfaceController
    {
        private ChatForm chatForm;
        private ProgramController programControl = null;
        private NetworkService networkService = null;

        private ExternalUser _currentlyActiveChatUser = null;
        internal MessageHistory messageHistory = null;

        private LinkedList<ExternalUser> onlineChatPartner = null;
        private LinkedList<ExternalUser> offlineChatPartner = null;
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
            onlineChatPartner = new LinkedList<ExternalUser>();

            /* TODO load history from file */
            //messageHistory.FillUpMessageHistory(programControl.fileService.ReadHistoryFile());
        }

        public ExternalUser CurrentlyActiveChatUser
        {
            get { return _currentlyActiveChatUser; }
            set { _currentlyActiveChatUser = value; chatForm.NotifyConnectedWithChange(); }
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
                else if (username.Equals(""))
                {
                    username = Configuration.localUser.Name;
                }
                Configuration.localUser.Name = username;
            }
            chatForm.UpdateUsernameLabel(username);
            programControl.FileService.UpdateUserName();
        }

        private static string GenerateRandomName()
        {
            Random random = new Random();
            return "Namenslos" + random.Next(1, 999);
        }

        internal void SendMessage(string text)
        {
            Message message = new Message(Configuration.localUser, CurrentlyActiveChatUser, text);

            bool success = networkService.SendMessage(message, CurrentlyActiveChatUser);
            if (success)
            {
                messageHistory.AddMessage(message);
                chatForm.UpdateMessageHistory();
            }
        }

        internal void InitiateCurrentlyActiveUser(Message message)
        {
            if (CurrentlyActiveChatUser.Name.StartsWith("#ManualConnect") || CurrentlyActiveChatUser.Name.StartsWith("#AutoConnect"))
            {
                CurrentlyActiveChatUser.Name = message.FromUser.Name;
                CurrentlyActiveChatUser.Id = message.FromUser.Id;
                chatForm.NotifyConnectedWithChange();
            }
        }

        internal void ReceiveMessage(Message message)
        {
            
            if (message.FromUser.Equals(CurrentlyActiveChatUser))
            {
                messageHistory.AddMessage(message);
                chatForm.UpdateMessageHistory();
            }
            else
            {
                Debug.WriteLine("Fehler: Die letzte Nachricht war nicht vom aktuellen Benutzer");
            }
        }

        internal void BroadcastRemove(ExternalUser exUser)
        {
            bool updateGui = false;
            ExternalUser toRemove = null;
            foreach (ExternalUser ex in onlineChatPartner)
            {
                if (ex.Equals(exUser))
                {
                    updateGui = true;
                    toRemove = ex;
                    break;
                }
            }

            if (updateGui)
            {
                onlineChatPartner.Remove(toRemove);
                ExternalUser[] namesOfOnlineChatPartner = new ExternalUser[onlineChatPartner.Count];
                int index = 0;
                foreach (ExternalUser ex in onlineChatPartner)
                {
                    Debug.WriteLine("names " + ex.Name);
                    namesOfOnlineChatPartner[index++] = ex;
                }
                chatForm.RemoveItemFromListOfClients(namesOfOnlineChatPartner);
            }
        }

        internal void BroadcastAdd(ExternalUser exUser)
        {
            if (exUser.Equals(new ExternalUser(Configuration.localUser, Configuration.localIpAddress, Configuration.selectedTcpPort)))
            {
                return;
            }
            bool updateGui = true;
            foreach (ExternalUser ex in onlineChatPartner)
            {
                if (ex.Equals(exUser))
                {
                    updateGui = false;
                    break;
                }
            }

            if (updateGui)
            {
                onlineChatPartner.AddLast(exUser);
                ExternalUser[] namesOfOnlineChatPartner = new ExternalUser[onlineChatPartner.Count];
                int index = 0;
                foreach (ExternalUser ex in onlineChatPartner)
                {
                    namesOfOnlineChatPartner[index++] = ex;
                }
                chatForm.AddItemFromListOfClients(namesOfOnlineChatPartner);
            }

            //onlineChatPartner.Remove
        }

        internal void ManuelConnectToIPAndPort(string ipAndPort)
        {
            if (ipAndPort == "")
            {
                return;
            }
            else
            {
                String[] split = ipAndPort.Split(':');
                try
                {
                    Debug.WriteLine("ipandport: " + split[0]);
                    Debug.WriteLine("ipandport: " + split[1]);
                    ExternalUser ex = new ExternalUser("#ManualConnect");
                    ex.IpAddress = IPAddress.Parse(split[0]);
                    ex.Port = int.Parse(split[1]);
                    if (programControl.NetworkService.ManualConnectToExUser(ex))
                    {

                    }
                    _currentlyActiveChatUser = ex;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Fehlerhafter String " + e.StackTrace);
                }
            }
        }
    }
}
