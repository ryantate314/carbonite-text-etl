using CarboniteTextMessageImport.XmlEntities;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarboniteTextMessageImport
{
   class SmsIterator : MessageEnumerator<Sms>
   {

      private static readonly ILog _log = LogManager.GetLogger(typeof(SmsIterator));

      public SmsIterator(string filename) : base(filename)
      {
         Reader.Read();
      }

      protected override Sms GetNextMessage()
      {
         Sms result = null;
         bool foundMessage = Reader.ReadToFollowing("sms");
         if (foundMessage)
         {
            result = parseMessage();
         }
         return result;
      }

      private Sms parseMessage()
      {
         Sms message = new Sms();
         message.Body = Reader.GetAttribute("body");
         message.Address = Reader.GetAttribute("address");
         message.ContactName = Reader.GetAttribute("contact_name");
         string dateString = Reader.GetAttribute("date");
         ulong epoch = UInt64.Parse(dateString);
         message.Date = XmlUtilities.ParseTimestamp(epoch);

         return message;
      }
   }
}
