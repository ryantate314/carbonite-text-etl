using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarboniteTextMessageImport.XmlEntities
{
   class XmlUtilities
   {
      public static DateTime ParseTimestamp(UInt64 timestamp)
      {
         DateTime date = new DateTime(1970, 1, 1);
         date = date.AddMilliseconds(timestamp);
         return date;
      }
   }
}
