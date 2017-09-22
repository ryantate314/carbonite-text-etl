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
      private List<MessageEnumerator<Sms>> _smsEnumerators;
      private List<MessageEnumerator<Mms>> _mmsEnumerators;

      public MessageReader(string filename)
      {
         _filename = filename;
         _smsEnumerators = new List<MessageEnumerator<Sms>>();
         _mmsEnumerators = new List<MessageEnumerator<Mms>>();
      }

      public SmsIterator getSmsIterator()
      {
         SmsIterator iterator = new SmsIterator(_filename);
         _smsEnumerators.Add(iterator);
         return iterator;
      }

      public MmsIterator getMmsIterator()
      {
         MmsIterator iterator = new MmsIterator(_filename);
         _mmsEnumerators.Add(iterator);
         return iterator;
      }
      
      public void Dispose()
      {
         foreach (var enumerator in _smsEnumerators)
         {
            enumerator.Dispose();
         }
         foreach (var enumerator in _mmsEnumerators)
         {
            enumerator.Dispose();
         }
      }
   }
}
