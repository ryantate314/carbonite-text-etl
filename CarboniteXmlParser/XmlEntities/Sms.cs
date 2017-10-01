using CarboniteXmlParser.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CarboniteXmlParser.XmlEntities
{
   public class Sms : Message
   {
      public int TPStatus { get; set; }
      public MessageType MessageType { get; set; }

      public override string GetMessageId()
      {
         string hash;
         using (var md5 = MD5.Create())
         {
            StringBuilder builder = new StringBuilder();
            builder.Append(Date.ToString());
            builder.Append(Body);
            hash = Convert.ToBase64String(md5.ComputeHash(Encoding.ASCII.GetBytes(builder.ToString())));
         }
         return hash;
      }
   }
}
