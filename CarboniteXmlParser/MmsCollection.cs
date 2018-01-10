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
      private string _filename;

      public MmsCollection(string filename)
      {
         _filename = filename;
      }

      private IEnumerator<Mms> ConstructEnumerator()
      {
         return new MmsEnumerator(_filename);
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
