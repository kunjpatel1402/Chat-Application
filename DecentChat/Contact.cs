using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecentChat
{
    public class Contact
    {
        public string name { get; set; }
        public int hash_val { get; set; }
        public Contact (string name, int hash_val)
        {
            this.name = name;
            this.hash_val = hash_val;
        }
        public override string ToString()
        {
            StringBuilder details = new StringBuilder();

            if (name != null) details.Append(name.ToString() + " " + hash_val.ToString() + "\n");
            else details.Append("null");
            return details.ToString();
        }
    }
}
