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
    public partial class ListForm : Form
    {
        public ListForm()
        {
            InitializeComponent();

            _messages = new List<Message>();
        }

        private ICollection<Message> _messages;

        private Message GetTemplateMessage()
        {
            if (_messages.Any())
            {
                return _messages.Last();
            }
            else
            {
                return new Message()
                {
                    Date = DateTime.Now
                };
            }
        }

        private void Render()
        {
            messageList.BeginUpdate();
            messageList.Items.Clear();
            
            foreach (var message in _messages)
            {
                messageList.Items.Add(MessageToListItem(message));
            }

            messageList.EndUpdate();
        }

        private void AddMessage(Message message)
        {
            _messages.Add(message);
            _messages = _messages.OrderBy(x => x.Date).ToList();
        }

        private ListViewItem MessageToListItem(Message message)
        {
            //Date, Message, Contact Name, Direction
            var item = new ListViewItem(new string[] { message.Date.ToString(), message.Body, message.ContactName, message.Direction.ToString() });
            item.Tag = message;
            return item;
        }

        private void addMessageButton_click(object sender, EventArgs e)
        {
            var message = new Message();
            var template = GetTemplateMessage();
            message.Date = template.Date;
            message.PhoneNumber = template.PhoneNumber;
            message.ContactName = template.ContactName;
            message.Direction = template.Direction == MessageDirection.Sent ? MessageDirection.Received : MessageDirection.Sent;

            var entryForm = new MessageEntryForm(message);
            var result = entryForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                AddMessage(message);
                Render();
            }

        }

        private void editMessageMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem selected = messageList.FocusedItem;
            var message = selected?.Tag as Message;
            if (message != null)
            {
                var editForm = new MessageEntryForm(message);
                var result = editForm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Render();
                }
            }
        }
    }
}
