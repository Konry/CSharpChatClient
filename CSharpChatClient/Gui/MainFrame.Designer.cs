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
            this.components = new System.ComponentModel.Container();
            this.labelUserNameValue = new System.Windows.Forms.Label();
            this.messageFlowBox = new System.Windows.Forms.RichTextBox();
            this.enterTextBox = new System.Windows.Forms.RichTextBox();
            this.labelConnectedWith = new System.Windows.Forms.Label();
            this.labelConnectedWithValue = new System.Windows.Forms.Label();
            this.labelIpAddress = new System.Windows.Forms.Label();
            this.labelIpAddressValue = new System.Windows.Forms.Label();
            this.labelPortValue = new System.Windows.Forms.Label();
            this.labelPort = new System.Windows.Forms.Label();
            this.buttonClearHistory = new System.Windows.Forms.Button();
            this.sendButton = new System.Windows.Forms.Button();
            this.labelUserName = new System.Windows.Forms.Label();
            this.externalUserListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.externalUserListBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // labelUserNameValue
            // 
            this.labelUserNameValue.AutoSize = true;
            this.labelUserNameValue.Location = new System.Drawing.Point(422, 25);
            this.labelUserNameValue.Name = "labelUserNameValue";
            this.labelUserNameValue.Size = new System.Drawing.Size(58, 13);
            this.labelUserNameValue.TabIndex = 1;
            this.labelUserNameValue.Text = "Own name";
            this.labelUserNameValue.Click += new System.EventHandler(this.LabelUserNameValue_Click);
            // 
            // messageFlowBox
            // 
            this.messageFlowBox.AcceptsTab = true;
            this.messageFlowBox.Location = new System.Drawing.Point(12, 12);
            this.messageFlowBox.Name = "messageFlowBox";
            this.messageFlowBox.ReadOnly = true;
            this.messageFlowBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.messageFlowBox.Size = new System.Drawing.Size(389, 186);
            this.messageFlowBox.TabIndex = 4;
            this.messageFlowBox.Text = "";
            this.messageFlowBox.TextChanged += new System.EventHandler(this.MessageFlowBox_TextChanged);
            // 
            // enterTextBox
            // 
            this.enterTextBox.Location = new System.Drawing.Point(13, 214);
            this.enterTextBox.Name = "enterTextBox";
            this.enterTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.enterTextBox.Size = new System.Drawing.Size(380, 35);
            this.enterTextBox.TabIndex = 5;
            this.enterTextBox.Text = "";
            this.enterTextBox.TextChanged += new System.EventHandler(this.EnterTextBox_TextChanged);
            this.enterTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.EnterTextBox_OnKeyUpHandler);
            // 
            // labelConnectedWith
            // 
            this.labelConnectedWith.AutoSize = true;
            this.labelConnectedWith.Location = new System.Drawing.Point(407, 49);
            this.labelConnectedWith.Name = "labelConnectedWith";
            this.labelConnectedWith.Size = new System.Drawing.Size(78, 13);
            this.labelConnectedWith.TabIndex = 6;
            this.labelConnectedWith.Text = "Verbunden mit:";
            this.labelConnectedWith.Click += new System.EventHandler(this.LabelConnectedWith_Click);
            // 
            // labelConnectedWithValue
            // 
            this.labelConnectedWithValue.AutoSize = true;
            this.labelConnectedWithValue.Location = new System.Drawing.Point(422, 62);
            this.labelConnectedWithValue.Name = "labelConnectedWithValue";
            this.labelConnectedWithValue.Size = new System.Drawing.Size(63, 13);
            this.labelConnectedWithValue.TabIndex = 7;
            this.labelConnectedWithValue.Text = "Niemandem";
            this.labelConnectedWithValue.Click += new System.EventHandler(this.LabelConnectedWithValue_Click);
            // 
            // labelIpAddress
            // 
            this.labelIpAddress.AutoSize = true;
            this.labelIpAddress.Location = new System.Drawing.Point(323, 257);
            this.labelIpAddress.Name = "labelIpAddress";
            this.labelIpAddress.Size = new System.Drawing.Size(58, 13);
            this.labelIpAddress.TabIndex = 8;
            this.labelIpAddress.Text = "IPAdresse:";
            // 
            // labelIpAddressValue
            // 
            this.labelIpAddressValue.AutoSize = true;
            this.labelIpAddressValue.Location = new System.Drawing.Point(376, 257);
            this.labelIpAddressValue.Name = "labelIpAddressValue";
            this.labelIpAddressValue.Size = new System.Drawing.Size(88, 13);
            this.labelIpAddressValue.TabIndex = 9;
            this.labelIpAddressValue.Text = "256.256.256.256";
            // 
            // labelPortValue
            // 
            this.labelPortValue.AutoSize = true;
            this.labelPortValue.Location = new System.Drawing.Point(465, 257);
            this.labelPortValue.Name = "labelPortValue";
            this.labelPortValue.Size = new System.Drawing.Size(37, 13);
            this.labelPortValue.TabIndex = 11;
            this.labelPortValue.Text = "65536";
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(460, 256);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(10, 13);
            this.labelPort.TabIndex = 10;
            this.labelPort.Text = ":";
            // 
            // buttonClearHistory
            // 
            this.buttonClearHistory.Image = global::CSharpChatClient.Properties.Resources.delete_32px;
            this.buttonClearHistory.Location = new System.Drawing.Point(444, 213);
            this.buttonClearHistory.Name = "buttonClearHistory";
            this.buttonClearHistory.Size = new System.Drawing.Size(38, 36);
            this.buttonClearHistory.TabIndex = 12;
            this.buttonClearHistory.UseVisualStyleBackColor = true;
            this.buttonClearHistory.Click += new System.EventHandler(this.ButtonClearHistory_Click);
            // 
            // sendButton
            // 
            this.sendButton.Image = global::CSharpChatClient.Properties.Resources.blue_mail_send_32px;
            this.sendButton.Location = new System.Drawing.Point(399, 213);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(39, 36);
            this.sendButton.TabIndex = 0;
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // labelUserName
            // 
            this.labelUserName.AutoSize = true;
            this.labelUserName.Location = new System.Drawing.Point(407, 12);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(78, 13);
            this.labelUserName.TabIndex = 13;
            this.labelUserName.Text = "Benutzername:";
            this.labelUserName.Click += new System.EventHandler(this.LabelUserName_Click);
            // 
            // externalUserListBindingSource
            // 
            this.externalUserListBindingSource.DataSource = typeof(CSharpChatClient.Model.ExternalUserList);
            // 
            // ChatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(514, 277);
            this.Controls.Add(this.labelUserName);
            this.Controls.Add(this.buttonClearHistory);
            this.Controls.Add(this.labelPortValue);
            this.Controls.Add(this.labelPort);
            this.Controls.Add(this.labelIpAddressValue);
            this.Controls.Add(this.labelIpAddress);
            this.Controls.Add(this.labelConnectedWithValue);
            this.Controls.Add(this.labelConnectedWith);
            this.Controls.Add(this.enterTextBox);
            this.Controls.Add(this.messageFlowBox);
            this.Controls.Add(this.labelUserNameValue);
            this.Controls.Add(this.sendButton);
            this.MinimumSize = new System.Drawing.Size(520, 300);
            this.Name = "ChatForm";
            this.Text = "Visual C# Chat";
            this.Load += new System.EventHandler(this.ChatForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.externalUserListBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.RichTextBox messageFlowBox;
        private System.Windows.Forms.RichTextBox enterTextBox;
        private System.Windows.Forms.Label labelUserNameValue;
        private System.Windows.Forms.Label labelConnectedWith;
        private System.Windows.Forms.Label labelConnectedWithValue;
        private System.Windows.Forms.Label labelIpAddress;
        private System.Windows.Forms.Label labelIpAddressValue;
        private System.Windows.Forms.Label labelPortValue;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.Button buttonClearHistory;
        private System.Windows.Forms.BindingSource externalUserListBindingSource;
        private System.Windows.Forms.Label labelUserName;
    }
}

