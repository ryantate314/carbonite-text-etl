using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CarboniteXmlParser
{
   class XmlReaderStatusWrapper
   {
      public XmlReader Reader { get; set; }
      public XmlReaderStatus Status { get; set; }
      public bool InUse { get; set; }
   }
}
