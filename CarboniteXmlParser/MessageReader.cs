using CarboniteXmlParser.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CarboniteXmlParser
{
   public class MessageReader : IDisposable
   {
      private string _filename;
      private XmlReaderRepository _readerRepo;

      public IEnumerable<Sms> TextMessages { get {
            return new SmsCollection(_readerRepo);
         }
      }
      public IEnumerable<Mms> MultimediaMessages { get {
            return new MmsCollection(_readerRepo);
         }
      }

      public MessageReader(string filename)
      {
         _filename = filename;
         _readerRepo = new XmlReaderRepository(_filename, new XmlReaderFactory());
      }
      
      public void Dispose()
      {
         _readerRepo.Dispose();
      }
   }
}
