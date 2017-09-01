using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CarboniteTextMessageImport
{
   class MessageReader : IDisposable
   {
      private XmlReader _reader;
      public MessageReader(string filename)
      {
         _reader = XmlReader.Create(filename);
         
      }




      public void Dispose()
      {
         if (_reader != null)
         {
            _reader.Dispose();
         }
      }
   }
}
