using CSharpChatClient.controller;
using CSharpChatClient.Controller;
using CSharpChatClient.Model;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CSharpChatClient
{
    public partial class ChatForm : Form
    {
        private ProgramController programControl = null;
        private GraphicalInterfaceController graphicControl = null;

        public static ExtendedUserList exUserList = null;
        //private bool exUserListChanged = false;

        private Object thisLock = new Object();

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

            exUserList = new ExtendedUserList();
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
            string username = ShowEnterUsernameBox("Der Benutzername ist " + Configuration.localUser.Name + ", wenn nicht einfach neuen eingeben:");
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
            toolTips.SetToolTip(this.buttonClearHistory, "Löscht die Historie die aktuell angezeigt wird.");
            toolTips.SetToolTip(this.labelUserName, "Durch klicken auf dieses Feld kann der Benutzername geändert werden.");
            toolTips.SetToolTip(this.labelUserNameValue, "Durch klicken auf dieses Feld kann der Benutzername geändert werden.");
            toolTips.SetToolTip(this.labelConnectedWith, "Durch klicken auf dieses Feld kann die Verbindung zu einem anderen Computer hergestellt werden.");
            toolTips.SetToolTip(this.labelConnectedWithValue, "Durch klicken auf dieses Feld kann die Verbindung zu einem anderen Computer hergestellt werden.");
            toolTips.SetToolTip(this.labelIpAddress, "Hier werden die aktuellen Adressdaten dieses Computers angezeigt.");
            toolTips.SetToolTip(this.labelIpAddressValue, "Hier werden die aktuellen Adressdaten dieses Computers angezeigt.");
            toolTips.SetToolTip(this.labelPort, "Hier werden die aktuellen Adressdaten dieses Computers angezeigt.");
            toolTips.SetToolTip(this.labelPortValue, "Hier werden die aktuellen Adressdaten dieses Computers angezeigt.");
        }

        private void AvailableConnectionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Logger.LogInfo("SelectedIndexChanged on SelectedIndexChanged!");
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void LabelUserName_Click(object sender, EventArgs e)
        {
            LabelUserNameValue_Click(sender, e);
        }

        private void LabelUserNameValue_Click(object sender, EventArgs e)
        {
            String username = ShowEnterUsernameBox("Sie wollen ihren Benutzernamen ändern?");
            graphicControl.ChangeUsername(username);
        }

        internal void InformUser(string text)
        {
            MessageBox.Show("Verbindung nicht möglich, da die Gegenseite nicht antwortet.");
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
            }
            else
            {
                sendButton.Enabled = false;
            }
        }

        private void EnterTextBox_OnKeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Debug.WriteLine("Enter Pressed");
                //                SendMessage();
                e.Handled = true;
                e.SuppressKeyPress = true;
                sendButton.PerformClick();
            }
        }

        private void ChatForm_OnFormClosing(object sender, EventArgs e)
        {
            programControl.Stop();
            Application.Exit();
        }


        private void LabelConnectedWith_Click(object sender, EventArgs e)
        {
            LabelConnectedWithValue_Click(sender, e);
        }

        private void LabelConnectedWithValue_Click(object sender, EventArgs e)
        {
            String ipAndPort = ShowEnterIpAddressAndPort();

            graphicControl.ManualConnectToIPAndPort(ipAndPort);
        }

        private void ButtonClearHistory_Click(object sender, EventArgs e)
        {
            graphicControl.ClearHistory();
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


        internal void UsernameLabel_UpdateText(string username)
        {
            labelUserNameValue.Text = username;
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