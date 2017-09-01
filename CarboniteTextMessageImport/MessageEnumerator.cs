using CarboniteTextMessageImport.XmlEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml;

namespace CarboniteTextMessageImport
{
   abstract class MessageEnumerator<T> : IEnumerator<T>, IDisposable
      where T : Message
   {
      private XmlReader _reader;

      protected XmlReader Reader {
         get {
            return _reader;
         }
      }

      private T _current;

      public T Current { get; }

      object IEnumerator.Current {
         get {
            return Current;
         }
      }

      public MessageEnumerator(string filename)
      {
         _reader = XmlReader.Create(filename);
      }

      public bool MoveNext()
      {
         _current = GetNextMessage();
         return Current != null;
      }

      public abstract T GetNextMessage();

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
