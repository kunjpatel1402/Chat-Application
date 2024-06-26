﻿using System;
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
        [DataMember(Name = "sender_node_name")]
        public string sender_node_name { get; set; }
        [DataMember(Name = "hash_code")]
        public int hash_code {  get; set; }
        [DataMember(Name = "alignment")]
        public LayoutOptions Alignment { get; set; }
        [DataMember(Name = "color")]
        public Color BubbleColor { get; set; }
    }
}
