using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarboniteTextMessageImport.XmlEntities
{
   class MessagePart
   {
      public int SequenceNum { get; set; }
      public byte[] Data { get; set; }
      public string Text { get; set; }
      public string FileName { get; set; }
      public string MimeType { get; set; }
   }
}
