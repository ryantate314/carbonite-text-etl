using CarboniteTextMessageImport.XmlEntities;
using CarboniteXmlParser.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarboniteXmlParser.XmlEntities
{
   //https://developer.android.com/reference/android/provider/Telephony.BaseMmsColumns.html
   public class Mms : Message
   {
      public string MessageId { get; set; }
      public ICollection<Address> Addresses { get; set; }
      public ICollection<MessagePart> Parts { get; set; }
      public MessageBox Box { get; set; }

      public Mms()
      {
         Addresses = new List<Address>();
         Parts = new List<MessagePart>();
      }

      public override string GetMessageId()
      {
         return MessageId;
      }
   }
}
