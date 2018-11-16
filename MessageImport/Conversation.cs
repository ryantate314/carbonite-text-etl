using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageImport
{
    class Conversation
    {
        public List<string> PhoneNumbers { get; set; }

        public Conversation()
        {
            PhoneNumbers = new List<string>();
        }

        public override int GetHashCode()
        {
            return String.Join(",", PhoneNumbers.OrderBy(x => x)).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
