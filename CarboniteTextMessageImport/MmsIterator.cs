using CarboniteTextMessageImport.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarboniteTextMessageImport
{
   class MmsIterator : MessageEnumerator<Mms>
   {
      public MmsIterator(string filename) : base(filename)
      {
         Reader.Read();
      }

      protected override Mms GetNextMessage()
      {
         throw new NotImplementedException();
      }
   }
}
