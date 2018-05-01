using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CarboniteXmlParser
{
   class XmlReaderFactory
   {
      public static XmlReader NewReader(string filename)
      {
         XmlReaderSettings settings = new XmlReaderSettings
         {
            CheckCharacters = false //Ignore invalid unicode (emojis)
         };
         return XmlReader.Create(filename, settings);
      }
   }
}
