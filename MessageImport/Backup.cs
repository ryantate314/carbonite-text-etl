using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarboniteXmlParser;
using log4net;
using Data;
using Data.Staging;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace MessageImport
{


    class Backup
    {

        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string _filename;
        private string _mediaFolder;

        private static readonly string DEFAULT_CONTACT_NAME = "(Unknown)";
        private static readonly string DEFAULT_CONTACT_NUMBER = "";

        private const string MY_PHONE_NUMBER = "4239564025";

        public Backup(string filename, string mediaFolder)
        {
            //_messages = new List<Data.Message>();
            this._filename = filename;
            this._mediaFolder = mediaFolder;
        }

        public void Run()
        {
            StagingContext context = new StagingContext();
            context.TruncateStaging();

            MessageReader reader = new MessageReader(_filename);
            List<MessageAddress> contacts = new List<MessageAddress>();
            Dictionary<string, Data.Staging.Message> ssmsMessages = new Dictionary<string, Data.Staging.Message>();

            foreach (var rawMessage in reader.TextMessages)
            {
                string messageId = rawMessage.GetMessageId();
                if (ssmsMessages.ContainsKey(messageId))
                {
                    logger.Warn($"({rawMessage.LineNumber}) Found duplicate messageID {messageId}.");
                    continue;
                }
                var message = ProcessSms(rawMessage, contacts);
                if (String.IsNullOrEmpty(rawMessage.Body))
                {
                    logger.Warn($"({rawMessage.LineNumber}) Found SMS with blank body sent on {message.SendDate}.");
                }
                ssmsMessages.Add(message.MessageId, message);
            }

            context.Messages.BulkAddMessages(ssmsMessages.Values.ToList());
            ssmsMessages = null;//Attempt to garbage collect
            List<Attachment> attachments = new List<Attachment>();

            Dictionary<string, Message> mediaMessages = new Dictionary<string, Message>();

            AttachmentFileRepository repo = new AttachmentFileRepository(_mediaFolder);
            foreach (var rawMessage in reader.MultimediaMessages)
            {
                if (rawMessage.Parts.Count == 0)
                {
                    logger.Warn($"({rawMessage.LineNumber}) Discarding MMS with blank body and no attachments. ID={rawMessage.MessageId}.");
                    continue;
                }
                if (mediaMessages.ContainsKey(rawMessage.GetMessageId()))
                {
                    logger.Warn($"({rawMessage.LineNumber}) Found duplicate multi-media message with id {rawMessage.GetMessageId()}.");
                    continue;
                }

                var message = ProcessMms(contacts, attachments, rawMessage, repo);
                mediaMessages.Add(message.MessageId, message);
            }
            context.Messages.BulkAddMessages(mediaMessages.Values.ToList());
            context.Attachments.BulkAddAttachments(attachments);
            context.Contacts.BulkAddContacts(contacts);

            Console.Write("Would you like to disambiguate contacts (Yn)? ");
            string disambiguateResponse = Console.ReadLine();
            if (disambiguateResponse.ToLower() != "n")
            {
                DisambiguateContacts(context);
            }

            context.MergeStaging(reader.BackupDate);
        }

        private static bool CompareLists<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            var ina = a.Except(b);
            var inb = b.Except(a);
            return !ina.Any() && !inb.Any();
        }

        /// <summary>
        /// Prompts the user for a decision on the correct name for each number with multiple or missing contact names.
        /// </summary>
        /// <param name="context"></param>
        private static void DisambiguateContacts(StagingContext context)
        {
            //TODO refactor contact option logic
            //Group MessageAddresses by number and show a list of possible contact name
            Dictionary<string, List<string>> groupedContactOptions = context.Contacts.GetContactOptions();
            //Select a sample list of text messages for each contact
            var sampleMessagesCache = context.Messages.GetSampleMessages(groupedContactOptions.Keys.ToList());


            //Using a data table because EF doesn't support table valued parameters
            List<ContactUpdate> updatedContacts = new List<ContactUpdate>();

            foreach (KeyValuePair<string, List<string>> entry in groupedContactOptions)
            {
                string newName = entry.Value.FirstOrDefault();
                if (entry.Value.Count > 1 || String.IsNullOrEmpty(newName) || newName == DEFAULT_CONTACT_NAME)
                {
                    var sampleMessages = sampleMessagesCache.Where(m => m.Number == entry.Key);
                    //context.Messages.Join(Select(m => m.Body).Take(5);
                    //Build a list of options for the user to choose from
                    Console.WriteLine($"Select an address for number {entry.Key}:");
                    for (int i = 0; i < entry.Value.Count; i++)
                    {
                        Console.Write($"({i + 1}) {entry.Value[i]}, ");
                    }
                    Console.WriteLine("Type for Manual Entry");
                    foreach (var message in sampleMessages)
                    {
                        Console.WriteLine(message.Body.Substring(0, Math.Min(message.Body.Length, Console.WindowWidth)));
                    }
                    bool valid;
                    do
                    {
                        valid = true;//Default true so leaving blank sets the default
                        Console.Write("(1): ");//Communicate default of 1
                                               //Read user response
                        string response = Console.ReadLine();
                        if (!String.IsNullOrEmpty(response))
                        {
                            int option = -1;
                            if (Int32.TryParse(response, out option))
                            {
                                option--;//Convert to 0-index.
                                         //The user selected an option rather than typing one
                                if (option >= 0 && option < entry.Value.Count)
                                {
                                    newName = entry.Value[option];
                                }
                                else
                                {
                                    Console.WriteLine("Invalid option.");
                                    valid = false;
                                }
                            }//End If response is an integer
                            else
                            {
                                //TODO allow 'd' to delete all messages
                                //Override
                                newName = response;
                            }
                        }//End If null or empty
                    } while (!valid);
                }//End if more than one option

                updatedContacts.Add(new ContactUpdate()
                {
                    NewName = newName,
                    Number = entry.Key
                });

            }//End foreach contact

            context.Contacts.UpdateContacts(updatedContacts);
        }

        private Message ProcessSms(CarboniteXmlParser.XmlEntities.Sms sms, List<MessageAddress> contacts)
        {
            var sanitizedAddress = SanitizeNumber(sms.Address);
            Message message = new Message()
            {
                Body = sms.Body,
                SendDate = sms.Date,
                MessageType = ConvertMessageType(sms.MessageType),
                MessageId = sms.GetMessageId(),
                ConversationId = GetConversationHash(new string[] { sanitizedAddress })
                //TODO handle status
            };
            logger.Info("Processing SMS from line " + sms.LineNumber);
            MessageAddress address = BuildMessageAddress(sms.ContactName, sms.Address, MessageTypeToAddressDirection(sms.MessageType), message.MessageId);
            contacts.Add(address);
            return message;
        }

        private MessageAddress BuildMessageAddress(string contactName, string number, AddressDirection direction, string messageId)
        {
            MessageAddress ma = new MessageAddress()
            {
                ContactName = SanitizeContactName(contactName),
                Direction = (byte)direction,
                Number = SanitizeNumber(number),
                MessageId = messageId
            };

            return ma;
        }

        private string GetConversationHash(IEnumerable<string> phoneNumbers)
        {
            if (phoneNumbers == null || !phoneNumbers.Any())
            {
                return null;
            }

            string list = String.Join(",", phoneNumbers.OrderBy(x => x));
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(list));
                var builder = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                {
                    builder.AppendFormat("{0:x2}", b);
                }
                return builder.ToString();
            }
        }


        private Message ProcessMms(List<MessageAddress> contacts, List<Attachment> attachments, CarboniteXmlParser.XmlEntities.Mms message, AttachmentFileRepository fileRepo)
        {
            Message objMessage = new Message()
            {
                SendDate = message.Date,
                MessageId = message.GetMessageId(),
                MessageType = ConvertMessageType(message.Box)
                //TODO handle status
            };
            logger.Info("Processing MMS from line " + message.LineNumber);
            StringBuilder bodyBuilder = new StringBuilder();//lol
            List<Attachment> messageAttachments = new List<Attachment>();
            foreach (var attachment in message.Parts)
            {
                if (attachment.MimeType == "text/plain")
                {
                    bodyBuilder.AppendLine(attachment.Text);
                }
                else if (attachment.MimeType != "application/smil")
                {
                    Attachment objAttachment = fileRepo.SaveAttachmentAsync(objMessage, attachment).Result;
                    //Ignore duplicate attachments.
                    if (!messageAttachments.Any(x => x.Path == objAttachment.Path))
                    {
                        messageAttachments.Add(objAttachment);
                    }
                }
            }
            attachments.AddRange(messageAttachments.Distinct());
            //if (objMessage.MessageType == MessageType.Received)
            //{
            //   //TODO split inline contact name by ~
            //   if (!String.IsNullOrEmpty(message.Address) && !message.Address.Contains("~"))
            //   {
            //      MessageAddress fromAddress = BuildMessageAddress(message.ContactName, message.Address, AddressDirection.From, message.GetMessageId());
            //      contacts.Add(fromAddress);
            //   }
            //}
            //Process the list of addresses in the mms conversation.
            //All contacts in the list only contain numbers, not contact names
            List<MessageAddress> messageContacts = new List<MessageAddress>();
            foreach (var address in message.Addresses)
            {
                string contactName = String.Empty;
                if (!String.IsNullOrEmpty(message.Address) && message.Address.Equals(address.Number))
                {
                    contactName = message.ContactName;
                }
                MessageAddress ma = BuildMessageAddress(contactName, address.Number, AddressTypeToAddressDirection(address.Type), message.GetMessageId());

                //Do not include me in the contact lists. It creates different conversations for MMS messages.
                //It can be assumed that I am a part of any message in here.
                //Perform after BuildMessageAddress in order to sanitize the phone number before comparing
                if (ma.Number != MY_PHONE_NUMBER)
                {
                    messageContacts.Add(ma);
                }
            }
            objMessage.Body = bodyBuilder.ToString().TrimEnd(new char[] { '\n', ' ' });

            objMessage.ConversationId = GetConversationHash(messageContacts.Select(x => x.Number));

            return objMessage;
        }

        private AddressDirection MessageTypeToAddressDirection(CarboniteXmlParser.Android.MessageType type)
        {
            switch (type)
            {
                case CarboniteXmlParser.Android.MessageType.Inbound:
                    return AddressDirection.From;
                case CarboniteXmlParser.Android.MessageType.Outbound:
                    return AddressDirection.To;
                default:
                    throw new Exception("Invalid message type: " + type);
            }
        }

        private MessageType ConvertMessageType(CarboniteXmlParser.Android.MessageType type)
        {
            switch (type)
            {
                case CarboniteXmlParser.Android.MessageType.Inbound:
                    return MessageType.Received;
                case CarboniteXmlParser.Android.MessageType.Outbound:
                    return MessageType.Sent;
                default:
                    throw new Exception("Invalid message type " + type);
            }
        }

        private MessageType ConvertMessageType(CarboniteXmlParser.Android.MessageBox box)
        {
            switch (box)
            {
                case CarboniteXmlParser.Android.MessageBox.Inbox:
                    return Data.MessageType.Received;
                case CarboniteXmlParser.Android.MessageBox.Sent:
                    return Data.MessageType.Sent;
                default:
                    throw new Exception("Unknown MessageType (" + box + ")");
            }
        }

        private AddressDirection AddressTypeToAddressDirection(CarboniteXmlParser.Android.AddressType addressType)
        {
            switch (addressType)
            {
                case CarboniteXmlParser.Android.AddressType.From:
                    return AddressDirection.From;
                case CarboniteXmlParser.Android.AddressType.To:
                case CarboniteXmlParser.Android.AddressType.BCC:
                case CarboniteXmlParser.Android.AddressType.CC:
                    return AddressDirection.To;
                default:
                    throw new Exception("Unknown Address Type: " + addressType);
            }
        }

        private string SanitizeNumber(string address)
        {
            return BackupUtils.SanitizePhoneNumber(address) ?? address;
        }

        private string SanitizeContactName(string name)
        {
            string sanitized = name;
            if (name == null)
            {
                sanitized = String.Empty;
            }
            else if (DEFAULT_CONTACT_NAME.Equals(name))
            {
                sanitized = "";
            }
            return sanitized;
        }
    }
}
