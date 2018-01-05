using Data.Staging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
   class MessageRepository : BatchRepository<Message>
   {
      public MessageRepository(string connectionString, int batchSize = 1000, bool autoCommit = true) : base(batchSize, autoCommit)
      {
      }

      public override void Save()
      {
         throw new NotImplementedException();
      }
   }
}
