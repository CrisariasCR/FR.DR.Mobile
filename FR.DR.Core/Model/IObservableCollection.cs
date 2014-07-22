using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;

namespace FR.Core.Model
{
    public interface IObservableCollection<T> :
        IList<T>, INotifyCollectionChanged
    {

    }

    public class SimpleObservableCollection<T> :
        ObservableCollection<T>, IObservableCollection<T>
    {
        public SimpleObservableCollection(List<T> source)
            : base(source)
        {
        }

        public SimpleObservableCollection()
            : base()
        {
        }
    }
}
