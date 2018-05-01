using Data.Staging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
   public class AttachmentRepository
   {
      private string _connectionString;

      public AttachmentRepository(string connectionString)
      {
         _connectionString = connectionString;
      }

      public void BulkAddAttachments(IEnumerable<Attachment> attachments)
      {
         using (var connection = new SqlConnection(_connectionString))
         {
            connection.Open();
            using (var copy = new SqlBulkCopy(connection))
            {
               copy.DestinationTableName = "Staging.Attachment";
               copy.WriteToServer(ToDataTable(attachments));
            }
         }
      }

      private DataTable ToDataTable(IEnumerable<Attachment> attachments)
      {
         DataTable table = new DataTable();
         table.Columns.Add("FileName", typeof(string));
         table.Columns.Add("Path", typeof(string));
         table.Columns.Add("MimeType", typeof(string));
         table.Columns.Add("MessageId", typeof(string));

         foreach (var attachment in attachments)
         {
            table.Rows.Add(attachment.FileName, attachment.Path, attachment.MimeType, attachment.MessageId);
         }

         return table;
      }
   }
}
