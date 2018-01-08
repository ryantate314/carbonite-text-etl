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

namespace MessageImport
{


   class Backup
   {

      private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      private string _filename;
      private string _mediaFolder;

      private static readonly string DEFAULT_CONTACT_NAME = "(Unknown)";
      private static readonly string DEFAULT_CONTACT_NUMBER = "";

      public Backup(string filename, string mediaFolder)
      {
         //_messages = new List<Data.Message>();
         this._filename = filename;
         this._mediaFolder = mediaFolder;
      }

      public void Run()
      {
         using (MessageReader reader = new MessageReader(_filename))
         using (StagingContext context = new StagingContext())
         {
            context.Database.Log = m => logger.Info(m);
            context.USP_Truncate_Staging();

            List<MessageAddress> contacts = new List<MessageAddress>();
            Dictionary<string, Data.Staging.Message> ssmsMessages = new Dictionary<string, Data.Staging.Message>();

            using (var smsIterator = reader.getSmsIterator())
            {
               while (smsIterator.MoveNext())
               {
                  string messageId = smsIterator.Current.GetMessageId();
                  if (ssmsMessages.ContainsKey(messageId))
                  {
                     logger.Warn($"({smsIterator.Current.LineNumber}) Found duplicate messageID {messageId}.");
                     continue;
                  }
                  var message = ProcessSms(smsIterator.Current, contacts);
                  if (String.IsNullOrEmpty(message.Body))
                  {
                     logger.Warn($"({smsIterator.Current.LineNumber}) Found SMS with blank body sent on {message.SendDate}.");
                  }
                  ssmsMessages.Add(message.MessageId, message);
               }
            }
            
            //Process media messages while the first batch is saving.
            Task ssmsInsert = context.BulkInsertAsync(ssmsMessages.Values.ToList());
            ssmsMessages = null;//Attempt to garbage collect
            List<Attachment> attachments = new List<Attachment>();

            Dictionary<string, Message> mediaMessages = new Dictionary<string, Message>();

            AttachmentRepository repo = new AttachmentRepository(_mediaFolder);
            using (var mmsIterator = reader.getMmsIterator())
            {
               while (mmsIterator.MoveNext())
               {
                  if (mmsIterator.Current.Parts.Count == 0)
                  {
                     logger.Warn($"({mmsIterator.Current.LineNumber}) Discarding MMS with blank body and no attachments. ID={mmsIterator.Current.MessageId}.");
                     continue;
                  }
                  if (mediaMessages.ContainsKey(mmsIterator.Current.GetMessageId()))
                  {
                     logger.Warn($"({mmsIterator.Current.LineNumber}) Found duplicate multi-media message with id {mmsIterator.Current.GetMessageId()}.");
                     continue;
                  }

                  var message = ProcessMms(contacts, attachments, mmsIterator.Current, repo);
                  mediaMessages.Add(message.MessageId, message);
               }
            }
            ssmsInsert.Wait();//Connection can only have one action going at once. (I think)
            context.BulkInsert(mediaMessages.Values.ToList());
            context.BulkInsert(attachments);
            context.BulkInsert(contacts);

            //TODO refactor contact option logic
            //Group MessageAddresses by number and show a list of possible contact name
            var contactOptions = context.USP_Select_Contact_Options();
            var groupedContactOptions = contactOptions.GroupBy(o => o.Number, o => o.ContactName)
                                                      .ToDictionary(go => go.Key, go => go.ToList());

            //Using a data table because EF doesn't support table valued parameters
            DataTable dt = new DataTable();
            dt.Columns.Add("Number", typeof(string));
            dt.Columns.Add("NewName", typeof(string));
            
            foreach (KeyValuePair<string, List<string>> entry in groupedContactOptions)
            {
               string newName = entry.Value.FirstOrDefault();
               if (entry.Value.Count > 1 || String.IsNullOrEmpty(newName) || newName == DEFAULT_CONTACT_NAME)
               {
                  //Select a sample list of text message
                  var sampleMessages = (from m in context.Messages
                                        join ma in context.MessageAddresses on m.MessageId equals ma.MessageId
                                        where ma.Number == entry.Key
                                        select m.Body
                                        ).Take(5);
                     
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
                     Console.WriteLine(message.Substring(0, Math.Min(message.Length, Console.WindowWidth)));
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
                           //Override
                           newName = response;
                        }
                     }//End If null or empty
                  } while (!valid);
               }//End if more than one option

               dt.Rows.Add(entry.Key, newName);

            }//End foreach contact

            var param = new SqlParameter("contactInfo", SqlDbType.Structured);
            param.Value = dt;
            param.TypeName = "dbo.ContactInfo";
            string command = "EXEC Staging.USP_Update_Contacts @contactInfo";
            context.Database.ExecuteSqlCommand(command, param);
            
            context.USP_Merge();
         }
      }

      private Message ProcessSms(CarboniteXmlParser.XmlEntities.Sms sms, List<MessageAddress> contacts)
      {
         Message message = new Message()
         {
            Body = sms.Body,
            SendDate = sms.Date,
            MessageType = ConvertMessageType(sms.MessageType),
            MessageId = sms.GetMessageId()
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


      private Message ProcessMms(List<MessageAddress> contacts, List<Attachment> attachments, CarboniteXmlParser.XmlEntities.Mms message, AttachmentRepository fileRepo)
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
         foreach (var attachment in message.Parts)
         {
            if (attachment.MimeType == "text/plain")
            {
               bodyBuilder.AppendLine(attachment.Text);
            }
            else if (attachment.MimeType != "application/smil")
            {
               Attachment objAttachment = fileRepo.SaveAttachmentAsync(objMessage, attachment).Result;
               attachments.Add(objAttachment);
            }
         }
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
         foreach (var address in message.Addresses)
         {
            string contactName = String.Empty;
            if (!String.IsNullOrEmpty(message.Address) && message.Address.Equals(address.Number))
            {
               contactName = message.ContactName;
            }
            MessageAddress ma = BuildMessageAddress(contactName, address.Number, AddressTypeToAddressDirection(address.Type), message.GetMessageId());
            contacts.Add(ma);
         }
         objMessage.Body = bodyBuilder.ToString().TrimEnd(new char[] { '\n', ' ' });
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
               return AddressDirection.To;
            default:
               throw new Exception("Unknown Address Type: " + addressType);
         }
      }

      private string SanitizeNumber(string address)
      {
         if (!String.IsNullOrEmpty(address))
         {
            if (address.Length == 12)
            {
               //Remove leading +1
               address = address.Substring(2);
            }
            else if (address.Length == 11)
            {
               //Remove leading 1
               address = address.Substring(1);
            }
         }
         else
         {
            address = DEFAULT_CONTACT_NAME;
         }
         return address;
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
