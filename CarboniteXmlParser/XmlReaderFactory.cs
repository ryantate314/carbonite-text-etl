using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CarboniteXmlParser
{
   public class XmlReaderFactory
   {
      public XmlReaderFactory()
      {

      }

      public XmlReader GetNewReader(string filename)
      {
         XmlReaderSettings settings = new XmlReaderSettings
         {
            CheckCharacters = false //Ignore invalid unicode (emojis)
         };
         XmlReader reader = XmlReader.Create(filename, settings);
         reader.Read();
         return reader;
      }
   }
}
