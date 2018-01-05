using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarboniteXmlParser;
using log4net;
using Data;
using Data.Staging;

namespace MessageImport
{


   class Backup
   {

      private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      private string _filename;
      private string _mediaFolder;

      public Backup(string filename, string mediaFolder)
      {
         //_messages = new List<Data.Message>();
         this._filename = filename;
         this._mediaFolder = mediaFolder;
      }

      public void run()
      {
         using (MessageReader reader = new MessageReader(_filename))
         using (StagingContext context = new StagingContext())
         {
            context.Database.Log = m => Console.Write(m);
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
            context.BulkInsert(contacts);
            context.BulkInsert(attachments);

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
            ContactName = contactName,
            Direction = (byte)direction,
            Number = SanitizeNumber(number),
            MessageId = messageId
         };

         return ma;
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
         if (objMessage.MessageType == MessageType.Received)
         {
            MessageAddress fromAddress = BuildMessageAddress(message.ContactName, message.Address, AddressDirection.From, message.GetMessageId());
            contacts.Add(fromAddress);
         }
         //Process the list of addresses in the mms conversation.
         //All contacts in the list only contain numbers, not contact names
         foreach (var address in message.Addresses)
         {
            if (address.Type == CarboniteTextMessageImport.Android.AddressType.To)
            {
               string contactName = String.Empty;
               if (message.Address.Equals(address.Number))
               {
                  contactName = message.ContactName;
               }
               MessageAddress ma = BuildMessageAddress(contactName, address.Number, AddressDirection.To, message.GetMessageId());
               contacts.Add(ma);
            }
         }
         objMessage.Body = bodyBuilder.ToString().TrimEnd(new char[] { '\n', ' ' });
         return objMessage;
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
            address = "(Unknown)";
         }
         return address;
      }
   }
}
