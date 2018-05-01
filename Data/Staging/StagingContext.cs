using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Staging
{
   public class StagingContext
   {

      private string _connectionString;

      public MessageRepository Messages { get; private set; }
      public AttachmentRepository Attachments { get; private set; }
      public ContactRepository Contacts { get; private set; }

      public StagingContext()
      {
         string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["StagingContext"].ConnectionString;

         Messages = new MessageRepository(connectionString);
         Attachments = new AttachmentRepository(connectionString);
         Contacts = new ContactRepository(connectionString);

         _connectionString = connectionString;
      }

      public void TruncateStaging()
      {
         using (var con = new SqlConnection(_connectionString))
         {
            con.Open();
            con.Execute("Staging.usp_Truncate_Staging", commandType: System.Data.CommandType.StoredProcedure);
         }
      }

      public void MergeStaging(DateTime backupDate)
      {
         using (var con = new SqlConnection(_connectionString))
         {
            con.Execute("dbo.usp_Merge", new { backupDate = backupDate }, commandType: System.Data.CommandType.StoredProcedure);
         }
      }

   }
}
