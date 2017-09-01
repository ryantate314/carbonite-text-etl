using CarboniteTextMessageImport.XmlEntities;
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
      public SmsIterator(string filename) : base(filename)
      {
         Reader.Read();
      }

      public override Sms GetNextMessage()
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


         return message;
      }
   }
}
