using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
      private DbContextTransaction _trans;


      public UnitOfWork(T context)
      {
         _context = context;
         _trans = context.Database.BeginTransaction();
      }

      public void Commit()
      {
         try
         {
            _context.SaveChanges();
            _trans.Commit();
         }
         catch (Exception)
         {
            Rollback();
            throw;
         }
      }

      public void Rollback()
      {
         _trans.Rollback();
      }

      public void Dispose()
      {
         _trans.Dispose();
      }
   }
}
