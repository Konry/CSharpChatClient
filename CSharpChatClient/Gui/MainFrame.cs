using System;
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

namespace CSharpChatClient
{
    public partial class ChatForm : Form
    {
        private int MainHeight = 500;
        private int MainWidth = 300;

        private int StartingPositionX = 0;
        private int StartingPositionY = 0;

        private GraphicalInterfaceController graphicControl = null;

        public ChatForm()
        {
            InitializeController();

            InitializeComponent();
            InitializeContent();

            GetInitialUsername();
        }

        private void InitializeController()
        {
            ProgramController programControl = new ProgramController(this);
            this.graphicControl = programControl.graphicControl;
        }

        private void GetInitialUsername()
        {
            string username = ShowEnterUsernameBox("Bitte geben Sie einen Benutzernamen an:");
            graphicControl.ChangeUsername(username);
        }

        private void InitializeContent()
        {
            this.StartPosition = FormStartPosition.CenterScreen;

            SendButton.Enabled = false;
            richTextBox1.Multiline = false;
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Clicked on Send Button! " + richTextBox1.Text);
            SendMessage();
            //this.MessageFlowBox
        }


        private void UserNameLabel_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Clicked on UserNameLabel!");
            String username = ShowEnterUsernameBox("Sie wollen ihren Benutzernamen ändern?");
            graphicControl.ChangeUsername(username);
        }

        private void AvailableConnectionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("SelectedIndexChanged on SelectedIndexChanged!");
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("Initiate ChatForm!");
        }

        private void MessageFlowBox_TextChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("TextChanged on MessageFlowBox!");
        }

        public void UpdateUsernameLabel(string username)
        {
            UserNameLabel.Text = username;
        }

        private void SendMessage()
        {
            if (richTextBox1.Text.Length > 0)
            {
                graphicControl.SendMessage(richTextBox1.Text);
                richTextBox1.ResetText();
            }
        }

        private string ShowEnterUsernameBox(String text)
        {
            Form prompt = new Form();
            prompt.Width = 500;
            prompt.Height = 150;
            prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
            prompt.Text = "Benutzernamen eingeben";
            prompt.StartPosition = FormStartPosition.CenterScreen;

            Label textLabel = new Label() { Left = 50, Top = 20, Width = 250, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Abbrechen", Left = 250, Width = 100, Top = 70, DialogResult = DialogResult.Cancel };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            DialogResult result = prompt.ShowDialog();
            if (result == DialogResult.Cancel)
            {
                return "";
            }
            return result == DialogResult.OK ? textBox.Text : "";
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if(richTextBox1.Text.Length > 0)
            {
                SendButton.Enabled = true;
            }
        }

        private void OnKeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Debug.WriteLine("Enter Pressed");
                SendMessage();
            }
        }
    }
}
