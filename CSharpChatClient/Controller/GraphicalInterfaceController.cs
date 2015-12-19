using CSharpChatClient.controller;
using CSharpChatClient.Model;
using System;
using System.Collections.Generic;
using System.Net;

namespace CSharpChatClient.Controller
{
    public class GraphicalInterfaceController
    {
        private ChatForm chatForm = null;
        private ProgramController programControl = null;
        private NetworkService networkService = null;

        private ExternalUser currentlyActiveChatUser = null;
        internal MessageHistory messageHistory = null;

        private LinkedList<ExternalUser> onlineChatPartner = null;
        //private LinkedList<ExternalUser> offlineChatPartner = null;

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

        internal void ChangeUsername(string username)
        {
            Message message = new Message(Configuration.localUser, CurrentlyActiveChatUser, "Rename:"+username);
            username = SetConfigurationUsername(username);
            chatForm.UsernameLabel_UpdateText(username);
            programControl.FileService.UpdateUserName();
            networkService.RenameUsernameNotifyRemote(message);
        }

        internal void SendMessage(string text)
        {
            Message message = new Message(Configuration.localUser, CurrentlyActiveChatUser, text);

            bool success = networkService.SendMessage(message);
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
                Logger.LogError("Fehler: Die letzte Nachricht war nicht vom aktuellen Benutzer");
            }
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
                    ExternalUser ex = new ExternalUser("#ManualConnect");
                    ex.IpAddress = IPAddress.Parse(split[0]);
                    ex.Port = int.Parse(split[1]);
                    if (!programControl.NetworkService.ManualConnectToExUser(ex))
                    {
                        chatForm.InformUser("Verbindung nicht möglich, da die Gegenseite nicht antwortet.");
                    }
                    currentlyActiveChatUser = ex;
                }
                catch (Exception e)
                {
                    Logger.LogException("Fehlerhafter String ", e);
                }
            }
        }

        public ExternalUser CurrentlyActiveChatUser
        {
            get { return currentlyActiveChatUser; }
            set { currentlyActiveChatUser = value; chatForm.NotifyConnectedWithChange(); }
        }

        private static string GenerateRandomName()
        {
            Random random = new Random();
            return "Namenslos" + random.Next(1, 999);
        }

        private static string SetConfigurationUsername(string username)
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

            return username;
        }

        internal void ClearHistory()
        {
            messageHistory.ClearHistory();
            chatForm.UpdateMessageHistory();
        }
    }
}