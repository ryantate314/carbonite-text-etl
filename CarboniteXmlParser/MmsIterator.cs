using Android.Provider.Telephony;
using CarboniteXmlParser.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
         message.Body = Reader.GetAttribute("body");
         message.ContactName = Reader.GetAttribute("contact_name");
         message.Body = Reader.GetAttribute("data");
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
               part.FileName = Reader.GetAttribute("name");
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
   }
}
