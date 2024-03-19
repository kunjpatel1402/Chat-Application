using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.Serialization;

namespace DecentChat
{
    [DataContract]
    public class Message
    {
        [DataMember(Name = "Date")]
        public DateTime Date { get; set; }

        [DataMember(Name = "From")]
        public int From { get; set; }

        [DataMember(Name = "To")]
        public int To { get; set; }

        [DataMember(Name = "Text")]
        public string Text { get; set; }
    }
}
