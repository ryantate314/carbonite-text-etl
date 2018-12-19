using CarboniteXmlParser.Android;
using CarboniteXmlParser.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FileCreatorTool
{
    public class FileGenerator
    {

        public DateTime BackupDate { get; set; }

        public FileGenerator()
        {
            BackupDate = DateTime.Now;
        }

        public void SaveFile(string filename, IEnumerable<Message> messages)
        {
            var file = new BackupFile();
            file.TextMessages = messages.Select(MessageToSms);
            file.BackupDate = BackupDate;

            var settings = new XmlWriterSettings()
            {
                Indent = true
            };

            using (var writer = XmlWriter.Create(filename, settings))
            {
                file.WriteXml(writer);
            }
        }

        private static MessageType DirectionToMessageType(MessageDirection direction)
        {
            switch (direction)
            {
                case MessageDirection.Received:
                    return MessageType.Inbound;
                case MessageDirection.Sent:
                    return MessageType.Outbound;
                default:
                    throw new InvalidCastException();
            }
        }

        private Sms MessageToSms(Message message)
        {
            var sms = new Sms()
            {
                Address = message.PhoneNumber,
                Body = message.Body,
                ContactName = message.ContactName,
                Date = message.Date,
                MessageType = DirectionToMessageType(message.Direction),
                Subject = ""
            };
            return sms;
        }

    }
}
