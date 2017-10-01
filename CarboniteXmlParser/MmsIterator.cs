using CarboniteTextMessageImport.Android;
using CarboniteTextMessageImport.XmlEntities;
using CarboniteXmlParser.Android;
using CarboniteXmlParser.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CarboniteXmlParser
{
   public class MmsIterator : MessageEnumerator<Mms>
   {
      public MmsIterator(string filename) : base(filename)
      {
      }

      protected override Mms GetNextMessage()
      {
         Mms result = null;
         bool foundMessage = Reader.ReadToFollowing("mms");
         if (foundMessage)
         {
            result = parseMessage();
         }
         return result;
      }

      private Mms parseMessage()
      {
         Mms message = new Mms();
         message.LineNumber = (Reader as IXmlLineInfo)?.LineNumber ?? -1;
         message.ContactName = Reader.GetAttribute("contact_name");
         message.Address = Reader.GetAttribute("address");
         string dateString = Reader.GetAttribute("date");
         ulong epoch = UInt64.Parse(dateString);
         message.Date = XmlUtilities.ParseTimestamp(epoch);
         message.MessageId = Reader.GetAttribute("m_id");
         message.Box = (MessageBox)Int32.Parse(Reader.GetAttribute("msg_box"));

         _parseParts(message);

         return message;
      }

      private void _parseParts(Mms message)
      {
         if (Reader.ReadToDescendant("parts") && Reader.ReadToDescendant("part"))
         {
            do
            {
               MessagePart part = new MessagePart();
               part.SequenceNum = Int32.Parse(Reader.GetAttribute("seq"));

               string filename = Reader.GetAttribute("fn");
               string name = Reader.GetAttribute("name");

               part.FileName = filename;
               if (filename == "null")
               {
                  part.FileName = name;
               }

               part.MimeType = Reader.GetAttribute("ct");
               string data = Reader.GetAttribute("data");
               if (data != null)
               {
                  part.Data = Convert.FromBase64String(data);
               }
               part.Text = Reader.GetAttribute("text");

               message.Parts.Add(part);
            } while (Reader.ReadToNextSibling("part"));
         }
      }

      private void _parseAddresses(Mms message)
      {
         if (Reader.ReadToDescendant("addrs") && Reader.ReadToDescendant("addr"))
         {
            do
            {
               Address address = new Address();
               address.Number = Reader.GetAttribute("address");
               address.Type = (AddressType)Int32.Parse(Reader.GetAttribute("type"));
               message.Addresses.Add(address);
            } while (Reader.ReadToNextSibling("addr"));
         }
      }
   }
}
