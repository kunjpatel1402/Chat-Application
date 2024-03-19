using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DecentChat
{
    [DataContract]
    public class Node
    {
        private string _node_name;
        private int _hash_val;
        private string _ip_address;
        private int _port;

        [DataMember]
        public string node_name
        {
            get { return _node_name; }
            set
            {
                if (_node_name != value)
                {
                    _node_name = value;
                    OnPropertyChanged(nameof(node_name));
                }
            }
        }

        [DataMember]
        public int hash_val
        {
            get { return _hash_val; }
            set
            {
                if (_hash_val != value)
                {
                    _hash_val = value;
                    OnPropertyChanged(nameof(hash_val));
                }
            }
        }

        [DataMember]
        public string ip_address
        {
            get { return _ip_address; }
            set
            {
                if (_ip_address != value)
                {
                    _ip_address = value;
                    OnPropertyChanged(nameof(ip_address));
                }
            }
        }

        [DataMember]
        public int port
        {
            get { return _port; }
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnPropertyChanged(nameof(port));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    eventHandler(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }

        public Node() { }
        public Node(string node_name = "", int hash_val = -1, string ip_address = "", int port = -1)
        {
            this.node_name = node_name;
            this.hash_val = hash_val;
            this.ip_address = ip_address;
            this.port = port;
        }
        public override string ToString()
        {
            return this.node_name + " " + this.hash_val.ToString() + " " + this.ip_address + " " + this.port.ToString() + "\n";
        }
        public static bool operator ==(Node a, Node b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            if ((a.node_name == b.node_name) && (a.hash_val == b.hash_val) && (a.ip_address == b.ip_address) && (a.port == b.port)) return true;
            else return false;
        }
        public static bool operator !=(Node a, Node b)
        {
            return !(a == b);
        }
    }
}
