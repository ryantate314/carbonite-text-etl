using CarboniteXmlParser.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml;

namespace CarboniteXmlParser
{
   public abstract class MessageEnumerator<T> : IEnumerator<T>, IDisposable
      where T : Message
   {
      private XmlReader _reader;

      protected XmlReader Reader {
         get {
            return _reader;
         }
      }

      private T _current;

      public T Current { get { return _current; } }

      object IEnumerator.Current {
         get {
            return Current;
         }
      }

      public MessageEnumerator(string filename)
      {
         XmlReaderSettings settings = new XmlReaderSettings
         {
            CheckCharacters = false //Ignore invalid unicode (emojis)
         };
         _reader = XmlReader.Create(filename, settings);
         _reader.Read();
      }

      public bool MoveNext()
      {
         _current = GetNextMessage();
         return Current != null;
      }

      protected abstract T GetNextMessage();

      public void Reset()
      {
         throw new NotSupportedException();
      }

      public void Dispose()
      {
         ((IDisposable)_reader).Dispose();
      }
   }
}
