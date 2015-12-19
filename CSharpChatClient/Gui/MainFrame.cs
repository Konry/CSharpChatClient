﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using CSharpChatClient.Controller;
using CSharpChatClient.controller;
using CSharpChatClient.Model;
using System.Threading;

namespace CSharpChatClient
{
    public partial class ChatForm : Form
    {
        private ProgramController programControl = null;
        private GraphicalInterfaceController graphicControl = null;

        public static ExternalUserList exUserList = null;
        //private bool exUserListChanged = false;

        private Object thisLock = new Object();

        public delegate void AvailableConnectionsListDelegate();
        public delegate void MessageBoxDelegate();
        public delegate void ConnectedWithLabelDelegate();
        public delegate void UsernameLabelDelegate(string username);

        public ChatForm()
        {
            InitializeController();

            InitializeComponent();
            InitializeContent();
            InitializeUsername();
        }

        ~ChatForm()
        {
            programControl.Stop();
        }

        private void InitializeController()
        {
            programControl = new ProgramController(this);
            this.graphicControl = programControl.GraphicControl;
        }

        private void InitializeContent()
        {
            this.StartPosition = FormStartPosition.CenterScreen;

            sendButton.Enabled = false;
            enterTextBox.Multiline = false;

            exUserList = new ExternalUserList();
            this.FormClosing += ChatForm_OnFormClosing;
        }

        private void InitializeUsername()
        {
            if (Configuration.localUser.Name.Equals(""))
            {
                GetInitialUsername();
            }
            else
            {
                SetOtherUsername();
            }
        }

        private void GetInitialUsername()
        {
            string username = ShowEnterUsernameBox("Bitte geben Sie einen Benutzernamen an:");
            graphicControl.ChangeUsername(username);
        }
        
        private void SetOtherUsername()
        {
            string username = ShowEnterUsernameBox("Der Benutzername ist "+Configuration.localUser.Name+ ", wenn nicht einfach neuen eingeben:");
            graphicControl.ChangeUsername(username);
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            labelIpAddressValue.Text = Configuration.localIpAddress.ToString();
            labelPortValue.Text = Configuration.selectedTcpPort.ToString();

            ToolTip toolTips = new ToolTip();
            toolTips.AutoPopDelay = 10000;
            toolTips.InitialDelay = 2000;
            toolTips.ReshowDelay = 500;
            toolTips.ShowAlways = true;

            toolTips.SetToolTip(this.sendButton, "Sendet die Nachricht, alternativ mit der Enter Taste.");
            toolTips.SetToolTip(this.buttonClearHistory, "Sendet die Nachricht, alternativ mit der Enter Taste.");
        }

        private void AvailableConnectionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("SelectedIndexChanged on SelectedIndexChanged!");
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void UserNameLabel_Click(object sender, EventArgs e)
        {
            String username = ShowEnterUsernameBox("Sie wollen ihren Benutzernamen ändern?");
            graphicControl.ChangeUsername(username);
        }

        private void MessageFlowBox_TextChanged(object sender, EventArgs e)
        {
            // Scroll automatically down.
            messageFlowBox.SelectionStart = messageFlowBox.Text.Length; 
            messageFlowBox.ScrollToCaret();
        }

        private void EnterTextBox_TextChanged(object sender, EventArgs e)
        {
            if (enterTextBox.Text.Length > 0)
            {
                sendButton.Enabled = true;
            } else
            {
                sendButton.Enabled = false;
            }
        }

