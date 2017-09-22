using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarboniteXmlParser.XmlEntities
{
   public class Sms : Message
   {
      public string Address { get; set; }
      public int TPStatus { get; set; }
      public Android.Provider.Telephony.MessageType MessageType { get; set; }
   }
}
