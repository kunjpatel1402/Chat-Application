using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;

namespace DecentChat
{
    public class TrulyObservableCollection<T> : ObservableCollection<T>
        where T : INotifyPropertyChanged
    {
        public TrulyObservableCollection()
            : base()
        {
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        public TrulyObservableCollection(IEnumerable<T> pItems)
            : base(pItems)
        {
            if (pItems != null)
            {
                foreach (T item in pItems)
                {
                    if (item != null)
                    {
                        item.PropertyChanged += ItemPropertyChanged;
                    }
                }
            }
            CollectionChanged += FullObservableCollectionCollectionChanged;
        }

        void FullObservableCollectionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Object item in e.NewItems)
                {
                    if (item != null)
                    {
                        ((INotifyPropertyChanged)item).PropertyChanged += ItemPropertyChanged;
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (Object item in e.OldItems)
                {
                    if (item != null)
                    {
                        ((INotifyPropertyChanged)item).PropertyChanged -= ItemPropertyChanged;
                    }
                }
            }
        }

        void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender, IndexOf((T)sender));
            OnCollectionChanged(args);
        }
    }

}
