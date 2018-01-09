using CarboniteXmlParser.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace CarboniteXmlParser
{
   class MmsCollection : IEnumerable<Mms>
   {
      private XmlReaderRepository _repo;

      public MmsCollection(XmlReaderRepository repo)
      {
         _repo = repo;
      }

      private IEnumerator<Mms> ConstructEnumerator()
      {
         return new MmsEnumerator(_repo);
      }

      public IEnumerator<Mms> GetEnumerator()
      {
         return ConstructEnumerator();
      }

      IEnumerator IEnumerable.GetEnumerator()
      {
         return ConstructEnumerator();
      }


   }
}
