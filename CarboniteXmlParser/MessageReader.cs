using CarboniteXmlParser.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CarboniteXmlParser
{
   public class MessageReader
   {
      private string _filename;

      public IEnumerable<Sms> TextMessages { get {
            return new SmsCollection(_filename);
         }
      }
      public IEnumerable<Mms> MultimediaMessages { get {
            return new MmsCollection(_filename);
         }
      }

      private DateTime _backupDate;
      public DateTime BackupDate {
         get {
            return _backupDate;
         }
      }

      private int _numMessages;
      public int NumMessages {
         get {
            return _numMessages;
         }
      }

      public MessageReader(string filename)
      {
         _filename = filename;
         //TODO test following code
         using (XmlReader reader = XmlReaderFactory.NewReader(filename))
         {
            reader.Read();
            if (reader.ReadToFollowing("smses"))
            {
               string numMessagesString = reader.GetAttribute("count");
               Int32.TryParse(numMessagesString, out _numMessages);

               string backupDateString = reader.GetAttribute("backup_date");
               ulong backupTimestamp;
               UInt64.TryParse(backupDateString, out backupTimestamp);
               _backupDate = XmlUtilities.ParseTimestamp(backupTimestamp);
            }
         }
      }
   }
}
