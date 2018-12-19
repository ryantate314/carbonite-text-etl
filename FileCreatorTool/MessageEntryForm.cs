using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileCreatorTool
{
    public partial class MessageEntryForm : Form
    {
        public MessageEntryForm(Message message)
        {
            InitializeComponent();
            _message = message;

            Render();
        }

        private Message _message;

        private void Render()
        {
            messageDatePicker.Value = _message.Date;
            messageTextBox.Text = _message.Body;
            contactNameTextBox.Text = _message.ContactName;
            phoneNumberTextBox.Text = _message.PhoneNumber;

            if (_message.Direction == MessageDirection.Received)
            {
                receivedRadioButton.Checked = true;
            }
            else if (_message.Direction == MessageDirection.Sent)
            {
                sentRadioButton.Checked = true;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            _message.Date = messageDatePicker.Value;
            _message.Body = messageTextBox.Text;
            _message.ContactName = contactNameTextBox.Text;
            _message.PhoneNumber = phoneNumberTextBox.Text;

            if (receivedRadioButton.Checked)
            {
                _message.Direction = MessageDirection.Received;
            }
            else
            {
                _message.Direction = MessageDirection.Sent;
            }

            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
