namespace CSharpChatClient
{
    partial class ChatForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.SendButton = new System.Windows.Forms.Button();
            this.UserNameLabel = new System.Windows.Forms.Label();
            this.AvailableConnectionsList = new System.Windows.Forms.ListBox();
            this.MessageFlowBox = new System.Windows.Forms.RichTextBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // SendButton
            // 
            this.SendButton.Location = new System.Drawing.Point(399, 226);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(93, 23);
            this.SendButton.TabIndex = 0;
            this.SendButton.Text = "Send";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // UserNameLabel
            // 
            this.UserNameLabel.AutoSize = true;
            this.UserNameLabel.Location = new System.Drawing.Point(416, 12);
            this.UserNameLabel.Name = "UserNameLabel";
            this.UserNameLabel.Size = new System.Drawing.Size(58, 13);
            this.UserNameLabel.TabIndex = 1;
            this.UserNameLabel.Text = "Own name";
            this.UserNameLabel.Click += new System.EventHandler(this.UserNameLabel_Click);
            // 
            // AvailableConnectionsList
            // 
            this.AvailableConnectionsList.FormattingEnabled = true;
            this.AvailableConnectionsList.IntegralHeight = false;
            this.AvailableConnectionsList.Location = new System.Drawing.Point(399, 31);
            this.AvailableConnectionsList.Name = "AvailableConnectionsList";
            this.AvailableConnectionsList.Size = new System.Drawing.Size(93, 176);
            this.AvailableConnectionsList.TabIndex = 2;
            this.AvailableConnectionsList.SelectedIndexChanged += new System.EventHandler(this.AvailableConnectionsList_SelectedIndexChanged);
            this.AvailableConnectionsList.Click += new System.EventHandler(this.ChatForm_Load);
            // 
            // MessageFlowBox
            // 
            this.MessageFlowBox.AcceptsTab = true;
            this.MessageFlowBox.Location = new System.Drawing.Point(12, 12);
            this.MessageFlowBox.Name = "MessageFlowBox";
            this.MessageFlowBox.ReadOnly = true;
            this.MessageFlowBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.MessageFlowBox.Size = new System.Drawing.Size(381, 184);
            this.MessageFlowBox.TabIndex = 4;
            this.MessageFlowBox.Text = "";
            this.MessageFlowBox.TextChanged += new System.EventHandler(this.MessageFlowBox_TextChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(13, 214);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBox1.Size = new System.Drawing.Size(380, 35);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            this.richTextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnKeyUpHandler);
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(504, 261);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.MessageFlowBox);
            this.Controls.Add(this.AvailableConnectionsList);
            this.Controls.Add(this.UserNameLabel);
            this.Controls.Add(this.SendButton);
            this.MinimumSize = new System.Drawing.Size(520, 300);
            this.Name = "ChatForm";
            this.Text = "Visual C# Chat";
            this.Load += new System.EventHandler(this.ChatForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SendButton;
        private System.Windows.Forms.Label UserNameLabel;
        private System.Windows.Forms.ListBox AvailableConnectionsList;
        private System.Windows.Forms.RichTextBox MessageFlowBox;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

