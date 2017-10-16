﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Staging;
using CarboniteXmlParser;
using CarboniteXmlParser.XmlEntities;
using System.Text.RegularExpressions;
using log4net;
using Data;

namespace MessageImport
{


   class Backup
   {

      private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      private class AddressContainer
      {
         public Address Address { get; set; }
         public bool Overridden { get; set; }
      }

      private List<AddressContainer> _addresses;
      //private List<Data.Message> _messages;

      private string _filename;
      private string _mediaFolder;

      public Backup(string filename, string mediaFolder)
      {
         _addresses = new List<AddressContainer>();
         //_messages = new List<Data.Message>();
         this._filename = filename;
         this._mediaFolder = mediaFolder;
      }

      public void run()
      {
         using (MessageReader reader = new MessageReader(_filename))
         using (StagingContext context = new StagingContext())
         {
            context.Database.Log = m => logger.Debug(m);
            context.USP_Truncate_Staging();
            //context.Database.ExecuteSqlCommand("EXECUTE USP_Truncate_Staging();");

            context.Database.CommandTimeout = 60 * 5;

            using (UnitOfWork<StagingContext> uow = new UnitOfWork<StagingContext>(context))
            {

               using (var smsIterator = reader.getSmsIterator())
               {
                  while (smsIterator.MoveNext())
                  {
                     var message = ProcessSms(smsIterator.Current, context);
                     //_messages.Add(message);
                     if (String.IsNullOrEmpty(message.Body))
                     {
                        logger.Warn($"({smsIterator.Current.LineNumber}) Found SMS with blank body sent on {message.SendDate}.");
                     }
                     context.Messages.Add(message);
                  }
               }

               //context.SaveChanges();

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
                     if (mmsIterator.Current.GetMessageId() == "04517B16AADF00004A700003")
                     {

                     }

                     var message = ProcessMms(mmsIterator.Current, uow, repo);
                     //_messages.Add(message);
                     context.Messages.Add(message);
                  }
               }

               foreach (var address in _addresses)
               {
                  context.Addresses.Add(address.Address);
               }

               context.SaveChanges();

               uow.Commit();
            }
         }
      }

      private Data.Staging.Message ProcessSms(Sms sms, StagingContext context)
      {
         Data.Staging.Message message = new Data.Staging.Message()
         {
            Body = sms.Body,
            SendDate = sms.Date,
            MessageType = ConvertMessageType(sms.MessageType),
            MessageId = sms.GetMessageId()
         };
         logger.Info("Processing SMS from line " + sms.LineNumber);
         if (message.MessageType == MessageType.Received)
         {
            //logger.Info("^Inbound message");
            message.FromAddress = AddAddress(sms.ContactName, sms.Address);
         }
         else
         {
            MessageAddress ma = new MessageAddress();
            ma.Address = AddAddress(sms.ContactName, sms.Address);
            ma.Message = message;
            context.MessageAddresses.Add(ma);
            message.MessageAddresses.Add(ma);
         }
         return message;
      }

      private Data.MessageType ConvertMessageType(CarboniteXmlParser.Android.MessageType type)
      {
         return (Data.MessageType)type;
      }

      private Data.MessageType ConvertMessageType(CarboniteXmlParser.Android.MessageBox box)
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

      private Address AddAddress(String number)
      {
         number = SanitizeNumber(number);
         AddressContainer container = _addresses.Find(a => a.Address.Number == number);
         if (container == null)
         {
            container = new AddressContainer();
            container.Address = new Address();
            container.Address.Number = number;
            _addresses.Add(container);
         }

         return container.Address;
      }

      //Address already exists, but with different name. Was searched by number.
      private void MergeAddressByDifferentName(AddressContainer objAddressContainer, string newName)
      {
         Address objAddress = objAddressContainer.Address;
         String acceptedName = objAddress.ContactName;
         //If the contact name was not already known, just use the new one without any prompting
         if (String.IsNullOrEmpty(objAddress.ContactName) || objAddress.ContactName == "(Unknown)")
         {
            acceptedName = newName;
         }
         else
         {
            string answer = "";
            bool looping = true;
            do
            {
               Console.WriteLine($"Inconsistent names for address {objAddress.Number}:");
               Console.Write($"(a) {objAddress.ContactName} or (b) {newName}?(a,b,new,custom): ");
               answer = Console.ReadLine();
               if (answer.ToLower() == "a")
               {
                  acceptedName = objAddress.ContactName;
                  looping = false;
               }
               else if (answer.ToLower() == "b")
               {
                  acceptedName = newName;
                  looping = false;
               }
               else if (answer.Length < 5)
               {
                  acceptedName = answer;
                  Console.Write("Are you sure you want to set the contact name to " + answer + "? (y/n): ");
                  char yn = (char)Console.Read();
                  looping = yn == 'y';
               }
            } while (looping);
            objAddressContainer.Overridden = true;
         }
      }

      //Address already exists, but with different name. Was searched by number.
      private void MergeAddressByDifferentNumber(AddressContainer objAddressContainer, string newNumber)
      {
         Address objAddress = objAddressContainer.Address;
         String acceptedNumber = objAddress.ContactName;
         //If the contact name was not already known, just use the new one without any prompting
         if (String.IsNullOrEmpty(objAddress.ContactName))
         {
            acceptedNumber = newNumber;
         }
         else
         {
            string answer = "";
            bool looping = true;
            do
            {
               Console.WriteLine($"Inconsistent addresses for contact {objAddress.ContactName}:");
               Console.Write($"(a) {objAddress.Number} or (b) {newNumber}?(a,b,new,custom): ");
               answer = Console.ReadLine();
               if (answer.ToLower() == "a")
               {
                  acceptedNumber = objAddress.ContactName;
                  looping = false;
               }
               else if (answer.ToLower() == "b")
               {
                  acceptedNumber = newNumber;
                  looping = false;
               }
               else if (answer.Length != 10)
               {
                  acceptedNumber = answer;
                  Console.Write("Are you sure you want to set the contact address to " + answer + "? (y/n): ");
                  char yn = (char)Console.Read();
                  looping = yn == 'y';
               }
            } while (looping);
            objAddressContainer.Overridden = true;
         }
      }

      private Address AddAddress(String name, String address)
      {
         Address objAddress;
         AddressContainer objAddressContainer;
         address = SanitizeNumber(address);
         if (name == "(Unknown)")
         {
            objAddressContainer = _addresses.Find(a => a.Address.Number == address);
         }
         else
         {
            objAddressContainer = _addresses.Find(a => a.Address.Number == address || a.Address.ContactName == name);
         }
         if (objAddressContainer != null)
         {
            objAddress = objAddressContainer.Address;
            if (objAddress.ContactName != name && !objAddressContainer.Overridden)
            {
               MergeAddressByDifferentName(objAddressContainer, name);
               
            } else if (objAddress.Number != address)
            {
               MergeAddressByDifferentNumber(objAddressContainer, address);
            }
         } else
         {
            //New address
            objAddress = new Address();
            objAddress.ContactName = name;
            objAddress.Number = address;
            _addresses.Add(new AddressContainer()
            {
               Address = objAddress,
               Overridden = false
            });
         }
         return objAddress;
      }

      private Data.Staging.Message ProcessMms(CarboniteXmlParser.XmlEntities.Mms message, UnitOfWork<StagingContext> uow, AttachmentRepository fileRepo)
      {
         Data.Staging.Message objMessage = new Data.Staging.Message()
         {
            SendDate = message.Date,
            MessageId = message.GetMessageId(),
            MessageType = ConvertMessageType(message.Box)
         };
         logger.Info("Processing MMS from line " + message.LineNumber);
         StringBuilder bodyBuilder = new StringBuilder();//lol
         foreach (var attachment in message.Parts)
         {
            if (attachment.MimeType == "text/plain")
            {
               bodyBuilder.AppendLine(attachment.Text);
            } else if (attachment.MimeType != "application/smil")
            {
               fileRepo.SaveAttachmentAsync(uow, objMessage, attachment).Wait();
            }
         }
         if (objMessage.MessageType == MessageType.Received)
         {
            objMessage.FromAddress = AddAddress(message.ContactName, message.Address);
         }
         foreach (var address in message.Addresses)
         {
            if (address.Type == CarboniteTextMessageImport.Android.AddressType.To)
            {
               Address objAddress = AddAddress(address.Number);
               MessageAddress ma = new MessageAddress();
               ma.Address = objAddress;
               ma.Message = objMessage;
               uow.Context.MessageAddresses.Add(ma);
               objMessage.MessageAddresses.Add(ma);
            }
         }
         Address inlineAddress = AddAddress(message.ContactName, message.Address);
         if (inlineAddress != null && !objMessage.MessageAddresses.Any(a => a.Address == inlineAddress))
         {
            if (objMessage.MessageType == MessageType.Received)
            {
               objMessage.FromAddress = inlineAddress;
            }
            else
            {
               MessageAddress ma = new MessageAddress()
               {
                  Address = inlineAddress,
                  Message = objMessage
               };
               uow.Context.MessageAddresses.Add(ma);
               objMessage.MessageAddresses.Add(ma);
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
            } else if (address.Length == 11)
            {
               //Remove leading 1
               address = address.Substring(1);
            }
         }
         return address;
      }
   }
}
