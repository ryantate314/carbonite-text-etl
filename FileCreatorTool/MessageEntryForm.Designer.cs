namespace FileCreatorTool
{
    partial class MessageEntryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.GroupBox messageDirectionGroupBox;
            this.sentRadioButton = new System.Windows.Forms.RadioButton();
            this.receivedRadioButton = new System.Windows.Forms.RadioButton();
            this.saveButton = new System.Windows.Forms.Button();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.messageDatePicker = new System.Windows.Forms.DateTimePicker();
            this.phoneNumberTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.contactNameTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            messageDirectionGroupBox = new System.Windows.Forms.GroupBox();
            messageDirectionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // messageDirectionGroupBox
            // 
            messageDirectionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            messageDirectionGroupBox.Controls.Add(this.sentRadioButton);
            messageDirectionGroupBox.Controls.Add(this.receivedRadioButton);
            messageDirectionGroupBox.Location = new System.Drawing.Point(323, 12);
            messageDirectionGroupBox.Name = "messageDirectionGroupBox";
            messageDirectionGroupBox.Size = new System.Drawing.Size(200, 34);
            messageDirectionGroupBox.TabIndex = 5;
            messageDirectionGroupBox.TabStop = false;
            // 
            // sentRadioButton
            // 
            this.sentRadioButton.AutoSize = true;
            this.sentRadioButton.Location = new System.Drawing.Point(70, 14);
            this.sentRadioButton.Name = "sentRadioButton";
            this.sentRadioButton.Size = new System.Drawing.Size(47, 17);
            this.sentRadioButton.TabIndex = 3;
            this.sentRadioButton.TabStop = true;
            this.sentRadioButton.Text = "Sent";
            this.sentRadioButton.UseVisualStyleBackColor = true;
            // 
            // receivedRadioButton
            // 
            this.receivedRadioButton.AutoSize = true;
            this.receivedRadioButton.Location = new System.Drawing.Point(123, 14);
            this.receivedRadioButton.Name = "receivedRadioButton";
            this.receivedRadioButton.Size = new System.Drawing.Size(71, 17);
            this.receivedRadioButton.TabIndex = 4;
            this.receivedRadioButton.TabStop = true;
            this.receivedRadioButton.Text = "Received";
            this.receivedRadioButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(356, 250);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // messageTextBox
            // 
            this.messageTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageTextBox.Location = new System.Drawing.Point(12, 52);
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(511, 163);
            this.messageTextBox.TabIndex = 1;
            // 
            // messageDatePicker
            // 
            this.messageDatePicker.CustomFormat = "yyyy/MM/dd h:mm:ss tt";
            this.messageDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.messageDatePicker.Location = new System.Drawing.Point(13, 26);
            this.messageDatePicker.Name = "messageDatePicker";
            this.messageDatePicker.Size = new System.Drawing.Size(200, 20);
            this.messageDatePicker.TabIndex = 2;
            this.messageDatePicker.Value = new System.DateTime(2018, 12, 19, 13, 39, 0, 0);
            // 
            // phoneNumberTextBox
            // 
            this.phoneNumberTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.phoneNumberTextBox.Location = new System.Drawing.Point(13, 236);
            this.phoneNumberTextBox.Name = "phoneNumberTextBox";
            this.phoneNumberTextBox.Size = new System.Drawing.Size(153, 20);
            this.phoneNumberTextBox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 220);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Phone Number";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(169, 220);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Contact Name";
            // 
            // contactNameTextBox
            // 
            this.contactNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.contactNameTextBox.Location = new System.Drawing.Point(172, 236);
            this.contactNameTextBox.Name = "contactNameTextBox";
            this.contactNameTextBox.Size = new System.Drawing.Size(153, 20);
            this.contactNameTextBox.TabIndex = 9;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Location = new System.Drawing.Point(442, 250);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // MessageEntryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 285);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.contactNameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.phoneNumberTextBox);
            this.Controls.Add(messageDirectionGroupBox);
            this.Controls.Add(this.messageDatePicker);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.saveButton);
            this.Name = "MessageEntryForm";
            this.Text = "MessageEntryForm";
            messageDirectionGroupBox.ResumeLayout(false);
            messageDirectionGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.DateTimePicker messageDatePicker;
        private System.Windows.Forms.RadioButton sentRadioButton;
        private System.Windows.Forms.RadioButton receivedRadioButton;
        private System.Windows.Forms.TextBox phoneNumberTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox contactNameTextBox;
        private System.Windows.Forms.Button cancelButton;
    }
}