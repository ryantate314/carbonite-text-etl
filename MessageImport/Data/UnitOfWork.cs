using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace MessageImport.Data
{
   class UnitOfWork<T> : IDisposable where T : DbContext
   {
      private T _context;
      public T Context {
         get {
            return _context;
         }
      }
      private TransactionScope _trans;


      public UnitOfWork(T context)
      {
         _context = context;
         //_trans = new TransactionScope();
      }

      public void Commit()
      {
         try
         {
            _context.SaveChanges();
            //_trans.Complete();
            //_trans.Commit();
         }
         catch (Exception)
         {
            Rollback();
            throw;
         }
      }

      public void Rollback()
      {
         //_trans.Dispose();
      }

      public void Dispose()
      {
         //_trans.Dispose();
      }
   }
}
