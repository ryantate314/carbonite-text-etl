using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCreatorTool
{
    public class Message
    {
        public DateTime Date { get; set; }
        public string Body { get; set; }
        public MessageDirection Direction { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactName { get; set; }
    }
}
