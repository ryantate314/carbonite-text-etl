using CarboniteXmlParser.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CarboniteXmlParser
{
   public class MessageReader
   {
      private string _filename;

      public IEnumerable<Sms> TextMessages { get {
            return new SmsCollection(_filename);
         }
      }
      public IEnumerable<Mms> MultimediaMessages { get {
            return new MmsCollection(_filename);
         }
      }

      public MessageReader(string filename)
      {
         _filename = filename;
      }
      
   }
}
