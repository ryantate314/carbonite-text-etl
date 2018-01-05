using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
   abstract class BatchRepository<T>
   {

      private bool _autoCommit;

      private int _batchSize;
      public int BatchSize {
         get {
            return _batchSize;
         }
      }

      private List<T> _batch { get; set; }
      /// <summary>
      /// The current batch of entities. This is a read-only list.
      /// </summary>
      public List<T> Batch {
         get {
            return _batch.ToList();
         }
      }

      public BatchRepository(int batchSize, bool autoCommit = true)
      {
         _autoCommit = autoCommit;
         _batch = new List<T>(batchSize);
      }

      public abstract void Save();

      public bool IsFull()
      {
         return _batch.Count >= BatchSize;
      }

      public void ClearBatch()
      {
         _batch.Clear();
      }

      public void AddEntity(T item)
      {
         if (IsFull() && _autoCommit)
         {
            Save();
            ClearBatch();
         }
         else if (IsFull() && !_autoCommit)
         {
            throw new Exception("Batch is full and auto-commit is set to false.");
         }

         _batch.Add(item);
      }
   }
}
