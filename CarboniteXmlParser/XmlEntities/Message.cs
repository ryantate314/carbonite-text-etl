using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarboniteXmlParser.XmlEntities
{
   public abstract class Message
   {
      
      public DateTime Date { get; set; }
      public string ContactName { get; set; }
      public string Address { get; set; }
      public int LineNumber { get; set; }

      public abstract string GetMessageId();
      
   }
}
