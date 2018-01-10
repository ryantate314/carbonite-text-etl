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
      private string _filename;

      public SmsCollection(string filename)
      {
         _filename = filename;
      }

      private IEnumerator<Sms> ConstructEnumerator()
      {
         return new SmsIterator(_filename);
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
