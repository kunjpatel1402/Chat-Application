using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DecentChat
{
    public class Contact : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Call this method to raise the PropertyChanged event
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _name;
        public string name 
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(name));
                }
            }
        }
        private int _hash_val;
        public int hash_val 
        {
            get
            {
                return _hash_val;
            }
            set
            {
                if (value != _hash_val)
                {
                    _hash_val = value;
                    OnPropertyChanged(nameof(hash_val));
                }
            }
        }
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
