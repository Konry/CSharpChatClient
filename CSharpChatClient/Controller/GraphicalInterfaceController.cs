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

        private ExtendedUser currentlyActiveChatUser = null;
        internal MessageHistory messageHistory = null;

        private LinkedList<ExtendedUser> onlineChatPartner = null;
        //private LinkedList<ExtendedUser> offlineChatPartner = null;

        public GraphicalInterfaceController(ProgramController programControl, ChatForm chatForm)
        {
            this.programControl = programControl;
            this.chatForm = chatForm;
            Initialize();
        }

        private void Initialize()
        {
            messageHistory = new MessageHistory();
            onlineChatPartner = new LinkedList<ExtendedUser>();
        }
        /// <summary>
        /// Changes the username and inform all instances
        /// </summary>
        /// <param name="username"></param>
        internal void ChangeUsername(string username)
        {
            Logger.LogInfo("changeto " + username);
            Message message = new Message(Configuration.localUser, CurrentlyActiveChatUser, "Rename:" + username);
            username = SetConfigurationUser(username);
            chatForm.UsernameLabel_UpdateText(username);
            programControl.FileService.UpdateUserName();
            programControl.NetworkService.RenameUsernameNotifyRemote(message);
        }

        /// <summary>
        /// Sends a new message to the network service and the updates the message history
        /// </summary>
        /// <param name="text"></param>
        internal void SendMessage(string text)
        {
            Message message = new Message(Configuration.localUser, CurrentlyActiveChatUser, text);

            bool success = programControl.NetworkService.SendMessage(message);
            if (success)
            {
                messageHistory.AddMessage(message);
                chatForm.UpdateMessageHistory();
            }
        }

        /// <summary>
        /// Initiate the current user
        /// </summary>
        /// <param name="message">Containing the new currentliy active user</param>
        internal void InitiateCurrentlyActiveUser(Message message)
        {
            if (CurrentlyActiveChatUser.Name.StartsWith("#ManualConnect") || CurrentlyActiveChatUser.Name.StartsWith("#AutoConnect"))
            {
                CurrentlyActiveChatUser.Name = message.FromUser.Name;
                CurrentlyActiveChatUser.Id = message.FromUser.Id;
                chatForm.NotifyConnectedWithChange();
            }
            else if (message.MessageContent.StartsWith("Rename:"))
            {
                string name = message.MessageContent.Replace("Rename:", "");
                Logger.LogInfo("Rename Currentyl active chat user to " + name);
                CurrentlyActiveChatUser.Name = name;
                chatForm.NotifyConnectedWithChange();
            }
        }


        /// <summary>
        /// Receive a message from a controller, updates the message history.
        /// Accept the message when currentlyActiveChatUser is matching the FromUser of the message.
        /// </summary>
        /// <param name="message"></param>
        internal void ReceiveMessage(Message message)
        {
            if (message.FromUser.Equals(CurrentlyActiveChatUser))
            {
                messageHistory.AddMessage(message);
                chatForm.UpdateMessageHistory();
            }
            else
            {
                Logger.LogError("Error, the last messsage was not from the currently active user.");
            }
        }

        /// <summary>
        /// Connect manually to the selected ip and port. 
        /// </summary>
        /// <param name="ipAndPort"></param>
        internal void ManualConnectToIPAndPort(string ipAndPort)
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
                    ExtendedUser ex = new ExtendedUser("#ManualConnect");
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

        public ExtendedUser CurrentlyActiveChatUser
        {
            get { return currentlyActiveChatUser; }
            set { currentlyActiveChatUser = value; chatForm.NotifyConnectedWithChange(); }
        }

        /// <summary>
        /// Sets the current configuration
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        internal static string SetConfigurationUser(string username)
        {
            if (Configuration.localUser == null)
            {
                if (username.Equals(""))
                {
                    username = User.GenerateRandomNameless();
                }
                Configuration.localUser = new User(username, User.GenerateUserID());
            }
            else
            {
                if (username.Equals("") && Configuration.localUser.Name.Equals(""))
                {
                    username = User.GenerateRandomNameless();
                }
                else if (username.Equals(""))
                {
                    username = Configuration.localUser.Name;
                }
                Configuration.localUser.Name = username;
            }

            return username;
        }

        /// <summary>
        /// Informs the gui and the message history to clear the history
        /// </summary>
        internal void ClearHistory()
        {
            messageHistory.ClearHistory();
            chatForm.UpdateMessageHistory();
        }

        internal void InformUser(string text)
        {
            chatForm.InformUser(text);
        }
    }
}