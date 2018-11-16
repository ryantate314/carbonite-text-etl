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
    public class ContactRepository
    {
        private string _connectionString;

        public ContactRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void BulkAddContacts(IEnumerable<MessageAddress> contacts)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var copy = new SqlBulkCopy(connection))
                {
                    copy.DestinationTableName = "Staging.MessageAddress";
                    copy.WriteToServer(ToDataTable(contacts));
                }
            }
        }

        public Dictionary<string, List<string>> GetContactOptions()
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                var options = con.Query<USP_Select_Contact_Options_Result>("Staging.USP_Select_Contact_Options", commandType: CommandType.StoredProcedure);
                return options
                         .GroupBy(o => o.Number, o => o.ContactName)
                         .ToDictionary(go => go.Key, go => go.ToList());
            }
        }

        public void UpdateContacts(IEnumerable<ContactUpdate> contacts)
        {
            using (var con = new SqlConnection(_connectionString))
            {
                con.Open();
                con.Execute("Staging.USP_Update_Contacts",
                   new
                   {
                       contactInfo = ToDataTable(contacts)
                                           .AsTableValuedParameter("dbo.ContactInfo")
                   },
                   commandType: CommandType.StoredProcedure);
            }
        }

        private DataTable ToDataTable(IEnumerable<ContactUpdate> contacts)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Number", typeof(string));
            table.Columns.Add("NewName", typeof(string));

            foreach (var contact in contacts)
            {
                table.Rows.Add(contact.Number, contact.NewName);
            }

            return table;
        }

        private DataTable ToDataTable(IEnumerable<MessageAddress> contacts)
        {
            DataTable table = new DataTable();
            table.Columns.Add("MessageId", typeof(string));
            table.Columns.Add("Number", typeof(string));
            table.Columns.Add("ContactName", typeof(string));
            table.Columns.Add("Direction", typeof(byte));

            foreach (var contact in contacts)
            {
                table.Rows.Add(contact.MessageId, contact.Number, contact.ContactName, contact.Direction);
            }

            return table;
        }
    }
}