        private void EnterTextBox_OnKeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Debug.WriteLine("Enter Pressed");
                SendMessage();
            }
        }

        private void ChatForm_OnFormClosing(object sender, EventArgs e)
        {
            programControl.Stop();
            Application.Exit();
        }

        private void labelConnectedWithValue_Click(object sender, EventArgs e)
        {
            String ipAndPort = ShowEnterIpAddressAndPort();

            graphicControl.ManuelConnectToIPAndPort(ipAndPort);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void MessageFlowBox_UpdateText()
        {
            messageFlowBox.Text = graphicControl.messageHistory.StringBuilder.ToString();
        }

        private void LabelConnectedWith_UpdateText()
        {
            if (graphicControl.CurrentlyActiveChatUser != null)
            {
                labelConnectedWithValue.Text = graphicControl.CurrentlyActiveChatUser.Name;
            }
            else
            {
                labelConnectedWithValue.Text = "---";
            }
        }

        private void UsernameLabel_UpdateText(string username)
        {
            labelUserName.Text = username;
        }


        /// <summary>
        /// Notitfy the GUI-Thread to update messageFlowBox Text
        /// </summary>
        internal void UpdateMessageHistory()
        {
            messageFlowBox.BeginInvoke(new MessageBoxDelegate(MessageFlowBox_UpdateText));
        }

        /// <summary>
        /// Notify the GUI-Thread to update the labelConnectedWithValue
        /// </summary>
        internal void NotifyConnectedWithChange()
        {
            labelConnectedWithValue.BeginInvoke(new ConnectedWithLabelDelegate(LabelConnectedWith_UpdateText));
        }

        /// <summary>
        /// Notify the GUI-Thread to update the ConnectedWithLabel
        /// </summary>
        internal void NotifyUsernameChange(string username)
        {
            labelUserName.BeginInvoke(new UsernameLabelDelegate(UsernameLabel_UpdateText));
        }

        private void LoadAvaiableConnectionsList()
        {
            lock (thisLock)
            {
                //if (exUserListChanged)
               // {
                    availableConnectionsList.Items.Clear();
                    foreach (ExternalUser exUser in exUserList)
                    {
                        if (!availableConnectionsList.Items.Contains(exUser.Name))
                        {
                            availableConnectionsList.Items.Add(exUser.Name);
                        }
                    }
                    //exUserListChanged = false;
                //}
            }
        }

        private void UpdateLabelIpAddress()
        {
            labelConnectedWithValue.Text = Configuration.localIpAddress.ToString();
        }

        private void UpdateLabelPort()
        {
            labelPortValue.Text = Configuration.selectedTcpPort.ToString();
        }

        private string ShowEnterUsernameBox(String text)
        {
            Form prompt = InitializePrompt("Benutzernamen eingeben");
            InitializePopUpButtons(prompt);
            InitializePopUpTextLabel(prompt, text);

            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };            
            prompt.Controls.Add(textBox);

            DialogResult result = prompt.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return "";
            }
            return result == DialogResult.OK ? textBox.Text : "";
        }


        private string ShowEnterIpAddressAndPort()
        {
            Form prompt = InitializePrompt("Verbinden mit...");
            InitializePopUpButtons(prompt);
            InitializePopUpTextLabel(prompt, "IPAdresse und Port bitte angeben.");

            TextBox textIpAddress = new TextBox() { Left = 50, Top = 50, Width = 150, Text = Configuration.localIpAddress.ToString() };
            TextBox textPort = new TextBox() { Left = 250, Top = 50, Width = 50, Text = Configuration.selectedTcpPort.ToString() };

            prompt.Controls.Add(textIpAddress);
            prompt.Controls.Add(textPort);

            DialogResult result = prompt.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return "";
            }
            return result == DialogResult.OK ? (textIpAddress.Text + ":" + textPort.Text) : "";
        }

        private static void InitializePopUpTextLabel(Form prompt, string text)
        {
            Label textLabel = new Label() { Left = 50, Top = 20, Width = 350, Text = text };
            prompt.Controls.Add(textLabel);
        }

        private void InitializePopUpButtons(Form prompt)
        {
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Abbrechen", Left = 250, Width = 100, Top = 70, DialogResult = DialogResult.Cancel };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);

            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;
        }

        private Form InitializePrompt(string text)
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 150;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.Text = text;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            return prompt;
        }

        internal void AddItemFromListOfClients(ExternalUser[] namesOfOnlineChatPartner)
        {
            lock (thisLock)
            {
                foreach (ExternalUser online in namesOfOnlineChatPartner)
                {
                    if (!exUserList.Contains(online)) { 
                        exUserList.Add(online);
                    }
                }
                availableConnectionsList.BeginInvoke(new AvailableConnectionsListDelegate(LoadAvaiableConnectionsList));
                //exUserListChanged = true;
            }
        }

        internal void RemoveItemFromListOfClients(ExternalUser[] namesOfOnlineChatPartner)
        {
            lock (thisLock)
            {
                exUserList.Clear();
                foreach (ExternalUser online in namesOfOnlineChatPartner)
                {
                    Debug.WriteLine("List :" + online.Name);
                    exUserList.Add(online);
                }
                availableConnectionsList.BeginInvoke(new AvailableConnectionsListDelegate(LoadAvaiableConnectionsList));
                //exUserListChanged = true;
            }
        }

        private void SendMessage()
        {
            if (enterTextBox.Text.Length > 0)
            {
                graphicControl.SendMessage(enterTextBox.Text);
                enterTextBox.ResetText();
            }
        }
    }
}
