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

namespace CSharpChatClient
{
    public partial class ChatForm : Form
    {
        private int MainHeight = 500;
        private int MainWidth = 300;

        private int StartingPositionX = 0;
        private int StartingPositionY = 0;

        public ChatForm()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Clicked on Send Button!");
            //this.MessageFlowBox
        }

        private void UserNameLabel_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Clicked on UserNameLabel!");
        }

        private void AvailableConnectionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("SelectedIndexChanged on SelectedIndexChanged!");
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            Debug.WriteLine("Load on ChatForm!");
        }

        private void MessageFlowBox_TextChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("TextChanged on MessageFlowBox!");
        }
    }
}
