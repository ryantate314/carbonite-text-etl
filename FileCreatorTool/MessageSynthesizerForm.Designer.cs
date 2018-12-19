namespace FileCreatorTool
{
    partial class ListForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ColumnHeader dateColumn;
            System.Windows.Forms.ColumnHeader messageColumn;
            this.messageList = new System.Windows.Forms.ListView();
            this.directionColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contactColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.messageListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editMessageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMessageButton = new System.Windows.Forms.Button();
            dateColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            messageColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.messageListContextMenu.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dateColumn
            // 
            dateColumn.Text = "Date";
            dateColumn.Width = 93;
            // 
            // messageColumn
            // 
            messageColumn.Text = "Message";
            messageColumn.Width = 633;
            // 
            // messageList
            // 
            this.messageList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.messageList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            dateColumn,
            messageColumn,
            this.contactColumn,
            this.directionColumn});
            this.messageList.ContextMenuStrip = this.messageListContextMenu;
            this.messageList.FullRowSelect = true;
            this.messageList.Location = new System.Drawing.Point(12, 27);
            this.messageList.Name = "messageList";
            this.messageList.Size = new System.Drawing.Size(927, 348);
            this.messageList.TabIndex = 0;
            this.messageList.UseCompatibleStateImageBehavior = false;
            this.messageList.View = System.Windows.Forms.View.Details;
            // 
            // directionColumn
            // 
            this.directionColumn.Text = "Direction";
            // 
            // contactColumn
            // 
            this.contactColumn.Text = "Contact";
            this.contactColumn.Width = 133;
            // 
            // messageListContextMenu
            // 
            this.messageListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editMessageMenuItem});
            this.messageListContextMenu.Name = "messageListContextMenu";
            this.messageListContextMenu.Size = new System.Drawing.Size(95, 26);
            // 
            // editMessageMenuItem
            // 
            this.editMessageMenuItem.Name = "editMessageMenuItem";
            this.editMessageMenuItem.Size = new System.Drawing.Size(94, 22);
            this.editMessageMenuItem.Text = "Edit";
            this.editMessageMenuItem.Click += new System.EventHandler(this.editMessageMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(951, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.exportToolStripMenuItem.Text = "Export";
            // 
            // addMessageButton
            // 
            this.addMessageButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addMessageButton.Location = new System.Drawing.Point(864, 381);
            this.addMessageButton.Name = "addMessageButton";
            this.addMessageButton.Size = new System.Drawing.Size(75, 23);
            this.addMessageButton.TabIndex = 2;
            this.addMessageButton.Text = "Add";
            this.addMessageButton.UseVisualStyleBackColor = true;
            this.addMessageButton.Click += new System.EventHandler(this.addMessageButton_click);
            // 
            // ListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 421);
            this.Controls.Add(this.addMessageButton);
            this.Controls.Add(this.messageList);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ListForm";
            this.Text = "Message Synthesizer";
            this.messageListContextMenu.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView messageList;
        private System.Windows.Forms.ColumnHeader directionColumn;
        private System.Windows.Forms.ColumnHeader contactColumn;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.Button addMessageButton;
        private System.Windows.Forms.ContextMenuStrip messageListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem editMessageMenuItem;
    }
}

