using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CarboniteXmlParser
{
   /// <summary>
   /// Provides a cache for half-used XmlReaders
   /// </summary>
   public class XmlReaderRepository : IDisposable
   {

      private List<XmlReaderStatusWrapper> _repo;
      private XmlReaderFactory _readerFactory;
      private string _filename;

      public XmlReaderRepository(string filename, XmlReaderFactory readerFactory)
      {
         _filename = filename;
         _readerFactory = readerFactory;
         _repo = new List<XmlReaderStatusWrapper>();
      }

      /// <summary>
      /// Releases an XmlReader to the repository.
      /// </summary>
      /// <param name="reader"></param>
      /// <param name="status"></param>
      public void Add(XmlReader reader, XmlReaderStatus status)
      {
         Add(reader, status, false);
      }

      private void Add(XmlReader reader, XmlReaderStatus status, bool inUse)
      {
         var existingEntry = GetWrapper(reader);
         if (existingEntry == null)
         {
            existingEntry = new XmlReaderStatusWrapper()
            {
               Reader = reader,
            };
            _repo.Add(existingEntry);
         }
         existingEntry.InUse = inUse;
         existingEntry.Status = status;
      }

      public XmlReader GetReader()
      {
         var newReader = _readerFactory.GetNewReader(_filename);
         Add(newReader, XmlReaderStatus.New, true);
         return newReader;
      }

      private XmlReaderStatusWrapper GetWrapper(XmlReader reader)
      {
         return _repo.FirstOrDefault(x => x.Reader == reader);
      }

      public XmlReader GetReader(XmlReaderStatus status)
      {
         XmlReader reader = null;
         var repoWrapper = _repo.Where(x => x.Status == status)
                                 .FirstOrDefault();
         if (repoWrapper == null)
         {
            reader = _readerFactory.GetNewReader(_filename);
            Add(reader, XmlReaderStatus.New, true);
         }
         else
         {
            repoWrapper.InUse = true;
            reader = repoWrapper.Reader;
         }
         return reader;
      }

      public void Dispose()
      {
         _repo.ForEach(x => x.Reader.Dispose());
      }

      /// <summary>
      /// Disposes the specified reader and removes it's refernce from the repository.
      /// </summary>
      /// <param name="reader"></param>
      public void Dispose(XmlReader reader)
      {
         var wrapper = GetWrapper(reader);
         if (wrapper != null)
         {
            _repo.Remove(wrapper);
            wrapper.Reader.Dispose();
         }
      }
   }
}
