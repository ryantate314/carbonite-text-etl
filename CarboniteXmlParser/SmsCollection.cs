using CarboniteXmlParser.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace CarboniteXmlParser
{
   class SmsCollection : IEnumerable<Sms>
   {
      private XmlReaderRepository _repo;

      public SmsCollection(XmlReaderRepository repo)
      {
         _repo = repo;
      }

      private IEnumerator<Sms> ConstructEnumerator()
      {
         return new SmsIterator(_repo);
      }

      public IEnumerator<Sms> GetEnumerator()
      {
         return ConstructEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return ConstructEnumerator();
      }
   }
}
