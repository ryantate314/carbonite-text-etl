using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarboniteXmlParser.XmlEntities
{
    public class XmlUtilities
    {
        public static DateTime ParseTimestamp(UInt64 timestamp)
        {
            //Epoch timestamps are always UTC
            DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            date = date.AddMilliseconds(timestamp);
            return date;
        }

        public static string FormatDate(DateTime date)
        {
            //"Jun 19, 2013 1:24:07 PM"
            return date.ToString("MMM d, yyyy h:mm:ss tt");
        }
    }
}
