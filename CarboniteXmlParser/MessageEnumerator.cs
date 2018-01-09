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
   /// <summary>
   /// Contains the logic to set up the XMLReader for the carbonite backup file.
   /// </summary>
   /// <typeparam name="T">The type of message being read.</typeparam>
   public abstract class MessageEnumerator<T> : IEnumerator<T>, IDisposable
      where T : Message
   {
      private XmlReader _reader;
      private XmlReaderRepository _repo;

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

      public MessageEnumerator(XmlReaderRepository repo)
      {
         _reader = repo.GetReader(XmlReaderStatus.AtMMS);
         _repo = repo;
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

      protected void ReleaseReader(XmlReaderStatus newStatus)
      {
         _repo.Add(_reader, newStatus);
      }

      #region IDisposable Support
      private bool disposedValue = false; // To detect redundant calls

      protected virtual void Dispose(bool disposing)
      {
         if (!disposedValue)
         {
            if (disposing)
            {
               _repo.Dispose(_reader);
            }

            disposedValue = true;
         }
      }

      // This code added to correctly implement the disposable pattern.
      public void Dispose()
      {
         // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
         Dispose(true);
      }
      #endregion

   }
}
