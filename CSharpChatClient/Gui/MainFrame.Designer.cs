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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatForm));
            this.labelUserName = new System.Windows.Forms.Label();
            this.availableConnectionsList = new System.Windows.Forms.ListBox();
            this.externalUserListBindingSource = new System.Windows.Forms.BindingSource(this.components);
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
            ((System.ComponentModel.ISupportInitialize)(this.externalUserListBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // labelUserName
            // 
            resources.ApplyResources(this.labelUserName, "labelUserName");
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Click += new System.EventHandler(this.UserNameLabel_Click);
            // 
            // availableConnectionsList
            // 
            resources.ApplyResources(this.availableConnectionsList, "availableConnectionsList");
            this.availableConnectionsList.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.externalUserListBindingSource, "Name", true));
            this.availableConnectionsList.FormattingEnabled = true;
            this.availableConnectionsList.Name = "availableConnectionsList";
            this.availableConnectionsList.ValueMember = "Id";
            this.availableConnectionsList.Click += new System.EventHandler(this.ChatForm_Load);
            this.availableConnectionsList.SelectedIndexChanged += new System.EventHandler(this.AvailableConnectionsList_SelectedIndexChanged);
            // 
            // externalUserListBindingSource
            // 
            this.externalUserListBindingSource.DataSource = typeof(CSharpChatClient.Model.ExternalUserList);
            // 
            // messageFlowBox
            // 
            this.messageFlowBox.AcceptsTab = true;
            resources.ApplyResources(this.messageFlowBox, "messageFlowBox");
            this.messageFlowBox.Name = "messageFlowBox";
            this.messageFlowBox.ReadOnly = true;
            this.messageFlowBox.TextChanged += new System.EventHandler(this.MessageFlowBox_TextChanged);
            // 
            // enterTextBox
            // 
            resources.ApplyResources(this.enterTextBox, "enterTextBox");
            this.enterTextBox.Name = "enterTextBox";
            this.enterTextBox.TextChanged += new System.EventHandler(this.EnterTextBox_TextChanged);
            this.enterTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.EnterTextBox_OnKeyUpHandler);
            // 
            // labelConnectedWith
            // 
            resources.ApplyResources(this.labelConnectedWith, "labelConnectedWith");
            this.labelConnectedWith.Name = "labelConnectedWith";
            // 
            // labelConnectedWithValue
            // 
            resources.ApplyResources(this.labelConnectedWithValue, "labelConnectedWithValue");
            this.labelConnectedWithValue.Name = "labelConnectedWithValue";
            this.labelConnectedWithValue.Click += new System.EventHandler(this.labelConnectedWithValue_Click);
            // 
            // labelIpAddress
            // 
            resources.ApplyResources(this.labelIpAddress, "labelIpAddress");
            this.labelIpAddress.Name = "labelIpAddress";
            // 
            // labelIpAddressValue
            // 
            resources.ApplyResources(this.labelIpAddressValue, "labelIpAddressValue");
            this.labelIpAddressValue.Name = "labelIpAddressValue";
            // 
            // labelPortValue
            // 
            resources.ApplyResources(this.labelPortValue, "labelPortValue");
            this.labelPortValue.Name = "labelPortValue";
            // 
            // labelPort
            // 
            resources.ApplyResources(this.labelPort, "labelPort");
            this.labelPort.Name = "labelPort";
            // 
            // buttonClearHistory
            // 
            resources.ApplyResources(this.buttonClearHistory, "buttonClearHistory");
            this.buttonClearHistory.Image = global::CSharpChatClient.Properties.Resources.delete_32px;
            this.buttonClearHistory.Name = "buttonClearHistory";
            this.buttonClearHistory.UseVisualStyleBackColor = true;
            this.buttonClearHistory.Click += new System.EventHandler(this.button1_Click);
            // 
            // sendButton
            // 
            resources.ApplyResources(this.sendButton, "sendButton");
            this.sendButton.Image = global::CSharpChatClient.Properties.Resources.blue_mail_send_32px;
            this.sendButton.Name = "sendButton";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // ChatForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.buttonClearHistory);
            this.Controls.Add(this.labelPortValue);
            this.Controls.Add(this.labelPort);
            this.Controls.Add(this.labelIpAddressValue);
            this.Controls.Add(this.labelIpAddress);
            this.Controls.Add(this.labelConnectedWithValue);
            this.Controls.Add(this.labelConnectedWith);
            this.Controls.Add(this.enterTextBox);
            this.Controls.Add(this.messageFlowBox);
            this.Controls.Add(this.availableConnectionsList);
            this.Controls.Add(this.labelUserName);
            this.Controls.Add(this.sendButton);
            this.Name = "ChatForm";
            this.Load += new System.EventHandler(this.ChatForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.externalUserListBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.ListBox availableConnectionsList;
        private System.Windows.Forms.RichTextBox messageFlowBox;
        private System.Windows.Forms.RichTextBox enterTextBox;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.Label labelConnectedWith;
        private System.Windows.Forms.Label labelConnectedWithValue;
        private System.Windows.Forms.Label labelIpAddress;
        private System.Windows.Forms.Label labelIpAddressValue;
        private System.Windows.Forms.Label labelPortValue;
        private System.Windows.Forms.Label labelPort;
        private System.Windows.Forms.Button buttonClearHistory;
        private System.Windows.Forms.BindingSource externalUserListBindingSource;
    }
}

