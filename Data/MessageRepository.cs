using Dapper;
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
    public class MessageRepository
    {
        private string _connectionString;

        public MessageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void BulkAddMessages(IEnumerable<Message> messages)
        {
            using (var connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["StagingContext"].ConnectionString))
            {
                connection.Open();
                using (var copy = new SqlBulkCopy(connection))
                {
                    copy.DestinationTableName = "Staging.Message";
                    copy.WriteToServer(ToDataTable(messages));
                }
            }
        }

        public IEnumerable<SimpleMessage> GetSampleMessages(ICollection<string> numbers)
        {
            //Select a sample list of text messages for each contact
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();

                DataTable table = new DataTable();
                table.Columns.Add("Number", typeof(string));

                foreach (string number in numbers)
                {
                    table.Rows.Add(number);
                }

                IEnumerable<SimpleMessage> messages = con.Query<SimpleMessage>("Staging.usp_GetSampleMessages",
                                                                               new { numbers = table.AsTableValuedParameter("Staging.PhoneNumberList") },
                                                                               commandType: CommandType.StoredProcedure);

                return messages;
            }
        }

        private static DataTable ToDataTable(IEnumerable<Message> messages)
        {
            var table = new DataTable();
            table.Columns.Add("MessageId", typeof(string));
            table.Columns.Add("SendDate", typeof(DateTime));
            table.Columns.Add("Body", typeof(string));
            table.Columns.Add("MessageType", typeof(byte));
            table.Columns.Add("Status", typeof(byte));
            table.Columns.Add("ConversationId", typeof(string));

            foreach (var message in messages)
            {
                table.Rows.Add(
                   message.MessageId,
                   message.SendDate,
                   message.Body,
                   message.MessageType,
                   message.Status,
                   message.ConversationId);
            }

            return table;
        }
    }
}
